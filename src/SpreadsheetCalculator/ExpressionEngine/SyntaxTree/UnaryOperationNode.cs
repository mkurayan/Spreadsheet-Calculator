using System;

namespace SpreadsheetCalculator.ExpressionEngine.SyntaxTree
{
    public class UnaryOperationNode : INode
    {
        private readonly INode _operandNode;
        
        private readonly Func<double, double> _unaryOperation;
        
        public UnaryOperationNode(INode operandNode, Func<double, double> operation)
        {
            _operandNode = operandNode;
            _unaryOperation = operation;
        }

        public double Evaluate(IDependencyResolver resolver)
        {
            return _unaryOperation(_operandNode.Evaluate(resolver));
        }
    }
}