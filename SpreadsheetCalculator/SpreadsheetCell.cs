using System.Collections.Generic;
using SpreadsheetCalculator.SpreadsheetCellToken;
using SpreadsheetCalculator.Utils;

namespace SpreadsheetCalculator
{
    /// <summary>
    /// Represent a cell in the spreadsheet.
    /// </summary>
    class SpreadsheetCell
    {
        /// <summary>
        /// Key of SpreadsheetCell.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Calculated cell value.
        /// </summary>
        public double? CalculatedValue { get; set; }

        private List<Token> _tokens;

        /// <summary>
        /// Create new SpreadsheetCell.
        /// </summary>
        /// <param name="rowNumber">Cell row number.</param>
        /// <param name="columnNumber">Cell column number.</param>
        /// <param name="value">Cell value.</param>
        public SpreadsheetCell(int rowNumber, int columnNumber, string value)
        {
            Key = string.Format("{0}{1}", AlphabeticHelper.IntToLetter(rowNumber), columnNumber + 1);

            _tokens = new List<Token>();
            foreach (var t in value.Split(" "))
            {
                var token = TokenFactory.ParseToken(t);

                _tokens.Add(token);
            }
        }

        public IEnumerable<Token> GetTokens()
        {
            foreach (Token t in _tokens)
            {
                yield return t;
              
            }
        }       
    }
}
