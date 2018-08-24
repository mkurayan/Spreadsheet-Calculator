using System;

namespace SpreadsheetCalculator.ExpressionCalculator
{
    /// <summary>
    /// Throw this exception in case if there is internal problem during calculation process.
    /// This exception should never be silently swallow.
    /// </summary>
    class CalculationInrernalException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the CalculationInrernalException class with
        /// a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public CalculationInrernalException(string message) : base(message)
        {
        }
    }
}
