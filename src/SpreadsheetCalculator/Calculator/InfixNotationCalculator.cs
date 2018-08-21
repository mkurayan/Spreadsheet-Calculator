using System;
using System.Collections.Generic;
using System.Linq;
using SpreadsheetCalculator.Parser;

namespace SpreadsheetCalculator.Calculator
{
    /// <summary>
    /// Evaluates expressions in infix notation.
    /// Use shunting-yard algorithm for parsing infix mathematical expressions (the idea is to convert infix notation to postfix and then evaluate result).
    /// </summary>
    class InfixNotationCalculator : IExpressionCalculator
    {
        /// <summary>
        /// We use this postfix calculator for evaluating postfix expression which we get after parsing original infix expression.
        /// </summary>
        private PostfixNotationCalculator Calculator { get; }

        /// <summary>
        /// Dictionary with all operators precedence.
        /// </summary>
        private IDictionary<string, int> OperatorsPrecedence = new Dictionary<string, int>
        {
            ["+"] = 1,
            ["-"] = 1,
            ["*"] = 2,
            ["/"] = 2
        };

        /// <summary>
        /// Collection of validators for mathematical expressions.
        /// </summary>
        private IEnumerable<Func<IEnumerable<Token>, (bool, string)>> FormulaValidators { get; }

        public InfixNotationCalculator()
        {
            Calculator = new PostfixNotationCalculator();

            FormulaValidators = new Func<IEnumerable<Token>, (bool, string)>[] {
                TokenTypeCheck,
                ParenthesesCheck,
                TokenSequenceCheck
            };
        }

        public double Calculate(IEnumerable<Token> tokens)
        {
            var parsedPostfixNotation = ShuntingYard(tokens);

            return Calculator.Calculate(parsedPostfixNotation);
        }

        public (bool isValid, string error) Validate(IEnumerable<Token> tokens)
        {
            foreach (var validator in FormulaValidators)
            {
                (bool isValid, string error) = validator(tokens);

                if (!isValid)
                {
                    // If any error found, stop further validation process and return error;
                    return (isValid, error);
                }
            }

            // No errors found, math expression valid.
            return (true, null); 
        }

        /// <summary>
        /// Convert a mathematical expression from infix (standard notation) to postfix (reverse polish notation). 
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        private IEnumerable<Token> ShuntingYard(IEnumerable<Token> tokens)
        {
            var result = new List<Token>();
            var stack = new Stack<Token>();

            foreach (var token in tokens)
            {
                switch (token.Type)
                {
                    case TokenType.Number:
                        result.Add(token);
                        break;
                    case TokenType.Operator:
                        while (stack.Count > 0 && stack.Peek().Type == TokenType.Operator && OperatorsPrecedence[token.Value] < OperatorsPrecedence[stack.Peek().Value])
                        {
                            result.Add(stack.Pop());
                        }

                        stack.Push(token);
                        break;
                    case TokenType.RoundBracket:
                        if (token.Value == "(")
                        {
                            stack.Push(token);
                        }
                        else if (token.Value == ")")
                        {
                            while (stack.Peek().Value != "(")
                            {
                                result.Add(stack.Pop());
                            }

                            stack.Pop();
                        }
                        else
                        {
                            throw new CalculationInrernalException($"Shunting Yard fail, invalid value in token found, token must contains round bracket, current symbol: {token.Value}");
                        }
                        break;
                    default:
                        throw new CalculationInrernalException($"Shunting Yard fail, unsupported token found. Token type: {token.Type}, value : {token.Value}");
                }
            }

            while (stack.Any())
            {
                result.Add(stack.Pop());
            }

            return result;
        }

        private static (bool, string) ParenthesesCheck(IEnumerable<Token> tokens)
        {
            var stack = new Stack<Token>();

            foreach (var token in tokens)
            {
                if (token.Type == TokenType.RoundBracket)
                {
                    if (token.Value == "(")
                    {
                        stack.Push(token);
                    }
                    else if (token.Value == ")")
                    {
                        if (stack.Count > 0)
                        {
                            stack.Pop();
                        }
                        else
                        {
                            return (false, "Parentheses do not match");
                        }
                    }
                    else
                    {
                        throw new CalculationInrernalException($"Validation fail, invalid value in token found, token must contains round bracket, current symbol: {token.Value}");
                    }
                }
            }

            return stack.Count == 0 ? (true, null) : (false, "Parentheses do not match");
        }

        private static (bool, string) TokenTypeCheck(IEnumerable<Token> tokens)
        {
            if (tokens.All(token =>
                 token.Type == TokenType.Number ||
                 token.Type == TokenType.Operator ||
                 token.Type == TokenType.RoundBracket
            ))
            {
                return (true, null);
            }
            else
            {
                return (false, "Invalid token in expression");
            }

        }

        private static (bool, string) TokenSequenceCheck(IEnumerable<Token> tokens)
        {
            var enumerator = tokens.GetEnumerator();

            if (enumerator.MoveNext())
            {
                Token current = enumerator.Current;

                if (!TokenIsNumberOrParenthesis(current))
                {
                    return (false, "Math expression starts with invalid token");
                }

                while (enumerator.MoveNext())
                {
                    var next = enumerator.Current;

                    if (!TokenSequenceHelper.IsTokensСanFollowOneAnother(current, next))
                    {
                        return (false, "Math expression contains illegal tokens sequence");
                    }

                    current = next;
                }

                if (!TokenIsNumberOrParenthesis(current))
                {
                    return (false, "Math expression ends with invalid token");
                }
            }

            return (true, null);
        }

        private static bool TokenIsNumberOrParenthesis(Token token) => token.Type == TokenType.Number || token.Type == TokenType.RoundBracket;


        /// <summary>
        /// Encapsulate set of validations rules for tokes sequence check.
        /// </summary>
        private class TokenSequenceHelper
        {
            private static Dictionary<Symbols, Symbols> validationRules = new Dictionary<Symbols, Symbols>()
            {
                [Symbols.Number] = Symbols.Operator | Symbols.CloseParenthesis,
                [Symbols.Operator] = Symbols.Number | Symbols.OpenParenthesis,
                [Symbols.OpenParenthesis] = Symbols.Number | Symbols.OpenParenthesis,
                [Symbols.CloseParenthesis] = Symbols.Operator | Symbols.CloseParenthesis
            };

            /// <summary>
            /// Check if token can follow another token.
            /// </summary>
            /// <param name="current">Current token.</param>
            /// <param name="next">Next token (after current token)</param>
            /// <returns>Validation result, true if tokens sequence valid, otherwise false</returns>
            public static bool IsTokensСanFollowOneAnother(Token current, Token next)
            {
                return (validationRules[TokenToSymbol(current)] & TokenToSymbol(next)) != Symbols.None;
            }

            private static Symbols TokenToSymbol(Token token)
            {
                switch (token.Type)
                {
                    case TokenType.Number:
                        return Symbols.Number;
                    case TokenType.Operator:
                        return Symbols.Operator;
                    case TokenType.RoundBracket:
                        if (token.Value == "(")
                        {
                            return Symbols.OpenParenthesis;
                        }
                        else
                        {
                            return Symbols.CloseParenthesis;
                        }
                    default:
                        throw new NotSupportedException($"Token sequence check fail, unsupported token found. Token type: {token.Type}, value : {token.Value}");
                }
            }

            [Flags]
            private enum Symbols
            {
                None = 0,
                Number = 1,
                Operator = 2,
                OpenParenthesis = 4,
                CloseParenthesis = 8
            }
        }
    }
}
