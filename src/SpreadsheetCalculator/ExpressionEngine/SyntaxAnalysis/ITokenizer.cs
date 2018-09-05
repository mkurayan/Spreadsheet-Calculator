using System.Collections.Generic;


namespace SpreadsheetCalculator.ExpressionEngine.SyntaxAnalysis
{
    interface ITokenizer
    {
        Token[] Tokenize(string text);
    }
}
