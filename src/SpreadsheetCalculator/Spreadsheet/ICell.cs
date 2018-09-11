namespace SpreadsheetCalculator.Spreadsheet
{
    /// <summary>
    /// Data contract for spreadsheet cells.
    /// </summary>
    internal interface ICell
    {
        /// <summary>
        /// Current cell state.
        /// </summary>
        CellState CellState { get; }

        /// <summary>
        /// Cell value.
        /// </summary>
        double? CellValue { get; }
    }
}