namespace SpreadsheetCalculator.ExpressionEngine.SyntaxTree
{
    internal interface INode
    {
        double Evaluate(IDependencyResolver resolver);
    }
}
