# Spreadsheet Calculator
## Example of work
You can create a spreadsheet directly in the console (enter each cell one by one) or load it from the file.
Here is an example of the spreadsheet from file input1.txt

| | A | B | C |
|-|-|-|-|
| 1 | B1 + 1 | 2 + 3 | -A1 + 2 |
| 2 | 10 / C1 | A2 * (C1 + 2) | 20 / (A1 - B2) |

Calculation result

| | A | B | C |
|-|-|-|-|
| 1 | 6 | 5 | -4 |
| 2 | -2.5 | 5 | 20 |

In order to test application, you can create a spreadsheet and view calculation result directly in the console.
```sh
dotnet SpreadsheetCalculator.dll
```

For big spreadsheets, you can specify input and output file.
```sh
dotnet SpreadsheetCalculator.dll input.txt output.txt
```
The spreadsheet input file is defined as follows:
  - Line 1: two integers, defining the columns and rows of the spreadsheet (n, m)
  - n*m lines each containing an expression which is the value of the corresponding cell 
  (cells enumerated in the order A1, B1, C1, ...1, A2, B2, C2, ...2 )

Program output its data in the same format.

## Input Validation
Program will automatically validate user input. 
If a cell cannot be calculated it will contain error label in spreadsheet output.

### Syntax error. Cell has invalid math expression.
If cell has a syntax error in the formula program cannot calculate it, and instead put **!SYNTAX#** label in cell output.

Example: Cell B1 contains syntax error (incomplete expression, second operand missed).
Cell C1 contains syntax error as well (unknown symbols in the formula).
In the output, those cells will have **!SYNTAX#** error.

| | A | B | C |
|-|-|-|-|
| 1 | 2 + 2 | A1 + | 42 - &? |

Result

| | A | B | C |
|-|-|-|-|
| 1 | 4 | !SYNTAX# | !SYNTAX# |

### Reference error. Cell formula has a reference to another cell which does not exist or contains error.
If cell formula is fine but contains references to the cell which does not exist or has an error
program cannot calculate it, and instead put **!VALUE#** label in cell output.

Example: Cell B1 refer cell C22 with is outside of the spreadsheet.
Cell C1 has a reference to cell B1 which contains error.
In the output, those cells will have **!VALUE#** error.

| | A | B | C |
|-|-|-|-|
| 1 | 55 | C22 | A1 + B1 |

Result

| | A | B | C |
|-|-|-|-|
| 1 | 55 | !VALUE# | !VALUE# |
## Cyclic dependencies
You should not have cyclic dependencies in the spreadsheet.
If the program finds a cyclic dependency in the spreadsheet it will report these to the user and exit. 

Example 1: Cell A1 refer cell B1, cell B1 refer cel A1.
As result, we have a cyclic dependency between cells A1 and B1.

| | A | B | C |
|-|-|-|-|
| 1 | B1 | C1 | 7 |

Result
```sh
Cyclic dependency found: A1 -> B1 -> A1
```
Example 2: Spreadsheet with cyclic dependency from file input2.txt
Cell A1 refer B1, B1 refer A2, A2 refer C1, C1 refer A1

| | A | B | C |
|-|-|-|-|
| 1 | B1 + 1 | 2 + 3 + A2 | -A1 + 2 |
| 2 | 10 / C1 | A2 * (C1 + 2) | 20 / (A1 - B2) |

Result
```sh
Cyclic dependency found: A1 -> B1 -> A2 -> C1 -> A1
```

