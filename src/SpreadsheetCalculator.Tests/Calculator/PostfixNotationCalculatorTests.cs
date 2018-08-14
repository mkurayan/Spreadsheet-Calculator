using SpreadsheetCalculator.DirectedGraph;
using SpreadsheetCalculator.Calculator;
using SpreadsheetCalculator.Parser;
using System;
using Xunit;

namespace SpreadsheetCalculator.Tests
{
    public class PostfixNotationCalculatorTests
    {
        PostfixNotationCalculator evaluator;
        StringParser stringParser;

        public PostfixNotationCalculatorTests()
        {
            evaluator = new PostfixNotationCalculator();
            stringParser = new StringParser();
        }

        [Fact]
        public void Calculate_SingleValue_VauleNotChanged()
        {
            var tokens = new Token[] { new Token(TokenType.Number, "1") };

            Assert.Equal(1, evaluator.Calculate(tokens));
        }

        [Theory]
        [InlineData("1 1 +", 2)]
        [InlineData("1 1 -", 0)]
        [InlineData("4 2 /", 2)]
        [InlineData("4 2 *", 8)]
        public void Calculate_BinaryOperatorWithTwoOperands_OperationResult(string expression, double expectedValue)
        {
            var tokens = stringParser.Parse(expression);

            Assert.Equal(expectedValue, evaluator.Calculate(tokens));
        }

        [Theory]
        [InlineData("5 1 2 + 4 * 3 - +", 14)]
        [InlineData("4 2 5 * + 1 3 2 * + /", 2)]
        public void Calculate_rpnExpression_ExpectedResult(string expression, double expectedValue)
        {
            var tokens = stringParser.Parse(expression);

            Assert.Equal(expectedValue, evaluator.Calculate(tokens));
        }

        [Theory]
        [InlineData("+")]
        [InlineData("-")]
        [InlineData("/")]
        [InlineData("*")]
        [InlineData("+ 1")]
        [InlineData("- 1")]
        [InlineData("/ 1")]
        [InlineData("* 1")]
        [InlineData("1 +")]
        [InlineData("1 -")]
        [InlineData("1 /")]
        [InlineData("1 *")]
        [InlineData("1 1 + 1")]
        [InlineData("1 1 - 1")]
        [InlineData("1 1 / 1")]
        [InlineData("1 1 * 1")]
        public void Calculate_IncompleteExpression_ThrowCalculationException(string invalidExpression)
        {
            var tokens = stringParser.Parse(invalidExpression);

            Assert.Throws<CalculationException>(() => evaluator.Calculate(tokens));
        }

        [Theory]
        [InlineData("1 + 1")]
        [InlineData("1 - 1")]
        [InlineData("1 / 1")]
        [InlineData("1 * 1")]
        public void Calculate_InfixNotationString_ThrowCalculationException(string invalidExpression)
        {
            var tokens = stringParser.Parse(invalidExpression);

            Assert.Throws<CalculationException>(() => evaluator.Calculate(tokens));
        }

        [Theory]
        [InlineData("x")]
        [InlineData("x + x")]
        [InlineData("1 1 + x +")]
        public void Calculate_UnknownSymbolsInString_ThrowCalculationException(string invalidExpression)
        {
            var tokens = stringParser.Parse(invalidExpression);

            Assert.Throws<CalculationException>(() => evaluator.Calculate(tokens));
        }

        [Theory]
        [InlineData("( 1 )")]
        [InlineData("1 1 + ( 2 2 + ) + ")]
        public void Calculate_ParenthesesInString_ThrowCalculationException(string invalidExpression)
        {
            var tokens = stringParser.Parse(invalidExpression);

            Assert.Throws<CalculationException> (() => evaluator.Calculate(tokens));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("x")]
        public void Calculate_InvalidNumberInToken_ThrowInvalidTokenException(string invalidNumber)
        {
            var tokens = new Token[] { new Token(TokenType.Number, invalidNumber) };

            Assert.Throws<TokenInvalidValueException>(() => evaluator.Calculate(tokens));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("x")]
        [InlineData("1")]
        [InlineData("^")]
        [InlineData("++")]
        [InlineData("--")]
        [InlineData("!")]
        [InlineData("&")]
        [InlineData("%")]
        public void Calculate_InvalidOperatorInToken_ThrowInvalidTokenException(string invalidOperator)
        {
            var tokens = new Token[] { new Token(TokenType.Operator, invalidOperator) };

            Assert.Throws<TokenInvalidValueException>(() => evaluator.Calculate(tokens));
        }

    }
}
