using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SpreadsheetCalculator
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var inputFile = GetArgumentByIndex(args, 0);
            var outputFile = GetArgumentByIndex(args, 1);

            var executor = new SpreadsheetExecutor();

            // If input file provided, we read spreadsheet from it.
            if (!string.IsNullOrEmpty(inputFile))
            {
                executor.SetInputStreamToFile(inputFile);
            }

            // If output file provided, we write spreadsheet into it.
            if (!string.IsNullOrEmpty(outputFile))
            {
                executor.SetOutputStreamToFile(outputFile);
            }

            executor.Execute();

#if DEBUG
            // In debug mode do not close console window immediately once it finished.
            if (Debugger.IsAttached)
            {
                Console.Write("Press any key to continue . . . ");
                Console.ReadLine();
            }
#endif
        }

        private static string GetArgumentByIndex(IReadOnlyList<string> args, int index)
        {
            return args.Count > index ? args[index] : null;
        }
    }
}
