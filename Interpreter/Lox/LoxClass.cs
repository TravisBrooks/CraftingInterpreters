using Lox.Callable;
using Lox.InterpreterVisitors;
using System.ComponentModel.DataAnnotations;

namespace Lox
{
    public class LoxClass : ICallable
    {
        private readonly Dictionary<string, LoxFunction> _methods;

        public LoxClass(string name, Dictionary<string, LoxFunction> methods)
        {
            Name = name;
            _methods = methods;
        }

        public string Name { get; }

        public object? Call(DeclarationVisitor visitor, List<object?> arguments)
        {
            var instance = new LoxInstance(this);
            var initializer = FindMethod("init");
            if (initializer is not null)
            {
                initializer.Bind(instance).Call(visitor, arguments);
            }
            return instance;
        }

        public int Arity()
        {
            var initializer = FindMethod("init");
            if (initializer is null)
            {
                return 0;
            }
            return initializer.Arity();
        }

        public override string ToString()
        {
            return Name;
        }

        public LoxFunction? FindMethod(string name)
        {
            return _methods.GetValueOrDefault(name);
        }
    }
}