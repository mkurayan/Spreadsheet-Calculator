using SpreadsheetCalculator.ExpressionEngine.Parsing;
using SpreadsheetCalculator.ExpressionEngine.Tokenization;
using SpreadsheetCalculator.Spreadsheet;

namespace SpreadsheetCalculator.IO.Console
{
    internal class ConsoleInput : IInputStreamReader
    {
        private const int ColumnsThreshold = 10;
        private const int RowsThreshold = 99;

        public IMathSpreadsheet Read()
        {
            var spreadsheet = new MathSpreadsheet(new Parser(), new Tokenizer());

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
                        CleanConsole(left, top);

                        System.Console.WriteLine(Print.PrintSpreadsheet(spreadsheet, true));
                    }
                    
                    System.Console.Write($"Please enter { new CellPosition(columnNumber, rowNumber) }: ");
                    var cellValue = System.Console.ReadLine();

                    spreadsheet.SetValue(columnNumber, rowNumber, cellValue);
                }
            }

            if (showInteractiveInput)
            {
                CleanConsole(left, top);
                System.Console.WriteLine(Print.PrintSpreadsheet(spreadsheet, true));
            }

            return spreadsheet;
        }

        private static void CleanConsole(int left, int top)
        {
            var linesToClear = System.Console.CursorTop - top;

            System.Console.SetCursorPosition(left, top);
            System.Console.Write(new string(' ', System.Console.WindowWidth * linesToClear));
            System.Console.SetCursorPosition(left, top);
        }
    }
}
