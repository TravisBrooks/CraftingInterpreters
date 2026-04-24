using System.Collections;
using System.Collections.Immutable;
using static TreeWalk.TokenType;

namespace TreeWalk
{
    internal class Scanner
    {
        private static readonly Dictionary<string, TokenType> keywords = new()
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
            ["while"] = WHILE
        };

        private readonly string source;
        private readonly List<Token> tokens;
        private int start;
        private int current;
        private int line;

        public Scanner(string source)
        {
            this.source = source;
            tokens = [];
            line = 1;
        }

        public IList<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                // We are at the beginning of the next lexeme.
                start = current;
                ScanToken();
            }
            tokens.Add(new Token(EOF, string.Empty, null, line));
            return tokens.ToImmutableList();
        }

        private bool IsAtEnd()
        {
            return current >= source.Length;
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
                    line++;
                    break;
                case '"':
                    this.String();
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
                        Lox.Error(line, $"Unexpected character: {c}");
                    }
                    break;
            }
        }

        private char Advance()
        {
            return source[current++];
        }

        private void AddToken(TokenType tokenType)
        {
            AddToken(tokenType, null);
        }

        private void AddToken(TokenType tokenType, object? literal)
        {
            // Slight difference from book because java's substring takes 2 indexes and c# takes start index and length.
            var txt = source.Substring(start, current - start);
            tokens.Add(new Token(tokenType, txt, literal, line));
        }

        private bool Match(char expected)
        {
            if (IsAtEnd())
            {
                return false;
            }
            var isMatch = source[current] == expected;
            if (isMatch)
            {
                current++;
            }
            return isMatch;
        }

        private char Peek()
        {
            if (IsAtEnd())
            {
                return '\0';
            }
            return source[current];
        }

        private void String()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n')
                {
                    line++;
                }
                Advance();
            }

            if (IsAtEnd())
            {
                Lox.Error(line, "Unterminated string.");
                return;
            }

            // The closing ".
            Advance();

            // Trim the surrounding quotes.
            var value = source.Substring(start + 1, current - start - 2);
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
            // the book turned rando chars at the end of a number to be an identifier, this was probably an oversight but i corrected it here
            if (!IsAtEnd())
            {
                if (!Char.IsWhiteSpace(source[current]))
                {
                    Lox.Error(line, $"Unexpected character in number literal: {source[current]}");
                    return;
                }
            }
            AddToken(NUMBER, Double.Parse(source.Substring(start, current - start)));
        }

        private char PeekNext()
        {
            if (current + 1 >= source.Length)
            {
                return '\0';
            }
            return source[current + 1];
        }

        private void Identifier()
        {
            while (char.IsLetterOrDigit(Peek()))
            {
                Advance();
            }
            var txt = source.Substring(start, current - start);
            if (keywords.TryGetValue(txt, out TokenType tknType))
            {
                AddToken(tknType);
            }
            else
            {
                AddToken(IDENTIFIER);
            }
        }
    }
}