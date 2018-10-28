using SpreadsheetCalculator.IO;
using SpreadsheetCalculator.IO.Console;
using SpreadsheetCalculator.IO.File;
using SpreadsheetCalculator.Spreadsheet;
using System;

namespace SpreadsheetCalculator
{
    internal class SpreadsheetExecutor
    {
        private IInputStreamReader _reader;
        private IInputStreamReader Reader
        {
            get => _reader ?? (_reader = new ConsoleInput());

            set => _reader = value;
        }

        private IOutputStreamWriter _writer;
        private IOutputStreamWriter Writer
        {
            get => _writer ?? (_writer = new ConsoleOutput());

            set => _writer = value;
        }

        public void SetInputStreamToFile(string path)
        {
            Reader = new FileInput(path);
        }

        public void SetOutputStreamToFile(string path)
        {
            Writer = new FileOutput(path);
        }
        
        public void Execute()
        {
            IMathSpreadsheet spreadsheet;

            try
            {
                spreadsheet = Reader.Read();
            }
            catch(SpreadsheetFormatException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            Console.WriteLine("Calculating spreadsheet...");

            try
            {
                spreadsheet.Calculate();
            }
            catch (SpreadsheetInternalException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            Writer.Write(spreadsheet);
        }
    }
}
