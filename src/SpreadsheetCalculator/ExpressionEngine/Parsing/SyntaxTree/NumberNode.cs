namespace SpreadsheetCalculator.ExpressionEngine.Parsing.SyntaxTree
{
    public class NumberNode : INode
    {
        private readonly double _nodeValue;

        public NumberNode(double value)
        {
            _nodeValue = value;
        }

        public double Evaluate(IDependencyResolver resolver)
        {
            return _nodeValue;
        }
    }
}