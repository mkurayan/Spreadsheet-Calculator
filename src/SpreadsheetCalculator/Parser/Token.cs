namespace SpreadsheetCalculator.Parser
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

        public override string ToString() => $"{Type}: {Value}";
    }
}
