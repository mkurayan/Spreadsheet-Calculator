using Moq;
using SpreadsheetCalculator.ExpressionEngine.SyntaxTree;
using SpreadsheetCalculator.Spreadsheet.CellParsing;
using System;
using Xunit;

namespace SpreadsheetCalculator.Tests.Spreadsheet
{
    public class InvalidCellExpressionTests
    {
        private readonly InvalidCellExpression _invalidExpression;

        public InvalidCellExpressionTests()
        {
            _invalidExpression = new InvalidCellExpression();
        }


        [Fact]
        public void InvalidCellExpression_AllPropertiesHaveExpectedValues()
        {
            Assert.False(_invalidExpression.IsValid);

            Assert.False(_invalidExpression.IsEmpty);

            Assert.Empty(_invalidExpression.CellReferences);
        }

        [Fact]
        public void Calculate_InvalidCellExpression_ThrowInvalidOperationException()
        {
            var resolver = new Mock<IDependencyResolver>();

            Assert.Throws<InvalidOperationException>(() => _invalidExpression.Calculate(resolver.Object));
        }
    }
}
