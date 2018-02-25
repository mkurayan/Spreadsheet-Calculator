namespace SpreadsheetCalculator.SpreadsheetCellToken
{
    /// <summary>
    /// Represent single token in spreadsheet cell.
    /// </summary>
    class Token
    {
        public string Value { get; }

        public TokenType Type { get; }

        public Token(TokenType type, string token)
        {
            Value = token;
            Type = type;
        }
    }
}
