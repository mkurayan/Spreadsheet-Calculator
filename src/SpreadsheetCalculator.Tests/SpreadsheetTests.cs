using SpreadsheetCalculator.ExpressionCalculator;
using System;
using Xunit;
using Moq;

namespace SpreadsheetCalculator.Tests
{
    public class SpreadsheetTests
    {
        Mock<IExpressionCalculator> ExpressionCalculatorMock;

        public SpreadsheetTests()
        {
            ExpressionCalculatorMock = new Mock<IExpressionCalculator>();
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
            Assert.Throws<ArgumentException>(() => new Spreadsheet(rowNumber, columnNumber, ExpressionCalculatorMock.Object));
        }

        [Fact]
        public void SpreadsheetConstructor_ExpressionCalculatorMissied_ThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Spreadsheet(2, 2, null));
        }

        [Fact]
        public void SpreadsheetConstructor_CorrectOptions_SpreadsheetCreated()
        {
            var spreadsheet = new Spreadsheet(2, 3, ExpressionCalculatorMock.Object);

            Assert.Equal(2, spreadsheet.RowNumber);
            Assert.Equal(3, spreadsheet.ColumnNumber);
        }

        [Theory]
        [InlineData(-1, -1)]
        [InlineData(0, -1)]
        [InlineData(-1, 0)]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        [InlineData(1, 1)]
        public void GetCell_CellIsOutOfSpreadsheetBoundaries_ThrowArgumentException(int rowNumber, int columnNumber)
        {
            var spreadsheet = new Spreadsheet(1, 1, ExpressionCalculatorMock.Object);

            Assert.Throws<ArgumentException>(() => spreadsheet.GetCell(rowNumber, columnNumber));
        }

        [Theory]
        [InlineData(-1, -1)]
        [InlineData(0, -1)]
        [InlineData(-1, 0)]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        [InlineData(1, 1)]
        public void SetCell_CellIsOutOfSpreadsheetBoundaries_ThrowArgumentException(int rowNumber, int columnNumber)
        {
            var spreadsheet = new Spreadsheet(1, 1, ExpressionCalculatorMock.Object);

            Assert.Throws<ArgumentException>(() => spreadsheet.SetCell(rowNumber, columnNumber, "x"));
        }

        [Fact]
        public void SetCell_CellValue_SpreadsheetHasCellValue()
        {
            var spreadsheet = new Spreadsheet(1, 1, ExpressionCalculatorMock.Object);

            var cellValue = "1"; ;

            spreadsheet.SetCell(0, 0, cellValue);

            Assert.Equal(cellValue, spreadsheet.GetCell(0, 0));
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
