using SpreadsheetCalculator.ExpressionEngine.SyntaxTree;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpreadsheetCalculator.Spreadsheet.CellParsing
{
    interface ICellExpression
    {
        bool IsValid { get; }

        bool IsEmpty { get; }

        IEnumerable<string> CellReferences { get; }

        double Calculate(IDependencyResolver resolver);
    }
}
