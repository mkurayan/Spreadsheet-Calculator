using SpreadsheetCalculator.ExpressionCalculator;
using System;
using Xunit;
using Moq;
using SpreadsheetCalculator.ExpressionParser;
using SpreadsheetCalculator.Spreadsheet;

namespace SpreadsheetCalculator.Tests
{
    public class SpreadsheetTests
    {
        Mock<IExpressionCalculator> ExpressionCalculatorMock;
        Mock<IStringParser> StringParserMock;

        public SpreadsheetTests()
        {
            ExpressionCalculatorMock = new Mock<IExpressionCalculator>();
            StringParserMock = new Mock<IStringParser>();
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
            Assert.Throws<ArgumentException>(() => new InMemorySpreadsheet(columnNumber, rowNumber, ExpressionCalculatorMock.Object, StringParserMock.Object));
        }

        [Fact]
        public void SpreadsheetConstructor_ExpressionCalculatorMissied_ThrowArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() => new InMemorySpreadsheet(2, 2, null, StringParserMock.Object));
        }

        [Fact]
        public void SpreadsheetConstructor_StringParserMissied_ThrowArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() => new InMemorySpreadsheet(2, 2, ExpressionCalculatorMock.Object, null));
        }

        [Fact]
        public void SpreadsheetConstructor_CorrectOptions_SpreadsheetCreated()
        {
            var spreadsheet = new InMemorySpreadsheet(2, 3, ExpressionCalculatorMock.Object, StringParserMock.Object);

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
            var spreadsheet = new InMemorySpreadsheet(1, 1, ExpressionCalculatorMock.Object, StringParserMock.Object);

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
            var spreadsheet = new InMemorySpreadsheet(1, 1, ExpressionCalculatorMock.Object, StringParserMock.Object);

            Assert.Throws<InvalidOperationException>(() => spreadsheet.SetCell(rowNumber, columnNumber, "x"));
        }

        [Fact]
        public void SetCell_CellValueButNotCalculate_GetCellInPendingState()
        {
            var spreadsheet = new InMemorySpreadsheet(1, 1, ExpressionCalculatorMock.Object, StringParserMock.Object);

            var cellValue = "dummy"; ;

            spreadsheet.SetCell(0, 0, cellValue);

            Assert.Equal("#PENDING!", spreadsheet.GetCell(0, 0));
        }

        [Fact]
        public void SetAllCellsInSpreadsheet_CellValueButNotCalculate_AllCellsHavePendingState()
        {
            int columnNumber = 2;
            int rowNumber = 3;

            string cellCoordinatesToString(int i, int j) => $"row: {i} column: {j}";

            var spreadsheet = new InMemorySpreadsheet(rowNumber, columnNumber, ExpressionCalculatorMock.Object, StringParserMock.Object);

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
                    Assert.Equal("#PENDING!", spreadsheet.GetCell(i, j));
                }
            }
        }
    }
}
