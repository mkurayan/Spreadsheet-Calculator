using SpreadsheetCalculator.Parser;
using System.Collections.Generic;

namespace SpreadsheetCalculator
{
    /// <summary>
    /// Data contract for spreadsheet cells.
    /// </summary>
    interface ICell
    {
        /// <summary>
        /// Cell is empty.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Current cell state.
        /// </summary>
        CellState CellState { get; }

        /// <summary>
        /// Parsed to tokens cell formula.
        /// </summary>
        IEnumerable<Token> CellTokens { get; }

        /// <summary>
        /// Cell dependencies from parsed to tokens cell formula.
        /// </summary>
        IEnumerable<Token> CellDependencies { get; }

        /// <summary>
        /// Set calculated cell value.
        /// </summary>
        /// <param name="value">Calculated value.</param>
        void SetValue(double value);

        /// <summary>
        /// Set cell error.
        /// </summary>
        /// <param name="state">Error state.</param>
        /// <exception cref="InvalidOperationException">Try to set non error state to cell.</exception>
        void SetError(CellState state);
    }
}