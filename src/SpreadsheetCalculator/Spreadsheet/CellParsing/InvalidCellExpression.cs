using SpreadsheetCalculator.ExpressionEngine.SyntaxTree;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpreadsheetCalculator.Spreadsheet.CellParsing
{
    internal class InvalidCellExpression : ICellExpression
    {
        public bool IsValid => false;

        public bool IsEmpty => false;

        public IEnumerable<string> CellReferences => Enumerable.Empty<string>();

        public double Calculate(IDependencyResolver resolver)
        {
            throw new InvalidOperationException("Cannot calculate invalid math expression.");
        }
    }
}
