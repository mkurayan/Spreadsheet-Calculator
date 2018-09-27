using SpreadsheetCalculator.ExpressionEngine.Parsing;
using SpreadsheetCalculator.ExpressionEngine.Tokenization;
using Xunit;
using SpreadsheetCalculator.Spreadsheet;

namespace SpreadsheetCalculator.IntegrationTests
{
    public class SpreadsheetTests
    {
        private readonly MathSpreadsheet _spreadsheet;

        public SpreadsheetTests()
        {
            _spreadsheet = new MathSpreadsheet(new Parser(), new Tokenizer());
        }

        [Fact]
        public void Calculate_SpreadsheetWithoutCellReferences_AllCellsCalculated()
        {
            _spreadsheet.SetSize(2, 3);

            for (var row = 1; row <= _spreadsheet.RowsCount; row++)
            {
                for (var col = 1; col <= _spreadsheet.ColumnsCount; col++)
                {
                    _spreadsheet.SetValue(col, row, $"{col} + {row}");
                }
            }

            _spreadsheet.Calculate();

            for (var row = 1; row <= _spreadsheet.RowsCount; row++)
            {
                for (var col = 1; col <= _spreadsheet.ColumnsCount; col++)
                {
                    var result = int.Parse(_spreadsheet.GetValue(col, row));

                    Assert.Equal(col + row, result);
                }
            }
        }

        [Fact]
        public void Calculate_SpreadsheetWithCellReferencesOnly_AllCellsCalculated()
        {
            _spreadsheet.SetSize(3, 2);

            /*
             *        A    |   B    |   C 
             *    _________|________|_______
             *  1 |   1    | A1 1 + | B1 1 +
             *  2 | C1 1 + | A2 1 + | B2 1 +  
             */

            _spreadsheet.SetValue(1, 1, "1");
            _spreadsheet.SetValue(2, 1, "A1 + 1");
            _spreadsheet.SetValue(3, 1, "B1 + 1");
            _spreadsheet.SetValue(1, 2, "C1 + 1");
            _spreadsheet.SetValue(2, 2, "A2 + 1");
            _spreadsheet.SetValue(3, 2, "B2 + 1");

            _spreadsheet.Calculate();

            var expected = 0;

            for (var row = 1; row <= _spreadsheet.RowsCount; row++)
            {
                for (var col = 1; col <= _spreadsheet.ColumnsCount; col++)
                {
                    expected++;

                    Assert.Equal(expected.ToString(), _spreadsheet.GetValue(col, row));
                }
            }
        }


        [Fact]
        public void Calculate_TypicalSpreadsheet_AllCellsCalculated()
        {
            _spreadsheet.SetSize(3, 2);

            /*
             *        A         |   B    |   C 
             *    ______________|________|_______
             *  1 |   B1        | 3 2 *  | A1
             *  2 | A1 B2 / 2 + |   3    | A2 B2 * 3 + 2 /
             *
             * =============================================
             * 
             *        A  |  B  |  C 
             *    _______|_____|_____
             *  1 |   6  |  6  |  6
             *  2 |   4  |  3  |  7.5
             */

            _spreadsheet.SetValue("A1", "B1");
            _spreadsheet.SetValue("B1", "3 * 2");
            _spreadsheet.SetValue("C1", "A1");
            _spreadsheet.SetValue("A2", "A1 / B2 + 2");
            _spreadsheet.SetValue("B2", "3");
            _spreadsheet.SetValue("C2", "(A2 * B2 + 3) / 2 ");

            _spreadsheet.Calculate();

            Assert.Equal("6", _spreadsheet.GetValue("A1"));
            Assert.Equal("6", _spreadsheet.GetValue("B1"));
            Assert.Equal("6", _spreadsheet.GetValue("C1"));
            Assert.Equal("4", _spreadsheet.GetValue("A2"));
            Assert.Equal("3", _spreadsheet.GetValue("B2"));
            Assert.Equal("7.5", _spreadsheet.GetValue("C2"));
        }

        [Fact]
        public void Calculate_SpreadsheetWithCircularReferences_ThrowSpreadsheetInternalException()
        {
            _spreadsheet.SetSize(3, 1);

            /*
             *    |  A   |  B  |   C 
             *    |______|_____|_______
             *  1 |  C1  |  1  |  A1   <--- Circular reference  
             */

            _spreadsheet.SetValue(1, 1, "C1");
            _spreadsheet.SetValue(2, 1, "1");
            _spreadsheet.SetValue(3, 1, "A1");
            
            Assert.Throws<SpreadsheetInternalException>(() => _spreadsheet.Calculate());
        }

        [Fact]
        public void Calculate_InvalidCellReferenceInsSpreadsheet_ShowInvalidValueError()
        {
            _spreadsheet.SetSize(3, 2);

            /*
             *    |   A   |  B  |   C 
             *    |_______|_____|_______
             *  1 |   C1  |  1  |  A0   <--- Invalid Cell reference
             *  2 |   C2  |  2  |  A99999999999999999999999999999999999999999   <--- Invalid Cell reference
             */

            _spreadsheet.SetValue(1, 1, "C1");
            _spreadsheet.SetValue(2, 1, "1");
            _spreadsheet.SetValue(3, 1, "A0");
            _spreadsheet.SetValue(1, 2, "C2");
            _spreadsheet.SetValue(2, 2, "2");
            _spreadsheet.SetValue(3, 2, "A99999999999999999999999999999999999999999");

            _spreadsheet.Calculate();


            Assert.Equal("#VALUE!", _spreadsheet.GetValue(1, 1));
            Assert.Equal("1", _spreadsheet.GetValue(2, 1));
            Assert.Equal("#VALUE!", _spreadsheet.GetValue(3, 1));
            Assert.Equal("#VALUE!", _spreadsheet.GetValue(1, 2));
            Assert.Equal("2", _spreadsheet.GetValue(2, 2));
            Assert.Equal("#VALUE!", _spreadsheet.GetValue(3, 2));
        }

        [Fact]
        public void Calculate_SpreadsheetWithInvalidMathematicalOperation_ShowNumberError()
        {
            _spreadsheet.SetSize(3, 1);

            /*
             *    |  A   |    B    |  C 
             *    |______|_________|_______
             *  1 |  0   |  1 / A1 | A1 / A1   
             */

            _spreadsheet.SetValue(1, 1, "0");
            _spreadsheet.SetValue(2, 1, "1 / A1");
            _spreadsheet.SetValue(3, 1, "A1 / A1");

            _spreadsheet.Calculate();

            Assert.Equal("0", _spreadsheet.GetValue(1, 1));
            Assert.Equal("Infinity", _spreadsheet.GetValue(2, 1));
            Assert.Equal("NaN", _spreadsheet.GetValue(3, 1));
        }

        [Fact]
        public void Calculate_SpreadsheetWithInvalidDataInCell_ShowNumberError()
        {
            _spreadsheet.SetSize(3, 2);

            /*
             *    |    A   |    B    |    C      |
             *    |________|_________|___________|
             *  1 | BlaBla |    A1   |  BlaBla + 1 
             *  2 |  1 + 1 |  1 2 3  |  1 2 + +
             */

            _spreadsheet.SetValue(1, 1, "BlaBla");
            _spreadsheet.SetValue(2, 1, "A1");
            _spreadsheet.SetValue(3, 1, "BlaBla + 1");
            _spreadsheet.SetValue(1, 2, "1 + +");
            _spreadsheet.SetValue(2, 2, "1 2 3");
            _spreadsheet.SetValue(3, 2, "1 2 + +");

            _spreadsheet.Calculate();

            Assert.Equal("#SYNTAX!", _spreadsheet.GetValue(1, 1));
            Assert.Equal("#VALUE!", _spreadsheet.GetValue(2, 1));
            Assert.Equal("#SYNTAX!", _spreadsheet.GetValue(3, 1));
            Assert.Equal("#SYNTAX!", _spreadsheet.GetValue(1, 2));
            Assert.Equal("#SYNTAX!", _spreadsheet.GetValue(2, 2));
            Assert.Equal("#SYNTAX!", _spreadsheet.GetValue(3, 2));
        }

        [Fact]
        public void Calculate_SpreadsheetWithEmptyCell_CellsContainsDefaultValues() 
        {            
            _spreadsheet.SetSize(2, 1);

            /*
             *    |  A  |  B   |   
             *    |_____|______|
             *  1 |     |      |
             */

            _spreadsheet.SetValue(1, 1, "");
            _spreadsheet.SetValue(2, 1, "");

            _spreadsheet.Calculate();

            Assert.Equal("0", _spreadsheet.GetValue(1, 1));
            Assert.Equal("0", _spreadsheet.GetValue(2, 1));
        }
    }
}
