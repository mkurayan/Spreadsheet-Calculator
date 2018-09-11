using SpreadsheetCalculator.Spreadsheet;

namespace SpreadsheetCalculator.IO.Console
{
    internal class ConsoleInput : IInputStreamReader
    {
        private const int ColumnsThreshold = 10;
        private const int RowsThreshold = 99;

        public void Read(IEditSpreadsheet spreadsheet)
        {
            var textSpreadsheet = new TextSpreadsheet();

            System.Console.WriteLine("Please provide spreadsheet size.");

            System.Console.Write("Columns count: ");

            if (!int.TryParse(System.Console.ReadLine(), out var columnsCount))
            {
                throw new SpreadsheetFormatException("Invalid columns count.");
            }

            System.Console.Write("Rows count: ");
            if (!int.TryParse(System.Console.ReadLine(), out var rowsCount))
            {
                throw new SpreadsheetFormatException("Invalid rows count");
            }

            var showInteractiveInput = columnsCount <= ColumnsThreshold && rowsCount <= RowsThreshold;
            if (!showInteractiveInput)
            {
                System.Console.WriteLine($"Spreadsheet size exceed {ColumnsThreshold} x {RowsThreshold} interactive output will be hidden because spreadsheet will not feet in console.");
            }

            textSpreadsheet.SetSize(columnsCount, rowsCount);
            spreadsheet.SetSize(columnsCount, rowsCount);

            System.Console.WriteLine(string.Empty);

            var left = System.Console.CursorLeft;
            var top = System.Console.CursorTop;

            for (var rowNumber = 1; rowNumber <= rowsCount; rowNumber++)
            {
                for (var columnNumber = 1; columnNumber <= columnsCount; columnNumber++)
                {
                    if (showInteractiveInput)
                    {
                        CleanConsole();

                        System.Console.WriteLine(Print.PrintSpreadsheet(textSpreadsheet));
                    }
                    
                    System.Console.Write($"Please enter { new CellPosition(columnNumber, rowNumber) }: ");
                    var cellValue = System.Console.ReadLine();

                    spreadsheet.SetValue(columnNumber, rowNumber, cellValue);
                    textSpreadsheet.SetValue(columnNumber, rowNumber, cellValue);
                }
            }

            if (showInteractiveInput)
            {
                CleanConsole();
                System.Console.WriteLine(Print.PrintSpreadsheet(textSpreadsheet));
            }

            void CleanConsole()
            {
                var linesToClear = System.Console.CursorTop - top;

                System.Console.SetCursorPosition(left, top);
                System.Console.Write(new string(' ', System.Console.WindowWidth * linesToClear));
                System.Console.SetCursorPosition(left, top);
            }
        }

        private class TextSpreadsheet : IViewSpreadsheet, IEditSpreadsheet
        {
            private Matrix<string> _matrix;

            public int ColumnsCount => _matrix.ColumnsCount;

            public int RowsCount => _matrix.RowsCount;

            public string GetValue(int column, int row)
            {
                return _matrix[column, row] ?? string.Empty;
            }

            public void SetSize(int columnsCount, int rowsCount)
            {
                _matrix = new Matrix<string>(columnsCount, rowsCount);
            }

            public void SetValue(int column, int row, string value)
            {
                _matrix[column, row] = value;
            }
        }
    }
}
