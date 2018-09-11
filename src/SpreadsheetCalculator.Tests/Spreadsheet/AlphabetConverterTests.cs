using SpreadsheetCalculator.Utils;
using System;
using Xunit;

namespace SpreadsheetCalculator.Tests.Spreadsheet
{
    public class AlphabetConverterTests
    {
        private Sample[] Samples { get; }

        public AlphabetConverterTests()
        {
            Samples = new Sample[] {
                new Sample(1, "A"),
                new Sample(2, "B"),
                new Sample(26, "Z"),
                new Sample(27, "AA"),
                new Sample(26 * 2, "AZ"),
                new Sample(26 * 2 + 1, "BA"),
                new Sample(26 * 26, "YZ"),
                new Sample(26 * 26 + 1, "ZA"),
                new Sample(26 * 26 + 26, "ZZ"),
                new Sample(26 * 26 + 26 + 1, "AAA"),
                new Sample((676 * 1) + (26 * 12) + 10, "ALJ"),
                new Sample(676 * 26  + 26 * 26 + 26, "ZZZ")
            };
        }

        [Fact]
        public void IntToLetters_Number_ExpectedAlphabetLetters()
        {
            foreach (var sample in Samples)
            {
                Assert.Equal(sample.Letters, AlphabetConverter.IntToLetters(sample.Number));
            }
        }

        [Fact]
        public void LettersToInt_AlphabeticLetters_ExpectedNumber()
        {
            foreach (var sample in Samples)
            {
                Assert.Equal(sample.Number, AlphabetConverter.LettersToInt(sample.Letters));
            }
        }

        [Fact]
        public void TwiceConvertation_AlphabeticLetters_TheSameLetters()
        {
            foreach (var sample in Samples)
            {
                Assert.Equal(sample.Letters, AlphabetConverter.IntToLetters((int)AlphabetConverter.LettersToInt(sample.Letters)));
            }
        }

        [Fact]
        public void IntToLetters_Zero_ThrowIndexOutOfRangeException()
        {
            Assert.Throws<IndexOutOfRangeException>(() => AlphabetConverter.IntToLetters(0));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void IntToLetters_NegativeNumber_ThrowIndexOutOfRangeException(int number)
        {
            Assert.Throws<IndexOutOfRangeException>(() => AlphabetConverter.IntToLetters(number));
        }

        [Fact]
        public void LettersToInt_Null_ThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => AlphabetConverter.LettersToInt(null));
        }

        [Theory]
        [InlineData("1")]
        [InlineData("+")]
        [InlineData("A1")]
        [InlineData("1A")]
        public void LettersToInt_UnknownSymbols_ThrowFormatException(string letters)
        {
            Assert.Throws<FormatException>(() => AlphabetConverter.LettersToInt(letters));
        }

        private struct Sample
        {
            public int Number { get; }
            public string Letters { get; }

            public Sample(int number, string letters)
            {
                Number = number;
                Letters = letters;
            }
        }
    }
}
