namespace SpreadsheetCalculator.Spreadsheet.CellParsing
{
    interface ICellParser
    {
        ICellExpression Parse(string text);
    }
}
