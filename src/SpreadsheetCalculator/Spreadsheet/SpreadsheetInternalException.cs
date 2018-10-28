using System;

namespace SpreadsheetCalculator.Spreadsheet
{
    internal class SpreadsheetInternalException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the SpreadsheetInternalException class with
        /// a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public SpreadsheetInternalException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the SpreadsheetInternalException class with a specified error
        /// message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception or null if no inner exception is specified.</param>
        public SpreadsheetInternalException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
