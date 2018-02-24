using System;

namespace SpreadsheetCalculator.Exceptions
{
    class CyclicDependencyException : Exception
    {
        public CyclicDependencyException(string message) : base(message)
        {
        }
    }
}
