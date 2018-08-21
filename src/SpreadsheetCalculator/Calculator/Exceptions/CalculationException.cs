using System;

namespace SpreadsheetCalculator.Calculator
{
    /// <summary>
    /// Throw this exception when there is a problem with user input and we cannot calculate cell value.
    /// This is not a critical exception, we can handle it and report about error.
    /// </summary>
    class CalculationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the CalculationException class with
        /// a specified error code.
        /// </summary>
        /// <param name="errorCode">The message that describes the error</param>
        public CalculationException(string message) : base(message)
        {
        }
    }
}
