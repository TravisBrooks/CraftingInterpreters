namespace TreeWalk
{
    public interface IVisitor<out T>
    {
        T Visit(IAstNode node);
    }
}