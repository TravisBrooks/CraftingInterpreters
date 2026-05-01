namespace TreeWalk
{
    public class Environment
    {
        private readonly Environment? _enclosing;
        private readonly Dictionary<string, object?> _values = new(StringComparer.Ordinal);

        // only called for the initial GlobalMemory, which has no enclosing environment.
        private Environment()
        {
            _enclosing = null;
        }

        public Environment(Environment enclosing)
        {
            _enclosing = enclosing;
        }

        public static Environment GlobalMemory { get; set; } = new();

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

            if (_enclosing is not null)
            {
                return _enclosing.Get(name);
            }

            throw new LoxRuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
        }

        public void Assign(Token name, object? value)
        {
            if (_values.ContainsKey(name.Lexeme))
            {
                _values[name.Lexeme] = value;
                return;
            }

            if (_enclosing is not null)
            {
                _enclosing.Assign(name, value);
                return;
            }

            throw new LoxRuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
        }
    }
}