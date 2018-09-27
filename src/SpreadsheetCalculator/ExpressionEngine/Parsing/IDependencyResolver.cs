namespace SpreadsheetCalculator.ExpressionEngine.Parsing
{
    public interface IDependencyResolver
    {
        double ResolveCellReference(string key);
    }
}
