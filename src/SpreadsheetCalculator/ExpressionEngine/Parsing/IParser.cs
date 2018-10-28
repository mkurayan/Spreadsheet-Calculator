using SpreadsheetCalculator.ExpressionEngine.Parsing.SyntaxTree;
using SpreadsheetCalculator.ExpressionEngine.Tokenization;
using System.Collections.Generic;

namespace SpreadsheetCalculator.ExpressionEngine.Parsing
{
    /// <summary>
    /// Parse a sequence of tokens into Syntax tree.
    /// </summary>
    internal interface IParser
    {
        INode Parse(IReadOnlyList<Token> tokens);
    }
}
