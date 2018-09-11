namespace SpreadsheetCalculator.Spreadsheet
{
    internal interface IViewSpreadsheet
    {
        /// <summary>
        /// Gets the number of columns in spreadsheet.
        /// </summary>
        int ColumnsCount { get; }

        /// <summary>
        /// Gets the number of rows in spreadsheet.
        /// </summary>
        int RowsCount { get; }

        /// <summary>
        /// Get value from spreadsheet cell.
        /// </summary>
        /// <param name="column">Column index in spreadsheet.</param>
        /// <param name="row">Row index in spreadsheet.</param>
        string GetValue(int column, int row);
    }
}
