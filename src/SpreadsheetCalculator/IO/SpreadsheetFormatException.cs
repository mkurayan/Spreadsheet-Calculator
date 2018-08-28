using System;
using System.Collections.Generic;
using System.Text;

namespace SpreadsheetCalculator.IO
{
    class SpreadsheetFormatException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the SpreadsheetFormatException class with
        /// a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public SpreadsheetFormatException(string message) : base(message)
        {
        }
    }
}
