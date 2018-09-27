namespace SpreadsheetCalculator.ExpressionEngine.Parsing.SyntaxTree
{
    public interface INode
    {
        double Evaluate(IDependencyResolver resolver);
    }
}
