using Moq;
using SpreadsheetCalculator.ExpressionEngine.Parsing;
using SpreadsheetCalculator.ExpressionEngine.Parsing.SyntaxTree;
using SpreadsheetCalculator.ExpressionEngine.Tokenization;
using Xunit;

namespace SpreadsheetCalculator.Tests.ExpressionEngine
{
    public class ParserTests
    {
        private Parser Parser { get; }

        private Mock<IDependencyResolver> Resolver { get; }

        public ParserTests()
        {
            Parser = new Parser();

            Resolver = new Mock<IDependencyResolver>();
        }

        [Fact]
        public void Parse_EmptyExpression_NumberNodeWithZeroValue()
        {
            var treeTopt = Parser.Parse(new Token[0]);

            Assert.IsType<NumberNode>(treeTopt);

            Assert.Equal(0, treeTopt.Evaluate(Resolver.Object));
        }

        [Fact]
        public void Parse_SingleNumber_NumberNodeWithOriginalValue()
        {
            var treeTopt = Parser.Parse(new Token[] { new Token(TokenType.Number, "1") });

            Assert.IsType<NumberNode>(treeTopt);

            Assert.Equal(1, treeTopt.Evaluate(Resolver.Object));
        }

        [Fact]
        public void Parse_UnaryOperationOnSingleNumber_UnnaryOperationAsTreeTop()
        {
            var treeTopt = Parser.Parse(new Token[] {
                new Token(TokenType.Subtract, "-"),
                new Token(TokenType.Number, "1")
            });

            Assert.IsType<UnaryOperationNode>(treeTopt);

            Assert.Equal(-1, treeTopt.Evaluate(Resolver.Object));
        }

        [Fact]
        public void Parse_MultipleUnaryOperationsOnSingleNumber_UnnaryOperationAsTreeTop()
        {
            var treeTopt = Parser.Parse(new Token[] {
                new Token(TokenType.Subtract, "-"),
                new Token(TokenType.Add, "+"),
                new Token(TokenType.Subtract, "-"),
                new Token(TokenType.Number, "1")
            });

            // Because even number of negative convertation is a positive, we can skip them.
            Assert.IsType<NumberNode>(treeTopt);

            Assert.Equal(1, treeTopt.Evaluate(Resolver.Object));
        }

        [Fact]
        public void Parse_BinaryOperation_BinaryNode()
        {
            var treeTopt = Parser.Parse(new Token[] {
                new Token(TokenType.Number, "1"),
                new Token(TokenType.Add, '+'),
                new Token(TokenType.Number, "1")
            });

            Assert.IsType<BinaryOperationNode>(treeTopt);

            Assert.Equal(2, treeTopt.Evaluate(Resolver.Object));
        }

        [Fact]
        public void Parse_TwoBinaryOperations_BinaryNode()
        {
            var treeTopt = Parser.Parse(new Token[] {
                new Token(TokenType.Number, "4"),
                new Token(TokenType.Add, '+'),
                new Token(TokenType.Number, "2"),
                new Token(TokenType.Subtract, '-'),
                new Token(TokenType.Number, '1'),
            });

            Assert.IsType<BinaryOperationNode>(treeTopt);

            Assert.Equal(5, treeTopt.Evaluate(Resolver.Object));
        }

        [Fact]
        public void Parse_SingleCellReference_ReferenceNode()
        {
            var cellRefference = "A1";
            var cellValue = 11;

            var treeTopt = Parser.Parse(new Token[] { new Token(TokenType.CellReference, cellRefference) });

            Assert.IsType<ReferenceNode>(treeTopt);

            var resolver = new Mock<IDependencyResolver>();
            resolver.Setup(r => r.ResolveCellReference(cellRefference)).Returns(cellValue);

            Assert.Equal(cellValue, treeTopt.Evaluate(resolver.Object));
        }

        [Fact]
        public void Parse_BinaryOperationWithCellReference_BinaryNode()
        {
            var cellRefference = "A1";
            var cellValue = 11;

            var treeTopt = Parser.Parse(new Token[] {
                new Token(TokenType.CellReference, cellRefference),
                new Token(TokenType.Add, '+'),
                new Token(TokenType.Number, "1")
            });

            var resolver = new Mock<IDependencyResolver>();
            resolver.Setup(r => r.ResolveCellReference(cellRefference)).Returns(cellValue);

            Assert.IsType<BinaryOperationNode>(treeTopt);

            Assert.Equal(cellValue + 1, treeTopt.Evaluate(resolver.Object));
        }

        [Fact]
        public void Parse_ExpressionWithBracket_SyntaxTree()
        {
            var treeTopt = Parser.Parse(new Token[] {
                new Token(TokenType.Number, "4"),
                new Token(TokenType.Add, '+'),
                new Token(TokenType.OpenParenthesis, '('),
                new Token(TokenType.Number, "6"),
                new Token(TokenType.Divide, '/'),
                new Token(TokenType.Number, '3'),
                new Token(TokenType.CloseParenthesis, ')'),
            });

            Assert.IsType<BinaryOperationNode>(treeTopt);

            Assert.Equal(6, treeTopt.Evaluate(Resolver.Object));
        }
    }
}
