namespace TreeWalk
{
    public abstract record Stmt : IAstNode
    {
        public virtual T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public record ExprStatement(Expr Expression) : Stmt;

    public record PrintStatement(Expr Expression) : Stmt;

    public record VarStatement(Token Name, Expr? Initializer) : Stmt;

    public record BlockStatement(IEnumerable<Stmt> Statements) : Stmt;
}