using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SpreadsheetCalculator.ExpressionEvaluator;

namespace SpreadsheetCalculator
{
    /// <summary>
    /// Concrete cell in the spreadsheet.
    /// </summary>
    class SpreadsheetCell
    {
        /// <summary>
        /// Calculated cell value.
        /// </summary>
        public double? CalculatedValue { get; private set; }

        private IEnumerable<Token> _tokens;

        /// <summary>
        /// Create new SpreadsheetCell.
        /// </summary>
        /// <param name="rowNumber">Cell row number.</param>
        /// <param name="columnNumber">Cell column number.</param>
        /// <param name="value">Cell value.</param>
        public SpreadsheetCell(string value)
        {
            _tokens = value.Split(" ")
                .Select(t => new Token(t))
                .ToList();
        }

        public IEnumerable<string> GetCellDependencies()
        {
            return _tokens
                .Where(t => t.IsCellReference)
                .Select(t => t.Value);
        }

        public void CalculateCell(IExpressionEvaluator calculator, Func<string, double?> cellDependenciesResolver)
        {
            var strTokens = _tokens.Select(token =>
            {
                if (token.IsCellReference)
                {
                    return cellDependenciesResolver(token.Value).ToString();
                }
                else
                {
                    return token.Value;
                }
            });

            CalculatedValue = calculator.Evaluate(strTokens);
        }
  
        /// <summary>
        /// Represent single token in spreadsheet cell.
        /// </summary>
        class Token
        {
            // This pattern check if token is reference to another cell in spreadsheet.
            private static readonly Regex CellReferencePattern = new Regex(@"^[A-Z]\d+$");

            public string Value { get; }

            public bool IsCellReference { get; }

            public Token(string token)
            {
                Value = token;
                IsCellReference = CellReferencePattern.IsMatch(token);
            }
        }
    }
}
