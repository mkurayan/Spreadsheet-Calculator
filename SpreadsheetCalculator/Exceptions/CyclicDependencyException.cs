using System;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("SpreadsheetCalculator.Tests")]

namespace SpreadsheetCalculator.Exceptions
{
    class CyclicDependencyException : Exception
    {
        public CyclicDependencyException(string message) : base(message)
        {
        }
    }
}
