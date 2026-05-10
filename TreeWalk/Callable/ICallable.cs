using Lox.InterpreterVisitors;

namespace Lox.Callable
{
    internal interface ICallable
    {
        object? Call(StatementVisitor visitor, List<object?> arguments);
        int Arity();
    }
}