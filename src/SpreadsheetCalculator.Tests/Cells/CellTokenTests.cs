using SpreadsheetCalculator.Cells;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SpreadsheetCalculator.Tests.Cells
{
    public class CellTokenTests
    {
        [Theory]
        [InlineData("1")]
        [InlineData("11")]
        [InlineData("+")]
        [InlineData("1 + 1")]
        [InlineData("SomeText")]
        [InlineData("A")]
        [InlineData("AA2")] // Looks like Ref but we support only one letter in refference.
        public void CreateNewCellToken_NotCellReferenceToken_TokenIsNotCellReference(string token)
        {
            CellToken cellToken = new CellToken(token);

            Assert.False(cellToken.IsCellReference);
        }


        [Theory]
        [InlineData("A1")]
        [InlineData("A123456789")]
        public void CreateNewCellToken_CellReferenceToken_TokenIsCellReference(string token)
        {
            CellToken cellToken = new CellToken(token);

            Assert.True(cellToken.IsCellReference);
        }
    }
}
