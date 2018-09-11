namespace SpreadsheetCalculator.ExpressionEngine.SyntaxTree
{
    internal interface IDependencyResolver
    {
        double ResolveCellReference(string key);
    }
}
