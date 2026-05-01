namespace TreeWalk
{
    /// <summary>
    ///     Following the unit pattern that F# uses so that i can have a return type for void methods that can be used in
    ///     expressions.
    /// </summary>
    public struct Unit
    {
        public static Unit Value => default;
    }
}