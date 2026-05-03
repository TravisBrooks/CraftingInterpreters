namespace TreeWalk
{
    public record Token(TokenType TokenType, string Lexeme, object? Literal, int Line);
}