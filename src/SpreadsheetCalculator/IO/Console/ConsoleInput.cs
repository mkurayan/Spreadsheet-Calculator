using SpreadsheetCalculator.Spreadsheet;
using SpreadsheetCalculator.Utils;
using System;

namespace SpreadsheetCalculator.IO
{
    class ConsoleInput : IInputStreamReader
    {
        private const int ColumnsThreshold = 10;
        private const int RowsThreshold = 99;

        private Print print = new Print();

        public void Read(IEditSpreadsheet spreadsheet)
        {
            TextSpreadsheet textSpreadsheet = new TextSpreadsheet();

            Console.WriteLine("Please provide spreadsheet size.");

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

            bool showInteractiveInput = columnsCount <= ColumnsThreshold && rowsCount <= RowsThreshold;
            if (!showInteractiveInput)
            {
                Console.WriteLine($"Spreadsheet size exceed {ColumnsThreshold} x {RowsThreshold} interactive output will be hidden because spreadsheet will not feet in console.");
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
                    if (showInteractiveInput)
                    {
                        CleanConsole();

                        Console.WriteLine(print.PrintSpreadsheet(textSpreadsheet));
                    }
                    
                    Console.Write($"Please enter { new CellPosition(columnNumber, rowNumber) }: ");
                    var cellValue = Console.ReadLine();

                    spreadsheet.SetValue(columnNumber, rowNumber, cellValue);
                    textSpreadsheet.SetValue(columnNumber, rowNumber, cellValue);
                }
            }

            if (showInteractiveInput)
            {
                CleanConsole();
                Console.WriteLine(print.PrintSpreadsheet(textSpreadsheet));
            }

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

            public string GetValue(string key)
            {
                var position = new CellPosition(key);

                return GetValue(position.Column, position.Row);
            }

            public void SetSize(int columnsCount, int rowsCount)
            {
                Matrix = new Matrix<string>(columnsCount, rowsCount);
            }

            public void SetValue(int column, int row, string value)
            {
                Matrix[column, row] = value;
            }

            public void SetValue(string key, string value)
            {
                var position = new CellPosition(key);

                SetValue(position.Column, position.Row, value);
            }
        }
    }
}
