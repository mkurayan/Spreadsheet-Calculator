using SpreadsheetCalculator.ExpressionEvaluator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace SpreadsheetCalculator.Tests
{
    public class SpreadsheetCellTests
    {
        IExpressionEvaluator expressionEvaluator;

        public SpreadsheetCellTests()
        {
            expressionEvaluator = new RpnEvaluator();
        }

        [Fact]
        public void Calculate_MathExpression_CorrectResult()
        {
            SpreadsheetCell cell = new SpreadsheetCell("1 1 +");

            cell.CalculateCell(expressionEvaluator, new Dictionary<string, SpreadsheetCell>());

            Assert.Equal("2.00000", cell.ToString());
        }

        [Theory]
        [InlineData("", new string[0] {})]
        [InlineData("1 1 +", new string[0] { })]
        [InlineData("A1", new string[] { "A1" })]
        [InlineData("A12345", new string[] { "A12345" })]
        [InlineData("A1 A2 +", new string[] { "A1", "A2" })]
        public void GetCellDependencies_ExpressionWithCellReferences_ListOfCellReferences(string expression, string[] cellReferences)
        {
            SpreadsheetCell cell = new SpreadsheetCell(expression);

            var allReffs = cell.GetCellDependencies();

            Assert.Equal(cellReferences.Length, allReffs.Count());

            
            foreach (var reff in cellReferences)
            {
                Assert.Contains(reff, allReffs);
            }
        }


        // ToDo: implemewnt unit tests
        // 1. Validate
        // 2. Calculate

        /*
        public void Validate_ExpressionWithCellReferenceInInvalidState_CellInErrorState(string expression)
        {
            SpreadsheetCell cell = new SpreadsheetCell(expression);


            //var reffs = new Dictionary<string, SpreadsheetCell>();
            //reffs["A1"] = new SpreadsheetCell("Bananan");


            //cell.ValidateCell(expressionEvaluator, )
        }
        */

    }
}
