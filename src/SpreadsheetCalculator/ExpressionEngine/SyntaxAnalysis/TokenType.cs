namespace SpreadsheetCalculator.ExpressionEngine.SyntaxAnalysis
{
    public enum TokenType
    {
        Number,

        CellReference,

        Add,

        Subtract,

        Multiply,

        Divide,

        OpenParenthesis,

        CloseParenthesis,
        
        EndOfExpression
    }
}
