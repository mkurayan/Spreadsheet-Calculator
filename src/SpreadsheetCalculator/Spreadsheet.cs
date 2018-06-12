using System.Collections.Generic;
using System.Linq;
using SpreadsheetCalculator.ExpressionEvaluator;
using SpreadsheetCalculator.SpreadsheetCellToken;
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
        }

        /// <summary>
        /// Set concrete cell in spreadsheet.
        /// </summary>
        /// <param name="rowNumber">Cell row number.</param>
        /// <param name="columnNumber">Cell column number.</param>
        /// <param name="value">Cell value.</param>
        public void SetCell(int rowNumber, int columnNumber, string value)
        {
            Cells[rowNumber, columnNumber] = new SpreadsheetCell(rowNumber, columnNumber, value);
        }

        /// <summary>
        /// Process Spreadsheet data.
        /// </summary>
        public void Calculate(IExpressionEvaluator calculator)
        {
            var sortedCells = TopologicalSort.Sort(
                SpreadsheetCells(), 
                cell => cell.GetTokens()
                    .Where(token => token.Type == TokenType.Reference)
                    .Select(cellRefToken => GetCell(cellRefToken.Value))
            );

            foreach (var cell in sortedCells)
            {
                var strTokens = cell.GetTokens().Select(token =>
                {
                    if (token.Type == TokenType.Reference)
                    {
                        return GetCell(token.Value).CalculatedValue.ToString();
                    }
                    else
                    {
                        return token.Value;
                    }
                });

                cell.CalculatedValue = calculator.Evaluate(strTokens);
            }
        }

        /// <summary>
        /// Get SpreadsheetCell by Key.
        /// </summary>
        /// <param name="key">Key of SpreadsheetCell</param>
        /// <returns></returns>
        private SpreadsheetCell GetCell(string key)
        {
            var rowNumber = AlphabeticHelper.LetterToInt(key[0]);
            var columnNumber = int.Parse(key.Substring(1)) - 1;

            return Cells[rowNumber, columnNumber];
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
