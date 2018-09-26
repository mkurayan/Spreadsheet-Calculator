using SpreadsheetCalculator.ExpressionEngine.SyntaxTree;
using System;
using System.Collections.Generic;

namespace SpreadsheetCalculator.Spreadsheet.CellParsing
{
    internal class InvalidCellExpression : ICellExpression
    {
        public bool IsValid => false;

        public bool IsEmpty => false;

        public HashSet<string> CellReferences => new HashSet<string>();

        public double Calculate(IDependencyResolver resolver)
        {
            throw new InvalidOperationException("Cannot calculate invalid math expression.");
        }
    }
}
