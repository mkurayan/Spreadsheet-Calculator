using System;
using System.Collections.Generic;
using System.Text;

namespace SpreadsheetCalculator.Spreadsheet
{
    class CellReferenceHelper
    {
        private const int AlphabeticShift = 65;

        private const int RowsShift = 1;

        public static string ToKey(int columnIndex, int rowIndex) => (char)(columnIndex + AlphabeticShift) + (rowIndex + RowsShift).ToString();

        public static (int columnIndex, int rowIndex) FromKey(string key) => (key[0] - AlphabeticShift, int.Parse(key.Substring(1)) - RowsShift);
    }
}
