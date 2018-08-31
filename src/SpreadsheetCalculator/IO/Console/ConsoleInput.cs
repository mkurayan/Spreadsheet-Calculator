using SpreadsheetCalculator.Spreadsheet;
using SpreadsheetCalculator.Utils;
using System;

namespace SpreadsheetCalculator.IO
{
    class ConsoleInput : IInputStreamReader
    {
        private Print print = new Print();

        public void Read(IEditSpreadsheet spreadsheet)
        {
            TextSpreadsheet textSpreadsheet = new TextSpreadsheet();

            Console.Write("Columns count: ");

            if (!int.TryParse(Console.ReadLine(), out int columnsCount))
            {
                throw new SpreadsheetFormatException("Invalid columns count.");
            }

            Console.Write("Rows count: ");
            if (!int.TryParse(Console.ReadLine(), out int rowsCount))
            {
                throw new SpreadsheetFormatException("Invalid rows count");
            }

            textSpreadsheet.SetSize(columnsCount, rowsCount);
            spreadsheet.SetSize(columnsCount, rowsCount);

            Console.WriteLine(string.Empty);

            var left = Console.CursorLeft;
            var top = Console.CursorTop;

            for (var rowNumber = 1; rowNumber <= rowsCount; rowNumber++)
            {
                for (var columnNumber = 1; columnNumber <= columnsCount; columnNumber++)
                {
                    CleanConsole();

                    Console.WriteLine(print.PrintSpreadsheet(textSpreadsheet));

                    var posotion = new CellPosition(columnNumber, rowNumber);

                    Console.Write($"Please enter { posotion }: ");
                    var cellValue = Console.ReadLine();

                    spreadsheet.SetValue(columnNumber, rowNumber, cellValue);
                    textSpreadsheet.SetValue(columnNumber, rowNumber, cellValue);
                }
            }

            CleanConsole();
            Console.WriteLine(print.PrintSpreadsheet(textSpreadsheet));

            void CleanConsole()
            {
                var linesToClear = Console.CursorTop - top;

                Console.SetCursorPosition(left, top);
                Console.Write(new string(' ', Console.WindowWidth * linesToClear));
                Console.SetCursorPosition(left, top);
            }
        }

        class TextSpreadsheet : IViewSpreadsheet, IEditSpreadsheet
        {
            private Matrix<string> Matrix;

            public int ColumnsCount => Matrix.ColumnsCount;

            public int RowsCount => Matrix.RowsCount;

            public string GetValue(int column, int row)
            {
                return Matrix[column, row] ?? string.Empty;
            }

            public void SetSize(int columnsCount, int rowsCount)
            {
                Matrix = new Matrix<string>(columnsCount, rowsCount);
            }

            public void SetValue(int column, int row, string value)
            {
                Matrix[column, row] = value;
            }
        }
    }
}
