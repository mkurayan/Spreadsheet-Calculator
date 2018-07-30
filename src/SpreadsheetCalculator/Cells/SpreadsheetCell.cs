using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SpreadsheetCalculator.Cells
{
    /// <summary>
    /// Concrete cell in the spreadsheet.
    /// </summary>
    class SpreadsheetCell : ISpreadsheetCell
    {
        /// <summary>
        /// Calculated cell value.
        /// </summary>
        private double? calculatedValue;

        public CellState CellState { get; private set; }

        public bool IsEmpty
        {
            get
            {
                return !CellTokens.Any();
            }
        }

        public IEnumerable<CellToken> CellTokens { get; private set; }

        public IEnumerable<CellToken> CellDependencies
        {
            get
            {
                return CellTokens
                    .Where(t => t.IsCellReference);
            }
        }

        /// <summary>
        /// Create new SpreadsheetCell.
        /// </summary>
        /// <param name="value">Cell value.</param>
        public SpreadsheetCell(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            CellState = CellState.Pending;

            CellTokens = value.Split(" ")
                   .Where(s => !string.IsNullOrWhiteSpace(s))
                   .Select(t => new CellToken(t))
                   .ToList();
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
                case CellState.Fulfilled:
                    if (calculatedValue.HasValue)
                    {
                        return calculatedValue.Value.ToString("F5");
                    }
                    else
                    {
                        throw new InvalidOperationException("Cell in inconsistent state.");
                    }
                case CellState.ValueError:
                    return "#VALUE!";
                case CellState.Pending:
                    return "#NOT_EVALUATED!";
                case CellState.NumberError:
                    return "#NUM!";
                default:
                    throw new NotImplementedException($"Unknown cell state: {CellState}");
            }
        }
    }
}
