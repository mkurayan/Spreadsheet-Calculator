using SpreadsheetCalculator.Spreadsheet;

namespace SpreadsheetCalculator.IO.Console
{
    internal class ConsoleOutput : IOutputStreamWriter
    {
        public void Write(IViewSpreadsheet spreadsheet)
        {
            System.Console.WriteLine("Spreadsheet size: {0} x {1}", spreadsheet.ColumnsCount, spreadsheet.RowsCount);

            System.Console.WriteLine(Print.PrintSpreadsheet(spreadsheet));
        }
    }
}
