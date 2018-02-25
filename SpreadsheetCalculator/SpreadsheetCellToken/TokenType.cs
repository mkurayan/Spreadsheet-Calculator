namespace SpreadsheetCalculator.SpreadsheetCellToken
{
    /// <summary>
    /// Available token types in spreadsheet cell.
    /// </summary>
    enum TokenType
    {
        /// <summary>
        /// Token contains mathematic operator or numeric value.
        /// </summary>
        Expression,

        /// <summary>
        /// Token contains reference to another cell.
        /// </summary>
        Reference
    }
}
