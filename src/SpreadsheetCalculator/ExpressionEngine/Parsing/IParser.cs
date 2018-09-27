using SpreadsheetCalculator.ExpressionEngine.Parsing.SyntaxTree;
using SpreadsheetCalculator.ExpressionEngine.Tokenization;

namespace SpreadsheetCalculator.ExpressionEngine.Parsing
{
    internal interface IParser
    {
        INode Parse(Token[] tokens);
    }
}
