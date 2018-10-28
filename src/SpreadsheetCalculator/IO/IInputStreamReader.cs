using SpreadsheetCalculator.Spreadsheet;

namespace SpreadsheetCalculator.IO
{
    /// <summary>
    /// Read data from input stream into spreadsheet.
    /// </summary>
    internal interface IInputStreamReader
    {
        IMathSpreadsheet Read();
    }
}
