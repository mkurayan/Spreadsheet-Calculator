namespace SpreadsheetCalculator.ExpressionEngine.Tokenization
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
