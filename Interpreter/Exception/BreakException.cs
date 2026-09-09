namespace Lox.Exception
{
    public class BreakException : System.Exception
    {
        public override string StackTrace => "suppressed for performance";
        public override string? Source => "suppressed for performance";
    }
}