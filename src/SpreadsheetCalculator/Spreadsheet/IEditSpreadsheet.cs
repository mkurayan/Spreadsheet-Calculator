namespace SpreadsheetCalculator.Spreadsheet
{
    interface IEditSpreadsheet
    {
        /// <summary>
        /// Set spreadsheet size.
        /// </summary>
        /// <param name="columnsCount">Columns in spreadsheet.</param>
        /// <param name="rowsCount">Rows in spreadsheet</param>
        void SetSize(int columnsCount, int rowsCount);

        /// <summary>
        /// Set cell value in spreadsheet.
        /// </summary>
        /// <param name="columnNumber">Column number.</param>
        /// <param name="rowNumber">Row number.</param>
        /// <param name="value">Cell value.</param>
        void SetValue(int columnIndex, int rowIndex, string value);
    }
}
