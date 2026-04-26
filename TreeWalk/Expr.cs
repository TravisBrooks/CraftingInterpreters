// The book goes through this tedious exercise of making a code generator for all the Expr types because java sucks at representing something like record.
// C# makes it straightforward so no need to do that GenerateAst nonsense.
namespace TreeWalk
{
    public abstract record Expr
    {
        public abstract T Accept<T>(IVisitor<T> visitor);
    }

    public record Literal(object? Value) : Expr
    {
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public record Unary(Token Operator, Expr Right) : Expr
    {
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public record Binary(Expr Left, Token Operator, Expr Right) : Expr
    {
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public record Grouping(Expr Expression) : Expr
    {
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}