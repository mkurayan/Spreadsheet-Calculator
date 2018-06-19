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

        public bool IsValid(IEnumerable<string> rpnTokens)
        {
            int counter = 0;

            foreach (var token in rpnTokens)
            {
                switch (token)
                {
                    case "*":
                    case "/":
                    case "+":
                    case "-":
                        if (counter < 2)
                        {
                            //The previous two tokens must be a values.
                            return false;
                        }
                       
                        counter--;
                        break;
                    case "--":
                    case "++":
                        if (counter < 1)
                        {
                            //Previous token must be a value.
                            return false;
                        }

                        break;
                    default:
                        if (Double.TryParse(token, out double n))
                        {
                            counter++;
                        }
                        else
                        {
                            //Unknown symbol.
                            return false;
                        }

                        break;
                }
            }

            // If expression is valid, result counter must be equal to 1.
            return counter == 1;
        }
    }
}
