using SpreadsheetCalculator.ExpressionEngine.SyntaxAnalysis;
using SpreadsheetCalculator.ExpressionEngine.SyntaxTree;
using System.Collections.Generic;
using System.Linq;

namespace SpreadsheetCalculator.Spreadsheet.CellParsing
{
    internal class ValidCellExpression : ICellExpression
    {
        private readonly INode _treeTop;

        public bool IsValid => true;

        public bool IsEmpty { get; }

        public HashSet<string> CellReferences { get; }

        public double Calculate(IDependencyResolver resolver)
        {
            if (IsEmpty)
            {
                // If expression is empty, nothing to calculate.
                return 0;
            }

            return _treeTop.Evaluate(resolver);
        }

        public ValidCellExpression(IReadOnlyCollection<Token> tokens, INode treeTop)
        {
            IsEmpty = tokens.Count == 0;

            CellReferences = tokens.Where(token => token.Type == TokenType.CellReference).Select(token => token.Value).ToHashSet();

            _treeTop = treeTop;
        }
    }
}
