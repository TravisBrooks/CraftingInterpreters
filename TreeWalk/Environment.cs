namespace Lox
{
    public class Environment
    {
        private readonly Stack<Dictionary<string, object?>> _scopedMemoryStack = new();

        public Environment()
        {
            // Add the global scope
            _scopedMemoryStack.Push(new Dictionary<string, object?>(StringComparer.Ordinal));
        }

        public LoxMode LoxMode { get; set; } = LoxMode.SCRIPT_MODE;

        public void EnterInnerScope()
        {
            _scopedMemoryStack.Push(new Dictionary<string, object?>(StringComparer.Ordinal));
        }

        public void ExitInnerScope()
        {
            // Ensure we don't pop the global scope
            if (_scopedMemoryStack.Count > 1)
            {
                _scopedMemoryStack.Pop();
            }
        }

        public void Define(string name, object? value)
        {
            CurrentScope[name] = value;
        }

        public object? Get(Token name)
        {
            foreach (var scope in _scopedMemoryStack)
            {
                if (scope.TryGetValue(name.Lexeme, out var value))
                {
                    return value;
                }
            }

            throw new LoxRuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
        }

        public void Assign(Token name, object? value)
        {
            var match = _scopedMemoryStack.FirstOrDefault(scope => scope.ContainsKey(name.Lexeme));
            if (match is null)
            {
                throw new LoxRuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
            }

            match[name.Lexeme] = value;
        }

        private Dictionary<string, object?> CurrentScope => _scopedMemoryStack.Peek();
    }
}