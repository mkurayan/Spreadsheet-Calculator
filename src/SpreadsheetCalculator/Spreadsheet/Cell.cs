using SpreadsheetCalculator.ExpressionParser;
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
        /// <summary>
        /// Calculated cell value.
        /// </summary>
        private double? calculatedValue;

        public CellState CellState { get; private set; }

        public bool IsEmpty => !CellTokens.Any();

        public IEnumerable<Token> CellDependencies => CellTokens.Where(t => t.Type == TokenType.CellReference);

        public IEnumerable<Token> CellTokens { get; }

        /// <summary>
        /// Create new SpreadsheetCell.
        /// </summary>
        /// <param name="value">Cell value.</param>
        public Cell(IEnumerable<Token> tokens)
        {
            CellTokens = tokens ?? throw new ArgumentNullException(nameof(tokens)); ;

            CellState = CellState.Pending;
        }

        public void SetValue(double value)
        {
            if (Double.IsNaN(value) || Double.IsInfinity(value))
            {
                SetError(CellState.NumberError);
            }
            else
            {
                CellState = CellState.Fulfilled;
                calculatedValue = value;
            }
        }

        public void SetError(CellState state)
        {
            if (state == CellState.Pending)
            {
                throw new InvalidOperationException($"Illegal 'Pending' state assignment");
            }

            if (state == CellState.Fulfilled)
            {
                throw new InvalidOperationException($"Illegal 'Fulfilled' state assignment, in order to mark cell as fulfilled use SetValue method.");
            }

            CellState = state;
            calculatedValue = null;
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
                    if (calculatedValue.HasValue)
                    {
                        return calculatedValue.Value.ToString();
                    }
                    else
                    {
                        throw new InvalidOperationException("Cell in inconsistent state.");
                    }
                case CellState.ValueError:
                    return "#VALUE!";
                case CellState.NumberError:
                    return "#NUM!";
                default:
                    throw new NotImplementedException($"Unknown cell state: {CellState}");
            }
        }
    }
}
