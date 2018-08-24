using SpreadsheetCalculator.Spreadsheet;
using System;
using System.IO;

namespace SpreadsheetCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputFile = args.Length > 0 ? args[0] :null;
            var outputFile = args.Length > 1 ? args[1] : null;

            InMemorySpreadsheet spreadsheet;
            using (TextReader reader = GetTextReader(inputFile))
            {
                spreadsheet = new SpreadsheetReader(reader).Read();
            }

            try
            {
                spreadsheet.Calculate();
            }
            catch (SpreadsheetInternallException ex)
            {
                // Report about exception which occurred during spreadsheet calculation and close application.
                Console.WriteLine(ex.Message);
                return;
            }

            using (TextWriter writer = GetTextWriter(outputFile))
            {
                new SpreadsheetWriter(writer).Write(spreadsheet);
            }
        }

        private static TextReader GetTextReader(string inputFile)
        {
            if (string.IsNullOrEmpty(inputFile))
            {
                return Console.In;
            }

            return new StreamReader(new FileStream(inputFile, FileMode.Open));
        }

        private static TextWriter GetTextWriter(string outputFile)
        {
            if (string.IsNullOrEmpty(outputFile))
            {
                return Console.Out;
            }

            return new StreamWriter(new FileStream(outputFile, FileMode.OpenOrCreate));
        }
    }
}
