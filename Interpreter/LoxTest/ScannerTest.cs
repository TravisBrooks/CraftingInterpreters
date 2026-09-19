using Lox;

namespace LoxTest
{
    public class ScannerTest : ScannerTestBase
    {
        [Fact]
        public void Identifiers()
        {
            var source = """
                         andy formless fo _ _123 _abc ab123
                         abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_
                         
                         // expect: IDENTIFIER andy null
                         // expect: IDENTIFIER formless null
                         // expect: IDENTIFIER fo null
                         // expect: IDENTIFIER _ null
                         // expect: IDENTIFIER _123 null
                         // expect: IDENTIFIER _abc null
                         // expect: IDENTIFIER ab123 null
                         // expect: IDENTIFIER abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_ null
                         // expect: EOF  null
                         """;
            var (tokens, output) = Scan(source);
            Assert.False(output.HasOutputError());
            
            Assert.Equal([
                new Token(TokenType.IDENTIFIER, "andy", null, 1),
                new Token(TokenType.IDENTIFIER, "formless", null, 1),
                new Token(TokenType.IDENTIFIER, "fo", null, 1),
                new Token(TokenType.IDENTIFIER, "_", null, 1),
                new Token(TokenType.IDENTIFIER, "_123", null, 1),
                new Token(TokenType.IDENTIFIER, "_abc", null, 1),
                new Token(TokenType.IDENTIFIER, "ab123", null, 1),
                new Token(TokenType.IDENTIFIER, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_", null, 1),
                new Token(TokenType.EOF, string.Empty, null, 1),
                ], tokens, new TokenTestComparer());
        }

        [Fact]
        public void Keywords()
        {
            var source = """
                         and class else false for fun if nil or return super this true var while
                         
                         // expect: AND and null
                         // expect: CLASS class null
                         // expect: ELSE else null
                         // expect: FALSE false null
                         // expect: FOR for null
                         // expect: FUN fun null
                         // expect: IF if null
                         // expect: NIL nil null
                         // expect: OR or null
                         // expect: RETURN return null
                         // expect: SUPER super null
                         // expect: THIS this null
                         // expect: TRUE true null
                         // expect: VAR var null
                         // expect: WHILE while null
                         // expect: EOF  null
                         """;
            var (tokens, output) = Scan(source);
            Assert.False(output.HasOutputError());

            Assert.Equal([
                new Token(TokenType.AND, "and", null, 1),
                new Token(TokenType.CLASS, "class", null, 1),
                new Token(TokenType.ELSE, "else", null, 1),
                new Token(TokenType.FALSE, "false", null, 1),
                new Token(TokenType.FOR, "for", null, 1),
                new Token(TokenType.FUN, "fun", null, 1),
                new Token(TokenType.IF, "if", null, 1),
                new Token(TokenType.NIL, "nil", null, 1),
                new Token(TokenType.OR, "or", null, 1),
                new Token(TokenType.RETURN, "return", null, 1),
                new Token(TokenType.SUPER, "super", null, 1),
                new Token(TokenType.THIS, "this", null, 1),
                new Token(TokenType.TRUE, "true", null, 1),
                new Token(TokenType.VAR, "var", null, 1),
                new Token(TokenType.WHILE, "while", null, 1),
                new Token(TokenType.EOF, string.Empty, null, 1),
            ], tokens, new TokenTestComparer());
        }

        [Fact]
        public void Numbers()
        {
            var source = """
                         123
                         123.456
                         .456
                         123.
                         
                         // expect: NUMBER 123 123.0
                         // expect: NUMBER 123.456 123.456
                         // expect: DOT . null
                         // expect: NUMBER 456 456.0
                         // expect: NUMBER 123 123.0
                         // expect: DOT . null
                         // expect: EOF  null
                         """;
            var (tokens, output) = Scan(source);
            Assert.False(output.HasOutputError());

            Assert.Equal([
                new Token(TokenType.NUMBER, "123", 123.0, 1),
                new Token(TokenType.NUMBER, "123.456", 123.456, 1),
                new Token(TokenType.DOT, ".", null, 1),
                new Token(TokenType.NUMBER, "456", 456.0, 1),
                new Token(TokenType.NUMBER, "123", 123.0, 1),
                new Token(TokenType.DOT, ".", null, 1),
                new Token(TokenType.EOF, string.Empty, null, 1),
            ], tokens, new TokenTestComparer());
        }

        [Fact]
        public void Punctuators()
        {
            var source = """
                         (){};,+-*!===<=>=!=<>/.
                         
                         // expect: LEFT_PAREN ( null
                         // expect: RIGHT_PAREN ) null
                         // expect: LEFT_BRACE { null
                         // expect: RIGHT_BRACE } null
                         // expect: SEMICOLON ; null
                         // expect: COMMA , null
                         // expect: PLUS + null
                         // expect: MINUS - null
                         // expect: STAR * null
                         // expect: BANG_EQUAL != null
                         // expect: EQUAL_EQUAL == null
                         // expect: LESS_EQUAL <= null
                         // expect: GREATER_EQUAL >= null
                         // expect: BANG_EQUAL != null
                         // expect: LESS < null
                         // expect: GREATER > null
                         // expect: SLASH / null
                         // expect: DOT . null
                         // expect: EOF  null
                         """;
            var (tokens, output) = Scan(source);
            Assert.False(output.HasOutputError());

            Assert.Equal([
                new Token(TokenType.LEFT_PAREN, "(", null, 1),
                new Token(TokenType.RIGHT_PAREN, ")", null, 1),
                new Token(TokenType.LEFT_BRACE, "{", null, 1),
                new Token(TokenType.RIGHT_BRACE, "}", null, 1),
                new Token(TokenType.SEMICOLON, ";", null, 1),
                new Token(TokenType.COMMA, ",", null, 1),
                new Token(TokenType.PLUS, "+", null, 1),
                new Token(TokenType.MINUS, "-", null, 1),
                new Token(TokenType.STAR, "*", null, 1),
                new Token(TokenType.BANG_EQUAL, "!=", null, 1),
                new Token(TokenType.EQUAL_EQUAL, "==", null, 1),
                new Token(TokenType.LESS_EQUAL, "<=", null, 1),
                new Token(TokenType.GREATER_EQUAL, ">=", null, 1),
                new Token(TokenType.BANG_EQUAL, "!=", null, 1),
                new Token(TokenType.LESS, "<", null, 1),
                new Token(TokenType.GREATER, ">", null, 1),
                new Token(TokenType.SLASH, "/", null, 1),
                new Token(TokenType.DOT, ".", null, 1),
                new Token(TokenType.EOF, string.Empty, null, 1),
            ], tokens, new TokenTestComparer());
        }

        [Fact]
        public void Strings()
        {
            var source = """
                         ""
                         "string"
                         
                         // expect: STRING "" 
                         // expect: STRING "string" string
                         // expect: EOF  null
                         """;
            var (tokens, output) = Scan(source);
            Assert.False(output.HasOutputError());

            // The escaped quotes look a little odd, its because scanner uses substrings of the source code so includes the quotes in the Lexeme
            Assert.Equal(
                [
                new Token(TokenType.STRING, "\"\"", string.Empty, 1),
                new Token(TokenType.STRING, "\"string\"", "string", 1),
                new Token(TokenType.EOF, string.Empty, null, 1)
                ], tokens, new TokenTestComparer());
        }

        [Fact]
        public void Whitespace()
        {
            var source = """
                         space    tabs				newlines
                         
                         
                         
                         
                         end
                         
                         // expect: IDENTIFIER space null
                         // expect: IDENTIFIER tabs null
                         // expect: IDENTIFIER newlines null
                         // expect: IDENTIFIER end null
                         // expect: EOF  null
                         """;
            var (tokens, output) = Scan(source);
            Assert.False(output.HasOutputError());

            Assert.Equal(
            [
                new Token(TokenType.IDENTIFIER, "space", null, 1),
                new Token(TokenType.IDENTIFIER, "tabs", null, 1),
                new Token(TokenType.IDENTIFIER, "newlines", null, 1),
                new Token(TokenType.IDENTIFIER, "end", null, 1),
                new Token(TokenType.EOF, string.Empty, null, 1)
            ], tokens, new TokenTestComparer());
        }
    }
}
