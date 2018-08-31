using SpreadsheetCalculator.Spreadsheet;
using System;
using System.IO;
using System.Linq;

namespace SpreadsheetCalculator.IO
{
    class FileInput : IInputStreamReader
    {
        public string File { get; private set; }

        public FileInput(string file)
        {
            File = file ?? throw new ArgumentNullException(nameof(file));
        }

        public void Read(IEditSpreadsheet spreadsheet)
        {
            Console.WriteLine("Load spreadsheet from file.");

            using (var sIn = new StreamReader(new FileStream(File, FileMode.Open)))
            {
                (int columnsCount, int rowsCount) = ParseHeader(sIn.ReadLine());

                spreadsheet.SetSize(columnsCount, rowsCount);

                for (var rowNumber = 1; rowNumber <= rowsCount; rowNumber++)
                {
                    for (var columnNumber = 0; columnNumber <= columnsCount; columnNumber++)
                    {
                        var cellValue = sIn.ReadLine();

                        spreadsheet.SetValue(rowNumber, columnNumber, cellValue);
                    }
                }
            }
        }

        private (int columnsCount, int rowsCount) ParseHeader(string header)
        {
            var options = header.Split(' ').Where(token => !string.IsNullOrWhiteSpace(token)).ToArray();

            if (options.Length < 2)
            {
                throw new SpreadsheetFormatException($"File header has invalid format. Should be: columnsCount rowsCount. But was: {header}");
            }

            if (!int.TryParse(options[0], out int columnsCount))
            {
                throw new SpreadsheetFormatException($"Invalid columns count: {options[0]}");
            }

            if (!int.TryParse(options[1], out int rowsCount))
            {
                throw new SpreadsheetFormatException($"Invalid rows count: {options[1]}");
            }

            return (columnsCount, rowsCount);
        }
    }
}
