using System;
using System.Linq;
using System.Collections.Generic;
using SpreadsheetCalculator.Spreadsheet;

namespace SpreadsheetCalculator.ExpressionParser
{
    /// <summary>
    /// Parse string to tokens.
    /// </summary>
    class StringParser : IStringParser
    {
        private HashSet<string> Operators = new HashSet<string> { "+", "-", "*", "/" };

        public IEnumerable<Token> Parse(string input) => input
            .Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(token => new Token(GetTokenType(token), token));

        private TokenType GetTokenType(string token)
        {
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

            if (CellPosition.IsCellPosition(token))
            {
                return TokenType.CellReference;
            }

            return TokenType.Unknown;
        }
    }
}
