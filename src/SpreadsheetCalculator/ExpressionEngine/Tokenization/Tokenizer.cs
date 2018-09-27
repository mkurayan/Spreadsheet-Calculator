using System;
using System.Collections.Generic;

namespace SpreadsheetCalculator.ExpressionEngine.Tokenization
{   
    /// <summary>
    /// Split string to tokens.
    /// </summary>
    internal class Tokenizer : ITokenizer
    {
        private static readonly Dictionary<char, TokenType> SymbolsMap = new Dictionary<char, TokenType> 
        {
            ['+'] = TokenType.Add,
            ['*'] = TokenType.Multiply,
            ['-'] = TokenType.Subtract,
            ['/'] = TokenType.Divide,
            ['('] = TokenType.OpenParenthesis,
            [')'] = TokenType.CloseParenthesis
        };

        private StackString _symbolsStack;

        public Token[] Tokenize(string str)
        {
            _symbolsStack = new StackString(str);
            
            var tokens = new List<Token>();

            while (_symbolsStack.TryPeek(out var ch))
            {
                if (char.IsWhiteSpace(ch))
                {
                    _symbolsStack.Pop();
                    continue;
                }

                tokens.Add(ReadToken());
            }

            _symbolsStack = null;

            return tokens.ToArray();
        }

        private Token ReadToken()
        {
            var nextChar = _symbolsStack.Peek();

            if (SymbolsMap.ContainsKey(nextChar))
            {
                var ch = _symbolsStack.Pop();
                return new Token(SymbolsMap[ch], ch);
            }

            // Token is number
            if (char.IsDigit(nextChar) || nextChar == '.')
            {
                return ReadNumberToken();
            }

            // Token is cell reference
            if (char.IsLetter(nextChar))
            {
                return ReadCellReferenceToken();
            }

            throw new SyntaxException("Unknown symbol.");
        }

        private int ReadDigits() => ReadSequence(char.IsDigit);

        private int ReadLetters() => ReadSequence(char.IsLetter);

        private int ReadSequence(Func<char, bool> check)
        {
            var count = 0;

            while (_symbolsStack.TryPeek(out var ch) && check(ch))
            {
                _symbolsStack.Pop();
                count++;
            }

            return count;
        }

        private Token ReadNumberToken()
        {
            var startIndex = _symbolsStack.Index;

            // Read integer part.
            var integerPart = ReadDigits();
            var decimalPart = 0;

            // Check if number contains decimal part. 
            if (_symbolsStack.TryPeek(out var nextChar) && nextChar == '.')
            {
                //Skip '.' symbol
                _symbolsStack.Pop();

                // Read decimal part.
                decimalPart = ReadDigits();
            }

            if (integerPart == 0 && decimalPart == 0)
            {
                throw new SyntaxException("Invalid number.");
            }

            return new Token(TokenType.Number, _symbolsStack.Substring(startIndex, _symbolsStack.Index - startIndex));
        }

        private Token ReadCellReferenceToken()
        {
            var startIndex = _symbolsStack.Index;

            // Read letters part.
            var letters = ReadLetters();

            // Read digits part. 
            var digits = ReadDigits();

            if (letters == 0 || digits == 0)
            {
                throw new SyntaxException("Invalid cell reference.");
            }
            
            return new Token(TokenType.CellReference, _symbolsStack.Substring(startIndex, _symbolsStack.Index - startIndex));
        }

        private class StackString
        {
            private readonly string _text;

            public int Index { get; private set; }

            public StackString(string str)
            {
                Index = 0;
                _text = str;
            }

            public bool TryPeek(out char ch)
            {
                if (Index < _text.Length)
                {
                    ch = Peek();
                    return true;
                }

                ch = char.MinValue;
                return false;
            }

            public char Peek()
            {
                return _text[Index];
            }

            public char Pop()
            {
                var ch = Peek();
                Index++;

                return ch;
            }

            public string Substring(int startIndex, int length)
            {
                return _text.Substring(startIndex, length);
            }
        }
    }
}
