using SpreadsheetCalculator.Utils;
using System.Numerics;
using System.Text.RegularExpressions;

namespace SpreadsheetCalculator.Spreadsheet
{
    internal class CellPosition
    {
        private static readonly Regex KeyPattern = new Regex(@"^([a-zA-Z]+)(\d+)$");

        public static string CoordinatesToKey(int column, int row)
        {
            return AlphabetConverter.IntToLetters(column) + row;
        }

        public static (int column, int row) KeyToCordinates(string key)
        {
            var result = KeyPattern.Match(key);

            var alphaPart = result.Groups[1].Value;
            var numberPart = result.Groups[2].Value;

            var columnIndex = AlphabetConverter.LettersToInt(alphaPart);
            var rowIndex = BigInteger.Parse(numberPart);

            var column = columnIndex > int.MaxValue ? -1 : (int)columnIndex;
            var row = rowIndex > int.MaxValue ? -1 : (int)rowIndex;

            return (column, row);
        }
    }    
}
