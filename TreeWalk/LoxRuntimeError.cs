namespace TreeWalk
{
    public class LoxRuntimeError : Exception
    {
        private readonly Token? _token;

        public LoxRuntimeError(Token? token, string message) : base(message)
        {
            _token = token;
        }

        public Token? Token => _token;
    }
}