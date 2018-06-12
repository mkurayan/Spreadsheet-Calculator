using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("SpreadsheetCalculator.Tests")]

namespace SpreadsheetCalculator.ExpressionEvaluator
{
    /// <summary>
    /// Evaluates expressions in RPN (Reverse Polish Notation)
    /// </summary>
    class RpnEvaluator : IExpressionEvaluator
    {
        public double Evaluate(IEnumerable<string> rpnTokens)
        {
            Stack<double> stack = new Stack<double>();
           
            foreach (var token in rpnTokens)
            {
                double number = 0;
                switch (token)
                {
                    case "*":
                        stack.Push(stack.Pop() * stack.Pop());
                        break;
                    case "/":
                        number = stack.Pop();
                        stack.Push(stack.Pop() / number);
                        break;
                    case "+":
                        stack.Push(stack.Pop() + stack.Pop());
                        break;
                    case "-":
                        number = stack.Pop();
                        stack.Push(stack.Pop() - number);
                        break;
                    case "--":
                        number = stack.Pop();
                        stack.Push(--number);
                        break;
                    case "++":
                        number = stack.Pop();
                        stack.Push(++number);
                        break;
                    default:
                        stack.Push(Double.Parse(token));
                        break;
                }
            }

            return stack.Pop();
        }
    }
}
