using SpreadsheetCalculator.ExpressionEngine.SyntaxTree;
using System.Collections.Generic;

namespace SpreadsheetCalculator.Spreadsheet.CellParsing
{
    internal interface ICellExpression
    {
        bool IsValid { get; }

        bool IsEmpty { get; }

        HashSet<string> CellReferences { get; }

        double Calculate(IDependencyResolver resolver);
    }
}
