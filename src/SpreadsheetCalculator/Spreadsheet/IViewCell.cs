namespace SpreadsheetCalculator.Spreadsheet
{
    internal interface IViewCell
    {
        /// <summary>
        /// Current cell state.
        /// </summary>
        CellState CellState { get; }

        /// <summary>
        /// Original value from user input.
        /// </summary>
        string OriginalValue { get; }

        /// <summary>
        /// Result of original value evaluation.
        /// </summary>
        string ResultValue { get; }
    }
}