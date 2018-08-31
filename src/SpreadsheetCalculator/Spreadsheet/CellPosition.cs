using SpreadsheetCalculator.Utils;
using System.Numerics;
using System.Text.RegularExpressions;

namespace SpreadsheetCalculator.Spreadsheet
{
    class CellPosition
    {
        private static Regex keyPattern = new Regex(@"^([a-zA-Z]+)(\d+)$");

        public int ColumnIndex { get; private set; }

        public int RowIndex { get; private set; }

        public string Key { get; private set; }

        public bool IsValid { get; set; }

        public CellPosition(int columnIndex, int rowIndex)
        {
            SetPosition(columnIndex, rowIndex);
        }

        public CellPosition(string key)
        {
            SetPosition(key);
        }

        public void SetPosition(int columnIndex, int rowIndex)
        {
            if (columnIndex <= 0 || rowIndex <= 0)
            {
                SetPositionError();
            }
            else
            {
                SetPosition(columnIndex, rowIndex, AlphabetConvertor.IntToLetters(columnIndex) + rowIndex);
            }
        }

        public void SetPosition(string key)
        {
            Match result = keyPattern.Match(key);

            string alphaPart = result.Groups[1].Value;
            string numberPart = result.Groups[2].Value;

            BigInteger columnIndex = AlphabetConvertor.LettersToInt(alphaPart);
            BigInteger rowIndex = BigInteger.Parse(numberPart);

            if (columnIndex > int.MaxValue || rowIndex > int.MaxValue)
            {
                SetPositionError();
            }
            else
            {
                SetPosition((int)columnIndex, (int)rowIndex, key);
            }
        }

        private void SetPositionError()
        {
            ColumnIndex = -1;
            RowIndex = -1;
            Key = string.Empty;
            IsValid = false;
        }

        private void SetPosition(int columnIndex, int rowIndex, string key)
        {
            ColumnIndex = columnIndex;
            RowIndex = rowIndex;
            Key = key;
            IsValid = true;
        }

        public static bool IsCellPosition(string key)
        {
            return keyPattern.IsMatch(key);
        }
    }    
}
