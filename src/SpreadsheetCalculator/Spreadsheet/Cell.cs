using SpreadsheetCalculator.ExpressionEngine.SyntaxTree;
using SpreadsheetCalculator.Spreadsheet.CellParsing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpreadsheetCalculator.Spreadsheet
{
    /// <summary>
    /// Concrete cell in the spreadsheet.
    /// </summary>
    class Cell : ICell
    {
        public double? CellValue { get; private set; }

        public CellState CellState { get; private set; }

        public IEnumerable<string> CellDependencies => _expression.CellReferences;

        private readonly ICellExpression _expression;

        /// <summary>
        /// Create new SpreadsheetCell.
        /// </summary>
        /// <param name="value">Cell value.</param>
        public Cell(ICellExpression expression)
        {
            _expression = expression;

            CellState = expression.IsValid ? CellState.Pending : CellState.SyntaxError;
        }

        public void Calculate(Dictionary<string, ICell> cellDependencies)
        {
            if (_expression.IsValid)
            {
                if (cellDependencies.Any(c => c.Value == null))
                {
                    SetError();
                    return;
                }

                if (cellDependencies.Any(c => c.Value.CellState == CellState.Pending))
                {
                    throw new SpreadsheetInternallException("Calculation flow error, one of the cell references were missed.");
                }

                if (cellDependencies.Any(c => c.Value.CellState != CellState.Fulfilled))
                {
                    SetError();
                    return;
                }

                double value = _expression.Calculate(new DependencyResolver(cellDependencies));

                SetValue(value);
            }

            void SetError()
            {
                CellState = CellState.CellValueError;
                CellValue = null;
            }

            void SetValue(double value)
            {
                CellState = CellState.Fulfilled;
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
                    return "#PENDING!";
                case CellState.Fulfilled:
                    if (CellValue.HasValue)
                    {
                        return CellValue.Value.ToString();
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

        class DependencyResolver : IDependencyResolver
        {
            readonly Dictionary<string, ICell> _cellDependencies;

            public DependencyResolver(Dictionary<string, ICell> cellDependencies)
            {
                _cellDependencies = cellDependencies;
            }

            public double ResolveCellreference(string key)
            {
                return _cellDependencies[key].CellValue.Value;
            }
        }
    }
}
