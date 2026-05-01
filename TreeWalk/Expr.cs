// The book goes through this tedious exercise of making a code generator for all the Expr types because java sucks at representing something like record.
// C# makes it straightforward so no need to do that GenerateAst nonsense.

namespace TreeWalk
{
    public abstract record Expr : IAstNode
    {
        public virtual T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public record Literal(object? Value) : Expr;

    public record Unary(Token Operator, Expr Right) : Expr;

    public record Binary(Expr Left, Token Operator, Expr Right) : Expr;

    public record Grouping(Expr Expression) : Expr;

    public record Variable(Token Name) : Expr;

    public record Assign(Token Name, Expr Value) : Expr;
}