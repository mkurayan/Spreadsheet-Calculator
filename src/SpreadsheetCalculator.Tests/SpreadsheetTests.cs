using SpreadsheetCalculator.ExpressionCalculator;
using System;
using Xunit;

namespace SpreadsheetCalculator.Tests
{
    public class SpreadsheetTests
    {
        IExpressionCalculator ExpressionCalculator;

        public SpreadsheetTests()
        {
            ExpressionCalculator = new RpnCalculator();
        }


        [Theory]
        [InlineData(0, 1)]
        [InlineData(-1, 1)]
        [InlineData(1000001, 1)]
        [InlineData(1, 0)]
        [InlineData(1, -1)]
        [InlineData(1, 1000001)]
        public void SpreadsheetConstructor_InvalidSize_ThrowArgumentException(int rowNumber, int columnNumber)
        {
            Assert.Throws<ArgumentException>(() => new Spreadsheet(rowNumber, columnNumber, ExpressionCalculator));
        }

        [Fact]
        public void SpreadsheetConstructor_ExpressionCalculatorMissied_ThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Spreadsheet(2, 2, null));
        }

        [Fact]
        public void SpreadsheetConstructor_CorrectOptions_SpreadsheetCreated()
        {
            var spreadsheet = new Spreadsheet(2, 3, ExpressionCalculator);

            Assert.Equal(2, spreadsheet.RowNumber);
            Assert.Equal(3, spreadsheet.ColumnNumber);
        }

        /*
        [Fact]
        public void SetCell_CellValue_SpreadsheetCellHasValue()
        {
            int columnNumber = 2;
            int rowNumber = 3;

            var spreadsheet = new Spreadsheet(rowNumber, columnNumber, ExpressionCalculator);

            for (int i = 0; i < rowNumber; i++)
            {
                for (int j = 0; i < columnNumber; j++)
                {
                    spreadsheet.SetCell(i, j, $"row: {i} column: {j}");
                }
            }

            for (int i = 0; i < rowNumber; i++)
            {
                for (int j = 0; i < columnNumber; j++)
                {
                    Assert.Equal($"row: {i} column: {j}", spreadsheet.GetCell(i, j));
                }
            }
        }
        */
    }
}
