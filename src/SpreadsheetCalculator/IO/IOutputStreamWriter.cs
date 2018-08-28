using SpreadsheetCalculator.Spreadsheet;

namespace SpreadsheetCalculator.IO
{
    /// <summary>
    /// Write data from spreadsheet into output stream.
    /// </summary>
    interface IOutputStreamWriter
    {
        void Write(IViewSpreadsheet spreadsheet);
    }
}
