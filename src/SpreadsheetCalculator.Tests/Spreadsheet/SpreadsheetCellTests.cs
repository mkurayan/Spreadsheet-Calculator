using System.Collections.Generic;
using Moq;
using SpreadsheetCalculator.ExpressionEngine.SyntaxTree;
using SpreadsheetCalculator.Spreadsheet;
using SpreadsheetCalculator.Spreadsheet.CellParsing;
using Xunit;

namespace SpreadsheetCalculator.Tests.Spreadsheet
{
    public class SpreadsheetCellTests
    {
        private readonly Mock<ICellExpression> _cellExpression;

        public SpreadsheetCellTests()
        {
            _cellExpression = new Mock<ICellExpression>();
        }


        [Fact]
        public void CreateNewSpreadsheetCell_ValidExpression_CellCreatedInPendingState()
        {
            _cellExpression.Setup(foo => foo.IsValid).Returns(true);

            var cell = new Cell(_cellExpression.Object);

            Assert.Equal(CellState.Pending, cell.CellState);

            Assert.Equal("#PENDING!", cell.ToString());

            Assert.False(cell.CellValue.HasValue);
        }

        [Fact]
        public void CreateNewSpreadsheetCell_InvalidExpression_CellCreatedInErrorState()
        {
            _cellExpression.Setup(foo => foo.IsValid).Returns(false);

            var cell = new Cell(_cellExpression.Object);

            Assert.Equal(CellState.SyntaxError, cell.CellState);

            Assert.Equal("#SYNTAX!", cell.ToString());

            Assert.False(cell.CellValue.HasValue);
        }

        [Fact]
        public void Calculate_InvalidExpression_CellRemainsInErrorState()
        {
            _cellExpression.Setup(foo => foo.IsValid).Returns(false);

            var cell = new Cell(_cellExpression.Object);

            Assert.Equal(CellState.SyntaxError, cell.CellState);

            cell.Calculate(null);

            Assert.Equal(CellState.SyntaxError, cell.CellState);

            Assert.False(cell.CellValue.HasValue);

            Assert.Equal("#SYNTAX!", cell.ToString());
        }

        [Fact]
        public void Calculate_EmptyExpression_CellValueSetToZero()
        {
            _cellExpression.Setup(foo => foo.IsValid).Returns(true);
            _cellExpression.Setup(foo => foo.IsEmpty).Returns(true);
            _cellExpression.Setup(foo => foo.Calculate(It.IsAny<IDependencyResolver>())).Returns(0);

            var cell = new Cell(_cellExpression.Object);

            cell.Calculate(new Dictionary<string, ICell>());

            Assert.Equal(CellState.Calculated, cell.CellState);

            Assert.Equal(0, cell.CellValue.Value);

            Assert.Equal("0", cell.ToString());
        }

        [Fact]
        public void Calculate_CellReferenceMissed_CellInErrorState()
        {
            _cellExpression.Setup(foo => foo.IsValid).Returns(true);
            _cellExpression.Setup(foo => foo.IsEmpty).Returns(false);

            var cell = new Cell(_cellExpression.Object);

            cell.Calculate(new Dictionary<string, ICell> { [""] = null });

            Assert.Equal(CellState.CellValueError, cell.CellState);

            Assert.False(cell.CellValue.HasValue);

            Assert.Equal("#VALUE!", cell.ToString());
        }

        [Theory]
        [InlineData(CellState.SyntaxError)]
        [InlineData(CellState.CellValueError)]
        public void Calculate_CellReferenceHasErrorState_CellInErrorState(CellState errorState)
        {
            _cellExpression.Setup(foo => foo.IsValid).Returns(true);
            _cellExpression.Setup(foo => foo.IsEmpty).Returns(false);

            var cell = new Cell(_cellExpression.Object);

            var cellWithError = new Mock<ICell>();
            cellWithError.Setup(foo => foo.CellState).Returns(errorState);

            cell.Calculate(new Dictionary<string, ICell> { [""] = cellWithError.Object });

            Assert.Equal(CellState.CellValueError, cell.CellState);

            Assert.False(cell.CellValue.HasValue);

            Assert.Equal("#VALUE!", cell.ToString());
        }
    }
}
