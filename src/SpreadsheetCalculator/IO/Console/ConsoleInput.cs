using SpreadsheetCalculator.Spreadsheet;
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

            for (var rowNumber = 0; rowNumber < rowsCount; rowNumber++)
            {
                for (var columnNumber = 0; columnNumber < columnsCount; columnNumber++)
                {
                    CleanConsole();

                    Console.WriteLine(print.PrintSpreadsheet(textSpreadsheet));

                    Console.Write($"Please enter {CellReferenceHelper.ToKey(columnNumber, rowNumber)}: ");
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
            private string[,] cells;

            public int ColumnsCount => cells.GetLength(0);

            public int RowsCount => cells.GetLength(1);

            public string GetValue(int columnIndex, int rowIndex)
            {
                return cells[columnIndex, rowIndex] ?? string.Empty;
            }

            public void SetSize(int columnsCount, int rowsCount)
            {
                cells = new string[columnsCount, rowsCount];
            }

            public void SetValue(int columnIndex, int rowIndex, string value)
            {
                cells[columnIndex, rowIndex] = value;
            }
        }
    }
}
