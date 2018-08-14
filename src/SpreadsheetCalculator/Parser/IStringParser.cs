using System.Collections.Generic;

namespace SpreadsheetCalculator.Parser
{
    interface IStringParser
    {
        /// <summary>
        /// Parse input string to tokens.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>Collection of tokens.</returns>
        IEnumerable<Token> Parse(string input);
    }
}
