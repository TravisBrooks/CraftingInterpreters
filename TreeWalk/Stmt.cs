namespace Lox
{
    public abstract record Stmt : IAstNode
    {
        public TResult Accept<TResult>(IVisitor<Stmt, TResult> visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// Explicit implementation of IAstNode.Accept to hide this more generic version from the public API of Stmt
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="visitor"></param>
        /// <returns></returns>
        TResult IAstNode.Accept<TResult>(IVisitor<IAstNode, TResult> visitor)
        {
            return Accept(visitor);
        }
    }

    public record ExprStatement(Expr Expression) : Stmt;

    public record PrintStatement(Expr Expression) : Stmt;

    public record VarStatement(Token Name, Expr? Initializer) : Stmt;

    public record BlockStatement(IEnumerable<Stmt> Statements) : Stmt;

    public record IfStatement(Expr Condition, Stmt ThenBranch, Stmt? ElseBranch) : Stmt;

    public record WhileStatement(Expr Condition, Stmt Body) : Stmt;

    public record BreakStatement : Stmt;

    public record ContinueStatement : Stmt;

    public record FunctionStatement(Token Name, FuncExpr FuncExpr) : Stmt;

    public record ReturnStatement(Token Keyword, Expr? Value) : Stmt;
}