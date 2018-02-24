using System;
using System.Collections.Generic;
using SpreadsheetCalculator.Calculation;
using SpreadsheetCalculator.Calculation.Tokens;

namespace SpreadsheetCalculator.RPN
{
    class RpnCalculator : ICalculator
    {
        public double Calculate(IEnumerable<IToken> rpnTokens)
        {
            Stack<double> stack = new Stack<double>();
           
            foreach (var token in rpnTokens)
            {
                double number = 0;
                if (token is OperatorToken)
                {
                    
                    switch (token.TokenValue)
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
                            throw new ArgumentException(string.Format("Unknown operator {0}", token.TokenValue));
                    }
                }
                else if (token is ValueToken)
                {
                    stack.Push(((ValueToken)token).GetValue());
                }
                else if (token is ReferenceToken)
                {
                    stack.Push(((ReferenceToken)token).GetValue(this));
                }
            }

            return stack.Pop();
        }
    }
}
