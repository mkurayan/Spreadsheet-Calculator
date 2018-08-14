using System;
using System.Collections.Generic;
using System.Linq;
using SpreadsheetCalculator.Calculator;
using SpreadsheetCalculator.Parser;
using SpreadsheetCalculator.DirectedGraph;

namespace SpreadsheetCalculator
{
    /// <summary>
    /// Spreadsheet of N x M size.
    /// </summary>
    class Spreadsheet
    {
        /// <summary>
        /// Define maximum possible rows in Spreadsheet.
        /// </summary>
        private const int MaxRowNumber = 1000000;

        /// <summary>
        /// Define maximum possible columns in Spreadsheet.
        /// Currently it limited because for columns we use only single letter [A-Z].
        /// </summary>
        private const int MaxColumnNumber = 26;

        /// <summary>
        /// Spreadsheet rows count.
        /// </summary>
        public int RowNumber { get; }

        /// <summary>
        /// Spreadsheet columns count.
        /// </summary>
        public int ColumnNumber { get; }

        /// <summary>
        /// Spreadsheet Cells.
        /// </summary>
        private ICell[,] Cells { get; }

        /// <summary>
        /// Spreadsheet cell evaluator.
        /// </summary>
        private IExpressionCalculator ExpressionCalculator { get; }

        /// <summary>
        /// Spreadsheet cell text parser.
        /// </summary>
        private IStringParser StringParser { get; }

        /// <summary>
        /// Create new Spreadsheet. 
        /// </summary>
        /// <param name="columnNumber">Spreadsheet columns count.</param>
        /// <param name="rowNumber">Spreadsheet rows count.</param>
        /// <param name="calculator">Spreadsheet cell calculator.</param>
        /// <param name="stringParser">Spreadsheet cell text parser.</param>
        public Spreadsheet(int columnNumber, int rowNumber, IExpressionCalculator calculator, IStringParser stringParser)
        {
            if (!IsInRange(columnNumber, 1, MaxColumnNumber))
            {
                throw new ArgumentException("Invalid column number.");
            }

            if (!IsInRange(rowNumber, 1, MaxRowNumber))
            {
                throw new ArgumentException("Invalid row number.");
            }

            RowNumber = rowNumber;
            ColumnNumber = columnNumber;

            ExpressionCalculator = calculator ?? throw new ArgumentNullException(nameof(calculator));

            StringParser = stringParser ?? throw new ArgumentNullException(nameof(stringParser));

            Cells = new ICell[columnNumber, rowNumber];
        }

        /// <summary>
        /// Set concrete cell in spreadsheet.
        /// </summary>
        /// <param name="columnNumber">Cell column number.</param>
        /// <param name="rowNumber">Cell row number.</param>
        /// <param name="value">Cell value.</param>
        public void SetCell(int columnNumber, int rowNumber, string value)
        {
            if (!IsCellInSpreadsheet(columnNumber, rowNumber))
            {
                throw new InvalidOperationException("Requested cell is out of spreadsheet.");
            }

            Cells[columnNumber, rowNumber] = new Cell(StringParser.Parse(value));
        }

        /// <summary>
        /// Get concrete cell value from spreadsheet.
        /// </summary>
        /// <param name="columnNumber">Cell column number.</param>
        /// <param name="rowNumber">Cell row number.</param>
        /// <param name="value">Cell value.</param>
        public string GetCell(int columnNumber, int rowNumber)
        {
            if (!IsCellInSpreadsheet(columnNumber, rowNumber))
            {
                throw new InvalidOperationException("Requested cell is out of spreadsheet.");
            }

            return Cells[columnNumber, rowNumber].ToString();
        }

        /// <summary>
        /// Calculate Spreadsheet cells.
        /// </summary>
        /// <exception cref="SpreadsheetInternallException">Cyclic dependency found.</exception>
        public void Calculate()
        {
            IList<ICell> sortedCells;

            try
            {
                sortedCells = TopologicalSort.Sort(
                    SpreadsheetCells(),
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
            // Convert alphabetic character to index.
            int colIndex = key[0] - 65;

            // Check for Int32 overflow.
            if (int.TryParse(key.Substring(1), out int rowIndex))
            {
                // For spreadsheet rows we begin numbering from one ( A1, A2, etc...)
                // So, in order to translate it to spreadsheet inner array we subtract 1 from row number.
                rowIndex--;

                // Check that result column & row indexes is inside current spreadsheet
                if (IsCellInSpreadsheet(colIndex, rowIndex))
                {
                    return Cells[colIndex, rowIndex];
                }
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
                    .Select(token => token.Type == TokenType.CellReference ? new Token(TokenType.Number, GetCellByKey(token.Value).ToString()) : token);

                try
                {
                    double calculatedValue = ExpressionCalculator.Calculate(cellTokens);

                    cell.SetValue(calculatedValue);
                }
                catch (CalculationException)
                {             
                    cell.SetError(CellState.ValueError);
                }
                catch (TokenInvalidValueException exception)
                {
                    throw new SpreadsheetInternallException("Error in cell parsing, some token contains invalid value.", exception);
                }
            }
        }

        private IEnumerable<ICell> SpreadsheetCells()
        {
            for (int col = 0; col < ColumnNumber; col++)
            {
                for (int row = 0; row < RowNumber; row++)
                {
                    yield return Cells[col, row];
                }
            }
        }

        private bool IsCellDependenciesMissied(ICell cell) => cell.CellDependencies.Select(token => GetCellByKey(token.Value)).Any(c => c == null);

        private bool IsCellInSpreadsheet(int colIndex, int rowIndex) => IsInRange(colIndex, 0, ColumnNumber - 1) && IsInRange(rowIndex, 0, RowNumber - 1);

        private static bool IsInRange(int target, int start, int end) => target >= start && target <= end;
    }
}
