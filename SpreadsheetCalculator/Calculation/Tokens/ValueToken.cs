using System;

namespace SpreadsheetCalculator.Calculation.Tokens
{
    class ValueToken : IToken
    {
        public string TokenValue { get; }

        public ValueToken(string token)
        {
            TokenValue = token;
        }

        public double GetValue()
        {
            return double.Parse(TokenValue);
        }

        /// <summary>
        /// Check if token contains value.
        /// </summary>
        /// <param name="token">Token value</param>
        /// <returns></returns>
        public static bool IsValueToken(string token)
        {
            Double num = 0;
            bool isDouble = false;

            // Check for empty string.
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            isDouble = Double.TryParse(token, out num);

            return isDouble;
        }
    }
}
