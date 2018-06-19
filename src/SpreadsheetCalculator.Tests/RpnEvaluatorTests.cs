using SpreadsheetCalculator.ExpressionEvaluator;
using Xunit;

namespace SpreadsheetCalculator.Tests
{
    public class RpnEvaluatorTests
    {
        RpnEvaluator evaluator;

        public RpnEvaluatorTests()
        {
            evaluator = new RpnEvaluator();
        }

        [Fact]
        public void Evaluate_SingleValue_VauleNotChanged()
        {
            var tokens = new string[] { "1" };

            Assert.True(evaluator.IsValid(tokens));

            Assert.Equal(1, evaluator.Evaluate(tokens));
        }

        [Theory]
        [InlineData("1 1 +", 2)]
        [InlineData("1 1 -", 0)]
        [InlineData("4 2 /", 2)]
        [InlineData("4 2 *", 8)]
        public void Evaluate_BinaryOperatorWithTwoOperands_OperationResult(string expression, double expectedValue)
        {
            var tokens = expression.Split(" ");

            Assert.True(evaluator.IsValid(tokens));

            Assert.Equal(expectedValue, evaluator.Evaluate(tokens));
        }

        [Theory]
        [InlineData("1 ++", 2)]
        [InlineData("1 --", 0)]
        public void Evaluate_UnaryOperatorWithSingleOperand_OperationResult(string expression, double expectedValue)
        {
            var tokens = expression.Split(" ");

            Assert.True(evaluator.IsValid(tokens));

            Assert.Equal(expectedValue, evaluator.Evaluate(tokens));
        }

        [Theory]
        [InlineData("5 1 2 + 4 * 3 - +", 14)]
        [InlineData("4 2 5 * + 1 3 2 * + /", 2)]
        public void Evaluate_rpnExpression_ExpectedResult(string expression, double expectedValue)
        {
            var tokens = expression.Split(" ");

            Assert.True(evaluator.IsValid(tokens));

            Assert.Equal(expectedValue, evaluator.Evaluate(tokens));
        }

        [Theory]
        [InlineData("+")]
        [InlineData("-")]
        [InlineData("/")]
        [InlineData("*")]
        [InlineData("++")]
        [InlineData("--")]
        [InlineData("+ 1")]
        [InlineData("- 1")]
        [InlineData("/ 1")]
        [InlineData("* 1")]
        [InlineData("++ 1")]
        [InlineData("-- 1")]
        [InlineData("1 +")]
        [InlineData("1 -")]
        [InlineData("1 /")]
        [InlineData("1 *")]
        [InlineData("1 1 + 1")]
        [InlineData("1 1 - 1")]
        [InlineData("1 1 / 1")]
        [InlineData("1 1 * 1")]
        public void Validate_IncompleteExpression_ValidationFail(string invalidExpression)
        {
            var tokens = invalidExpression.Split(" ");

            Assert.False(evaluator.IsValid(tokens));
        }

        [Theory]
        [InlineData("1 + 1")]
        [InlineData("1 - 1")]
        [InlineData("1 / 1")]
        [InlineData("1 * 1")]
        public void Validate_InfixNotationString_ValidationFail(string invalidExpression)
        {
            var tokens = invalidExpression.Split(" ");

            Assert.False(evaluator.IsValid(tokens));
        }

        [Theory]
        [InlineData("x")]
        [InlineData("x + x")]
        [InlineData("1 1 + x +")]
        public void Validate_UnknownSymbolsInString_ValidationFail(string invalidExpression)
        {
            var tokens = invalidExpression.Split(" ");

            Assert.False(evaluator.IsValid(tokens));
        }

        [Theory]
        [InlineData("( 1 )")]
        [InlineData("1 1 + ( 2 2 + ) + ")]
        public void Validate_ParenthesesInString_ValidationFail(string invalidExpression)
        {
            var tokens = invalidExpression.Split(" ");

            Assert.False(evaluator.IsValid(tokens));
        }
    }
}
