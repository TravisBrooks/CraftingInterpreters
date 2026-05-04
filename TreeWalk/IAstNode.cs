namespace Lox
{
    /// <summary>
    /// A node in an abstract syntax tree (such as an expression or statement) that can be visited by a visitor.
    /// </summary>
    public interface IAstNode
    {
        TResult Accept<TResult>(IVisitor<IAstNode, TResult> visitor);
    }
}