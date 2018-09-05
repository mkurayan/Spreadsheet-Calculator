using System;

namespace SpreadsheetCalculator.ExpressionEngine
{
    /// <summary>
    /// Throw this exception in case if there is a problem with formula syntax.
    /// </summary>
    class SyntaxException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the SyntaxException class.
        /// </summary>
        public SyntaxException() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the SyntaxException class with
        /// a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public SyntaxException(string message) : base(message)
        {
        }
    }
}
