using System;
using System.IO;
using SpreadsheetCalculator.Exceptions;

//ToDo: 
// 1. Add mock objects for unit tests. Example: Mock IExpressionCalculator for SpreadsheetTests.
// 2. Thing about SetCell && GetCell validation in Spreadsheet class.
// 3. ISpreadsheetCellFactory in order to inject SpreadsheetCell into Spreadsheet ?
// 4. Spreadsheet cell "Pending" state, what if return original value if cell not calculated yet???

namespace SpreadsheetCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputFile = args.Length > 0 ? args[0] :null;
            var outputFile = args.Length > 1 ? args[1] : null;

            Spreadsheet spreadsheet;
            using (TextReader reader = GetTextReader(inputFile))
            {
                spreadsheet = new SpreadsheetReader(reader).Read();
            }

            try
            {
                spreadsheet.Calculate();
            }
            catch (CyclicDependencyException ex)
            {
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
