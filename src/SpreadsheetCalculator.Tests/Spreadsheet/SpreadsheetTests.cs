using System;
using Moq;
using SpreadsheetCalculator.ExpressionEngine.Parsing;
using SpreadsheetCalculator.ExpressionEngine.Tokenization;
using SpreadsheetCalculator.Spreadsheet;
using Xunit;

namespace SpreadsheetCalculator.Tests.Spreadsheet
{
    public class SpreadsheetTests
    {
        private readonly MathSpreadsheet _spreadsheet;

        public SpreadsheetTests()
        {            
            var parser = new Mock<Parser>();
            var tokenizer = new Mock<Tokenizer>();
            
            _spreadsheet = new MathSpreadsheet(parser.Object, tokenizer.Object);            
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
            Assert.Throws<IndexOutOfRangeException>(() => _spreadsheet.SetSize(columnNumber, rowNumber));
        }

        [Fact]
        public void SpreadsheetConstructor_ArgumentsIsNullMissed_ThrowNullArgumentException()
        {            
            Assert.Throws<ArgumentNullException>(() => new MathSpreadsheet(null, new Mock<ITokenizer>().Object));
            Assert.Throws<ArgumentNullException>(() => new MathSpreadsheet(new Mock<IParser>().Object, null));
        }

        [Fact]
        public void SpreadsheetConstructor_CorrectOptions_SpreadsheetCreated()
        {
            _spreadsheet.SetSize(2, 3);

            Assert.Equal(2, _spreadsheet.ColumnsCount);
            Assert.Equal(3, _spreadsheet.RowsCount);
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
            _spreadsheet.SetSize(1, 1);

            Assert.Throws<IndexOutOfRangeException>(() => _spreadsheet.GetValue(rowNumber, columnNumber));
        }

        [Theory]
        [InlineData(-1, 1)]
        [InlineData(1, -1)]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        [InlineData(0, 0)]
        public void SetCell_CellIsOutOfSpreadsheetBoundaries_ThrowIndexOutOfRangeException(int columnNumber, int rowNumber)
        {
            _spreadsheet.SetSize(1, 1);

            Assert.Throws<IndexOutOfRangeException>(() => _spreadsheet.SetValue(rowNumber, columnNumber, "x"));
        }

        [Fact]
        public void SetCell_CellValueButNotCalculate_GetCellInPendingState()
        {
            _spreadsheet.SetSize(1, 1);

            const string cellValue = "1";

            _spreadsheet.SetValue(1, 1, cellValue);

            Assert.Equal("1", _spreadsheet.GetValue(1, 1).OriginalValue);
        }

        [Fact]
        public void SetAllCellsInSpreadsheet_CellValueButNotCalculate_AllCellsHavePendingState()
        {
            const int columnNumber = 2;
            const int rowNumber = 3;

            string CellCoordinatesToString(int i, int j) => $"{i} + {j}";

            _spreadsheet.SetSize(rowNumber, columnNumber);

            for (var i = 1; i <= rowNumber; i++)
            {
                for (var j = 1; j <= columnNumber; j++)
                {
                    _spreadsheet.SetValue(i, j, CellCoordinatesToString(i, j));
                }
            }

            for (var i = 1; i <= rowNumber; i++)
            {
                for (var j = 1; j <= columnNumber; j++)
                {
                    Assert.Equal(CellCoordinatesToString(i, j), _spreadsheet.GetValue(i, j).OriginalValue);
                }
            }
        }
    }
}
