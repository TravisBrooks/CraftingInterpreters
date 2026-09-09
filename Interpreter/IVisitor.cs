namespace Lox
{
    public interface IVisitor<in TNode, out TResult> where TNode : IAstNode
    {
        TResult Visit(TNode node);
    }
}