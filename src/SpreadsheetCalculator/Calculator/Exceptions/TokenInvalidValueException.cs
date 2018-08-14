using System;
using System.Collections.Generic;
using System.Text;

namespace SpreadsheetCalculator.Calculator
{
    /// <summary>
    /// Use this exception in case if there is internal problem with token parsing.
    /// </summary>
    class TokenInvalidValueException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the InvalidTokenException class with
        /// a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public TokenInvalidValueException(string message) : base(message)
        {
        }
    }
}
