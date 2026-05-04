namespace Lox
{
    public class LoxRuntimeError : Exception
    {
        public LoxRuntimeError(Token? token, string message) : base(message)
        {
            Token = token;
        }

        public Token? Token { get; }
    }
}