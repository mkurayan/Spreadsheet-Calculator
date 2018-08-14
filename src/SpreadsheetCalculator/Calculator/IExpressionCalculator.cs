using SpreadsheetCalculator.Parser;
using System.Collections.Generic;

namespace SpreadsheetCalculator.Calculator
{
    interface IExpressionCalculator
    {
        /// <summary>
        /// Evaluate expression.
        /// </summary>
        /// <param name="tokens">Expression tokens.</param>
        /// <returns>Calculated value.</returns>
        double Calculate(IEnumerable<Token> tokens);
    }
}
