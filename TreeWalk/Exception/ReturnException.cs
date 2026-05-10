namespace Lox.Exception
{
    public class ReturnException : System.Exception
    {
        public ReturnException(object? value)
        {
            Value = value;
        }

        public object? Value { get; }

        public override string StackTrace => "suppressed for performance";
        public override string? Source => "suppressed for performance";
    }
}
