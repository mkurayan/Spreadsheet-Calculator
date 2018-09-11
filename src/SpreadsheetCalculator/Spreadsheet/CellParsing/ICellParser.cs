namespace SpreadsheetCalculator.Spreadsheet.CellParsing
{
    internal interface ICellParser
    {
        ICellExpression Parse(string text);
    }
}
