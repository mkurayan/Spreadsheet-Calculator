namespace SpreadsheetCalculator.ExpressionEngine.SyntaxTree
{
    public interface IDependencyResolver
    {
        double ResolveCellReference(string key);
    }
}
