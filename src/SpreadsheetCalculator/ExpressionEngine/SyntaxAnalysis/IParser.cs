using SpreadsheetCalculator.ExpressionEngine.SyntaxTree;


namespace SpreadsheetCalculator.ExpressionEngine.SyntaxAnalysis
{
    internal interface IParser
    {
        INode Parse(Token[] tokens);
    }
}
