using System;
using System.Globalization;
using Moq;
using SpreadsheetCalculator.ExpressionEngine.Parsing.SyntaxTree;
using SpreadsheetCalculator.ExpressionEngine.Tokenization;
using SpreadsheetCalculator.Spreadsheet;
using Xunit;

namespace SpreadsheetCalculator.Tests.Spreadsheet
{
    public class SpreadsheetCellTests
    {
        private readonly Mock<INode> _treeTop;

        public SpreadsheetCellTests()
        {
            _treeTop = new Mock<INode>();
        }

        [Fact]
        public void CreateCell_CellTextIsNull_ThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new Cell(null, new Token[0], _treeTop.Object));
        }

        [Fact]
        public void CreateCell_TokensIsNull_ThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new Cell(string.Empty, null, _treeTop.Object));
        }

        [Fact]
        public void CreateCell_SyntaxTreeIsNull_ThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new Cell(string.Empty, new Token[0], null));
        }

        [Fact]
        public void CreateCell_ValidExpression_CellCreatedInPendingState()
        {
            var cell = new Cell(string.Empty, new Token[0], _treeTop.Object);

            Assert.Equal(CellState.Pending, cell.CellState);

            Assert.Equal(string.Empty, cell.OriginalValue);

            Assert.Equal("#PENDING!", cell.ResultValue);

            Assert.False(cell.CalculatedValue.HasValue);
        }

        [Fact]
        public void CreateCell_EmptyExpression_NoCellReferences()
        {
            var cell = new Cell(string.Empty, new Token[0], _treeTop.Object);

            Assert.Equal(0, cell.CellDependencies.Count);
        }

        [Fact]
        public void CreateCell_MultipleReferenceToCell_CorrectDependenciesCount()
        {
            Token[] tokens = {
                new Token(TokenType.CellReference, "A1"),
                new Token(TokenType.CellReference, "A1"),
                new Token(TokenType.CellReference, "A1"),
                new Token(TokenType.CellReference, "A2"),
                new Token(TokenType.CellReference, "A2"),
                new Token(TokenType.CellReference, "A3")
            };

            var cell = new Cell(string.Empty, tokens, _treeTop.Object);

            Assert.Equal(3, cell.CellDependencies.Count);
        }

        [Fact]
        public void CreateCell_WithoutCellReferences_NoCellReferences()
        {
            Token[] tokens = {
                new Token(TokenType.Number, ""),
                new Token(TokenType.Add, ""),
                new Token(TokenType.Subtract, ""),
                new Token(TokenType.Divide, ""),
                new Token(TokenType.Multiply, ""),
                new Token(TokenType.OpenParenthesis, ""),
                new Token(TokenType.CloseParenthesis, ""),
                new Token(TokenType.EndOfExpression, "")
            };

            var cell = new Cell(string.Empty, tokens, _treeTop.Object);

            Assert.Equal(0, cell.CellDependencies.Count);
        }

        [Fact]
        public void CreateCell_SyntaxException_CellCreatedInSyntaxErrorState()
        {
            const string expressionWithError = "Expression with syntax error";

            var cell = new Cell(expressionWithError);

            Assert.Equal(CellState.SyntaxError, cell.CellState);

            Assert.Equal(expressionWithError, cell.OriginalValue);

            Assert.Equal("#SYNTAX!", cell.ResultValue);

            Assert.False(cell.CalculatedValue.HasValue);
        }
        

        [Fact]
        public void PendingCell_SetValue_CellGoesToCalculatedState()
        {
            var cell = new Cell(string.Empty, new Token[0], _treeTop.Object);

            const double expectedValue = 5.5;

            cell.SetValue(expectedValue);

            Assert.Equal(CellState.Calculated, cell.CellState);

            Assert.Equal(expectedValue.ToString(CultureInfo.InvariantCulture), cell.ResultValue);

            Assert.True(cell.CalculatedValue.HasValue);

            Assert.Equal(expectedValue, cell.CalculatedValue.Value);
        }

        [Fact]
        public void CellWithSyntaxError_SetValue_InvalidOperationExceptionThrown()
        {
            var cell = new Cell("Cell with syntax error");

            Assert.Throws<InvalidOperationException>(() => cell.SetValue(0));
        }

        [Theory]
        [InlineData(CellState.CellValueError)]
        [InlineData(CellState.SyntaxError)]
        [InlineData(CellState.Pending)]
        [InlineData(CellState.Calculated)]
        public void CellWithSyntaxError_SetError_InvalidOperationExceptionThrown(CellState errorState)
        {
            var cell = new Cell("Cell with syntax error");

            Assert.Throws<InvalidOperationException>(() => cell.SetError(errorState));
        }

        [Theory]
        [InlineData(CellState.CellValueError, "#VALUE!")]
        [InlineData(CellState.SyntaxError, "#SYNTAX!")]
        public void PendingCell_SetValueError_CellGoesToErrorState(CellState errorState, string expectedResult)
        {
            var cell = new Cell(string.Empty, new Token[0], _treeTop.Object);

            cell.SetError(errorState);

            Assert.Equal(errorState, cell.CellState);

            Assert.Equal(expectedResult, cell.ResultValue);

            Assert.False(cell.CalculatedValue.HasValue);
        }
    }
}
