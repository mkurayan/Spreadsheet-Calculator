using SpreadsheetCalculator.ExpressionParser;
using System.Linq;
using Xunit;

namespace SpreadsheetCalculator.Tests.Tokens
{
    public class StringParserTests
    {
        StringParser Parser { get; }

        public StringParserTests()
        {
            Parser = new StringParser();
        }

        [Theory]
        [InlineData("")]
        [InlineData("    ")]
        public void Parse_EmptyString_EmptyTokensList(string value)
        {
            var tokens = Parser.Parse(value);

            Assert.Empty(Parser.Parse(value));
        }

        [Theory]
        [InlineData("", 0)]
        [InlineData("    ", 0)]
        [InlineData("1", 1)]
        [InlineData("11", 1)]
        [InlineData("text", 1)]
        [InlineData("a + b", 3)]
        [InlineData("   a    +    b   ", 3)]
        [InlineData("( 1 + A1 ) / 2", 7)]
        public void Parse_ExpressionWithSeveralTokens_CorrectTokensCount(string value, int expectedTokenCount)
        {
            var tokens = Parser.Parse(value);

            Assert.Equal(expectedTokenCount, tokens.Count());
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("1")]
        [InlineData("11")]
        [InlineData("+")]
        [InlineData("-")]
        [InlineData("/")]
        [InlineData("*")]
        [InlineData("(")]
        [InlineData(")")]
        [InlineData("A")]
        [InlineData("AB")]
        public void Parse_NotCellreference_DoesNotContainCellreferenceToken(string value)
        {
            Assert.DoesNotContain(Parser.Parse(value), token => token.Type == TokenType.CellReference);
        }

        [Theory]
        [InlineData("A1")]
        [InlineData("AA1")]
        [InlineData("ABCD1")]
        [InlineData("A123456789")]
        public void CreateNewCellToken_CellReference_ContainsCellReferenceToken(string value)
        {
            Assert.Contains(Parser.Parse(value), token => token.Type == TokenType.CellReference);
        }

        [Theory]
        [InlineData("(")]
        [InlineData(")")]
        public void CreateNewCellToken_Parenthesis_ContainsParenthesisToken(string value)
        {
            Assert.Contains(Parser.Parse(value), token => token.Type == TokenType.RoundBracket);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("11")]
        [InlineData("+")]
        [InlineData("-")]
        [InlineData("/")]
        [InlineData("*")]
        [InlineData("A")]
        [InlineData("AB")]
        [InlineData("AA2")]
        [InlineData("A1")]
        [InlineData("A22")]
        [InlineData("   ")]
        [InlineData("[ ] { }")]
        public void CreateNewCellToken_NotParenthesis_DoesNotContainsParenthesisToken(string value)
        {
            Assert.DoesNotContain(Parser.Parse(value), token => token.Type == TokenType.RoundBracket);
        }

        [Theory]
        [InlineData("+")]
        [InlineData("-")]
        [InlineData("*")]
        [InlineData("/")]
        public void CreateNewCellToken_Operator_ContainsOperatorToken(string value)
        {
            Assert.Contains(Parser.Parse(value), token => token.Type == TokenType.Operator);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("11")]
        [InlineData("(")]
        [InlineData(")")]
        [InlineData("A")]
        [InlineData("AB")]
        [InlineData("AA2")]
        [InlineData("A1")]
        [InlineData("A22")]
        [InlineData("   ")]
        public void CreateNewCellToken_NotOperator_DoesNotContainsOperatorToken(string value)
        {
            Assert.DoesNotContain(Parser.Parse(value), token => token.Type == TokenType.Operator);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("22")]
        [InlineData("123456789")]
        [InlineData("0.55")]
        public void CreateNewCellToken_Number_ContainsNumberToken(string value)
        {
            Assert.Contains(Parser.Parse(value), token => token.Type == TokenType.Number);
        }

        [Theory]
        [InlineData("+")]
        [InlineData("-")]
        [InlineData("*")]
        [InlineData("/")]
        [InlineData("(")]
        [InlineData(")")]
        [InlineData("A")]
        [InlineData("AB")]
        [InlineData("AA2")]
        [InlineData("A1")]
        [InlineData("A22")]
        [InlineData("   ")]
        public void CreateNewCellToken_Number_DoesNotContainNumberToken(string value)
        {
            Assert.DoesNotContain(Parser.Parse(value), token => token.Type == TokenType.Number);
        }

        [Theory]
        [InlineData("AB")]
        [InlineData("A1B")]
        [InlineData("?")]
        [InlineData("^")]
        [InlineData("[")]
        [InlineData("]")]
        [InlineData("{")]
        [InlineData("}")]
        [InlineData("%")]
        public void CreateNewCellToken_Number_ContainsUnknownToken(string value)
        {
            Assert.Contains(Parser.Parse(value), token => token.Type == TokenType.Unknown);
        }
    }
}
