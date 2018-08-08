using System;
using System.Collections.Generic;
using System.Text;

namespace SpreadsheetCalculator.ExpressionCalculator
{
    /// <summary>
    /// Evaluates expressions in infix notation.
    /// Use shunting-yard algorithm for parsing infix mathematical expressions (the idea is to convert infix notation to postfix and then evaluate result).
    /// </summary>
    class InfixNotationCalculator : IExpressionCalculator
    {
        /// <summary>
        /// We use this postfix calculator for evaluating postfix expression which we get after parsing original infix expression.
        /// </summary>
        private PostfixNotationCalculator Calculator { get; }

        public InfixNotationCalculator()
        {
            Calculator = new PostfixNotationCalculator();
        }

        public double Calculate(IEnumerable<string> tokens)
        {
            throw new NotImplementedException();
        }

        public bool Vaildate(IEnumerable<string> tokens)
        {
            throw new NotImplementedException();
        }
    }
}
