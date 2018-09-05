using System;
using System.Collections.Generic;
using System.Linq;

using SpreadsheetCalculator.DirectedGraph;
using SpreadsheetCalculator.ExpressionEngine.SyntaxAnalysis;
using SpreadsheetCalculator.Spreadsheet.CellParsing;

namespace SpreadsheetCalculator.Spreadsheet
{
    /// <summary>
    /// Spreadsheet of N x M size.
    /// </summary>
    class MathSpreadsheet : IViewSpreadsheet, IEditSpreadsheet
    {
        // Define maximum possible rows in Spreadsheet.
        private const int MaxRowNumber = 999999;
       
        // Define maximum possible columns in Spreadsheet.
        private const int MaxColumnNumber = 18278; // "ZZZ"

        // Store spreadsheet Cells.
        private Matrix<Cell> Matrix { get; set; }

        // Parse cell text.
        private ICellParser Parser { get; set; }

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
        public MathSpreadsheet(ICellParser parser)
        {
            Parser = parser ?? throw new ArgumentNullException(nameof(parser));
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

            Matrix = new Matrix<Cell>(columnNumber, rowNumber);
        }

        /// <summary>
        /// Implements <see cref="IEditSpreadsheet.SetValue(int, int, string)"/>.
        /// </summary>
        public void SetValue(int column, int row, string value)
        {
            if (Matrix.InMatrix(column, row))
            {
                var parsedExpression = Parser.Parse(value);

                if (!parsedExpression.IsValid)
                {
                    Console.WriteLine($"Invalid expression {new CellPosition(column, row)}: {value}");
                }

                Matrix[column, row] = new Cell(parsedExpression);
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
            IList<Cell> sortedCells;

            try
            {
                sortedCells = TopologicalSort.Sort(
                    Matrix.AsEnumerable(),
                    cell => cell.CellDependencies
                        .Select(cellRef => GetCellByKey(cellRef))
                        .Where(reff => reff != null)
                );
            }
            catch (CyclicDependencyException exception)
            {
                throw new SpreadsheetInternallException("Cannot calculate spreadsheet, there is cyclic dependencies between cells.", exception);
            }

            foreach (var cell in sortedCells)
            {
                Dictionary<string, ICell> cellDependencies = cell.CellDependencies.ToDictionary(cellRef => cellRef, cellRef => (ICell)GetCellByKey(cellRef));

                cell.Calculate(cellDependencies);
            }
        }

        /// <summary>
        /// Get Spreadsheet cell by cell reference.
        /// Convert cell reference to column & row indexes in spreadsheet inner array representation.
        /// </summary>
        /// <param name="key">Cell reference: A1, A2, etc...</param>
        /// <returns><see cref="ICell"/> </returns>
        private Cell GetCellByKey(string key)
        {
            var position = new CellPosition(key);

            // Check that cell reference inside current spreadsheet
            if (Matrix.InMatrix(position.Column, position.Row))
            {
                return Matrix[position.Column, position.Row];
            }

            return null;
        }
    }
}
