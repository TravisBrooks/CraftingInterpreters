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

    public record LiteralExpr(object? Value) : Expr;

    public record UnaryExpr(Token Operator, Expr Right) : Expr;

    public record BinaryExpr(Expr Left, Token Operator, Expr Right) : Expr;

    public record GroupingExpr(Expr Expression) : Expr;

    public record VariableExpr(Token Name) : Expr;

    public record AssignExpr(Token Name, Expr Value) : Expr;

    public record LogicalExpr(Expr Left, Token Operator, Expr Right) : Expr;

    public record CallExpr(Expr Callee, Token Paren, IEnumerable<Expr> Arguments) : Expr;

    public record FunExpr(IList<Token> Parameters, BlockStatement Body) : Expr;
}