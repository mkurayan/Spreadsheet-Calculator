namespace SpreadsheetCalculator.ExpressionParser
{
    enum TokenType
    {
        /// <summary>
        /// Token contains number.
        /// </summary>
        Number,

        /// <summary>
        /// Token contains reference to cell in spreadsheet.
        /// </summary>
        CellReference,

        /// <summary>
        /// Token contains one of the supported mathematical operators.
        /// </summary>
        Operator,

        /// <summary>
        /// Token contains round bracket.
        /// </summary>
        RoundBracket,

        /// <summary>
        /// Token contains unknown expression. 
        /// </summary>
        Unknown
    }
}
