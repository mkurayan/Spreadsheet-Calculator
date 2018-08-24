using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SpreadsheetCalculator.ExpressionParser
{
    /// <summary>
    /// Parse string to tokens.
    /// </summary>
    class StringParser : IStringParser
    {
        // This pattern check if token is reference to another cell in spreadsheet.
        private static readonly Regex CellReferencePattern = new Regex(@"^[A-Z]\d+$");

        private HashSet<string> Operators = new HashSet<string> { "+", "-", "*", "/" };

        public IEnumerable<Token> Parse(string input) => input
            .Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(token => new Token(GetTokenType(token), token));

        private TokenType GetTokenType(string token)
        {
            if (CellReferencePattern.IsMatch(token))
            {
                return TokenType.CellReference;
            }

            if (token == "(" || token == ")")
            {
                return TokenType.RoundBracket;
            }

            if (Operators.Contains(token))
            {
                return TokenType.Operator;
            }

            if (double.TryParse(token, out double value))
            {
                return TokenType.Number;
            }

            return TokenType.Unknown;
        }
    }
}
