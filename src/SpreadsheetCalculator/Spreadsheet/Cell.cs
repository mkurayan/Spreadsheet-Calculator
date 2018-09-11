using SpreadsheetCalculator.ExpressionEngine.SyntaxTree;
using SpreadsheetCalculator.Spreadsheet.CellParsing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SpreadsheetCalculator.Spreadsheet
{
    /// <summary>
    /// Concrete cell in the spreadsheet.
    /// </summary>
    internal class Cell : ICell
    {
        public double? CellValue { get; private set; }

        public CellState CellState { get; private set; }

        public IEnumerable<string> CellDependencies => _expression.CellReferences;

        private readonly ICellExpression _expression;

        /// <summary>
        /// Create new SpreadsheetCell.
        /// </summary>
        /// <param name="expression">Parsed math expression.</param>
        public Cell(ICellExpression expression)
        {
            _expression = expression;

            CellState = expression.IsValid ? CellState.Pending : CellState.SyntaxError;
        }

        /// <summary>
        /// Calculate cell in spreadsheet,
        /// </summary>
        /// <param name="cellDependencies">Dictionary with all cell dependencies (references to another cells)</param>
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
                    throw new SpreadsheetInternalException("Calculation flow error, one of the cell references were missed.");
                }

                if (cellDependencies.Any(c => c.Value.CellState != CellState.Calculated))
                {
                    SetError();
                    return;
                }

                var value = _expression.Calculate(new DependencyResolver(cellDependencies));

                SetValue(value);
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
                    return "#PENDING!";
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
