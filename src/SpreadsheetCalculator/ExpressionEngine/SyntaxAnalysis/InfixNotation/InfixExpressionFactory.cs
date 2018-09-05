namespace SpreadsheetCalculator.ExpressionEngine.SyntaxAnalysis.InfixNotation
{
    class InfixExpressionFactory : IExpressionFactory
    {
        public IParser CreateParser()
        {
            return new InfixNotationParser();
        }

        public ITokenizer CreateTokenizer()
        {
            return new InfixNotationTokenizer();
        }
    }
}
