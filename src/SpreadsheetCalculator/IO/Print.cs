using SpreadsheetCalculator.Spreadsheet;
using SpreadsheetCalculator.Utils;
using System.Linq;
using System.Text;

namespace SpreadsheetCalculator.IO
{
    class Print
    {
        public string PrintSpreadsheet(IViewSpreadsheet spreadsheet)
        {
            VirtualSpreadsheet view = new VirtualSpreadsheet(spreadsheet);

            return PrintContent(view);
        }

        public string PrintContent(IViewSpreadsheet spreadsheet)
        {
            var columnsLength =
              Enumerable.Range(1, spreadsheet.ColumnsCount)
                  .Select(column =>
                      GetColumn(spreadsheet, column).Max(value => value.Length)
                  ).ToArray();

            // create the string format with padding
            var format = Enumerable.Range(0, spreadsheet.ColumnsCount)
                .Select(i => " | {" + i + ",-" + columnsLength[i] + "}")
                .Aggregate((s, a) => s + a) + " |";

            // create the divider            
            var divider = " " + new string('-', columnsLength.Sum() + columnsLength.Length * 3 + 1) + " ";

            var builder = new StringBuilder();

            builder.AppendLine(divider);

            for (var i = 1; i <= spreadsheet.RowsCount; i++)
            {
                builder.AppendLine(string.Format(format, GetRow(spreadsheet, i)));
                builder.AppendLine(divider);
            }

            return builder.ToString();
        }

        private string[] GetRow(IViewSpreadsheet spreadsheet, int rowIndex)
        {
            return Enumerable.Range(1, spreadsheet.ColumnsCount)
                        .Select(columnIndex => spreadsheet.GetValue(columnIndex, rowIndex))
                        .ToArray();

        }

        private string[] GetColumn(IViewSpreadsheet spreadsheet, int columnIndex)
        {
            return Enumerable.Range(1, spreadsheet.RowsCount)
                        .Select(rowIndex => spreadsheet.GetValue(columnIndex, rowIndex))
                        .ToArray();
        }

        class VirtualSpreadsheet : IViewSpreadsheet
        {
            private IViewSpreadsheet View { get; }

            public int ColumnsCount => View.ColumnsCount + 1;

            public int RowsCount => View.RowsCount + 1;

            public VirtualSpreadsheet(IViewSpreadsheet view)
            {
                View = view;
            }

            public string GetValue(int columnIndex, int rowIndex)
            {
                if (columnIndex == 1 && rowIndex == 1)
                {
                    return "/";
                }

                if (rowIndex == 1)
                {
                    return AlphabetConvertor.IntToLetters(columnIndex - 1);
                }

                if (columnIndex == 1)
                {
                    return (rowIndex - 1).ToString();
                }

                return View.GetValue(columnIndex - 1, rowIndex - 1);
            }
        }
    }
}
