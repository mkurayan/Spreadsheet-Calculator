using SpreadsheetCalculator.ExpressionEngine;
using SpreadsheetCalculator.ExpressionEngine.SyntaxAnalysis;
using System;

namespace SpreadsheetCalculator.Spreadsheet.CellParsing
{
    class CellParser : ICellParser
    {
        private readonly IExpressionFactory _expressionFactory;

        public CellParser(IExpressionFactory expressionFactory)
        {
            _expressionFactory = expressionFactory ?? throw new ArgumentNullException(nameof(expressionFactory));
        }

        public ICellExpression Parse(string text)
        {
            try
            {
                var tokens = _expressionFactory.CreateTokenizer().Tokenize(text);

                var syntaxTree = _expressionFactory.CreateParser().Parse(tokens);

                return new ValidCellExpression(tokens, syntaxTree);

            }
            catch (SyntaxException)
            {
                return new InvalidCellExpression();
            }
        }
    }
}
