using System;

namespace SpreadsheetCalculator.Spreadsheet
{
    class SpreadsheetInternallException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the SpreadsheetInternallException class with
        /// a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public SpreadsheetInternallException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the System.Exception class with a specified error
        /// message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception or null if no inner exception is specified.</param>
        public SpreadsheetInternallException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
