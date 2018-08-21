using System.Linq;
using Xunit;
using System;
using SpreadsheetCalculator.Parser;

namespace SpreadsheetCalculator.Tests
{
    public class SpreadsheetCellTests
    {
        [Fact]
        public void CreateNewSpreadsheetCell_WithAnyTokens_CellCreatedInPendingState()
        {
            Cell cell = new Cell(Enumerable.Empty<Token>());

            Assert.Equal(CellState.Pending, cell.CellState);

            Assert.Equal("#PENDING!", cell.ToString());
        }

        [Fact]
        public void CreateNewSpreadsheetCell_EmptyTokensList_EmptyCellCreated()
        {
            Cell cell = new Cell(Enumerable.Empty<Token>());

            Assert.True(cell.IsEmpty);
        }


        [Fact]
        public void ParseExpressionToTokens_TokensListWithoutCellReferences_ListOfTokensWithoutCellReferences()
        {
            Cell cell = new Cell(new[] {
                new Token(TokenType.Number, "1"),
                new Token(TokenType.Operator, "+"),
                new Token(TokenType.RoundBracket, "["),
                new Token(TokenType.Unknown, "&?~")
            });

            Assert.Empty(cell.CellDependencies);

            Assert.Equal(4, cell.CellTokens.Count());
        }

        [Fact]
        public void ParseExpressionToTokens_TokensListWithCellReferences_ListOfTokensWithCellReferences()
        {
            Cell cell = new Cell(new[] {
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
            Cell cell = new Cell(Enumerable.Empty<Token>());

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
            Cell cell = new Cell(Enumerable.Empty<Token>());

            cell.SetValue(value);

            Assert.Equal(CellState.NumberError, cell.CellState);
        }


        [Theory]
        [InlineData(CellState.ValueError, "#VALUE!")]
        [InlineData(CellState.NumberError, "#NUM!")]
        public void SetError_CellErrorState_CellInErorrState(CellState errorState, string expectedOutput)
        {
            Cell cell = new Cell(Enumerable.Empty<Token>());

            cell.SetError(errorState);

            Assert.Equal(errorState, cell.CellState);

            Assert.Equal(expectedOutput, cell.ToString());
        }

        [Theory]
        [InlineData(CellState.Fulfilled)]
        [InlineData(CellState.Pending)]
        public void SetError_NonErrorState_InvalidOperationExceptionThrown(CellState nonErrorState)
        {
            Cell cell = new Cell(Enumerable.Empty<Token>());

            Assert.Throws<InvalidOperationException>(() => cell.SetError(nonErrorState));
        }
    }
}
