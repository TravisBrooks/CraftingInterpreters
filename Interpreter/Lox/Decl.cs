namespace Lox
{
    public abstract record Decl : IAstNode
    {
        public TResult Accept<TResult>(IVisitor<Decl, TResult> visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// Explicit implementation of IAstNode.Accept to hide this more generic version from the public API of Decl
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="visitor"></param>
        /// <returns></returns>
        TResult IAstNode.Accept<TResult>(IVisitor<IAstNode, TResult> visitor)
        {
            return Accept(visitor);
        }
    }

    public record FunDecl(Token Name, FunExpr FunExpr) : Decl;

    public record VarDecl(Token Name, Expr? Initializer) : Decl;
}