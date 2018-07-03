using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using SpreadsheetCalculator.ExpressionEvaluator;
[assembly: InternalsVisibleTo("SpreadsheetCalculator.Tests")]

namespace SpreadsheetCalculator
{
    /// <summary>
    /// Concrete cell in the spreadsheet.
    /// </summary>
    class SpreadsheetCell
    {
        /// <summary>
        /// Calculated cell value.
        /// </summary>
        private double calculatedValue;

        /// <summary>
        /// Current cell state.
        /// </summary>
        private CellState cellState;


        /// <summary>
        /// Parsed to tokens cell formula.
        /// </summary>
        private IEnumerable<Token> tokens;

        /// <summary>
        /// Create new SpreadsheetCell.
        /// </summary>
        /// <param name="rowNumber">Cell row number.</param>
        /// <param name="columnNumber">Cell column number.</param>
        /// <param name="value">Cell value.</param>
        public SpreadsheetCell(string value)
        {
            cellState = CellState.Pending;

            tokens = value.Split(" ")
                .Select(t => new Token(t))
                .ToList();
        }

        public IEnumerable<string> GetCellDependencies()
        {
            return tokens
                .Where(t => t.IsCellReference)
                .Select(t => t.Value);
        }

        public bool ValidateCell(IExpressionEvaluator calculator, IDictionary<string, SpreadsheetCell> cellReferences)
        {
            var summaryState = GetCellReferencesSummaryState(cellReferences);

            if (summaryState != CellState.Ok)
            {
                // If there is an error in any cell references, the whole expression will be invalid.
                cellState = summaryState;
                return false;
            }

            var strTokens = ParseCellTokens(cellReferences);

            if (calculator.VaildateExpression(strTokens))
            {
                return true;
            }
            else
            {
                cellState = CellState.ValueError;
                return false;
            }   
        }

        public void CalculateCell(IExpressionEvaluator calculator, IDictionary<string, SpreadsheetCell> cellReferences)
        {
            var strTokens = ParseCellTokens(cellReferences);

            try
            {
                calculatedValue = calculator.Evaluate(strTokens);
                cellState = CellState.Ok;
            }
            catch (DivideByZeroException)
            {
                cellState = CellState.DivideByZeroError;
            }
            catch (OverflowException)
            {
                cellState = CellState.NumberError;
            }
        }

        private IEnumerable<string> ParseCellTokens(IDictionary<string, SpreadsheetCell> cellReferences)
        {
            return tokens.Select(token =>
            {
                if (token.IsCellReference)
                {
                    var cell = cellReferences[token.Value];

                    if (cell.cellState == CellState.Ok)
                    {
                        return cell.calculatedValue.ToString();
                    }
                    else
                    {
                        throw new InvalidOperationException($"Cell reference is in invalid state: {cell.cellState}");
                    }
                }
                else
                {
                    return token.Value;
                }
            });
        }

        private CellState GetCellReferencesSummaryState(IDictionary<string, SpreadsheetCell> cellDependencies)
        {
            foreach (string cellReff in GetCellDependencies())
            {
                if (cellDependencies.ContainsKey(cellReff) && cellDependencies[cellReff] != null)
                {
                    var reff = cellDependencies[cellReff];

                    if (reff.cellState != CellState.Ok)
                    {
                        return reff.cellState;
                    }
                }
                else
                {
                    return CellState.ValueError;
                }
            }

            return CellState.Ok;
        }

        /// <summary>
        /// Provide information about spreadsheet cell value.
        /// </summary>
        /// <returns>Cell value</returns>
        public override string ToString()
        {
            switch (cellState)
            {
                case CellState.Ok:
                    return calculatedValue.ToString("F5");
                case CellState.ValueError:
                    return "#VALUE!";
                case CellState.DivideByZeroError:
                    return "#DIV/0!";
                case CellState.Pending:
                    return "#NOT_EVALUATED!";
                case CellState.NumberError:
                    return "#NUM!";
                default:
                    throw new NotImplementedException($"Unknown cell state: {cellState}");
            }
        }

        /// <summary>
        /// Represent single token in spreadsheet cell.
        /// </summary>
        class Token
        {
            // This pattern check if token is reference to another cell in spreadsheet.
            private static readonly Regex CellReferencePattern = new Regex(@"^[A-Z]\d+$");

            public string Value { get; }

            public bool IsCellReference { get; }

            public Token(string token)
            {
                Value = token;
                IsCellReference = CellReferencePattern.IsMatch(token);
            }
        }

        /// <summary>
        /// Describe inner SpreadsheetCell states.
        /// </summary>
        enum CellState
        {
            /// <summary>
            /// Cell not evaluated yet.
            /// </summary>
            Pending,

            /// <summary>
            /// Cell evaluated and contains correct value.
            /// </summary>
            Ok,

            /// <summary>
            /// In your formula a number is divided by zero.
            /// </summary>
            DivideByZeroError,

            /// <summary>
            /// There's something wrong with the way your formula is typed. 
            /// Or, there's something wrong with the cells you are referencing." 
            /// </summary>
            ValueError,

            /// <summary>
            /// Formula contains numeric values that aren’t valid.
            /// </summary>
            NumberError
        }
    }
}
