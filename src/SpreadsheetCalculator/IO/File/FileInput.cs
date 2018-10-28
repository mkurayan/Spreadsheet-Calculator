using System;
using System.IO;
using System.Linq;
using SpreadsheetCalculator.ExpressionEngine.Parsing;
using SpreadsheetCalculator.ExpressionEngine.Tokenization;
using SpreadsheetCalculator.Spreadsheet;

namespace SpreadsheetCalculator.IO.File
{
    internal class FileInput : IInputStreamReader
    {
        private string File { get; }

        public FileInput(string file)
        {
            File = file ?? throw new ArgumentNullException(nameof(file));
        }

        public IMathSpreadsheet Read()
        {
            var spreadsheet = new MathSpreadsheet(new Parser(), new Tokenizer());

            System.Console.WriteLine("Load spreadsheet from file.");

            using (var sIn = new StreamReader(new FileStream(File, FileMode.Open)))
            {
                var (columnsCount, rowsCount) = ParseHeader(sIn.ReadLine());

                spreadsheet.SetSize(columnsCount, rowsCount);

                for (var rowNumber = 1; rowNumber <= rowsCount; rowNumber++)
                {
                    for (var columnNumber = 1; columnNumber <= columnsCount; columnNumber++)
                    {
                        var cellValue = sIn.ReadLine();

                        spreadsheet.SetValue(columnNumber, rowNumber, cellValue);
                    }
                }
            }

            return spreadsheet;
        }

        private static (int columnsCount, int rowsCount) ParseHeader(string header)
        {
            var options = header.Split(' ').Where(token => !string.IsNullOrWhiteSpace(token)).ToArray();

            if (options.Length < 2)
            {
                throw new SpreadsheetFormatException($"File header has invalid format. Should be: columnsCount rowsCount. But was: {header}");
            }

            if (!int.TryParse(options[0], out var columnsCount))
            {
                throw new SpreadsheetFormatException($"Invalid columns count: {options[0]}");
            }

            if (!int.TryParse(options[1], out var rowsCount))
            {
                throw new SpreadsheetFormatException($"Invalid rows count: {options[1]}");
            }

            return (columnsCount, rowsCount);
        }
    }
}
