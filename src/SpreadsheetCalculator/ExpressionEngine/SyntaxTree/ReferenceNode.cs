namespace SpreadsheetCalculator.ExpressionEngine.SyntaxTree
{
    public class ReferenceNode: INode
    {
        private readonly string _cellReference;

        public ReferenceNode(string reference)
        {
            _cellReference = reference;
        }

        public double Evaluate(IDependencyResolver resolver)
        {
            return resolver.ResolveCellReference(_cellReference);
        }    
    }
}