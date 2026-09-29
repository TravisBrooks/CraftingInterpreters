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

        /// <summary>
        /// For builtin functions like "clock", adds a value onto the global scope
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void DefineIntrinsic(string name, object? value)
        {
            _GetGlobalEnv().Define(name, value);
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

        public object? GetGlobal(Token name)
        {
            return _GetGlobalEnv().Get(name);
        }

        /// <summary>
        /// For builtin functions like "clock", but will grab any matching name in the global scope
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object? GetIntrinsic(string name)
        {
            return _GetGlobalEnv()._values.GetValueOrDefault(name);
        }

        public object? GetAt(int distance, Token name)
        {
            var env = Ancestor(distance);
            if (!env._values.TryGetValue(name.Lexeme, out var val))
            {
                throw new RuntimeException(name, $"Undefined variable '{name.Lexeme}'.");
            }
            return val;
        }

        private Environment Ancestor(int distance)
        {
            var environment = this;
            for (var i = 0; i < distance; i++)
            {
                environment = environment!.Enclosing;
            }
            return environment!;
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

        public void AssignAt(int distance, Token name, object? value)
        {
            Ancestor(distance)._values[name.Lexeme] = value;
        }

        public void AssignGlobal(Token name, object? value)
        {
            _GetGlobalEnv().Assign(name, value);
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