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
            Assert.Throws<ArgumentException>(() => new Spreadsheet(columnNumber, rowNumber, ExpressionCalculatorMock.Object));
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

            Assert.Equal(2, spreadsheet.ColumnNumber);
            Assert.Equal(3, spreadsheet.RowNumber);
        }

        [Theory]
        [InlineData(-1, -1)]
        [InlineData(0, -1)]
        [InlineData(-1, 0)]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        [InlineData(1, 1)]
        public void GetCell_CellIsOutOfSpreadsheetBoundaries_ThrowArgumentException(int columnNumber, int rowNumber)
        {
            var spreadsheet = new Spreadsheet(1, 1, ExpressionCalculatorMock.Object);

            Assert.Throws<InvalidOperationException>(() => spreadsheet.GetCell(rowNumber, columnNumber));
        }

        [Theory]
        [InlineData(-1, -1)]
        [InlineData(0, -1)]
        [InlineData(-1, 0)]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        [InlineData(1, 1)]
        public void SetCell_CellIsOutOfSpreadsheetBoundaries_ThrowArgumentException(int columnNumber, int rowNumber)
        {
            var spreadsheet = new Spreadsheet(1, 1, ExpressionCalculatorMock.Object);

            Assert.Throws<InvalidOperationException>(() => spreadsheet.SetCell(rowNumber, columnNumber, "x"));
        }

        [Fact]
        public void SetCell_CellValue_GetOriginalValueFromSpreadsheet()
        {
            var spreadsheet = new Spreadsheet(1, 1, ExpressionCalculatorMock.Object);

            var cellValue = "dummy"; ;

            spreadsheet.SetCell(0, 0, cellValue);

            Assert.Equal(cellValue, spreadsheet.GetCell(0, 0));
        }

        [Fact]
        public void SetAllCellsInSpreadsheet_CellValue_AllCellsHaveOriginalValues()
        {
            int columnNumber = 2;
            int rowNumber = 3;

            string cellCoordinatesToString(int i, int j) => $"row: {i} column: {j}";

            var spreadsheet = new Spreadsheet(rowNumber, columnNumber, ExpressionCalculatorMock.Object);

            for (int i = 0; i < rowNumber; i++)
            {
                for (int j = 0; j < columnNumber; j++)
                {
                    spreadsheet.SetCell(i, j, cellCoordinatesToString(i, j));
                }
            }

            for (int i = 0; i < rowNumber; i++)
            {
                for (int j = 0; j < columnNumber; j++)
                {
                    Assert.Equal(cellCoordinatesToString(i, j), spreadsheet.GetCell(i, j));
                }
            }
        }
    }
}
