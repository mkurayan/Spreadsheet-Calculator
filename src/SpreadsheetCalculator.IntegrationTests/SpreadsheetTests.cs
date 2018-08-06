using SpreadsheetCalculator.Exceptions;
using SpreadsheetCalculator.ExpressionCalculator;
using Xunit;

namespace SpreadsheetCalculator.IntegrationTests
{
    public class SpreadsheetTests
    {
        [Fact]
        public void Calculate_SpereadsheetWithoutCellReferences_AllCellsCalculated()
        {
            var spreadsheet = new Spreadsheet(2, 3, new RpnCalculator());

            for (int row = 0; row < spreadsheet.RowNumber; row++)
            {
                for (int col = 0; col < spreadsheet.ColumnNumber; col++)
                {
                    spreadsheet.SetCell(col, row, $"{col} {row} +");
                }
            }

            spreadsheet.Calculate();

            for (int row = 0; row < spreadsheet.RowNumber; row++)
            {
                for (int col = 0; col < spreadsheet.ColumnNumber; col++)
                {
                    int result = int.Parse(spreadsheet.GetCell(col, row));

                    Assert.Equal(col + row, result);
                }
            }
        }

        [Fact]
        public void Calculate_SpereadsheetWithCellReferencesOnly_AllCellsCalculated()
        {
            var spreadsheet = new Spreadsheet(3, 2, new RpnCalculator());

            /*
             *        A    |   B    |   C 
             *    _________|________|_______
             *  1 |   1    | A1 1 + | B1 1 +
             *  2 | C1 1 + | A2 1 + | B2 1 +  
             */

            spreadsheet.SetCell(0, 0, "1");
            spreadsheet.SetCell(1, 0, "A1 1 +");
            spreadsheet.SetCell(2, 0, "B1 1 +");
            spreadsheet.SetCell(0, 1, "C1 1 +");
            spreadsheet.SetCell(1, 1, "A2 1 +");
            spreadsheet.SetCell(2, 1, "B2 1 +");

            spreadsheet.Calculate();

            int expected = 0;

            for (int row = 0; row < spreadsheet.RowNumber; row++)
            {
                for (int col = 0; col < spreadsheet.ColumnNumber; col++)
                {
                    expected++;

                    Assert.Equal(expected.ToString(), spreadsheet.GetCell(col, row));
                }
            }
        }


        [Fact]
        public void Calculate_TypicalSpereadsheet_AllCellsCalculated()
        {
            var spreadsheet = new Spreadsheet(3, 2, new RpnCalculator());

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

            spreadsheet.SetCell(0, 0, "B1");
            spreadsheet.SetCell(1, 0, "3 2 *");
            spreadsheet.SetCell(2, 0, "A1");
            spreadsheet.SetCell(0, 1, "A1 B2 / 2 +");
            spreadsheet.SetCell(1, 1, "3");
            spreadsheet.SetCell(2, 1, "A2 B2 * 3 + 2 / ");

            spreadsheet.Calculate();

            Assert.Equal("6", spreadsheet.GetCell(0, 0));
            Assert.Equal("6", spreadsheet.GetCell(1, 0));
            Assert.Equal("6", spreadsheet.GetCell(2, 0));
            Assert.Equal("4", spreadsheet.GetCell(0, 1));
            Assert.Equal("3", spreadsheet.GetCell(1, 1));
            Assert.Equal("7.5", spreadsheet.GetCell(2, 1));
        }

        [Fact]
        public void Calculate_SpereadsheetWithCircularReferences_ThrowCyclicDependencyException()
        {
            var spreadsheet = new Spreadsheet(3, 1, new RpnCalculator());

            /*
             *    |  A   |  B  |   C 
             *    |______|_____|_______
             *  1 |  C1  |  1  |  A1   <--- Circulal reference  
             */

            spreadsheet.SetCell(0, 0, "C1");
            spreadsheet.SetCell(1, 0, "1");
            spreadsheet.SetCell(2, 0, "A1");
            
            Assert.Throws<CyclicDependencyException>(() => spreadsheet.Calculate());
        }

        [Fact]
        public void Calculate_InvalidCellReferenceInsSpereadsheet_ShowInvalidValueError()
        {
            var spreadsheet = new Spreadsheet(3, 2, new RpnCalculator());

            /*
             *    |   A   |  B  |   C 
             *    |_______|_____|_______
             *  1 |   C1  |  1  |  A0   <--- Invalid Cell reference
             *  2 |   C2  |  2  |  A99999999999999999999999999999999999999999   <--- Invalid Cell reference
             */

            spreadsheet.SetCell(0, 0, "C1");
            spreadsheet.SetCell(1, 0, "1");
            spreadsheet.SetCell(2, 0, "A0");
            spreadsheet.SetCell(0, 1, "C2");
            spreadsheet.SetCell(1, 1, "2");
            spreadsheet.SetCell(2, 1, "A99999999999999999999999999999999999999999");

            spreadsheet.Calculate();


            Assert.Equal("#VALUE!", spreadsheet.GetCell(0, 0));
            Assert.Equal("1", spreadsheet.GetCell(1, 0));
            Assert.Equal("#VALUE!", spreadsheet.GetCell(2, 0));
            Assert.Equal("#VALUE!", spreadsheet.GetCell(0, 1));
            Assert.Equal("2", spreadsheet.GetCell(1, 1));
            Assert.Equal("#VALUE!", spreadsheet.GetCell(2, 1));
        }

        [Fact]
        public void Calculate_SpereadsheetWithInvalidMathematicalOperation_ShowNumberError()
        {
            var spreadsheet = new Spreadsheet(3, 1, new RpnCalculator());

            /*
             *    |  A   |    B    |  C 
             *    |______|_________|_______
             *  1 |  0   |  2 A1 / |  B1 1 +   
             */

            spreadsheet.SetCell(0, 0, "0");
            spreadsheet.SetCell(1, 0, "2 A1 /");
            spreadsheet.SetCell(2, 0, "B1 1 + ");

            spreadsheet.Calculate();

            Assert.Equal("0", spreadsheet.GetCell(0, 0));
            Assert.Equal("#NUM!", spreadsheet.GetCell(1, 0));
            Assert.Equal("#NUM!", spreadsheet.GetCell(2, 0));
        }

        [Fact]
        public void Calculate_SpereadsheetWithInvalidDataInCell_ShowNumberError()
        {
            var spreadsheet = new Spreadsheet(3, 2, new RpnCalculator());

            /*
             *    |    A   |    B    |    C      |
             *    |________|_________|___________|
             *  1 | BlaBla |    A1   |  BlaBla 1 + 
             *  2 |  1 + 1 |  1 2 3  |  1 2 + +
             */

            spreadsheet.SetCell(0, 0, "BlaBla");
            spreadsheet.SetCell(1, 0, "A1");
            spreadsheet.SetCell(2, 0, "BlaBla 1 + ");
            spreadsheet.SetCell(0, 1, "1 + 1");
            spreadsheet.SetCell(1, 1, "1 2 3");
            spreadsheet.SetCell(2, 1, "1 2 + +");

            spreadsheet.Calculate();

            Assert.Equal("#VALUE!", spreadsheet.GetCell(0, 0));
            Assert.Equal("#VALUE!", spreadsheet.GetCell(1, 0));
            Assert.Equal("#VALUE!", spreadsheet.GetCell(2, 0));
            Assert.Equal("#VALUE!", spreadsheet.GetCell(0, 1));
            Assert.Equal("#VALUE!", spreadsheet.GetCell(1, 1));
            Assert.Equal("#VALUE!", spreadsheet.GetCell(2, 1));
        }

        [Fact]
        public void Calculate_SpereadsheetWithEmptyCell_CellsContainsDefaultValues() 
        {
            var spreadsheet = new Spreadsheet(2, 1, new RpnCalculator());

            /*
             *    |  A  |  B   |   
             *    |_____|______|
             *  1 |     |      |
             */

            spreadsheet.SetCell(0, 0, "");
            spreadsheet.SetCell(1, 0, "");

            spreadsheet.Calculate();

            Assert.Equal("0", spreadsheet.GetCell(0, 0));
            Assert.Equal("0", spreadsheet.GetCell(1, 0));
        }
    }
}
