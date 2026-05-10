using Lox.InterpreterVisitors;

namespace Lox.Callable
{
    /// <summary>
    /// Implementation of the clock native function
    /// </summary>
    internal class Clock : ICallable
    {
        public int Arity()
        {
            return 0;
        }

        public object? Call(StatementVisitor visitor, List<object?> arguments)
        {
            return DateTime.Now;
        }

        public override string ToString()
        {
            return "<native fn clock()>";
        }
    }
}
