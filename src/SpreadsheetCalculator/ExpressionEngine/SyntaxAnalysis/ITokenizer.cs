namespace SpreadsheetCalculator.ExpressionEngine.SyntaxAnalysis
{
    internal interface ITokenizer
    {
        Token[] Tokenize(string text);
    }
}
