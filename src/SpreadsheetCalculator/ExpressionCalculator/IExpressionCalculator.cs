using System.Collections.Generic;

namespace SpreadsheetCalculator.ExpressionCalculator
{
    interface IExpressionCalculator
    {
        /// <summary>
        /// Evaluate expression.
        /// </summary>
        /// <param name="tokens">Expression tokens.</param>
        /// <returns>Calculated value.</returns>
        double Calculate(IEnumerable<string> tokens);

        /// <summary>
        /// Check if expression is valid.
        /// </summary>
        /// <param name="tokens">Expression tokens.</param>
        /// <returns>Validation result.</returns>
        bool Vaildate(IEnumerable<string> tokens);
    }
}
