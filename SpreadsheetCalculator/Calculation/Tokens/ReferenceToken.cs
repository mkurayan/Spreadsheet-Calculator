using System.Text.RegularExpressions;

namespace SpreadsheetCalculator.Calculation.Tokens
{
    class ReferenceToken : IToken
    {
        public string TokenValue { get; }

        public SpreadsheetCell Cell { get; private set; }

        private static readonly Regex Pattern = new Regex(@"^[A-Z]\d+$");

        public ReferenceToken(string token)
        {
            TokenValue = token;
        }

        public void SetReference(SpreadsheetCell cell)
        {
            if (TokenValue == cell.Key)
            {
                Cell = cell;
            }
        }

        public double GetValue(ICalculator calculator)
        {
            return Cell.Calculate(calculator);
        }

        /// <summary>
        /// Check if token contains reference.
        /// </summary>
        /// <param name="token">Token value</param>
        /// <returns></returns>
        public static bool IsReferenceToken(string token)
        {
            return Pattern.IsMatch(token);
        }
    }

}
