using System.Collections.Generic;

namespace SpreadsheetCalculator.ExpressionEngine.SyntaxAnalysis.InfixNotation
{
    internal class InfixExpressionFactory : IExpressionFactory
    {
        public IParser CreateParser()
        {
            return new InfixNotationParser();
        }

        public ITokenizer CreateTokenizer()
        {
            return new Tokenizer(new Dictionary<char, TokenType>
            {
                ['+'] = TokenType.Add,
                ['*'] = TokenType.Multiply,
                ['-'] = TokenType.Subtract,
                ['/'] = TokenType.Divide,
                ['('] = TokenType.OpenParenthesis,
                [')'] = TokenType.CloseParenthesis
            });
        }
    }
}
