using System;
using System.Collections.Generic;
using System.Linq;
using SpreadsheetCalculator.Cells;
using SpreadsheetCalculator.ExpressionCalculator;
using SpreadsheetCalculator.Utils;

namespace SpreadsheetCalculator
{
    /// <summary>
    /// Spreadsheet of N x M size.
    /// </summary>
    class Spreadsheet
    {
        /// <summary>
        /// Define maximum possible row numbers in Spreadsheet.
        /// </summary>
        private const int MaxRowNumber = 1000000;

        /// <summary>
        /// Define maximum column numbers in Spreadsheet.
        /// Currently it limited because for columns we use single letter [A-Z].
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
        private ISpreadsheetCell[,] Cells { get; }

        /// <summary>
        /// Spreadsheet cell evaluator.
        /// </summary>
        private IExpressionCalculator ExpressionCalculator;

        /// <summary>
        /// Create new Spreadsheet. 
        /// </summary>
        /// <param name="rowNumber">Spreadsheet rows count.</param>
        /// <param name="columnNumber">Spreadsheet columns count.</param>
        /// <param name="evaluator">Spreadsheet cell evaluator.</param>
        public Spreadsheet(int rowNumber, int columnNumber, IExpressionCalculator calculator)
        {
            if (!IsInRange(rowNumber, 1, MaxRowNumber))
            {
                throw new ArgumentException("Invalid row number.");
            }

            if (!IsInRange(columnNumber, 1, MaxColumnNumber))
            {
                throw new ArgumentException("Invalid column number.");
            }

            RowNumber = rowNumber;
            ColumnNumber = columnNumber;

            ExpressionCalculator = calculator ?? throw new ArgumentException("ExpressionCalculator is null.");

            Cells = new ISpreadsheetCell[rowNumber, columnNumber];
        }

        /// <summary>
        /// Set concrete cell in spreadsheet.
        /// </summary>
        /// <param name="rowNumber">Cell row number.</param>
        /// <param name="columnNumber">Cell column number.</param>
        /// <param name="value">Cell value.</param>
        public void SetCell(int rowNumber, int columnNumber, string value)
        {
            Cells[rowNumber, columnNumber] = new SpreadsheetCell(value);
        }

        /// <summary>
        /// Get concrete cell value from spreadsheet.
        /// </summary>
        /// <param name="rowNumber">Cell row number.</param>
        /// <param name="columnNumber">Cell column number.</param>
        /// <param name="value">Cell value.</param>
        public string GetCell(int rowNumber, int columnNumber)
        {
            return Cells[rowNumber, columnNumber].ToString();
        }


        /// <summary>
        /// Process Spreadsheet data.
        /// </summary>
        public void Calculate()
        {
            var sortedCells = TopologicalSort.Sort(
                SpreadsheetCells(),
                cell => cell.CellDependencies
                    .Select(cellRef => GetCellByKey(cellRef.Value))
                    .Where(reff => reff != null)
            );

            foreach (var cell in sortedCells)
            {
                // Check dependencies state.
                var reffSummary = GetCellReferencesSummaryState(cell);

                if (reffSummary == CellState.Fulfilled)
                {
                    CalculateCell(cell);
                }
                else
                {
                    cell.SetError(reffSummary);
                }
            }
        }

        private static bool IsInRange(int target, int start, int end)
        {
            return target >= start && target <= end;
        }

        private ISpreadsheetCell GetCellByKey(string key)
        {
            // Convert alphabetic character to index.
            int rowNumber = key[0] - 65;

            if (int.TryParse(key.Substring(1), out int columnNumber))
            {
                // Check boundaries.
                if (IsInRange(rowNumber, 0, RowNumber) && IsInRange(columnNumber, 0, ColumnNumber))
                {
                    return Cells[rowNumber, columnNumber];
                }
            }

            return null;
        }

        private IEnumerable<ISpreadsheetCell> SpreadsheetCells()
        {
            for (int x = 0; x < RowNumber; x++)
            {
                for (int y = 0; y < ColumnNumber; y++)
                {
                    yield return Cells[x, y];
                }
            }
        }

        private CellState GetCellReferencesSummaryState(ISpreadsheetCell cell)
        {
            foreach (CellToken cellReff in cell.CellDependencies)
            {
                var reff = GetCellByKey(cellReff.Value);

                if (reff == null)
                {
                    // Reference to not existed cell.
                    return CellState.ValueError;
                }
                else if (reff.CellState != CellState.Fulfilled)
                {
                    return reff.CellState;
                }
            }

            return CellState.Fulfilled;
        }

        private void CalculateCell(ISpreadsheetCell cell)
        {
            if (cell.IsEmpty)
            {
                cell.SetValue(0);
            }
            else
            {
                var cellTokens = cell.CellTokens.Select(token =>
                {
                    if (token.IsCellReference)
                    {
                        var reff = GetCellByKey(token.Value);

                        if (reff.CellState == CellState.Fulfilled)
                        {
                            return reff.ToString();
                        }
                        else
                        {
                            throw new InvalidOperationException($"Cell reference is in invalid state: {reff.CellState}");
                        }
                    }
                    else
                    {
                        return token.Value;
                    }
                });

                if (ExpressionCalculator.Vaildate(cellTokens))
                {
                    double calculatedValue = ExpressionCalculator.Calculate(cellTokens);

                    cell.SetValue(calculatedValue);
                }
                else
                {
                    cell.SetError(CellState.ValueError);
                }
            }
        }
    }
}
