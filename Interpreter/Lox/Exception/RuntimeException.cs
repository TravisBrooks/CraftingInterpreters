namespace Lox.Exception
{
    public class RuntimeException : System.Exception
    {
        public RuntimeException(Token? token, string message) : base(message)
        {
            Token = token;
        }

        public Token? Token { get; }
    }
}