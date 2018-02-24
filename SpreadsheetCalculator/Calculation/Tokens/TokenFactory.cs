using System;

namespace SpreadsheetCalculator.Calculation.Tokens
{
    class TokenFactory
    {
        public static IToken ParseToken(string token)
        {
            if (OperatorToken.IsOperatorToken(token))
            {
                return new OperatorToken(token);
            }
            else if (ValueToken.IsValueToken(token))
            {
                return new ValueToken(token);
            }
            else if (ReferenceToken.IsReferenceToken(token))
            {
                return new ReferenceToken(token);
            }

            throw new ArgumentException(String.Format("Invalid token: {0}", token));
        }
    }
}
