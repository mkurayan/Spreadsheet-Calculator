using SpreadsheetCalculator.ExpressionEngine.SyntaxAnalysis;
using SpreadsheetCalculator.ExpressionEngine.SyntaxTree;
using System.Collections.Generic;
using System.Linq;

namespace SpreadsheetCalculator.Spreadsheet.CellParsing
{
    class ValidCellExpression : ICellExpression
    {
        private readonly INode _treeTop;

        public bool IsValid => true;

        public bool IsEmpty { get; }

        public IEnumerable<string> CellReferences { get; }

        public double Calculate(IDependencyResolver resolver)
        {
            if (IsEmpty)
            {
                // If expression is empty, nothing to calculate.
                return 0;
            }

            return _treeTop.Evaluate(resolver);
        }

        public ValidCellExpression(IEnumerable<Token> tokens, INode treeTop)
        {
            IsEmpty = !tokens.Any();

            CellReferences = IsEmpty ?
                Enumerable.Empty<string>() :
                tokens.Where(token => token.Type == TokenType.CellReference).Select(token => token.Value).ToList();

            _treeTop = treeTop;
        }
    }
}
