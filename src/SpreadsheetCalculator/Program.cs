using System;
using SpreadsheetCalculator.ExpressionEngine.Parsing;
using SpreadsheetCalculator.ExpressionEngine.Tokenization;
using SpreadsheetCalculator.IO;
using SpreadsheetCalculator.Spreadsheet;
using SpreadsheetCalculator.IO.Console;
using SpreadsheetCalculator.IO.File;

namespace SpreadsheetCalculator
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var inputFile = args.Length > 0 ? args[0] :null;
            var outputFile = args.Length > 1 ? args[1] : null;

            var spreadsheet = new MathSpreadsheet(new Parser(), new Tokenizer());

            var reader = GetTextReader(inputFile);

            try
            {
                reader.Read(spreadsheet);

                Console.WriteLine("Calculating spreadsheet...");
                Console.WriteLine(string.Empty);

                spreadsheet.Calculate();
            }
            catch (Exception ex) when (ex is SpreadsheetFormatException || ex is SpreadsheetInternalException)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            GetTextWriter(outputFile).Write(spreadsheet);
        }

        private static IInputStreamReader GetTextReader(string inputFile)
        {
            if (string.IsNullOrEmpty(inputFile))
            {
                return new ConsoleInput();
            }

            return new FileInput(inputFile);
        }

        private static IOutputStreamWriter GetTextWriter(string outputFile)
        {
            if (string.IsNullOrEmpty(outputFile))
            {
                return new ConsoleOutput();
            }
            
            return new FileOutput(outputFile);
        }
    }
}
