namespace SpreadsheetCalculator.ExpressionEngine.SyntaxAnalysis
{
    interface IExpressionFactory
    {
        IParser CreateParser();
        ITokenizer CreateTokenizer();
    }
}
