using SpreadsheetCalculator.Utils;
using System.Numerics;
using System.Text.RegularExpressions;

namespace SpreadsheetCalculator.Spreadsheet
{
    internal class CellPosition
    {
        private static readonly Regex KeyPattern = new Regex(@"^([a-zA-Z]+)(\d+)$");

        public int Column { get; private set; }

        public int Row { get; private set; }

        private string Key { get; set; }

        /// <summary>
        /// Create cell position object from cell coordinates. 
        /// </summary>
        /// <param name="column">Column number in spreadsheet.</param>
        /// <param name="row">Row number in spreadsheet.</param>
        public CellPosition(int column, int row)
        {
            SetPosition(column, row);
        }

        /// <summary>
        /// Create cell position object from cell Id. 
        /// </summary>
        /// <param name="key">Spreadsheet cell Id. Example: A1, A2, etc.</param>
        public CellPosition(string key)
        {
            SetPosition(key);
        }
        
        public override string ToString()
        {
            return Key;
        }

        private void SetPosition(int column, int row)
        {
            Column = column;
            Row = row;

            Key = AlphabetConverter.IntToLetters(column) + row;
        }

        private void SetPosition(string key)
        {
            var result = KeyPattern.Match(key);

            var alphaPart = result.Groups[1].Value;
            var numberPart = result.Groups[2].Value;

            var columnIndex = AlphabetConverter.LettersToInt(alphaPart);
            var rowIndex = BigInteger.Parse(numberPart);

            Column = columnIndex > int.MaxValue ? -1 : (int)columnIndex;
            Row = rowIndex > int.MaxValue ? -1 : (int)rowIndex;
        }
    }    
}
