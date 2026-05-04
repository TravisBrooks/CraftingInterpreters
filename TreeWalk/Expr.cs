// The book goes through this tedious exercise of making a code generator for all the Expr types because java sucks at representing something like record.
// C# makes it straightforward so no need to do that GenerateAst nonsense.

namespace Lox
{
    public abstract record Expr : IAstNode
    {
        public TResult Accept<TResult>(IVisitor<Expr, TResult> visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// Explicit implementation of IAstNode.Accept to hide this more generic version from the public API of Expr
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="visitor"></param>
        /// <returns></returns>
        TResult IAstNode.Accept<TResult>(IVisitor<IAstNode, TResult> visitor)
        {
            return Accept(visitor);
        }
    }

    public record Literal(object? Value) : Expr;

    public record Unary(Token Operator, Expr Right) : Expr;

    public record Binary(Expr Left, Token Operator, Expr Right) : Expr;

    public record Grouping(Expr Expression) : Expr;

    public record Variable(Token Name) : Expr;

    public record Assign(Token Name, Expr Value) : Expr;

    public record Logical(Expr Left, Token Operator, Expr Right) : Expr;
}