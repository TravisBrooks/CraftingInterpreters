using Lox.Exception;
using System.Collections.ObjectModel;

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

        public void DefineGlobal(string name, object? value)
        {
            _GetGlobalEnv()._values[name] = value;
        }

        public object? GetGlobal(string name)
        {
            return _GetGlobalEnv()._values.GetValueOrDefault(name);
        }

        private Environment _GetGlobalEnv()
        {
            var env = this;
            while (env.Enclosing is not null)
            {
                env = env.Enclosing;
            }
            return env;
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

        /// <summary>
        /// Exposes the internal state in way that cannot be mutated, here for testing purposes
        /// </summary>
        /// <returns></returns>
        public ReadOnlyDictionary<string, object?> GetInternalState()
        {
            return new ReadOnlyDictionary<string, object?>(_values);
        }
    }
}