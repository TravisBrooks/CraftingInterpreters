namespace TreeWalk
{
    public class Environment
    {
        private readonly Stack<Dictionary<string, object?>> _scopedMemoryStack = new();

        public Environment()
        {
            EnterInnerScope();
        }

        private Dictionary<string, object?> CurrentScope => _scopedMemoryStack.Peek();

        public LoxMode LoxMode { get; set; } = LoxMode.SCRIPT_MODE;

        public void EnterInnerScope()
        {
            _scopedMemoryStack.Push(new Dictionary<string, object?>(StringComparer.Ordinal));
        }

        public void ExitInnerScope()
        {
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
            foreach (var scope in _scopedMemoryStack)
            {
                if (scope.ContainsKey(name.Lexeme))
                {
                    scope[name.Lexeme] = value;
                    return;
                }
            }

            throw new LoxRuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
        }
    }
}