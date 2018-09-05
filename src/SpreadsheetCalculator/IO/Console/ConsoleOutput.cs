using SpreadsheetCalculator.Spreadsheet;
using System;

namespace SpreadsheetCalculator.IO
{
    class ConsoleOutput : IOutputStreamWriter
    {
        private readonly Print _print = new Print();

        public void Write(IViewSpreadsheet spreadsheet)
        {
            Console.WriteLine("Spreadsheet size: {0} x {1}", spreadsheet.ColumnsCount, spreadsheet.RowsCount);

            Console.WriteLine(_print.PrintSpreadsheet(spreadsheet));
        }
    }
}
