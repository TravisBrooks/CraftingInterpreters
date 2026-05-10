namespace Lox.Exception
{
    public class ContinueException : System.Exception
    {
        public override string StackTrace => "suppressed for performance";
        public override string? Source => "suppressed for performance";
    }
}