namespace TreeWalk
{

    /// <summary>
    /// Follow the unit pattern that F# uses to have a return type for what otherwise would be void methods that can be used in expressions.
    /// I really only use this in StatementVisitor so that it can use the IVisitor interface, but i could expand its usage to all public methods.
    /// </summary>
    public struct Unit
    {
        public static Unit Value => default;
    }
}