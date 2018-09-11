using SpreadsheetCalculator.ExpressionEngine.SyntaxTree;
using System.Collections.Generic;


namespace SpreadsheetCalculator.ExpressionEngine.SyntaxAnalysis
{
    internal interface IParser
    {
        INode Parse(IEnumerable<Token> tokens);
    }
}
