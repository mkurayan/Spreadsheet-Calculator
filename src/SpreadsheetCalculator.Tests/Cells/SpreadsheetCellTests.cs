using System.Linq;
using Xunit;
using SpreadsheetCalculator.Cells;
using System;
using SpreadsheetCalculator.Tokens;

namespace SpreadsheetCalculator.Tests
{
    public class SpreadsheetCellTests
    {
        [Fact]
        public void CreateNewSpreadsheetCell_WithAnyTokens_CellCreatedInPendingState()
        {
            SpreadsheetCell cell = new SpreadsheetCell(Enumerable.Empty<Token>());

            Assert.Equal(CellState.Pending, cell.CellState);

            Assert.Equal("#PENDING!", cell.ToString());
        }

        [Fact]
        public void CreateNewSpreadsheetCell_EmptyTokensList_EmptyCellCreated()
        {
            SpreadsheetCell cell = new SpreadsheetCell(Enumerable.Empty<Token>());

            Assert.True(cell.IsEmpty);
        }


        [Fact]
        public void ParseExpressionToTokens_TokensListWithoutCellReferences_ListOfTokensWithoutCellReferences()
        {
            SpreadsheetCell cell = new SpreadsheetCell(new[] {
                new Token(TokenType.Number, "1"),
                new Token(TokenType.Operator, "+"),
                new Token(TokenType.Parenthesis, "["),
                new Token(TokenType.Unknown, "&?~")
            });

            Assert.Empty(cell.CellDependencies);

            Assert.Equal(4, cell.CellTokens.Count());
        }

        [Fact]
        public void ParseExpressionToTokens_TokensListWithCellReferences_ListOfTokensWithCellReferences()
        {
            SpreadsheetCell cell = new SpreadsheetCell(new[] {
                new Token(TokenType.CellReference, "A1"),
                new Token(TokenType.Operator, "+"),
                new Token(TokenType.CellReference, "A2")
            });

            Assert.Equal(2, cell.CellDependencies.Count());

            Assert.Equal(3, cell.CellTokens.Count());
        }

        [Fact]
        public void SetValue_ValidValue_CellInFulfilledState()
        {
            SpreadsheetCell cell = new SpreadsheetCell(Enumerable.Empty<Token>());

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
            SpreadsheetCell cell = new SpreadsheetCell(Enumerable.Empty<Token>());

            cell.SetValue(value);

            Assert.Equal(CellState.NumberError, cell.CellState);
        }


        [Theory]
        [InlineData(CellState.ValueError, "#VALUE!")]
        [InlineData(CellState.NumberError, "#NUM!")]
        public void SetError_CellErrorState_CellInErorrState(CellState errorState, string expectedOutput)
        {
            SpreadsheetCell cell = new SpreadsheetCell(Enumerable.Empty<Token>());

            cell.SetError(errorState);

            Assert.Equal(errorState, cell.CellState);

            Assert.Equal(expectedOutput, cell.ToString());
        }

        [Theory]
        [InlineData(CellState.Fulfilled)]
        [InlineData(CellState.Pending)]
        public void SetError_NonErrorState_InvalidOperationExceptionThrown(CellState nonErrorState)
        {
            SpreadsheetCell cell = new SpreadsheetCell(Enumerable.Empty<Token>());

            Assert.Throws<InvalidOperationException>(() => cell.SetError(nonErrorState));
        }
    }
}
