namespace SpreadsheetCalculator.ExpressionEngine.Tokenization
{
    internal interface ITokenizer
    {
        Token[] Tokenize(string text);
    }
}
