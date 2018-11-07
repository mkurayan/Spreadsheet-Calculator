using SpreadsheetCalculator.Spreadsheet;
using Xunit;

namespace SpreadsheetCalculator.Tests.Spreadsheet
{
    public class CellPositionTests
    {
        [Theory]
        [InlineData(1, 1, "A1")]
        [InlineData(2, 1, "B1")]
        [InlineData(1, 2, "A2")]
        [InlineData(2, 2, "B2")]
        [InlineData(26, 1, "Z1")]
        [InlineData(27, 1, "AA1")]
        [InlineData(27, 999, "AA999")]
        public void CoordinatesToKey_ValidCoordinates_CellKey(int column, int row, string expectedKey)
        {
            var key = CellPosition.CoordinatesToKey(column, row);

            Assert.Equal(expectedKey, key);
        }

        [Theory]
        [InlineData("A1", 1, 1)]
        [InlineData("B1", 2, 1)]
        [InlineData("A2", 1, 2)]
        [InlineData("B2", 2, 2)]
        [InlineData("Z1", 26, 1)]
        [InlineData("AA1", 27, 1)]
        [InlineData("AA999", 27, 999)]
        public void KeyToCordinates_ValidCellKey_CellCoordinates(string key, int expectedColumn, int expectedRow)
        {
            (int column, int row) = CellPosition.KeyToCordinates(key);

            Assert.Equal(expectedColumn, column);
            Assert.Equal(expectedRow, row);
        }

        [Fact]
        public void KeyToCordinates_ToBigColumnNumber_CellCoordinates()
        {
            (int column, int row) = CellPosition.KeyToCordinates("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA1");

            Assert.Equal(-1, column);
            Assert.Equal(1, row);
        }


        [Fact]
        public void KeyToCordinates_ToBigRowNumber_CellCoordinates()
        {
            (int column, int row) = CellPosition.KeyToCordinates("A111111111111111111111111111111111111111111111111111111");

            Assert.Equal(1, column);
            Assert.Equal(-1, row);
        }
    }
}
