using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("SpreadsheetCalculator.Tests")]
namespace SpreadsheetCalculator.Cells
{
    /// <summary>
    /// Describe inner SpreadsheetCell states.
    /// </summary>
    public enum CellState
    {
        /// <summary>
        /// Cell not evaluated yet.
        /// </summary>
        Pending,

        /// <summary>
        /// Cell evaluated and contains correct value.
        /// </summary>
        Fulfilled,

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
