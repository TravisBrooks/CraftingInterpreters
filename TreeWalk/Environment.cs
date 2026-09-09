using Lox.Exception;

namespace Lox
{
    public class Environment
    {
        private readonly Dictionary<string, object?> _values;

        public Environment(Environment? enclosing = null)
        {
            _values = new Dictionary<string, object?>();
            Enclosing = enclosing;
        }

        public Environment? Enclosing { get; }

        public void Define(string name, object? value)
        {
            _values[name] = value;
        }

        public void DefineGlobal(string name, object? value)
        {
            if (Enclosing is not null)
            {
                Enclosing.DefineGlobal(name, value);
            }
            else
            {
                _values[name] = value;
            }
        }

        public object? Get(Token name)
        {
            if (_values.TryGetValue(name.Lexeme, out var value))
            {
                return value;
            }
            if (Enclosing is not null)
            {
                return Enclosing.Get(name);
            }
            throw new RuntimeException(name, $"Undefined variable '{name.Lexeme}'.");
        }

        public void Assign(Token name, object? value)
        {
            // Check if the variable exists in the current environment
            if (_values.ContainsKey(name.Lexeme))
            {
                _values[name.Lexeme] = value;
                return;
            }
            // If not, check the enclosing environment(s)
            if (Enclosing is not null)
            {
                Enclosing.Assign(name, value);
                return;
            }
            throw new RuntimeException(name, $"Undefined variable '{name.Lexeme}'.");
        }

        public static void ExecuteInScope(EnvironmentContext ctxt, Action action)
        {
            var previous = ctxt.Environment;
            ctxt.Environment = new Environment(previous);
            try
            {
                action();
            }
            finally
            {
                ctxt.Environment = previous;
            }
        }
    }
}