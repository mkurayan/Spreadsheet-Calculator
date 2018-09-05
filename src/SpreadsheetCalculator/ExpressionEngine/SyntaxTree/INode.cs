namespace SpreadsheetCalculator.ExpressionEngine.SyntaxTree
{
    interface INode
    {
        double Evaluate(IDependencyResolver resolver);
    }
}
