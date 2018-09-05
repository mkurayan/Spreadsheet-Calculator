using Moq;
using SpreadsheetCalculator.ExpressionEngine.SyntaxAnalysis;
using SpreadsheetCalculator.ExpressionEngine.SyntaxTree;
using SpreadsheetCalculator.Spreadsheet.CellParsing;
using System.Linq;
using Xunit;

namespace SpreadsheetCalculator.Tests.Spreadsheet
{
    public class ValidCellExpressionTests
    {
        [Fact]
        public void CreateValidCellExpression_EmptyTokensList_EmptyExpressionCreated()
        {
            ValidCellExpression exp = new ValidCellExpression(Enumerable.Empty<Token>(), null);

            Assert.True(exp.IsValid);

            Assert.True(exp.IsEmpty);

            Assert.Empty(exp.CellReferences);
        }

        [Fact]
        public void CalculateExpression_EmptyExpression_ReturnZero()
        {
            Mock<IDependencyResolver> resolver = new Mock<IDependencyResolver>();

            ValidCellExpression exp = new ValidCellExpression(Enumerable.Empty<Token>(), null);

            Assert.Equal(0, exp.Calculate(resolver.Object));
        }

        [Fact]
        public void CreateCellExpression_WithoutCellReferences_CellReferencesPropertyEmpty()
        {
            ValidCellExpression exp = new ValidCellExpression(new[] {
                new Token(TokenType.Number, ""),
                new Token(TokenType.Add, ""),
                new Token(TokenType.Subtract, ""),
                new Token(TokenType.Divide, ""),
                new Token(TokenType.Multiply, ""),
                new Token(TokenType.OpenParenthesis, ""),
                new Token(TokenType.CloseParenthesis, ""),
            }, null);

            Assert.Empty(exp.CellReferences);
        }

        [Fact]
        public void CreateCellExpression_WithCellReferences_ExpectedCellReferencesCount()
        {
            ValidCellExpression exp = new ValidCellExpression(new[] {
                new Token(TokenType.Number, "1"),
                new Token(TokenType.Add, "+"),
                new Token(TokenType.CellReference, "A1"),
                new Token(TokenType.Add, "+"),
                new Token(TokenType.CellReference, "A2")
            }, null);

            Assert.Equal(2, exp.CellReferences.Count());
        }

        [Fact]
        public void CalculateExpression_WithCellReferences_ExpectedResult()
        {
            double expectedResult = 10;

            Mock<INode> node = new Mock<INode>();
            node.Setup(foo => foo.Evaluate(It.IsAny<IDependencyResolver>())).Returns(expectedResult);

            ValidCellExpression exp = new ValidCellExpression(new[] {
                //dummy fields.
                new Token(TokenType.CellReference, "A1"),
                new Token(TokenType.Add, "+"),
                new Token(TokenType.CellReference, "A2")
            }, node.Object);

            Assert.Equal(expectedResult, exp.Calculate(new Mock<IDependencyResolver>().Object));
        }
    }
}
