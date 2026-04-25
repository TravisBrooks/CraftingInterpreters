namespace TreeWalk
{
    public record class Token(TokenType TokenType, string Lexeme, object? Literal, int Line);
}