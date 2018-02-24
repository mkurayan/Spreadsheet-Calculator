using System.Collections.Generic;
using SpreadsheetCalculator.Calculation.Tokens;

namespace SpreadsheetCalculator.Calculation
{
    interface ICalculator
    {
        /// <summary>
        /// Calculate expression.
        /// </summary>
        /// <param name="tokens">Expression tokens.</param>
        double Calculate(IEnumerable<IToken> tokens);
    }
}
