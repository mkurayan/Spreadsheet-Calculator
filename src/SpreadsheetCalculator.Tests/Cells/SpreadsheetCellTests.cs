using System.Linq;
using Xunit;
using SpreadsheetCalculator.Cells;
using SpreadsheetCalculator.ExpressionCalculator;
using System;

namespace SpreadsheetCalculator.Tests
{
    public class SpreadsheetCellTests
    {
        IExpressionCalculator ExpressionCalculator;

        public SpreadsheetCellTests()
        {
            ExpressionCalculator = new RpnCalculator();
        }

        [Fact]
        public void CreateNewSpreadsheetCell_MathExpression_CellCreatedInPendingState()
        {
            var cellText = "raw text";

            SpreadsheetCell cell = new SpreadsheetCell(cellText);

            Assert.Equal(CellState.Pending, cell.CellState);

            Assert.Equal(cellText, cell.ToString());
        }

        [Theory]
        [InlineData("")]
        [InlineData("    ")]
        public void CreateNewSpreadsheetCell_EmptyString_EmptyCellCreated(string value)
        {
            SpreadsheetCell cell = new SpreadsheetCell(value);

            Assert.True(cell.IsEmpty);
        }

        [Theory]
        [InlineData("", 0)]
        [InlineData("    ", 0)]
        [InlineData("1", 1)]
        [InlineData("11", 1)]
        [InlineData("text", 1)]
        [InlineData("a + b", 3)]
        [InlineData("   a    +    b   ", 3)]
        public void ParseExpressionToTokensl_ExpressionWithSeveralTokens_CorrectTokensCount(string value, int expectedTokenCount)
        {
            SpreadsheetCell cell = new SpreadsheetCell(value);

            Assert.Equal(expectedTokenCount, cell.CellTokens.Count());
        }


        [Fact]
        public void ParseExpressionToTokens_MathExpressionWithoutCellReferences_ListOfTokensWithoutCellReferences()
        {
            SpreadsheetCell cell = new SpreadsheetCell("1 1 +");

            Assert.Empty(cell.CellDependencies);

            Assert.Equal(3, cell.CellTokens.Count());
        }

        [Fact]
        public void ParseExpressionToTokens_MathExpressionWithCellReferences_ListOfTokensWithCellReferences()
        {
            SpreadsheetCell cell = new SpreadsheetCell("A1 A2 +");

            Assert.Equal(2, cell.CellDependencies.Count());

            Assert.Equal(3, cell.CellTokens.Count());
        }

        [Fact]
        public void SetValue_ValidValue_CellInFulfilledState()
        {
            SpreadsheetCell cell = new SpreadsheetCell("1 1 +");

            int value = 2;

            cell.SetValue(value);

            Assert.Equal(CellState.Fulfilled, cell.CellState);

            Assert.Equal(value.ToString(), cell.ToString());
        }

        [Theory]
        [InlineData(double.NaN)]
        [InlineData(double.PositiveInfinity)]
        [InlineData(double.NegativeInfinity)]
        public void SetValue_InvalidValue_CellInErrorState(double value)
        {
            SpreadsheetCell cell = new SpreadsheetCell("");

            cell.SetValue(value);

            Assert.Equal(CellState.NumberError, cell.CellState);
        }


        [Theory]
        [InlineData(CellState.ValueError, "#VALUE!")]
        [InlineData(CellState.NumberError, "#NUM!")]
        public void SetError_CellErrorState_CellInErorrState(CellState errorState, string expectedOutput)
        {
            SpreadsheetCell cell = new SpreadsheetCell("");

            cell.SetError(errorState);

            Assert.Equal(errorState, cell.CellState);

            Assert.Equal(expectedOutput, cell.ToString());
        }

        [Theory]
        [InlineData(CellState.Fulfilled)]
        [InlineData(CellState.Pending)]
        public void SetError_NonErrorState_InvalidOperationExceptionThrown(CellState nonErrorState)
        {
            SpreadsheetCell cell = new SpreadsheetCell("A1 A2 +");

            Assert.Throws<InvalidOperationException>(() => cell.SetError(nonErrorState));
        }
    }
}
