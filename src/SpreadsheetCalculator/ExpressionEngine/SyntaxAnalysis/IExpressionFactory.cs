namespace SpreadsheetCalculator.ExpressionEngine.SyntaxAnalysis
{
    internal interface IExpressionFactory
    {
        IParser CreateParser();
        ITokenizer CreateTokenizer();
    }
}
