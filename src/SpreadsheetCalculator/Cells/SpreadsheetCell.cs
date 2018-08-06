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

        /// <summary>
        /// Original cell text.
        /// </summary>
        private readonly string cellText;

        /// <summary>
        /// Cell tokens which parsed from original cell text.
        /// </summary>
        private readonly Lazy<IEnumerable<CellToken>> cellTokens;

        public CellState CellState { get; private set; }

        public bool IsEmpty => !cellTokens.Value.Any();

        public IEnumerable<CellToken> CellDependencies => cellTokens.Value.Where(t => t.IsCellReference);

        public IEnumerable<CellToken> CellTokens => cellTokens.Value;

        /// <summary>
        /// Create new SpreadsheetCell.
        /// </summary>
        /// <param name="value">Cell value.</param>
        public SpreadsheetCell(string value)
        {
            cellText = value ?? throw new ArgumentNullException("value");

            cellTokens = new Lazy<IEnumerable<CellToken>>(
               () => cellText.Split(" ")
                      .Where(s => !string.IsNullOrWhiteSpace(s))
                      .Select(t => new CellToken(t))
                      .ToList()
            );

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
                    // If cell not processed yet, we return original cell value.
                    return cellText;
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
