namespace SpreadsheetCalculator.ExpressionEngine.SyntaxAnalysis
{
    struct Token
    {
        public TokenType Type { get; }

        public string Value { get; }

        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }

        public Token(TokenType type, char value) : this(type, value.ToString())
        {
        }
    }
}
