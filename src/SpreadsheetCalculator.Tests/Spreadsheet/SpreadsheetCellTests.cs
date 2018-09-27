using System.Collections.Generic;
using Moq;
using SpreadsheetCalculator.ExpressionEngine;
using SpreadsheetCalculator.ExpressionEngine.Parsing;
using SpreadsheetCalculator.ExpressionEngine.Tokenization;
using SpreadsheetCalculator.Spreadsheet;
using Xunit;

namespace SpreadsheetCalculator.Tests.Spreadsheet
{
    public class SpreadsheetCellTests
    {
        Mock<IParser> _parser;
        Mock<ITokenizer> _tokenizer;

        public SpreadsheetCellTests()
        {
            _parser = new Mock<IParser>();
            _tokenizer = new Mock<ITokenizer>();
        }

        [Fact]
        public void CreateNewSpreadsheetCell_ValidExpression_CellCreatedInPendingState()
        {
            var cell = new Cell(_parser.Object, _tokenizer.Object, string.Empty);

            Assert.Equal(CellState.Pending, cell.CellState);

            Assert.Equal(string.Empty, cell.ToString());

            Assert.False(cell.CellValue.HasValue);
        }

        [Fact]
        public void CreateCell_EmptyExpression_NoCellReferences()
        {
            _tokenizer.Setup(foo => foo.Tokenize(It.IsAny<string>())).Returns(new Token[0]);

            var cell = new Cell(_parser.Object, _tokenizer.Object, string.Empty);

            Assert.Equal(0, cell.CellDependencies.Count);
        }

        [Fact]
        public void CreateCell_MultipleReferenceToCell_CorrectDependenciesCount()
        {
            _tokenizer.Setup(foo => foo.Tokenize(It.IsAny<string>())).Returns(new[] {
                new Token(TokenType.CellReference, "A1"),
                new Token(TokenType.CellReference, "A1"),
                new Token(TokenType.CellReference, "A1"),
                new Token(TokenType.CellReference, "A2"),
                new Token(TokenType.CellReference, "A2"),
                new Token(TokenType.CellReference, "A3")
            });

            var cell = new Cell(_parser.Object, _tokenizer.Object, string.Empty);

            Assert.Equal(3, cell.CellDependencies.Count);
        }

        [Fact]
        public void CreateCell_WithoutCellReferences_NoCellReferences()
        {
            _tokenizer.Setup(foo => foo.Tokenize(It.IsAny<string>())).Returns(new[] {
                new Token(TokenType.Number, ""),
                new Token(TokenType.Add, ""),
                new Token(TokenType.Subtract, ""),
                new Token(TokenType.Divide, ""),
                new Token(TokenType.Multiply, ""),
                new Token(TokenType.OpenParenthesis, ""),
                new Token(TokenType.CloseParenthesis, ""),
                new Token(TokenType.EndOfExpression, "")
            });

            var cell = new Cell(_parser.Object, _tokenizer.Object, string.Empty);

            Assert.Equal(0, cell.CellDependencies.Count);
        }

        [Fact]
        public void CreateCell_TokenizerThrowSyntaxException_CellCreatedInSyntaxErrorState()
        {
            _tokenizer.Setup(foo => foo.Tokenize(It.IsAny<string>())).Throws(new SyntaxException("Dummy"));

            var cell = new Cell(_parser.Object, _tokenizer.Object, string.Empty);

            Assert.Equal(CellState.SyntaxError, cell.CellState);

            Assert.Equal("#SYNTAX!", cell.ToString());

            Assert.False(cell.CellValue.HasValue);
        }

        [Fact]
        public void CreateCell_ParserThrowSyntaxException_CellCreatedInSyntaxErrorState()
        {
            _parser.Setup(foo => foo.Parse(It.IsAny<Token[]>())).Throws(new SyntaxException("Dummy"));

            var cell = new Cell(_parser.Object, _tokenizer.Object, string.Empty);

            Assert.Equal(CellState.SyntaxError, cell.CellState);

            Assert.Equal("#SYNTAX!", cell.ToString());

            Assert.False(cell.CellValue.HasValue);
        }
        

        [Fact]
        public void Calculate_InvalidExpression_CellRemainsInErrorState()
        {
            _parser.Setup(foo => foo.Parse(It.IsAny<Token[]>())).Throws(new SyntaxException("Dummy"));

            var cell = new Cell(_parser.Object, _tokenizer.Object, string.Empty);

            Assert.Equal(CellState.SyntaxError, cell.CellState);

            cell.Calculate(null);

            Assert.Equal(CellState.SyntaxError, cell.CellState);

            Assert.False(cell.CellValue.HasValue);

            Assert.Equal("#SYNTAX!", cell.ToString());
        }

        [Fact]
        public void Calculate_EmptyExpression_CellValueSetToZero()
        {
            _tokenizer.Setup(foo => foo.Tokenize(It.IsAny<string>())).Returns(new Token[0]);

            var cell = new Cell(_parser.Object, _tokenizer.Object, string.Empty);

            cell.Calculate(new Dictionary<string, ICell>());

            Assert.Equal(CellState.Calculated, cell.CellState);

            Assert.Equal(0, cell.CellValue.Value);

            Assert.Equal("0", cell.ToString());
        }

        [Fact]
        public void Calculate_CellReferenceMissed_CellInErrorState()
        {
            _tokenizer.Setup(foo => foo.Tokenize(It.IsAny<string>())).Returns(new[] {
                new Token(TokenType.CellReference, "A1"),
                new Token(TokenType.Add, "+"),
                new Token(TokenType.Number, "1")
            });

            var cell = new Cell(_parser.Object, _tokenizer.Object, string.Empty);

            cell.Calculate(new Dictionary<string, ICell> { ["A1"] = null });

            Assert.Equal(CellState.CellValueError, cell.CellState);

            Assert.False(cell.CellValue.HasValue);

            Assert.Equal("#VALUE!", cell.ToString());
        }

        [Theory]
        [InlineData(CellState.SyntaxError)]
        [InlineData(CellState.CellValueError)]
        public void Calculate_CellReferenceHasErrorState_CellInErrorState(CellState errorState)
        {
            _tokenizer.Setup(foo => foo.Tokenize(It.IsAny<string>())).Returns(new[] {
                new Token(TokenType.CellReference, "A1"),
                new Token(TokenType.Add, "+"),
                new Token(TokenType.Number, "1")
            });
            
            var cellWithError = new Mock<ICell>();
            cellWithError.Setup(foo => foo.CellState).Returns(errorState);

            var cell = new Cell(_parser.Object, _tokenizer.Object, string.Empty);

            cell.Calculate(new Dictionary<string, ICell> { ["A1"] = cellWithError.Object });


            Assert.Equal(CellState.CellValueError, cell.CellState);

            Assert.False(cell.CellValue.HasValue);

            Assert.Equal("#VALUE!", cell.ToString());
        }
    }
}
