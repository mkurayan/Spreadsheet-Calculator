using SpreadsheetCalculator.ExpressionCalculator;
using SpreadsheetCalculator.ExpressionParser;
using Xunit;

namespace SpreadsheetCalculator.Tests
{
    public class InfixNotationCalculatorTests
    {
        InfixNotationCalculator calculator;
        StringParser stringParser;

        public InfixNotationCalculatorTests()
        {
            calculator = new InfixNotationCalculator();
            stringParser = new StringParser();
        }

        [Fact]
        public void Calculate_SingleValue_VauleNotChanged()
        {
            var tokens = new Token[] { new Token(TokenType.Number, "1") };

            Assert.Equal(1, calculator.Calculate(tokens));
        }

        [Theory]
        [InlineData("1 + 1", 2)]
        [InlineData("1 - 1", 0)]
        [InlineData("4 / 2", 2)]
        [InlineData("4 * 2", 8)]
        public void Calculate_BinaryOperatorWithTwoOperands_ValidOperationResult(string expression, double expectedValue)
        {
            var tokens = stringParser.Parse(expression);

            Assert.Equal(expectedValue, calculator.Calculate(tokens));
        }

        [Theory]
        [InlineData("4 + 2 * 5", 14)]
        [InlineData("2 * 5 + 4", 14)]
        [InlineData("3 * ( 5 - 3 )", 6)]
        [InlineData("( 10 + 10 ) / ( 8 / 2 )", 5)]
        [InlineData("( ( 3 + 3 ) + ( 2 * 2 ) ) / 2", 5)]
        [InlineData("20 / ( 2 * 2 )", 5)]
        [InlineData("( 1 )  + ( 1 )", 2)]
        public void Calculate_MathFormula_ValidTotal(string expression, double expectedValue)
        {
            var tokens = stringParser.Parse(expression);

            Assert.Equal(expectedValue, calculator.Calculate(tokens));
        }

        [Theory]
        [InlineData("1")]
        [InlineData("1 + 1")]
        [InlineData("1 - 1")]
        [InlineData("1 / 1")]
        [InlineData("1 * 1")]
        [InlineData("( 1 + 1 )")]
        [InlineData("( 1 - 1 )")]
        [InlineData("( 1 / 1 )")]
        [InlineData("( 1 * 1 )")]
        [InlineData("4 + 2 * 5")]
        [InlineData("( ( 3 + 3 ) + ( 2 * 2 ) ) / 2")]
        public void Validate_CorrectExpression_ExpressionValid(string invalidExpression)
        {
            var tokens = stringParser.Parse(invalidExpression);

            Assert.True(calculator.Validate(tokens).isValid);
        }

        [Theory]
        [InlineData(") 1 + 1")]
        [InlineData("1 + 1 (")]
        [InlineData("( 1 + 1 ")]
        [InlineData(" 1 + 1 )")]
        [InlineData("( 1 + 1 ) / ( 1 + 1")]
        public void Validate_ParenthesesMismatch_ExpressionInvalid(string invalidExpression)
        {
            var tokens = stringParser.Parse(invalidExpression);

            Assert.False(calculator.Validate(tokens).isValid);
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
        public void Validate_IncompleteExpression_ExpressionInvalid(string invalidExpression)
        {
            var tokens = stringParser.Parse(invalidExpression);

            Assert.False(calculator.Validate(tokens).isValid);
        }

        [Theory]
        [InlineData("1 1 +")]
        [InlineData("1 1 -")]
        [InlineData("1 1 /")]
        [InlineData("1 1 *")]
        [InlineData("1 1 1 + +")]
        public void Validate_PostfixNotationString_ExpressionInvalid(string invalidExpression)
        {
            var tokens = stringParser.Parse(invalidExpression);

            Assert.False(calculator.Validate(tokens).isValid);
        }

        [Theory]
        [InlineData("+ 1 1")]
        [InlineData("- 1 1")]
        [InlineData("/ 1 1")]
        [InlineData("* 1 1")]
        [InlineData("+ + 1 1 1")]
        public void Validate_PrefixNotationString_ExpressionInvalid(string invalidExpression)
        {
            var tokens = stringParser.Parse(invalidExpression);

            Assert.False(calculator.Validate(tokens).isValid);
        }

        [Theory]
        [InlineData("x")]
        [InlineData("x + x")]
        [InlineData("1 1 + x +")]
        public void Validate_UnknownSymbolsInString_ExpressionInvalid(string invalidExpression)
        {
            var tokens = stringParser.Parse(invalidExpression);

            Assert.False(calculator.Validate(tokens).isValid);
        }
    }
}
