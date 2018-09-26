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
            var exp = new ValidCellExpression(new Token[] {}, null);

            Assert.True(exp.IsValid);

            Assert.True(exp.IsEmpty);

            Assert.Empty(exp.CellReferences);
        }
        
        [Fact]
        public void CreateNew_MultipleReferenceToCell_CorrectDependenciesCount()
        {
            var exp = new ValidCellExpression(new[] {
                new Token(TokenType.CellReference, "A1"),
                new Token(TokenType.CellReference, "A1"),
                new Token(TokenType.CellReference, "A1"),
                new Token(TokenType.CellReference, "A2"),
                new Token(TokenType.CellReference, "A2"),
                new Token(TokenType.CellReference, "A3")
            }, null);

            
            Assert.Equal(3, exp.CellReferences.Count);
        }

        [Fact]
        public void CalculateExpression_EmptyExpression_ReturnZero()
        {
            var resolver = new Mock<IDependencyResolver>();

            var exp = new ValidCellExpression(new Token[] {}, null);

            Assert.Equal(0, exp.Calculate(resolver.Object));
        }

        [Fact]
        public void CreateCellExpression_WithoutCellReferences_CellReferencesPropertyEmpty()
        {
            var exp = new ValidCellExpression(new[] {
                new Token(TokenType.Number, ""),
                new Token(TokenType.Add, ""),
                new Token(TokenType.Subtract, ""),
                new Token(TokenType.Divide, ""),
                new Token(TokenType.Multiply, ""),
                new Token(TokenType.OpenParenthesis, ""),
                new Token(TokenType.CloseParenthesis, "")
            }, null);

            Assert.Empty(exp.CellReferences);
        }

        [Fact]
        public void CreateCellExpression_WithCellReferences_ExpectedCellReferencesCount()
        {
            var exp = new ValidCellExpression(new[] {
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
            const double expectedResult = 10;

            var node = new Mock<INode>();
            node.Setup(foo => foo.Evaluate(It.IsAny<IDependencyResolver>())).Returns(expectedResult);

            var exp = new ValidCellExpression(new[] {
                //dummy fields.
                new Token(TokenType.CellReference, "A1"),
                new Token(TokenType.Add, "+"),
                new Token(TokenType.CellReference, "A2")
            }, node.Object);

            Assert.Equal(expectedResult, exp.Calculate(new Mock<IDependencyResolver>().Object));
        }
    }
}
