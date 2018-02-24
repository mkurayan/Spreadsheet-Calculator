using System;
using System.Collections.Generic;
using System.Linq;
using SpreadsheetCalculator.Calculation;
using SpreadsheetCalculator.Calculation.Tokens;
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

        private List<IToken> _tokens;

        private double? _cachedValue;

        /// <summary>
        /// Create new SpreadsheetCell with specific value.
        /// </summary>
        /// <param name="rowNumber">Cell row number.</param>
        /// <param name="columnNumber">Cell column number.</param>
        /// <param name="value">Cell value.</param>
        public SpreadsheetCell(int rowNumber, int columnNumber)
        {
            Key = string.Format("{0}{1}", AlphabeticHelper.IntToLetter(rowNumber), columnNumber + 1);
        }


        /// <summary>
        /// Set cell value.
        /// </summary>
        /// <param name="value">Cell value.</param>
        /// <param name="getDependency">Dependency resolver.</param>
        public void SetValue(string value, Func<int, int, SpreadsheetCell> getDependency)
        {
            _tokens = new List<IToken>();
            foreach (var t in value.Split(" "))
            {
                var token = TokenFactory.ParseToken(t);

                if (token is ReferenceToken)
                {
                    var rowNumber = AlphabeticHelper.LetterToInt(token.TokenValue[0]);
                    var columnNumber = int.Parse(token.TokenValue.Substring(1)) - 1;

                    ((ReferenceToken) token).SetReference(getDependency(rowNumber, columnNumber));
                }

                _tokens.Add(token);
            }
        }

        /// <summary>
        /// Get current cell dependencies.
        /// </summary>
        public IEnumerable<SpreadsheetCell> GetDependencies()
        {
            return _tokens.OfType<ReferenceToken>().Select(x=> x.Cell);
        }

        /// <summary>
        /// Calculate cell value.
        /// </summary>
        /// <param name="calculator">ICalculator encapsulate calculation strategy (RPN etc.) </param>
        public double Calculate(ICalculator calculator)
        {
            if (!_cachedValue.HasValue)
            {
                _cachedValue = calculator.Calculate(_tokens);
            } 


            return _cachedValue.Value;
        }

        public double? GetCellValue()
        {
            return _cachedValue;
        }
    }
}
