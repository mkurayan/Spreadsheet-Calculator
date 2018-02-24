using System.Collections.Generic;
using SpreadsheetCalculator.Calculation;
using SpreadsheetCalculator.Utils;

namespace SpreadsheetCalculator
{
    /// <summary>
    /// Spreadsheet of N x M size.
    /// </summary>
    class Spreadsheet
    {
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
        public SpreadsheetCell[,] Cells { get; }

        /// <summary>
        /// Create new Spreadsheet. 
        /// </summary>
        /// <param name="rowNumber">Spreadsheet rows count.</param>
        /// <param name="columnNumber">Spreadsheet columns count.</param>
        public Spreadsheet(int rowNumber, int columnNumber)
        {
            RowNumber = rowNumber;
            ColumnNumber = columnNumber;

            Cells = new SpreadsheetCell[rowNumber, columnNumber];

            for (var i = 0; i < rowNumber; i++)
            {
                for (var j = 0; j < columnNumber; j++)
                {
                    Cells[i, j] = new SpreadsheetCell(i, j);
                }
            }
        }

        /// <summary>
        /// Set concrete cell in spreadsheet.
        /// </summary>
        /// <param name="rowNumber">Cell row number.</param>
        /// <param name="columnNumber">Cell column number.</param>
        /// <param name="value">Cell value.</param>
        public void SetCell(int rowNumber, int columnNumber, string value)
        {
            var cell = Cells[rowNumber, columnNumber];

            cell.SetValue(value, (i, j) => Cells[i, j]);
        }

        /// <summary>
        /// Process Spreadsheet data.
        /// </summary>
        public void Calculate(ICalculator calculator)
        {
            var sort = TopologicalSort.Sort(SpreadsheetCells(), x => x.GetDependencies());

            foreach (var cell in sort)
            {
                cell.Calculate(calculator);
            }
        }

        private IEnumerable<SpreadsheetCell> SpreadsheetCells()
        {
            for (int x = 0; x < RowNumber; x++)
            {
                for (int y = 0; y < ColumnNumber; y++)
                {
                    yield return Cells[x, y];
                }
            }
        }
    }
}
