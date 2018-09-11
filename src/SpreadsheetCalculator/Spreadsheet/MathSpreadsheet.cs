using System;
using System.Collections.Generic;
using System.Linq;

using SpreadsheetCalculator.DirectedGraph;
using SpreadsheetCalculator.Spreadsheet.CellParsing;

namespace SpreadsheetCalculator.Spreadsheet
{
    /// <summary>
    /// Spreadsheet of N x M size.
    /// </summary>
    internal class MathSpreadsheet : IViewSpreadsheet, IEditSpreadsheet
    {
        // Define maximum possible rows in Spreadsheet.
        private const int MaxRowNumber = 999999;
       
        // Define maximum possible columns in Spreadsheet.
        private const int MaxColumnNumber = 18278; // "ZZZ"

        // Store spreadsheet Cells.
        private Matrix<Cell> Matrix { get; set; }

        // Parse cell text.
        private ICellParser Parser { get; }

        /// <summary>
        /// Rows count in spreadsheet.
        /// </summary>
        public int RowsCount => Matrix.RowsCount;

        /// <summary>
        /// Columns count in spreadsheet.
        /// </summary>
        public int ColumnsCount => Matrix.ColumnsCount;

        /// <summary>
        /// Created new spreadsheet 
        /// </summary>
        /// <param name="parser"></param>
        public MathSpreadsheet(ICellParser parser)
        {
            Parser = parser ?? throw new ArgumentNullException(nameof(parser));
        }

        /// <summary>
        /// Set spreadsheet size.
        /// </summary>
        /// <param name="columnNumber">Columns number.</param>
        /// <param name="rowNumber">Rows number.</param>
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
        /// Set cell value in spreadsheet.
        /// </summary>
        /// <param name="column">Column number.</param>
        /// <param name="row">Row number.</param>
        /// <param name="value">New cell value.</param>
        public void SetValue(int column, int row, string value)
        {
            if (!Matrix.InMatrix(column, row))
            {
                throw new IndexOutOfRangeException("Requested cell is out of spreadsheet.");
            }
            
            var parsedExpression = Parser.Parse(value);

            if (!parsedExpression.IsValid)
            {
                Console.WriteLine($"Invalid expression {new CellPosition(column, row)}: {value}");
            }

            Matrix[column, row] = new Cell(parsedExpression);
        }

        /// <summary>
        /// Set cell value in spreadsheet.
        /// </summary>
        /// <param name="key">Spreadsheet cell Id. Example: A1, A2, etc.</param>
        /// <param name="value">New cell value.</param>
        public void SetValue(string key, string value)
        {
            var position = new CellPosition(key);

            SetValue(position.Column, position.Row, value);
        }


        /// <summary>
        /// Get cell value from spreadsheet.
        /// </summary>
        /// <param name="column">Column number in spreadsheet.</param>
        /// <param name="row">Row number in spreadsheet.</param>
        /// <returns>Current cell value.</returns>
        public string GetValue(int column, int row)
        {
            if (Matrix.InMatrix(column, row))
            {
                return Matrix[column, row].ToString();
            }

            throw new IndexOutOfRangeException("Requested cell is out of spreadsheet.");   
        }

        /// <summary>
        /// Get cell value from spreadsheet.
        /// </summary>
        /// <param name="key">Spreadsheet cell Id. Example: A1, A2, etc.</param>
        /// <returns>Current cell value.</returns>
        public string GetValue(string key)
        {
            var position = new CellPosition(key);

            return GetValue(position.Column, position.Row);
        }

        /// <summary>
        /// Calculate all cells in spreadsheet.
        /// </summary>
        /// <exception cref="SpreadsheetInternalException">Cyclic dependency found.</exception>
        public void Calculate()
        {
            IList<Cell> sortedCells;

            try
            {
                sortedCells = TopologicalSort.Sort(
                    Matrix.AsEnumerable(),
                    cell => cell.CellDependencies
                        .Select(GetCellByKey)
                        .Where(cellReference => cellReference!= null)
                );
            }
            catch (CyclicDependencyException exception)
            {
                throw new SpreadsheetInternalException("Cannot calculate spreadsheet, there is cyclic dependencies between cells.", exception);
            }

            foreach (var cell in sortedCells)
            {
                var cellDependencies = cell.CellDependencies.ToDictionary(cellRef => cellRef, cellRef => (ICell)GetCellByKey(cellRef));

                cell.Calculate(cellDependencies);
            }
        }
        
        // Get Spreadsheet cell by cell reference.
        // Convert cell reference to column & row indexes in spreadsheet inner representation.
        private Cell GetCellByKey(string key)
        {
            var position = new CellPosition(key);

            // Check that cell reference inside current spreadsheet
            return Matrix.InMatrix(position.Column, position.Row) ? Matrix[position.Column, position.Row] : null;
        }
    }
}
