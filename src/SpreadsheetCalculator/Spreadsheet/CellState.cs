namespace SpreadsheetCalculator.Spreadsheet
{
    /// <summary>
    /// Describe spreadsheet cell state.
    /// </summary>
    public enum CellState
    {
        /// <summary>
        /// Cell not evaluated yet.
        /// </summary>
        Pending,

        /// <summary>
        /// Cell calculated and contains correct value.
        /// </summary>
        Calculated,

        /// <summary>
        /// Cell formula not valid, there is syntax error in it.
        /// </summary>
        SyntaxError,

        /// <summary>
        /// There's something wrong with the way your formula is typed.
        /// Or, there's something wrong with the cells you are referencing.
        /// </summary>
        CellValueError
    }
}
