using System;
using System.Collections.Generic;
using System.Linq;
using SpreadsheetCalculator.ExpressionCalculator;
using SpreadsheetCalculator.ExpressionParser;
using SpreadsheetCalculator.DirectedGraph;

namespace SpreadsheetCalculator.Spreadsheet
{
    /// <summary>
    /// Spreadsheet of N x M size.
    /// </summary>
    class MathSpreadsheet : IViewSpreadsheet, IEditSpreadsheet
    {
        /// <summary>
        /// Define maximum possible rows in Spreadsheet.
        /// </summary>
        private const int MaxRowNumber = 999999;

        /// <summary>
        /// Define maximum possible columns in Spreadsheet.
        /// </summary>
        private const int MaxColumnNumber = 18278; // "ZZZ"

        /// <summary>
        /// Spreadsheet Cells.
        /// </summary>
        private Matrix<ICell> Matrix { get; set; }

        /// <summary>
        /// Spreadsheet cell evaluator.
        /// </summary>
        private IExpressionCalculator ExpressionCalculator { get; }

        /// <summary>
        /// Spreadsheet cell text parser.
        /// </summary>
        private IStringParser StringParser { get; }

        /// <summary>
        /// Implements <see cref="IViewSpreadsheet.RowsCount"/>.
        /// </summary>
        public int RowsCount => Matrix.RowsCount;

        /// <summary>
        /// Implements <see cref="IViewSpreadsheet.ColumnsCount"/>.
        /// </summary>
        public int ColumnsCount => Matrix.ColumnsCount;

        /// <summary>
        /// Create new Spreadsheet.
        /// </summary>
        /// <param name="calculator">Spreadsheet cell calculator.</param>
        /// <param name="stringParser">Spreadsheet cell text parser.</param>
        public MathSpreadsheet(IExpressionCalculator calculator, IStringParser stringParser)
        {
            ExpressionCalculator = calculator ?? throw new ArgumentNullException(nameof(calculator));

            StringParser = stringParser ?? throw new ArgumentNullException(nameof(stringParser));
        }

        /// <summary>
        /// Implements <see cref="IEditSpreadsheet.SetSize"/>.
        /// </summary>
        public void SetSize(int columnNumber, int rowNumber)
        {
            if (columnNumber > MaxColumnNumber)
            {
                throw new IndexOutOfRangeException($"Spreadsheet is to big. Max allowed columns {MaxColumnNumber}");
            }

            if (rowNumber > MaxRowNumber)
            {
                throw new IndexOutOfRangeException($"Spreadsheet is to big. Max allowed rows {MaxRowNumber}");
            }

            if (columnNumber <= 0 || rowNumber <= 0)
            {
                throw new IndexOutOfRangeException($"Invalid spreadsheet size {columnNumber} x {rowNumber}");
            }

            Matrix = new Matrix<ICell>(columnNumber, rowNumber);
        }

        /// <summary>
        /// Implements <see cref="IEditSpreadsheet.SetValue(int, int, string)"/>.
        /// </summary>
        public void SetValue(int column, int row, string value)
        {
            if (Matrix.InMatrix(column, row))
            {
                Matrix[column, row] = new Cell(StringParser.Parse(value));
                return;
            }

            throw new IndexOutOfRangeException("Requested cell is out of spreadsheet.");
        }

        /// <summary>
        /// Implements <see cref="IEditSpreadsheet.SetValue(string, string)"/>.
        /// </summary>
        public void SetValue(string key, string value)
        {
            var position = new CellPosition(key);

            SetValue(position.Column, position.Row, value);
        }

        /// <summary>
        /// Implements <see cref="IViewSpreadsheet.GetValue(int, int)"/>.
        /// </summary>
        public string GetValue(int column, int row)
        {
            if (Matrix.InMatrix(column, row))
            {
                return Matrix[column, row].ToString();
            }

            throw new IndexOutOfRangeException("Requested cell is out of spreadsheet.");   
        }

        /// <summary>
        /// Implements <see cref="IViewSpreadsheet.GetValue(string)"/>.
        /// </summary>
        public string GetValue(string key)
        {
            var position = new CellPosition(key);

            return GetValue(position.Column, position.Row);
        }

        /// <summary>
        /// Calculate all cells in spreadsheet.
        /// </summary>
        /// <exception cref="SpreadsheetInternallException">Cyclic dependency found.</exception>
        public void Calculate()
        {
            IList<ICell> sortedCells;

            try
            {
                sortedCells = TopologicalSort.Sort(
                    Matrix.AsEnumerable(),
                    cell => cell.CellDependencies
                        .Select(cellRef => GetCellByKey(cellRef.Value))
                        .Where(reff => reff != null)
                );
            }
            catch (CyclicDependencyException exception)
            {
                throw new SpreadsheetInternallException("Cannot calculate spreadsheet, there is cyclic dependencies between cells.", exception);
            }

            foreach (var cell in sortedCells)
            {
                CalculateCell(cell);
            }
        }

        /// <summary>
        /// Get Spreadsheet cell by cell reference.
        /// Convert cell reference to column & row indexes in spreadsheet inner array representation.
        /// </summary>
        /// <param name="key">Cell reference: A1, A2, etc...</param>
        /// <returns></returns>
        private ICell GetCellByKey(string key)
        {
            var position = new CellPosition(key);

            // Check that cell reference inside current spreadsheet
            if (Matrix.InMatrix(position.Column, position.Row))
            {
                return Matrix[position.Column, position.Row];
            }

            return null;
        }

        private void CalculateCell(ICell cell)
        {
            if (cell.IsEmpty)
            {
                cell.SetValue(0);
            }
            else 
            {
                var cellDependencies = cell.CellDependencies.Select(token => GetCellByKey(token.Value));

                if (cellDependencies.Any(c => c == null))
                {
                    cell.SetError(CellState.ValueError);
                    return;
                }

                if (cellDependencies.Any(c => c.CellState == CellState.Pending))
                {
                    throw new SpreadsheetInternallException("Calculation flow error, one of the cell references were missed.");
                }

                if (cellDependencies.Any(c => c.CellState != CellState.Fulfilled))
                {
                    cell.SetError(CellState.ValueError);
                    return;
                }

                var cellTokens = cell.CellTokens
                    .Select(token => token.Type == TokenType.CellReference ? new Token(TokenType.Number, GetCellByKey(token.Value).ToString()) : token)
                    .ToList();

                (bool validationResult, string validationError) = ExpressionCalculator.Validate(cellTokens);

                if (validationResult)
                {
                    try
                    {
                        double calculatedValue = ExpressionCalculator.Calculate(cellTokens);

                        cell.SetValue(calculatedValue);
                    }
                    catch (CalculationException)
                    {
                        cell.SetError(CellState.ValueError);
                    }
                    catch (CalculationInrernalException exception)
                    {
                        throw new SpreadsheetInternallException("Internal error during cell calculation.", exception);
                    }
                }
                else
                {
                    cell.SetError(CellState.ValueError);
                }
            }
        }
    }
}
