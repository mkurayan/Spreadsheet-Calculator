using System.Collections.Generic;

namespace SpreadsheetCalculator.Tokens
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
