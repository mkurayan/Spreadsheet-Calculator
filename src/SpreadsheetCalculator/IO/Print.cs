using SpreadsheetCalculator.Spreadsheet;
using System.Linq;
using System.Text;

namespace SpreadsheetCalculator.IO
{
    class Print
    {
        public string PrintSpreadsheet(IViewSpreadsheet spreadsheet)
        {
            var columnsLength =
              Enumerable.Range(0, spreadsheet.ColumnsCount)
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

            for (var i = 0; i < spreadsheet.RowsCount; i++)
            {
                builder.AppendLine(string.Format(format, GetRow(spreadsheet, i)));
                builder.AppendLine(divider);
            }

            return builder.ToString();
        }

        private string[] GetRow(IViewSpreadsheet spreadsheet, int rowIndex)
        {
            return Enumerable.Range(0, spreadsheet.ColumnsCount)
                        .Select(columnIndex => spreadsheet.GetValue(columnIndex, rowIndex))
                        .ToArray();

        }

        private string[] GetColumn(IViewSpreadsheet spreadsheet, int columnIndex)
        {
            return Enumerable.Range(0, spreadsheet.RowsCount)
                        .Select(rowIndex => spreadsheet.GetValue(columnIndex, rowIndex))
                        .ToArray();
        }
    }
}
