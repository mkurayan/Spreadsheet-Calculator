using Xunit;
using SpreadsheetCalculator.ExpressionCalculator;
using SpreadsheetCalculator.ExpressionParser;
using SpreadsheetCalculator.Spreadsheet;

namespace SpreadsheetCalculator.IntegrationTests
{
    public class SpreadsheetTests
    {
        PostfixNotationCalculator Calculator { get; }

        StringParser Parser { get; }

        public SpreadsheetTests()
        {
            Calculator = new PostfixNotationCalculator();
            Parser = new StringParser();
        }

        [Fact]
        public void Calculate_SpereadsheetWithoutCellReferences_AllCellsCalculated()
        {
            var spreadsheet = new MathSpreadsheet(Calculator, Parser);

            spreadsheet.SetSize(2, 3);

            for (int row = 1; row <= spreadsheet.RowsCount; row++)
            {
                for (int col = 1; col <= spreadsheet.ColumnsCount; col++)
                {
                    spreadsheet.SetValue(col, row, $"{col} {row} +");
                }
            }

            spreadsheet.Calculate();

            for (int row = 1; row <= spreadsheet.RowsCount; row++)
            {
                for (int col = 1; col <= spreadsheet.ColumnsCount; col++)
                {
                    int result = int.Parse(spreadsheet.GetValue(col, row));

                    Assert.Equal(col + row, result);
                }
            }
        }

        [Fact]
        public void Calculate_SpereadsheetWithCellReferencesOnly_AllCellsCalculated()
        {
            var spreadsheet = new MathSpreadsheet(Calculator, Parser);

            spreadsheet.SetSize(3, 2);

            /*
             *        A    |   B    |   C 
             *    _________|________|_______
             *  1 |   1    | A1 1 + | B1 1 +
             *  2 | C1 1 + | A2 1 + | B2 1 +  
             */

            spreadsheet.SetValue(1, 1, "1");
            spreadsheet.SetValue(2, 1, "A1 1 +");
            spreadsheet.SetValue(3, 1, "B1 1 +");
            spreadsheet.SetValue(1, 2, "C1 1 +");
            spreadsheet.SetValue(2, 2, "A2 1 +");
            spreadsheet.SetValue(3, 2, "B2 1 +");

            spreadsheet.Calculate();

            int expected = 0;

            for (int row = 1; row <= spreadsheet.RowsCount; row++)
            {
                for (int col = 1; col <= spreadsheet.ColumnsCount; col++)
                {
                    expected++;

                    Assert.Equal(expected.ToString(), spreadsheet.GetValue(col, row));
                }
            }
        }


        [Fact]
        public void Calculate_TypicalSpereadsheet_AllCellsCalculated()
        {
            var spreadsheet = new MathSpreadsheet(Calculator, Parser);

            spreadsheet.SetSize(3, 2);

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

            spreadsheet.SetValue("A1", "B1");
            spreadsheet.SetValue("B1", "3 2 *");
            spreadsheet.SetValue("C1", "A1");
            spreadsheet.SetValue("A2", "A1 B2 / 2 +");
            spreadsheet.SetValue("B2", "3");
            spreadsheet.SetValue("C2", "A2 B2 * 3 + 2 / ");

            spreadsheet.Calculate();

            Assert.Equal("6", spreadsheet.GetValue("A1"));
            Assert.Equal("6", spreadsheet.GetValue("B1"));
            Assert.Equal("6", spreadsheet.GetValue("C1"));
            Assert.Equal("4", spreadsheet.GetValue("A2"));
            Assert.Equal("3", spreadsheet.GetValue("B2"));
            Assert.Equal("7.5", spreadsheet.GetValue("C2"));
        }

        [Fact]
        public void Calculate_SpereadsheetWithCircularReferences_ThrowSpreadsheetInternallException()
        {
            var spreadsheet = new MathSpreadsheet(Calculator, Parser);

            spreadsheet.SetSize(3, 1);

            /*
             *    |  A   |  B  |   C 
             *    |______|_____|_______
             *  1 |  C1  |  1  |  A1   <--- Circular reference  
             */

            spreadsheet.SetValue(1, 1, "C1");
            spreadsheet.SetValue(2, 1, "1");
            spreadsheet.SetValue(3, 1, "A1");
            
            Assert.Throws<SpreadsheetInternallException>(() => spreadsheet.Calculate());
        }

        [Fact]
        public void Calculate_InvalidCellReferenceInsSpereadsheet_ShowInvalidValueError()
        {
            var spreadsheet = new MathSpreadsheet(Calculator, Parser);

            spreadsheet.SetSize(3, 2);

            /*
             *    |   A   |  B  |   C 
             *    |_______|_____|_______
             *  1 |   C1  |  1  |  A0   <--- Invalid Cell reference
             *  2 |   C2  |  2  |  A99999999999999999999999999999999999999999   <--- Invalid Cell reference
             */

            spreadsheet.SetValue(1, 1, "C1");
            spreadsheet.SetValue(2, 1, "1");
            spreadsheet.SetValue(3, 1, "A0");
            spreadsheet.SetValue(1, 2, "C2");
            spreadsheet.SetValue(2, 2, "2");
            spreadsheet.SetValue(3, 2, "A99999999999999999999999999999999999999999");

            spreadsheet.Calculate();


            Assert.Equal("#VALUE!", spreadsheet.GetValue(1, 1));
            Assert.Equal("1", spreadsheet.GetValue(2, 1));
            Assert.Equal("#VALUE!", spreadsheet.GetValue(3, 1));
            Assert.Equal("#VALUE!", spreadsheet.GetValue(1, 2));
            Assert.Equal("2", spreadsheet.GetValue(2, 2));
            Assert.Equal("#VALUE!", spreadsheet.GetValue(3, 2));
        }

        [Fact]
        public void Calculate_SpereadsheetWithInvalidMathematicalOperation_ShowNumberError()
        {
            var spreadsheet = new MathSpreadsheet(Calculator, Parser);

            spreadsheet.SetSize(3, 1);

            /*
             *    |  A   |    B    |  C 
             *    |______|_________|_______
             *  1 |  0   |  2 A1 / |  B1 1 +   
             */

            spreadsheet.SetValue(1, 1, "0");
            spreadsheet.SetValue(2, 1, "2 A1 /");
            spreadsheet.SetValue(3, 1, "B1 1 + ");

            spreadsheet.Calculate();

            Assert.Equal("0", spreadsheet.GetValue(1, 1));
            Assert.Equal("#NUM!", spreadsheet.GetValue(2, 1));
            Assert.Equal("#VALUE!", spreadsheet.GetValue(3, 1));
        }

        [Fact]
        public void Calculate_SpereadsheetWithInvalidDataInCell_ShowNumberError()
        {
            var spreadsheet = new MathSpreadsheet(Calculator, Parser);

            spreadsheet.SetSize(3, 2);

            /*
             *    |    A   |    B    |    C      |
             *    |________|_________|___________|
             *  1 | BlaBla |    A1   |  BlaBla 1 + 
             *  2 |  1 + 1 |  1 2 3  |  1 2 + +
             */

            spreadsheet.SetValue(1, 1, "BlaBla");
            spreadsheet.SetValue(2, 1, "A1");
            spreadsheet.SetValue(3, 1, "BlaBla 1 + ");
            spreadsheet.SetValue(1, 2, "1 + 1");
            spreadsheet.SetValue(2, 2, "1 2 3");
            spreadsheet.SetValue(3, 2, "1 2 + +");

            spreadsheet.Calculate();

            Assert.Equal("#VALUE!", spreadsheet.GetValue(1, 1));
            Assert.Equal("#VALUE!", spreadsheet.GetValue(2, 1));
            Assert.Equal("#VALUE!", spreadsheet.GetValue(3, 1));
            Assert.Equal("#VALUE!", spreadsheet.GetValue(1, 2));
            Assert.Equal("#VALUE!", spreadsheet.GetValue(2, 2));
            Assert.Equal("#VALUE!", spreadsheet.GetValue(3, 2));
        }

        [Fact]
        public void Calculate_SpereadsheetWithEmptyCell_CellsContainsDefaultValues() 
        {
            var spreadsheet = new MathSpreadsheet(Calculator, Parser);

            spreadsheet.SetSize(2, 1);

            /*
             *    |  A  |  B   |   
             *    |_____|______|
             *  1 |     |      |
             */

            spreadsheet.SetValue(1, 1, "");
            spreadsheet.SetValue(2, 1, "");

            spreadsheet.Calculate();

            Assert.Equal("0", spreadsheet.GetValue(1, 1));
            Assert.Equal("0", spreadsheet.GetValue(2, 1));
        }
    }
}
