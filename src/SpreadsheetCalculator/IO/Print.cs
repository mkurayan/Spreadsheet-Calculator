using System.Collections.Generic;
using SpreadsheetCalculator.Spreadsheet;
using SpreadsheetCalculator.Utils;
using System.Linq;
using System.Text;
using System;

namespace SpreadsheetCalculator.IO
{
    internal static class Print
    {
        public static string PrintSpreadsheet(IViewSpreadsheet spreadsheet, bool original = false)
        {
            var view = new VirtualSpreadsheet(spreadsheet, original);

            return PrintContent(view);
        }

        private static string PrintContent(VirtualSpreadsheet spreadsheet)
        {
            var columnsLength =
              Enumerable.Range(1, spreadsheet.ColumnsCount)
                  .Select(columnIndex =>
                      GetColumn(spreadsheet, columnIndex).Max(value => value.Length)
                  ).ToArray();

            // create the string format with padding
            var format = Enumerable.Range(0, spreadsheet.ColumnsCount)
                .Select(i => " | {" + i + ",-" + columnsLength[i] + "}")
                .Aggregate((s, a) => s + a) + " |" + Environment.NewLine;

            // create the divider            
            var divider = " " + new string('-', columnsLength.Sum() + columnsLength.Length * 3 + 1) + " ";

            var builder = new StringBuilder();

            builder.AppendLine(divider);

            for (var rowIndex = 1; rowIndex <= spreadsheet.RowsCount; rowIndex++)
            {
                // ReSharper disable once CoVariantArrayConversion
                object[] row = GetRow(spreadsheet, rowIndex).ToArray();
                
                builder.AppendFormat(format, row);
                builder.AppendLine(divider);
            }

            return builder.ToString();
        }

        private static IEnumerable<string> GetRow(VirtualSpreadsheet spreadsheet, int rowIndex)
        {
            return Enumerable
                .Range(1, spreadsheet.ColumnsCount)
                .Select(columnIndex => spreadsheet.GetValue(columnIndex, rowIndex));
        }

        private static IEnumerable<string> GetColumn(VirtualSpreadsheet spreadsheet, int columnIndex)
        {
            return Enumerable
                .Range(1, spreadsheet.RowsCount)
                .Select(rowIndex => spreadsheet.GetValue(columnIndex, rowIndex));
        }

        private class VirtualSpreadsheet
        {
            private readonly bool _original;

            private IViewSpreadsheet View { get; }

            public int ColumnsCount => View.ColumnsCount + 1;

            public int RowsCount => View.RowsCount + 1;

            public VirtualSpreadsheet(IViewSpreadsheet view, bool original)
            {
                View = view;

                _original = original;
            }

            public string GetValue(int column, int row)
            {
                if (column == 1 && row == 1)
                {
                    return "/";
                }

                if (row == 1)
                {
                    return AlphabetConverter.IntToLetters(column - 1);
                }

                if (column == 1)
                {
                    return (row - 1).ToString();
                }

                var value = View.GetValue(column - 1, row - 1);

                if (value == null)
                {
                    return string.Empty;
                }
               
                return _original ? value.OriginalValue : value.ResultValue;
            }
        }
    }
}
