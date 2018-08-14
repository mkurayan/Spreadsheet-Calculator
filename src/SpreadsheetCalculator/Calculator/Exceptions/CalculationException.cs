using System;

namespace SpreadsheetCalculator.Calculator
{
    /// <summary>
    /// Use this exception when user input contain invalid mathematical formula.
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
