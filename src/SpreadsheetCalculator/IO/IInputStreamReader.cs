using SpreadsheetCalculator.Spreadsheet;

namespace SpreadsheetCalculator.IO
{
    /// <summary>
    /// Read data from input stream into spreadsheet.
    /// </summary>
    interface IInputStreamReader
    {
        void Read(IEditSpreadsheet spreadsheet);
    }
}
