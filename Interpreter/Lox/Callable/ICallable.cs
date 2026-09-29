using Lox.InterpreterVisitors;

namespace Lox.Callable
{
    internal interface ICallable
    {
        object? Call(DeclarationVisitor visitor, List<object?> arguments);
        int Arity();
    }
}