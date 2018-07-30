using System;
using System.Runtime.CompilerServices;

namespace SpreadsheetCalculator.Exceptions
{
    class CyclicDependencyException : Exception
    {
        public CyclicDependencyException(string message) : base(message)
        {
        }
    }
}
