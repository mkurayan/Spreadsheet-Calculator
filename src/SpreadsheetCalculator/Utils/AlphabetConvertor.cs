using System;
using System.Numerics;
using System.Text.RegularExpressions;

namespace SpreadsheetCalculator.Utils
{
    class AlphabetConvertor
    {
        private const int AlphabetBase = 26;

        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private static readonly Regex LettersPattern = new Regex(@"^[A-Z]+$");

        public static string IntToLetters(int index)
        {
            if (index <= 0)
            {
                throw new IndexOutOfRangeException("Index must be a positive number");
            }

            return ToLetters(index);
        }

        public static BigInteger LettersToInt(string letters)
        {
            if (letters == null)
            {
                throw new ArgumentNullException($"Cannot be null: {nameof(letters)}");
            }

            letters = letters.ToUpper();

            if (!LettersPattern.IsMatch(letters))
            {
                throw new FormatException($"Unknown letter in string: {letters}");
            }

            return ToInt(letters.ToUpper());
        }

        private static string ToLetters(int index)
        {
            index--;

            string letters = "";

            if (index / AlphabetBase > 0)
            {
                letters += ToLetters(index / AlphabetBase);
            }

            letters += Alphabet[index % AlphabetBase];

            return letters;
        }

        private static BigInteger ToInt(string letters)
        {
            BigInteger index = 0;

            if (letters.Length > 1)
            {
                index += AlphabetBase * (ToInt(letters.Remove(letters.Length - 1)));
            }

            index += Alphabet.IndexOf(letters[letters.Length - 1]) + 1;

            return index;
        }
    }
}
