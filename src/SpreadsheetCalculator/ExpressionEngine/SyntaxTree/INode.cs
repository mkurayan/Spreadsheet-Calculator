namespace SpreadsheetCalculator.ExpressionEngine.SyntaxTree
{
    public interface INode
    {
        double Evaluate(IDependencyResolver resolver);
    }
}
