using SpreadsheetCalculator.Utils;
using System.Numerics;
using System.Text.RegularExpressions;

namespace SpreadsheetCalculator.Spreadsheet
{
    class CellPosition
    {
        private static Regex keyPattern = new Regex(@"^([a-zA-Z]+)(\d+)$");

        public int Column { get; private set; }

        public int Row { get; private set; }

        public string Key { get; private set; }

        public CellPosition(int columnIndex, int rowIndex)
        {
            SetPosition(columnIndex, rowIndex);
        }

        public CellPosition(string key)
        {
            SetPosition(key);
        }

        public void SetPosition(int column, int row)
        {
            Column = column;
            Row = row;

            Key = AlphabetConvertor.IntToLetters(column) + row;
        }

        public void SetPosition(string key)
        {
            Match result = keyPattern.Match(key);

            string alphaPart = result.Groups[1].Value;
            string numberPart = result.Groups[2].Value;

            BigInteger columnIndex = AlphabetConvertor.LettersToInt(alphaPart);
            BigInteger rowIndex = BigInteger.Parse(numberPart);

            Column = columnIndex > int.MaxValue ? -1 : (int)columnIndex;
            Row = rowIndex > int.MaxValue ? -1 : (int)rowIndex;
        }

        public override string ToString()
        {
            return Key;
        }

        public static bool IsCellPosition(string key)
        {
            return keyPattern.IsMatch(key);
        }
    }    
}
