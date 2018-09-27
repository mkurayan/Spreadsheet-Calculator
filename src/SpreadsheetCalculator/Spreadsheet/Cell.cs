using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SpreadsheetCalculator.ExpressionEngine;
using SpreadsheetCalculator.ExpressionEngine.Parsing;
using SpreadsheetCalculator.ExpressionEngine.Parsing.SyntaxTree;
using SpreadsheetCalculator.ExpressionEngine.Tokenization;

namespace SpreadsheetCalculator.Spreadsheet
{
    /// <summary>
    /// Concrete cell in the spreadsheet.
    /// </summary>
    internal class Cell : ICell
    {
        public double? CellValue { get; private set; }

        public CellState CellState { get; private set; }

        public IReadOnlyCollection<string> CellDependencies { get; }

        private readonly INode _syntaxTree;

        private readonly bool _isEmpty;

        private readonly string _value;

        /// <summary>
        /// Create new SpreadsheetCell.
        /// </summary>
        /// <param name="parser">Parser, parse tokens to syntax tree.</param>
        /// <param name="tokenizer">Tokenizer, split string to tokens.</param>
        /// <param name="value">Math expression.</param>
        public Cell(IParser parser, ITokenizer tokenizer, string value)
        {
            _value = value;

            CellState = CellState.Pending;

            try
            {
                var tokens = tokenizer.Tokenize(value);

                _isEmpty = tokens.Length == 0;

                CellDependencies = tokens
                    .Where(token => token.Type == TokenType.CellReference)
                    .Select(token => token.Value)
                    .ToHashSet();
                
                _syntaxTree = parser.Parse(tokens);
            }
            catch (SyntaxException)
            {
                CellState = CellState.SyntaxError;

                // Empty..
                CellDependencies = new string[0];

                _syntaxTree = null;
            }
        }

        /// <summary>
        /// Calculate cell in spreadsheet,
        /// </summary>
        /// <param name="cellDependencies">Dictionary with all cell dependencies (references to another cells)</param>
        public void Calculate(Dictionary<string, ICell> cellDependencies)
        {
            if (CellState == CellState.Pending)
            {
                if (_isEmpty)
                {
                    SetValue(0);
                    return;
                }
            
                if (cellDependencies.Any(c => c.Value == null))
                {
                    SetError();
                    return;
                }

                if (cellDependencies.Any(c => c.Value.CellState == CellState.Pending))
                {
                    throw new SpreadsheetInternalException("Calculation flow error, one of the cell references were missed.");
                }

                if (cellDependencies.Any(c => c.Value.CellState != CellState.Calculated))
                {
                    SetError();
                    return;
                }

                SetValue(_syntaxTree.Evaluate(new DependencyResolver(cellDependencies)));
            }

            void SetError()
            {
                CellState = CellState.CellValueError;
                CellValue = null;
            }

            void SetValue(double value)
            {
                CellState = CellState.Calculated;
                CellValue = value;
            }
        }

        /// <summary>
        /// Provide information about spreadsheet cell value.
        /// </summary>
        /// <returns>Cell value</returns>
        public override string ToString()
        {
            switch (CellState)
            {
                case CellState.Pending:
                    // In Pending state when cell not calculated yet, we return original cell value.
                    return _value;
                case CellState.Calculated:
                    if (CellValue.HasValue)
                    {
                        return CellValue.Value.ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        throw new InvalidOperationException("Cell in inconsistent state.");
                    }
                case CellState.CellValueError:
                    return "#VALUE!";
                case CellState.SyntaxError:
                    return "#SYNTAX!";
                default:
                    throw new NotImplementedException($"Unknown cell state: {CellState}");
            }
        }

        private class DependencyResolver : IDependencyResolver
        {
            private readonly Dictionary<string, ICell> _cellDependencies;

            public DependencyResolver(Dictionary<string, ICell> cellDependencies)
            {
                _cellDependencies = cellDependencies;
            }

            public double ResolveCellReference(string key)
            {
                if (_cellDependencies.TryGetValue(key, out var cell) && cell.CellState == CellState.Calculated && cell.CellValue.HasValue)
                {
                    return cell.CellValue.Value;
                }

                throw new SpreadsheetInternalException("Calculation flow error, cannot resolve cell reference.");
            }
        }
    }
}
