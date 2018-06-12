using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace SpreadsheetCalculator.SpreadsheetCellToken
{
    class TokenFactory
    {
        private static readonly Regex Pattern = new Regex(@"^[A-Z]\d+$");

        public static Token ParseToken(string token)
        {
            return new Token(Pattern.IsMatch(token) ? TokenType.Reference : TokenType.Expression, token);
        }
    }
}
