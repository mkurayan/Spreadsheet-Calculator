namespace SpreadsheetCalculator.ExpressionEngine.Tokenization
{
    /// <summary>
    /// Convert a sequence of characters into a sequence of tokens.
    /// </summary>
    internal interface ITokenizer
    {
        Token[] Tokenize(string text);
    }
}
