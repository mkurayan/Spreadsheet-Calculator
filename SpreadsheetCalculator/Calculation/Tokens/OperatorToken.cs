using System.Linq;

namespace SpreadsheetCalculator.Calculation.Tokens
{
    class OperatorToken : IToken
    {
        public string TokenValue { get; }

        private static readonly string[] AvailableOperatos = { "+", "-", "/", "*", "++", "--" };

        public OperatorToken(string str)
        {
            TokenValue = str;
        }

        /// <summary>
        /// Check if token contains operator.
        /// </summary>
        /// <param name="token">Token value</param>
        /// <returns></returns>
        public static bool IsOperatorToken(string token)
        {
            return AvailableOperatos.Contains(token);
        }
    }

}
