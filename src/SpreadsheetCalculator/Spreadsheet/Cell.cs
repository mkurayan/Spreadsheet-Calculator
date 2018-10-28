using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SpreadsheetCalculator.ExpressionEngine.Parsing.SyntaxTree;
using SpreadsheetCalculator.ExpressionEngine.Tokenization;

namespace SpreadsheetCalculator.Spreadsheet
{
    /// <summary>
    /// Concrete cell in the spreadsheet.
    /// </summary>
    internal class Cell : IViewCell
    {
        public string OriginalValue { get; }

        public string ResultValue
        {
            get
            {
                switch (CellState)
                {
                    case CellState.Pending:
                        return "#PENDING!";
                    case CellState.Calculated:
                        if (CalculatedValue.HasValue)
                        {
                            return CalculatedValue.Value.ToString(CultureInfo.InvariantCulture);
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
        }

        public double? CalculatedValue { get; private set; }

        public CellState CellState { get; private set; }

        public IReadOnlyCollection<string> CellDependencies { get; }

        public INode SyntaxTreeTop { get; }

        public Cell(string value, Token[] tokens, INode syntaxTreeTop)
        {
            OriginalValue = value ?? throw new ArgumentNullException(nameof(value));

            SyntaxTreeTop = syntaxTreeTop ?? throw new ArgumentNullException(nameof(syntaxTreeTop));

            CellDependencies = (tokens ?? throw new ArgumentNullException(nameof(tokens)))
                    .Where(token => token.Type == TokenType.CellReference)
                    .Select(token => token.Value)
                    .ToHashSet();

            CellState = CellState.Pending;
        }

        public Cell(string value)
        {
            OriginalValue = value ?? throw new ArgumentNullException(nameof(value));

            CellDependencies = new string[0];

            SyntaxTreeTop = null;

            CellState = CellState.SyntaxError;
        }

        public void SetError(CellState errorState)
        {
            CheckThatCellInPendingState();

            if (errorState == CellState.CellValueError || errorState == CellState.SyntaxError)
            {
                CellState = errorState;
                CalculatedValue = null;
            }
            else
            {
                throw new ArgumentException($"Non error state passed to method. State: {errorState}");
            }
        }

        public void SetValue(double value)
        {
            CheckThatCellInPendingState();

            CellState = CellState.Calculated;
            CalculatedValue = value;
        }

        private void CheckThatCellInPendingState()
        {
            if (CellState != CellState.Pending)
            {
                throw new InvalidOperationException($"Cell not in pending state. Current state: {CellState}");
            }
        }
    }
}
