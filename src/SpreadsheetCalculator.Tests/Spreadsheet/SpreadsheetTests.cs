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

        MathSpreadsheet Spreadsheet;

        public SpreadsheetTests()
        {
            ExpressionCalculatorMock = new Mock<IExpressionCalculator>();
            StringParserMock = new Mock<IStringParser>();

            Spreadsheet = new MathSpreadsheet(ExpressionCalculatorMock.Object, StringParserMock.Object);
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(-1, 1)]
        [InlineData(1000001, 1)]
        [InlineData(1, 0)]
        [InlineData(1, -1)]
        [InlineData(1, 1000001)]
        public void SpreadsheetConstructor_InvalidSize_ThrowIndexOutOfRangeException(int rowNumber, int columnNumber)
        {
            Assert.Throws<IndexOutOfRangeException>(() => Spreadsheet.SetSize(columnNumber, rowNumber));
        }

        [Fact]
        public void SpreadsheetConstructor_ExpressionCalculatorMissied_ThrowArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() => new MathSpreadsheet(null, StringParserMock.Object));
        }

        [Fact]
        public void SpreadsheetConstructor_StringParserMissied_ThrowArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() => new MathSpreadsheet(ExpressionCalculatorMock.Object, null));
        }

        [Fact]
        public void SpreadsheetConstructor_CorrectOptions_SpreadsheetCreated()
        {
            Spreadsheet.SetSize(2, 3);

            Assert.Equal(2, Spreadsheet.ColumnsCount);
            Assert.Equal(3, Spreadsheet.RowsCount);
        }

        [Theory]
        [InlineData(-1, -1)]
        [InlineData(1, -1)]
        [InlineData(-1, 1)]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        [InlineData(0, 0)]
        public void GetCell_CellIsOutOfSpreadsheetBoundaries_ThrowIndexOutOfRangeException(int columnNumber, int rowNumber)
        {
            Spreadsheet.SetSize(1, 1);

            Assert.Throws<IndexOutOfRangeException>(() => Spreadsheet.GetValue(rowNumber, columnNumber));
        }

        [Theory]
        [InlineData(-1, 1)]
        [InlineData(1, -1)]
        [InlineData(-1, 1)]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        [InlineData(0, 0)]
        public void SetCell_CellIsOutOfSpreadsheetBoundaries_ThrowIndexOutOfRangeException(int columnNumber, int rowNumber)
        {
            Spreadsheet.SetSize(1, 1);

            Assert.Throws<IndexOutOfRangeException>(() => Spreadsheet.SetValue(rowNumber, columnNumber, "x"));
        }

        [Fact]
        public void SetCell_CellValueButNotCalculate_GetCellInPendingState()
        {
            Spreadsheet.SetSize(1, 1);

            var cellValue = "dummy"; ;

            Spreadsheet.SetValue(1, 1, cellValue);

            Assert.Equal("#PENDING!", Spreadsheet.GetValue(1, 1));
        }

        [Fact]
        public void SetAllCellsInSpreadsheet_CellValueButNotCalculate_AllCellsHavePendingState()
        {
            int columnNumber = 2;
            int rowNumber = 3;

            string cellCoordinatesToString(int i, int j) => $"row: {i} column: {j}";

            Spreadsheet.SetSize(rowNumber, columnNumber);

            for (int i = 1; i <= rowNumber; i++)
            {
                for (int j = 1; j <= columnNumber; j++)
                {
                    Spreadsheet.SetValue(i, j, cellCoordinatesToString(i, j));
                }
            }

            for (int i = 1; i <= rowNumber; i++)
            {
                for (int j = 1; j <= columnNumber; j++)
                {
                    Assert.Equal("#PENDING!", Spreadsheet.GetValue(i, j));
                }
            }
        }
    }
}
