using System;
using System.Collections.Generic;
using System.Linq;
using SpreadsheetCalculator.Parser;

namespace SpreadsheetCalculator.Calculator
{
    /// <summary>
    /// Evaluates expressions in postfix notation (Reverse Polish Notation).
    /// </summary>
    class PostfixNotationCalculator : IExpressionCalculator
    {
        private readonly Dictionary<string, Func<double, double, double>> MathOperators = new Dictionary<string, Func<double, double, double>>
        {
            ["+"] = (first, second) => first + second,
            ["*"] = (first, second) => first * second,
            ["-"] = (first, second) => second - first,
            ["/"] = (first, second) => second / first
        };

        public double Calculate(IEnumerable<Token> tokens)
        {
            Stack<double> stack = new Stack<double>();

            foreach (var token in tokens)
            {
                switch (token.Type)
                {
                    case TokenType.Number:
                        double parsedValue;
                        try
                        {
                            parsedValue = double.Parse(token.Value);
                        }
                        catch (OverflowException)
                        {
                            // User input might contains to big or to small numbers, we cannot process such values, so we throw MathFormulaParsingException.
                            throw new CalculationException("Formula contains to big or to small number: " + token.Value);
                        }

                        stack.Push(parsedValue);
                        break;
                    case TokenType.Operator:
                        stack.Push(MathOperators[token.Value](stack.Pop(), stack.Pop()));
                        break;
                    default:
                        throw new CalculationInrernalException($"Calculation fail, unsupported token found. Token type: {token.Type}, value : {token.Value}");
                }
            }

            if (stack.Count != 1)
            {
                throw new CalculationInrernalException($"Calculation fail, expected single result, but was: {stack.Count}");
            }

            return stack.Pop();
        }

        public (bool isValid, string error) Validate(IEnumerable<Token> tokens)
        {
            if (tokens.All(token => token.Type == TokenType.Number || token.Type == TokenType.Operator))
            {
                int counter = 0;

                foreach (var token in tokens)
                {
                    if (token.Type == TokenType.Operator)
                    {
                        if (counter < 2)
                        {
                            return (false, "The previous two tokens must be a values.");
                        }

                        counter--;
                    }
                    else
                    {
                        counter++;
                    }
                }

                // If expression is valid, result counter must be equal to 1.
                return counter == 1 ? (true, null) : (false, "Invalid math expression.");
            }

            return (false, "Invalid token in formula.");
        }
    }
}
