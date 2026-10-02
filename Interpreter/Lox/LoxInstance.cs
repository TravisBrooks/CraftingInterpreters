using Lox.Exception;

namespace Lox
{
    public class LoxInstance
    {
        private readonly LoxClass _loxClass;
        private readonly Dictionary<string, object?> _fields = new();

        public LoxInstance(LoxClass loxClass)
        {
            _loxClass = loxClass;
        }

        public object? Get(Token name)
        {
            if (_fields.TryGetValue(name.Lexeme, out var value))
            {
                return value;
            }

            var method = _loxClass.FindMethod(name.Lexeme);
            if (method is not null)
            {
                return method.Bind(this);
            }

            throw new RuntimeException(name, $"Undefined property '{name.Lexeme}'.");
        }

        public void Set(Token name, object? value)
        {
            _fields[name.Lexeme] = value;
        }

        public override string ToString()
        {
            return $"{_loxClass.Name} instance";
        }
    }
}