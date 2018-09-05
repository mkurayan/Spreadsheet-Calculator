using System.Collections.Generic;

namespace SpreadsheetCalculator.ExpressionEngine.SyntaxAnalysis.InfixNotation
{
    class InfixNotationTokenizer : Tokenizer
    { 
        private static readonly Dictionary<char, TokenType> Map = new Dictionary<char, TokenType>()
        {
            ['+'] = TokenType.Add,
            ['*'] = TokenType.Multiply,
            ['-'] = TokenType.Subtract,
            ['/'] = TokenType.Divide,
            ['('] = TokenType.OpenParenthesis,
            [')'] = TokenType.CloseParenthesis
        };
       
        protected override Dictionary<char, TokenType> SymbolsMap => Map;
    }
}
