namespace SpreadsheetCalculator.Spreadsheet
{
    interface IEditSpreadsheet
    {
        /// <summary>
        /// Set spreadsheet size.
        /// </summary>
        /// <param name="columnsCount">Columns in spreadsheet.</param>
        /// <param name="rowsCount">Rows in spreadsheet.</param>
        void SetSize(int columnsCount, int rowsCount);

        /// <summary>
        /// Set cell value.
        /// </summary>
        /// <param name="column">Column number.</param>
        /// <param name="row">Row number.</param>
        /// <param name="value">Cell value.</param>
        void SetValue(int column, int row, string value);

        /// <summary>
        /// Set cell value.
        /// </summary>
        /// <param name="key">Cell position.</param>
        void SetValue(string key, string value);
    }
}
