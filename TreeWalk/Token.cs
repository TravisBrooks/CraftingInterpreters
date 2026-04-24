using System.Text.Json;

namespace TreeWalk
{
    public record class Token(TokenType tokenType, string lexeme, object? literal, int line)
    {
    }
}