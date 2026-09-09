namespace Lox
{

    /// <summary>
    /// Follow the unit pattern that F# uses to have a return type for what otherwise would be void methods that can be used in expressions.
    /// </summary>
    public struct Unit
    {
        public static Unit Value => default;

        /// <summary>
        /// Override + so that you can chain multiple Unit returning methods together without needing to create a new Unit variable for each one.
        /// It's logically similar to concatenating two empty strings to create a third empty string.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Unit operator +(Unit lhs, Unit rhs)
        {
            return Value;
        }
    }
}