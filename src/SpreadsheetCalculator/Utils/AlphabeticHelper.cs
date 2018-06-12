namespace SpreadsheetCalculator.Utils
{
    static class AlphabeticHelper
    {
        const string Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Convert index to alphabetic character.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string IntToLetter(int index)
        {
            var value = "";

            if (index >= Letters.Length)
                value += Letters[index / Letters.Length - 1];

            value += Letters[index % Letters.Length];

            return value;
        }

        /// <summary>
        /// Convert alphabetic character to index.
        /// </summary>
        /// <param name="letter"></param>
        /// <returns></returns>
        public static int LetterToInt(char letter)
        {
            // 65 - 'A'
            return letter - 65;
        }
    }
}
