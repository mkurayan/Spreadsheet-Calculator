namespace SpreadsheetCalculator.ExpressionEngine.SyntaxTree
{
    interface IDependencyResolver
    {
        double ResolveCellreference(string key);
    }
}
