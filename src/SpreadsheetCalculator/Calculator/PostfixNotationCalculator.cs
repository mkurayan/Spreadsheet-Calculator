using System;
using System.Collections.Generic;
using SpreadsheetCalculator.DirectedGraph;
using SpreadsheetCalculator.Parser;

namespace SpreadsheetCalculator.Calculator
{
    /// <summary>
    /// Evaluates expressions in postfix notation (Reverse Polish Notation).
    /// </summary>
    class PostfixNotationCalculator : IExpressionCalculator
    {
        private readonly Dictionary<string, Action<Stack<double>>> MathFunctions = new Dictionary<string, Action<Stack<double>>>
        {
            ["+"] = (stack) => stack.Push(stack.Pop() + stack.Pop()),
            ["*"] = (stack) => stack.Push(stack.Pop() * stack.Pop()),
            ["-"] = (stack) =>
            {
                double number = stack.Pop();
                stack.Push(stack.Pop() - number);
            },
            ["/"] = (stack) =>
            {
                double number = stack.Pop();
                stack.Push(stack.Pop() / number);
            }
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
                            parsedValue = Double.Parse(token.Value);
                        }
                        catch (OverflowException)
                        {
                            // User input might contains to big or to small numbers, we cannot process such values, so we throw MathFormulaParsingException.
                            throw new CalculationException("Formula contains to big or to small number: " + token.Value);
                        }
                        catch
                        {
                            // Internal code error!!!
                            // Token with 'Number' type should never contains null or string in invalid format.
                            throw new TokenInvalidValueException("Token with 'Number' type contains illegal value: " + token.Value);
                        }

                        stack.Push(parsedValue);
                        break;
                    case TokenType.Operator:
                        if (token.Value == null || !MathFunctions.ContainsKey(token.Value))
                        {
                            // Internal code error!!!
                            // Implementation for this operation missed or operator invalid itself.
                            throw new TokenInvalidValueException("Unsupported operator: " + token.Value);
                        }

                        // Currently we support only binary operations, so stack already must contains at least 2 elements for calculations.
                        if (stack.Count < 2)
                        {
                            // User input contains invalid expression, we cannot calculate this formula.
                            throw new CalculationException("Invalid math expression.");
                        }

                        MathFunctions[token.Value](stack);
                        break;
                    default:
                        // We should pass into calculator only Numbers and Operators.
                        throw new CalculationException("Illegal token: " + token.Value);
                }
            }

            if (stack.Count != 1)
            {
                // User input contains invalid expression, we cannot calculate this formula.
                throw new CalculationException("Invalid math expression.");
            }

            return stack.Pop();
        }
    }
}
