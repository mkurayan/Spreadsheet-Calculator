namespace SpreadsheetCalculator.Spreadsheet
{
    internal interface IMathSpreadsheet : IViewSpreadsheet
    {
        /// <summary>
        /// Calculate all cells in spreadsheet.
        /// </summary>
        void Calculate();
    }
}
