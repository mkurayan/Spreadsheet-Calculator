using SpreadsheetCalculator.Calculator;
using SpreadsheetCalculator.Parser;
using System;
using System.IO;
using System.Linq;

namespace SpreadsheetCalculator
{
    /// <summary>
    /// Read spreadsheet from StdIn.
    /// </summary>
    class SpreadsheetReader
    {
        /// <summary>
        /// Spreadsheet data source.
        /// </summary>
        private TextReader _tIn;

        /// <summary>
        /// Create new SpreadsheetReader.
        /// </summary>
        /// <param name="tIn">Input reader that can read a sequential series of characters. Will be used as spreadsheet data source.</param>
        public SpreadsheetReader(TextReader tIn)
        {
            _tIn = tIn ?? throw new ArgumentNullException(nameof(tIn));
        }

        /// <summary>
        /// Read Spreadsheet from input source.
        /// </summary>
        public Spreadsheet Read()
        {
            // Read spreadsheet size. 
            var arr = _tIn.ReadLine().Split(' ').Select(Int32.Parse).ToArray();

            if (arr.Length < 2)
            {
                throw new ArgumentException("Spreadsheet size not provided or provided incorrectly.");
            }

            var spreadsheet = new Spreadsheet(arr[0], arr[1], new PostfixNotationCalculator(), new StringParser());

            for (var rowNumber = 0; rowNumber < spreadsheet.RowNumber; rowNumber++)
            {
                for (var columnNumber = 0; columnNumber < spreadsheet.ColumnNumber; columnNumber++)
                {
                    var cellValue = _tIn.ReadLine();

                    spreadsheet.SetCell(rowNumber, columnNumber, cellValue);
                }
            }

            return spreadsheet;
        }
    }
}
