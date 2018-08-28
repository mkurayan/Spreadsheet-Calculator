using SpreadsheetCalculator.Spreadsheet;
using System;
using System.IO;

namespace SpreadsheetCalculator.IO
{
    class FileOutput : IOutputStreamWriter
    {
        public string File { get; private set; }

        public FileOutput(string file)
        {
            File = file ?? throw new ArgumentNullException(nameof(file));
        }

        public void Write(IViewSpreadsheet spreadsheet)
        {
            Console.WriteLine("Save spreadsheet to file.");

            using (var sOut = new StreamWriter(new FileStream(File, FileMode.OpenOrCreate)))
            {
                sOut.WriteLine("{0} {1}", spreadsheet.ColumnsCount, spreadsheet.RowsCount);

                for (var rowNumber = 0; rowNumber < spreadsheet.RowsCount; rowNumber++)
                {
                    for (var columnNumber = 0; columnNumber < spreadsheet.ColumnsCount; columnNumber++)
                    {
                        sOut.WriteLine(spreadsheet.GetValue(rowNumber, columnNumber));
                    }
                }
            }
        }
    }
}
