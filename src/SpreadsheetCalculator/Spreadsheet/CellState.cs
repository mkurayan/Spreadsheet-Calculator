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
        /// Cell evaluated and contains correct value.
        /// </summary>
        Fulfilled,

        SyntaxError,

        CellValueError
 
        ///// <summary>
        ///// There's something wrong with the way your formula is typed. 
        ///// Or, there's something wrong with the cells you are referencing." 
        ///// </summary>
        //ValueError,

        ///// <summary>
        ///// Cell contains numeric values that aren’t valid.
        ///// </summary>
        //NumberError
    }
}
