namespace TreeWalk
{
    public interface IVisitor<T>
    {
        T Visit(Expr e);
    }
}
