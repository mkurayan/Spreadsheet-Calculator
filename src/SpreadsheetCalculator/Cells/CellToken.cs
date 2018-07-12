using System.Text.RegularExpressions;

namespace SpreadsheetCalculator.Cells
{
    /// <summary>
    /// Represent single token in spreadsheet cell.
    /// </summary>
    class CellToken
    {
        // This pattern check if token is reference to another cell in spreadsheet.
        private static readonly Regex CellReferencePattern = new Regex(@"^[A-Z]\d+$");

        public string Value { get; }

        public bool IsCellReference { get; }

        public CellToken(string token)
        {
            Value = token;
            IsCellReference = CellReferencePattern.IsMatch(token);
        }
    }
}
