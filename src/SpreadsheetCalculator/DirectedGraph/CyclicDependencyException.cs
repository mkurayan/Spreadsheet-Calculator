using System;

namespace SpreadsheetCalculator.DirectedGraph
{
    /// <summary>
    /// Use this exception when cyclic dependencies found. 
    /// </summary>
    internal class CyclicDependencyException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the CyclicDependencyException class with
        /// a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public CyclicDependencyException(string message) : base(message)
        {
        }
    }
}
