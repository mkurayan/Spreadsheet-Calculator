using System.Collections.Generic;

namespace SpreadsheetCalculator.ExpressionEvaluator
{
    interface IExpressionEvaluator
    {
        /// <summary>
        /// Evaluate expression.
        /// </summary>
        /// <param name="tokens">Expression tokens.</param>
        /// <returns>Calculated value.</returns>
        double Evaluate(IEnumerable<string> tokens);

        /// <summary>
        /// Check if expression is valid.
        /// </summary>
        /// <param name="tokens">Expression tokens.</param>
        /// <returns>Validation result.</returns>
        bool VaildateExpression(IEnumerable<string> tokens);
    }
}
