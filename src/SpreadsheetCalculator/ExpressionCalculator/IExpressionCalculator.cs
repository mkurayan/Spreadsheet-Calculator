using SpreadsheetCalculator.ExpressionParser;
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
        double Calculate(IEnumerable<Token> tokens);

        /// <summary>
        /// Validate mathematical expression.
        /// </summary>
        /// <param name="tokens">Expression tokens.</param>
        /// <returns>Validation result true/false, in case if validation fail error field will contains reason.</returns>
        (bool isValid, string error) Validate(IEnumerable<Token> tokens);
    }
}
