using System.Collections.Immutable;
using static Lox.TokenType;

namespace Lox
{
    internal class Scanner
    {
        private static readonly Dictionary<string, TokenType> Keywords = new()
        {
            ["and"] = AND,
            ["class"] = CLASS,
            ["else"] = ELSE,
            ["false"] = FALSE,
            ["for"] = FOR,
            ["fun"] = FUN,
            ["if"] = IF,
            ["nil"] = NIL,
            ["or"] = OR,
            ["print"] = PRINT,
            ["return"] = RETURN,
            ["super"] = SUPER,
            ["this"] = THIS,
            ["true"] = TRUE,
            ["var"] = VAR,
            ["while"] = WHILE,
            ["break"] = BREAK,
            ["continue"] = CONTINUE,
        };

        private readonly ErrorLogger _errorLogger;
        private readonly string _source;
        private readonly List<Token> _tokens;
        private int _current;
        private int _line;
        private int _start;

        public Scanner(ErrorLogger errorLogger, string source)
        {
            _source = source;
            _tokens = [];
            _line = 1;
            _errorLogger = errorLogger;
        }

        public ImmutableList<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                // We are at the beginning of the next lexeme.
                _start = _current;
                ScanToken();
            }

            _tokens.Add(new Token(EOF, string.Empty, null, _line));
            return _tokens.ToImmutableList();
        }

        private bool IsAtEnd()
        {
            return _current >= _source.Length;
        }

        private void ScanToken()
        {
            var c = Advance();
            switch (c)
            {
                case '(':
                    AddToken(LEFT_PAREN);
                    break;
                case ')':
                    AddToken(RIGHT_PAREN);
                    break;
                case '{':
                    AddToken(LEFT_BRACE);
                    break;
                case '}':
                    AddToken(RIGHT_BRACE);
                    break;
                case ',':
                    AddToken(COMMA);
                    break;
                case '.':
                    AddToken(DOT);
                    break;
                case '-':
                    AddToken(MINUS);
                    break;
                case '+':
                    AddToken(PLUS);
                    break;
                case ';':
                    AddToken(SEMICOLON);
                    break;
                case '*':
                    AddToken(STAR);
                    break;
                case '%':
                    AddToken(MODULO);
                    break;
                case '!':
                    AddToken(Match('=') ? BANG_EQUAL : BANG);
                    break;
                case '=':
                    AddToken(Match('=') ? EQUAL_EQUAL : EQUAL);
                    break;
                case '<':
                    AddToken(Match('=') ? LESS_EQUAL : LESS);
                    break;
                case '>':
                    AddToken(Match('=') ? GREATER_EQUAL : GREATER);
                    break;
                case '/':
                    if (Match('/'))
                    {
                        // A comment goes until the end of the line.
                        while (Peek() != '\n' && !IsAtEnd())
                        {
                            Advance();
                        }
                    }
                    else
                    {
                        AddToken(SLASH);
                    }

                    break;
                case ' ':
                case '\r':
                case '\t':
                    // Ignore whitespace.
                    break;
                case '\n':
                    _line++;
                    break;
                case '"':
                    String();
                    break;
                default:
                    if (char.IsDigit(c))
                    {
                        Number();
                    }
                    else if (char.IsLetter(c))
                    {
                        Identifier();
                    }
                    else
                    {
                        _errorLogger.ReportError(Peek(), $"Unexpected character: {c}");
                    }

                    break;
            }
        }

        private char Advance()
        {
            return _source[_current++];
        }

        private void AddToken(TokenType tokenType, object? literal = null)
        {
            // Slight difference from book because java's substring takes 2 indexes and c# takes start index and length.
            var txt = _source.Substring(_start, _current - _start);
            _tokens.Add(new Token(tokenType, txt, literal, _line));
        }

        private bool Match(char expected)
        {
            if (IsAtEnd())
            {
                return false;
            }

            var isMatch = _source[_current] == expected;
            if (isMatch)
            {
                _current++;
            }

            return isMatch;
        }

        private char Peek()
        {
            return IsAtEnd() ? '\0' : _source[_current];
        }

        private void String()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n')
                {
                    _line++;
                }

                Advance();
            }

            if (IsAtEnd())
            {
                _errorLogger.ReportError(_line, "Unterminated string.");
                return;
            }

            // The closing quote
            Advance();

            // Trim the surrounding quotes
            var value = _source.Substring(_start + 1, _current - _start - 2);
            AddToken(STRING, value);
        }

        private void Number()
        {
            while (char.IsDigit(Peek()))
            {
                Advance();
            }

            if (Peek() == '.' && char.IsDigit(PeekNext()))
            {
                // Consume the "."
                Advance();
                while (char.IsDigit(Peek()))
                {
                    Advance();
                }
            }

            // The book allowed various characters after a number that were turned into an identifier token. That was probably an oversight.
            var allowedNextChars = new[] { ' ', '\r', '\t', '\n', ')', '}', ']', ';', '\0', '+', '-', '*', '/' };
            if (!allowedNextChars.Contains(Peek()))
            {
                _errorLogger.ReportError(Peek(), "Invalid character after number.");
                return;
            }

            AddToken(NUMBER, double.Parse(_source.Substring(_start, _current - _start)));
        }

        private char PeekNext()
        {
            if (_current + 1 >= _source.Length)
            {
                return '\0';
            }

            return _source[_current + 1];
        }

        private void Identifier()
        {
            while (char.IsLetterOrDigit(Peek()))
            {
                Advance();
            }

            var txt = _source.Substring(_start, _current - _start);
            AddToken(Keywords.GetValueOrDefault(txt, IDENTIFIER));
        }
    }
}