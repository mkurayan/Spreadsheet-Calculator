using SpreadsheetCalculator.ExpressionEngine.SyntaxTree;
using System;

namespace SpreadsheetCalculator.ExpressionEngine.SyntaxAnalysis.InfixNotation
{
    internal class InfixNotationParser : IParser
    {
        private TokensSequence _tokensSequence;
        
        public INode Parse(Token[] tokens)
        {
            // Empty expression, nothing to parse.
            if (tokens.Length == 0)
            {
                return null;
            }
                        
            _tokensSequence = new TokensSequence(tokens);
               
            // Start expression parsing.
            var expr = ParseAddSubtract();

            // Check that whole expression parsed.
            if (_tokensSequence.Current.Type != TokenType.EndOfExpression)
                throw new SyntaxException($"Unexpected token: {_tokensSequence.Current.Value}");

            return expr;
        } 

        // Parse add/subtract operations
        private INode ParseAddSubtract()
        {
            // Parse the left hand side
            var lhs = ParseMultiplyDivide();

            while (_tokensSequence.Current.Type == TokenType.Add || _tokensSequence.Current.Type == TokenType.Subtract)
            {
                Func<double, double, double> op;
                if (_tokensSequence.Current.Type == TokenType.Add)
                {
                    op = (a, b) => a + b;
                }
                else
                {
                    op = (a, b) => a - b;
                }
          
                // Skip the operator
                _tokensSequence.MoveNext();
                
                // Parse the right hand side of the expression
                var rhs = ParseMultiplyDivide();
                
                lhs = new BinaryOperationNode(lhs, rhs, op);
            }

            return lhs;
        }

        // Parse multiply/divide operations
        private INode ParseMultiplyDivide()
        {
            // Parse the left hand side
            var lhs = ParseUnary();

            while (_tokensSequence.Current.Type == TokenType.Multiply || _tokensSequence.Current.Type == TokenType.Divide)
            {
                Func<double, double, double> op;
                if (_tokensSequence.Current.Type == TokenType.Multiply)
                {
                    op = (a, b) => a * b;
                }
                else
                {
                    op = (a, b) => a / b;
                }
         
                // Skip the operator
                _tokensSequence.MoveNext();
                
                // Parse the right hand side of the expression
                var rhs = ParseUnary();
                
                lhs = new BinaryOperationNode(lhs, rhs, op);
            }

            return lhs;
        }


        // Parse unary operators (we can count only negative operations).
        private INode ParseUnary()
        {
            var negativeCount = 0;
            
            while (_tokensSequence.Current.Type == TokenType.Add || _tokensSequence.Current.Type == TokenType.Subtract)
            {
                if (_tokensSequence.Current.Type == TokenType.Subtract)
                {
                    negativeCount++;
                }

                // Skip
                _tokensSequence.MoveNext();
            }
            
            var node = ParseLeaf();

            if (negativeCount > 0 && negativeCount % 2 != 0)
            {
                node = new UnaryOperationNode(node, n => -n);
            }

            return node;
        }

        // Parse a leaf node
        private INode ParseLeaf()
        {
            INode node;
            
            switch (_tokensSequence.Current.Type)
            {
                case TokenType.Number:
                    node = new NumberNode(double.Parse(_tokensSequence.Current.Value));

                    _tokensSequence.MoveNext();
                    
                    break;
                case TokenType.CellReference:
                    node = new ReferenceNode(_tokensSequence.Current.Value);

                    _tokensSequence.MoveNext();
                        
                    break;
                case TokenType.OpenParenthesis:
                    // Skip '('
                    _tokensSequence.MoveNext();

                    // Parse expression inside the parenthesis.
                    node = ParseAddSubtract();

                    // Check for ')'
                    if (_tokensSequence.Current.Type != TokenType.CloseParenthesis)
                        throw new SyntaxException("Missing close parenthesis");
    
                    // Skip ')'
                    _tokensSequence.MoveNext();
                    
                    break;
                default:
                    throw new SyntaxException($"Unexpected token: {_tokensSequence.Current.Value}");
            }

            return node;
        }
        
        private class TokensSequence
        {
            private readonly Token[] _tokens;
            private int _index;

            private readonly Token _endOfExpression = new Token(TokenType.EndOfExpression, string.Empty);

            public Token Current { get; private set; }

            public TokensSequence(Token[] tokens)
            {
                _tokens = tokens;
                _index = -1;

                MoveNext();
            }

            public void MoveNext()
            {
                if (_index < _tokens.Length - 1)
                {
                    _index++;

                    Current = _tokens[_index];
                }
                else
                {
                    Current = _endOfExpression;    
                }
            }
        }
    }
}
