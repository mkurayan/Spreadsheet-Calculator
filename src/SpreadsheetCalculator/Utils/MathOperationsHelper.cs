using System.Collections.Generic;

namespace SpreadsheetCalculator.Utils
{
    static class MathOperationsHelper
    {
        public static IDictionary<string, OperatorDescription> Operators = new Dictionary<string, OperatorDescription>
        {
            ["+"] = new OperatorDescription { Operator = "+", Precedence = 1 },
            ["-"] = new OperatorDescription { Operator = "-", Precedence = 1 },
            ["*"] = new OperatorDescription { Operator = "*", Precedence = 2 },
            ["/"] = new OperatorDescription { Operator = "/", Precedence = 2 }
        };
    }

    class OperatorDescription
    {
        public string Operator { get; set; }

        public int Precedence { get; set; }
    }
}
