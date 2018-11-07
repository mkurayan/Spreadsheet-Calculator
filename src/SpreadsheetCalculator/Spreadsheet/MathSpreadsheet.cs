using System;
using System.Collections.Generic;
using System.Linq;
using SpreadsheetCalculator.DirectedGraph;
using SpreadsheetCalculator.ExpressionEngine;
using SpreadsheetCalculator.ExpressionEngine.Parsing;
using SpreadsheetCalculator.ExpressionEngine.Tokenization;
using SpreadsheetCalculator.Utils;

namespace SpreadsheetCalculator.Spreadsheet
{
    /// <summary>
    /// Spreadsheet of N x M size.
    /// </summary>
    internal class MathSpreadsheet : IMathSpreadsheet, IDependencyResolver
    {
        // Define maximum possible rows in Spreadsheet.
        private const int MaxRowNumber = 999999;
       
        // Define maximum possible columns in Spreadsheet.
        private const int MaxColumnNumber = 18278; // "ZZZ"

        // Store spreadsheet Cells.
        private Matrix _matrix;

        private readonly IParser _parser;
        
        private readonly ITokenizer _tokenizer;

        /// <summary>
        /// Rows count in spreadsheet.
        /// </summary>
        public int RowsCount => _matrix.RowsCount;

        /// <summary>
        /// Columns count in spreadsheet.
        /// </summary>
        public int ColumnsCount => _matrix.ColumnsCount;

        /// <summary>
        /// Created new spreadsheet 
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="tokenizer"></param>
        public MathSpreadsheet(IParser parser, ITokenizer tokenizer)
        {
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
            _tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
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

            _matrix = new Matrix(columnNumber, rowNumber);
        }

        /// <summary>
        /// Set cell value in spreadsheet.
        /// </summary>
        /// <param name="column">Column number.</param>
        /// <param name="row">Row number.</param>
        /// <param name="value">New cell value.</param>
        public void SetValue(int column, int row, string value)
        {
            if (!_matrix.InMatrix(column, row))
            {
                throw new IndexOutOfRangeException("Requested cell is out of spreadsheet.");
            }

            // We interpret null as empty string.
            if (value == null)
            {
                value = string.Empty;
            }

            var cell = CreateCell(value);

            if (cell.CellState == CellState.SyntaxError)
            {
                Console.WriteLine($"Invalid expression { CellPosition.CoordinatesToKey(column, row) }: {value}");
            }

            _matrix[column, row] = cell;
        }

        /// <summary>
        /// Set cell value in spreadsheet.
        /// </summary>
        /// <param name="key">Spreadsheet cell Id. Example: A1, A2, etc.</param>
        /// <param name="value">New cell value.</param>
        public void SetValue(string key, string value)
        {
            (var column, var row) = CellPosition.KeyToCordinates(key);

            SetValue(column, row, value);
        }


        /// <summary>
        /// Get cell value from spreadsheet.
        /// </summary>
        /// <param name="column">Column number in spreadsheet.</param>
        /// <param name="row">Row number in spreadsheet.</param>
        /// <returns>Current cell value.</returns>
        public IViewCell GetValue(int column, int row)
        {
            if (_matrix.InMatrix(column, row))
            {
                return _matrix[column, row];
            }

            throw new IndexOutOfRangeException("Requested cell is out of spreadsheet.");
        }

        /// <summary>
        /// Get cell value from spreadsheet.
        /// </summary>
        /// <param name="key">Spreadsheet cell Id. Example: A1, A2, etc.</param>
        /// <returns>Current cell value.</returns>
        public IViewCell GetValue(string key)
        {
            (var column, var row) = CellPosition.KeyToCordinates(key);

            return GetValue(column, row);
        }

        /// <summary>
        /// Get calculated value for cell. If cell not found, null or contains error exception will be thrown.
        /// </summary>
        /// <param name="key">Cell Id.</param>
        /// <returns>Calculated value.</returns>
        public double ResolveCellReference(string key)
        {
            var cell = GetCellByKey(key);

            if (cell != null && cell.CellState == CellState.Calculated && cell.CalculatedValue.HasValue)
            {
                return cell.CalculatedValue.Value;
            }

            throw new SpreadsheetInternalException("Cannot resolve cell reference.");
        }

        /// <summary>
        /// Calculate all cells in spreadsheet.
        /// </summary>
        /// <exception cref="SpreadsheetInternalException">Cyclic dependency found.</exception>
        public void Calculate()
        {
            IList<string> sortedKeys;

            try
            {
                sortedKeys = TopologicalSort.Sort(
                    GetAllKeys(),
                    key => GetCellByKey(key).CellDependencies.Where(d => CellInSpreadsheet(d))
                );
            }
            catch (CyclicDependencyException exception)
            {
                throw new SpreadsheetInternalException($"Cannot calculate spreadsheet, there is cyclic dependencies between cells. {exception.Message}", exception);
            }

            foreach (var key in sortedKeys)
            {
                CalculateCell(GetCellByKey(key));
            }

            // Validate that all cells were calculated.
            foreach (var key in GetAllKeys())
            {
                if (GetCellByKey(key).CellState == CellState.Pending)
                {
                    throw new SpreadsheetInternalException($"The cell {key} was lost during the calculation.");
                }
            }
        }

        private Cell CreateCell(string value)
        {
            Cell cell;

            try
            {
                var tokens = _tokenizer.Tokenize(value);

                var treeTop = _parser.Parse(tokens);

                cell = new Cell(value, tokens, treeTop);
            }
            catch (SyntaxException)
            {
                cell = new Cell(value);
            }

            return cell;
        }

        private void CalculateCell(Cell cell)
        {
            var cellDependencies = cell.CellDependencies.ToDictionary(cellRef => cellRef, cellRef => GetCellByKey(cellRef));

            if (cell.CellState == CellState.Pending)
            {
                if (cellDependencies.Any(c => c.Value == null))
                {
                    cell.SetError(CellState.CellValueError);
                    return;
                }

                if (cellDependencies.Any(c => c.Value.CellState == CellState.Pending))
                {
                    var dependentCellInPendingState = cellDependencies.First(c => c.Value.CellState == CellState.Pending);

                    throw new SpreadsheetInternalException($"Cannot calculate current cell: {cell.OriginalValue}, because cell {dependentCellInPendingState.Key} not calculated yet.");
                }

                if (cellDependencies.Any(c => c.Value.CellState != CellState.Calculated))
                {
                    cell.SetError(CellState.CellValueError);
                    return;
                }

                cell.SetValue(cell.SyntaxTreeTop.Evaluate(this));
            }
        }


        // Get Spreadsheet cell by cell reference.
        // Convert cell reference to column & row indexes in spreadsheet inner representation.
        private Cell GetCellByKey(string key)
        {
            (int column, int row) = CellPosition.KeyToCordinates(key);

            // Check that cell reference inside current spreadsheet
            return _matrix.InMatrix(column, row) ? _matrix[column, row] : null;
        }

        // Check if cell in spreadsheet.
        private bool CellInSpreadsheet(string key)
        {
            (int column, int row) = CellPosition.KeyToCordinates(key);

            return _matrix.InMatrix(column, row);
        }

        // Get plain collection (row by row) of all cells in Spreadsheet.       
        private IEnumerable<string> GetAllKeys()
        {
            for (var column = 1; column <= ColumnsCount; column++)
            {
                for (var row = 1; row <= RowsCount; row++)
                {
                    yield return CellPosition.CoordinatesToKey(column, row);
                }
            }
        }

        /// <summary>
        /// A rectangular array of objects arranged in rows and columns.
        /// </summary>
        private class Matrix
        {
            private Cell[,] Cells { get; }

            public int RowsCount => Cells.GetLength(1);

            public int ColumnsCount => Cells.GetLength(0);

            /// <summary>
            /// Create new matrix with specified size.
            /// </summary>
            /// <param name="columnsCount">Columns count in matrix.</param>
            /// <param name="rowsCount">Rows count in matrix.</param>
            public Matrix(int columnsCount, int rowsCount)
            {
                Cells = new Cell[columnsCount, rowsCount];
            }

            /// <summary>
            /// Check if cell coordinates is inside matrix.
            /// </summary>
            /// <param name="column">Column in matrix.</param>
            /// <param name="row">Row in matrix.</param>
            /// <returns></returns>
            public bool InMatrix(int column, int row)
            {
                return InRange(column - 1, 0, ColumnsCount - 1) && InRange(row - 1, 0, RowsCount - 1);

                bool InRange(int index, int start, int end) => start <= index && index <= end;
            }


            /// <summary>
            /// Define the indexer to allow client code to use [] notation.
            /// </summary>
            /// <param name="column">Column in matrix.</param>
            /// <param name="row">Row in matrix.</param>
            /// <returns></returns>
            public Cell this[int column, int row]
            {
                get => Cells[column - 1, row - 1];
                set => Cells[column - 1, row - 1] = value;
            }
        }
    }
}
