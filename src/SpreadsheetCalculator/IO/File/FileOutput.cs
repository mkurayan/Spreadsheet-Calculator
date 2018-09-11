using System;
using System.IO;
using SpreadsheetCalculator.Spreadsheet;

namespace SpreadsheetCalculator.IO.File
{
    internal class FileOutput : IOutputStreamWriter
    {
        private string File { get; }

        public FileOutput(string file)
        {
            File = file ?? throw new ArgumentNullException(nameof(file));
        }

        public void Write(IViewSpreadsheet spreadsheet)
        {
            System.Console.WriteLine("Save spreadsheet to file.");

            using (var sOut = new StreamWriter(new FileStream(File, FileMode.OpenOrCreate)))
            {
                sOut.WriteLine("{0} {1}", spreadsheet.ColumnsCount, spreadsheet.RowsCount);

                for (var rowNumber = 1; rowNumber < spreadsheet.RowsCount; rowNumber++)
                {
                    for (var columnNumber = 1; columnNumber < spreadsheet.ColumnsCount; columnNumber++)
                    {
                        sOut.WriteLine(spreadsheet.GetValue(rowNumber, columnNumber));
                    }
                }
            }
        }
    }
}
