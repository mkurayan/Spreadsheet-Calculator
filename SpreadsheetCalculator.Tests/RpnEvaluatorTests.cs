using SpreadsheetCalculator.ExpressionEvaluator;
using Xunit;

namespace SpreadsheetCalculator.Tests
{
    public class RpnEvaluatorTests
    {
        [Fact]
        public void Evaluate_SingleValue_VauleNotChanged()
        {
            Assert.Equal(1, (new RpnEvaluator()).Evaluate(new string[] { "1" }));
        }

        [Theory]
        [InlineData("1 1 +", 2)]
        [InlineData("1 1 -", 0)]
        [InlineData("4 2 /", 2)]
        [InlineData("4 2 *", 8)]
        public void Evaluate_BinaryOperatorWithTwoOperands_OperationResult(string expression, double expectedValue)
        {
            var tokens = expression.Split(" ");
            Assert.Equal(expectedValue, (new RpnEvaluator()).Evaluate(tokens));
        }

        [Theory]
        [InlineData("1 ++", 2)]
        [InlineData("1 --", 0)]
        public void Evaluate_UnaryOperatorWithSingleOperand_OperationResult(string expression, double expectedValue)
        {
            var tokens = expression.Split(" ");
            Assert.Equal(expectedValue, (new RpnEvaluator()).Evaluate(tokens));
        }

        [Theory]
        [InlineData("5 1 2 + 4 * 3 - +", 14)]
        [InlineData("4 2 5 * + 1 3 2 * + /", 2)]
        public void Evaluate_rpnExpression_ExpectedResult(string expression, double expectedValue)
        {
            var tokens = expression.Split(" ");     
            Assert.Equal(expectedValue, (new RpnEvaluator()).Evaluate(tokens));
        }
    }
}
