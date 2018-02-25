using System.Collections.Generic;

namespace SpreadsheetCalculator.ExpressionEvaluator
{
    interface IExpressionEvaluator
    {
        /// <summary>
        /// Evaluate expression.
        /// </summary>
        /// <param name="tokens">Expression tokens.</param>
        double Evaluate(IEnumerable<string> tokens);
    }
}
