using System.Linq;
using SpreadsheetCalculator.ExpressionEngine;
using SpreadsheetCalculator.ExpressionEngine.SyntaxAnalysis;
using SpreadsheetCalculator.ExpressionEngine.SyntaxAnalysis.InfixNotation;
using Xunit;

namespace SpreadsheetCalculator.Tests.ExpressionEngine
{
   
    public class InfixNotationTokenizerTests
    {
        private InfixNotationTokenizer Tokenizer { get; }

        public InfixNotationTokenizerTests()
        {
            Tokenizer = new InfixNotationTokenizer();
        }

        [Theory]
        [InlineData("")]
        [InlineData("    ")]
        public void Tokenize_EmptyString_EmptyTokensList(string value)
        {
            Assert.Empty(Tokenizer.Tokenize(value));
        }

        [Theory]
        [InlineData("", 0)]
        [InlineData("    ", 0)]
        [InlineData("1", 1)]
        [InlineData("11", 1)]
        [InlineData("A1", 1)]
        [InlineData("1 + 1", 3)]
        [InlineData("   1    +    1   ", 3)]
        [InlineData("( 1 + A1 ) / 2", 7)]
        public void Tokenize_ExpressionWithSeveralTokens_CorrectTokensCount(string value, int expectedTokenCount)
        {
            var tokens = Tokenizer.Tokenize(value);

            Assert.Equal(expectedTokenCount, tokens.Length);
        }

        [Theory]        
        [InlineData("?")]
        [InlineData("!")]
        [InlineData("%")]
        [InlineData("&")]
        [InlineData("[")]
        [InlineData("]")]
        [InlineData("{")]
        [InlineData("}")]
        [InlineData("a")]
        [InlineData("A")]
        [InlineData("z")]
        [InlineData("zZ")]
        [InlineData("some text")]
        [InlineData("A1A")]
        [InlineData("+A")]
        [InlineData("(A)")]
        [InlineData("1+A")]
        [InlineData("1+?")]
        [InlineData("(1+2)/e")]
        [InlineData("A1.A")]
        [InlineData(".")]
        [InlineData(".A")]
        [InlineData(".(")]
        [InlineData(".+")]
        [InlineData("(.)")]
        [InlineData("(.a)")]
        public void Tokenize_ExpressionContainsInvalidSymbols_ThrowSyntaxException(string value)
        {
            Assert.Throws<SyntaxException>(() => Tokenizer.Tokenize(value));
        }
       
        [Theory]
        [InlineData("1 + 1 - 1 * 1 / 1", 0)]
        [InlineData("44", 0)]
        [InlineData("A1", 1)]
        [InlineData("AA1", 1)]
        [InlineData("ABCD1", 1)]
        [InlineData("A123456789", 1)]
        [InlineData("1 + A1", 1)]
        [InlineData("9 / A1", 1)]
        [InlineData("(A1)", 1)]
        [InlineData("(A1 + B2) / 3", 2)]
        [InlineData("9 / (A1 + b7)", 2)]
        public void Tokenize_MathExpression_ContainsExpectedCellReferencesCount(string value, int expectedCount)
        {
            var tokens = Tokenizer.Tokenize(value);

            Assert.Equal(expectedCount, tokens.Count(token => token.Type == TokenType.CellReference));
        }

        [Theory]
        [InlineData("", 0)]
        [InlineData("A1", 0)]
        [InlineData("A1+B1", 0)]
        [InlineData("1", 1)]
        [InlineData("22", 1)]
        [InlineData("123456789", 1)]
        [InlineData("0.55", 1)]
        [InlineData("-0.55", 1)]
        [InlineData("-123456789", 1)]
        [InlineData("1+1", 2)]
        [InlineData("1 + 1 - 1 * 1 / 1", 5)]
        [InlineData("A1 + 1", 1)]
        [InlineData("1 + A1", 1)]
        [InlineData("-1 - -A1", 1)]
        [InlineData("9 / (A1 + b7)", 1)]
        [InlineData("9 / (2 + b7)", 2)]
        public void Tokenize_MathExpression_ContainsExpectedTokensWithNumber(string value, int expectedCount)
        {
            var tokens = Tokenizer.Tokenize(value);

            Assert.Equal(expectedCount, tokens.Count(token => token.Type == TokenType.Number));
        }

        [Theory]
        [InlineData("+", TokenType.Add)]
        [InlineData("-", TokenType.Subtract)]
        [InlineData("*", TokenType.Multiply)]
        [InlineData("/", TokenType.Divide)]
        [InlineData("(", TokenType.OpenParenthesis)]
        [InlineData(")", TokenType.CloseParenthesis)]
        public void CreateNewCellToken_SingleSymbol_ValidToken(string value, TokenType expectedType)
        {
            Assert.Equal(expectedType, Tokenizer.Tokenize(value).First().Type);
        }

        [Theory]
        [InlineData("", 0)]
        [InlineData("A1", 0)]
        [InlineData("A1+11", 0)]
        [InlineData("0.55", 0)]
        [InlineData("1 + 1 - 1 * 1 / 1", 0)]
        [InlineData("(A1)", 1)]
        [InlineData("(1) + (A1)", 2)]
        [InlineData("((1) - (A1))", 3)]
        [InlineData("((1) - (A1))/(4 * (z7+f4))", 5)]
        public void Tokenize_MathExpression_ContainsExpectedParenthesisNumber(string value, int expectedCount)
        {
            var tokens = Tokenizer.Tokenize(value);

            Assert.Equal(expectedCount, tokens.Count(token => token.Type == TokenType.OpenParenthesis));
            Assert.Equal(expectedCount, tokens.Count(token => token.Type == TokenType.CloseParenthesis));
        }
    }
}
