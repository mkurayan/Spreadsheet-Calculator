using SpreadsheetCalculator.ExpressionEngine.SyntaxTree;
using System.Collections.Generic;


namespace SpreadsheetCalculator.ExpressionEngine.SyntaxAnalysis
{
    interface IParser
    {
        INode Parse(IEnumerable<Token> tokens);
    }
}
