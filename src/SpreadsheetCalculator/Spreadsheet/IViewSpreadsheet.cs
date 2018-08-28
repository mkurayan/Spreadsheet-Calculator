namespace SpreadsheetCalculator.Spreadsheet
{
    interface IViewSpreadsheet
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
        /// <param name="columnIndex">Column index in spreadsheet.</param>
        /// <param name="rowNumber">Row index in spreadsheet.</param>
        string GetValue(int columnIndex, int rowIndex);
    }
}
