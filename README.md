# Spreadsheet Calculator

A spreadsheet consists of a two dimensional array of cells, labeled A1, A2, etc. Rows are 
identified using letters, columns by numbers. Each cell contains either an integer (its value) or 
an expression. Expressions contain integers, cell references, and the operators '+', '-', '*', '/', '++', '--' 
with the usual rules of evaluation.
Cell input is RPN and evaluated in stack order.

The spreadsheet input is defined as follows:
  - Line 1: two integers, defining the width and height of the spreadsheet (n, m)
  - n*m lines each containing an expression which is the value of the corresponding cell 
  (cells enumerated in the order A1, A2, A<n>, B1, (...))
  
  
Program output its data in the same format, but each cell reduced to a
single floating point value.

| Input| Output|
|----| ------------- 
|3 2 | 3 2 
|A2  | 20.00000 |
|4 5 * | 20.00000 |
|A1 | 20.00000 |
|A1 B2 / 2 + | 8.66667 |
|3 | 3.00000 |
|39 B1 B2 * / |1.50000 |


## Examples 
### input 1.txt

| | 1        | 2           | 3  |
|----| ------------- |---------------| ------|
|A | A2 | 4 5 * | A1|
|B  | A1 B2 / 2 + |3 |39 B1 B2 * /| 

#### Results:

20.00000

20.00000

20.00000

8.66667

3.00000

1.50000



### input 2.txt
| | 1        | 2           | 3  |
|----| ------------- |---------------| ------|
|A | A2 | A1 | 3|
|B  | A1 B2 / 2 + |3 |39 B1 B2 * /| 

Program detect cyclic dependencies in the input data and report these to user.
#### Results:
Cyclic dependency found.

