using Lox.Exception;

namespace Lox
{
    public class ErrorLogger
    {
        private readonly IConsole _console;
        private readonly List<string> _errors = [];
        private readonly List<string> _runtimeErrors = [];

        public ErrorLogger(IConsole console)
        {
            _console = console;
        }

        public void ReportError(int line, string where, string message)
        {
            _errors.Add($"ERROR [line {line}] Error{where}: {message}");
        }

        public void ReportError(int line, string message)
        {
            ReportError(line, string.Empty, message);
        }

        public void ReportError(Token token, string errorMessage)
        {
            var whereStr = token.TokenType == TokenType.EOF ? " at end" : $" at '{token.Lexeme}'";
            ReportError(token.Line, whereStr, errorMessage);
        }

        public void ReportRuntimeError(RuntimeException lre)
        {
            var lineNumber = lre.Token is null ? "(Unknown)" : lre.Token.Line.ToString("N0");
            _runtimeErrors.Add($"RUNTIME ERROR [line {lineNumber}]: {lre.Message}");
        }

        public bool HadError()
        {
            return _errors.Count > 0;
        }

        public bool HadRuntimeError()
        {
            return _runtimeErrors.Count > 0;
        }

        public void LogAllErrors()
        {
            foreach (var error in _errors)
            {
                _console.WriteErrorLine(error);
            }
        }

        public void LogAllRuntimeErrors()
        {
            foreach (var error in _runtimeErrors)
            {
                _console.WriteErrorLine(error);
            }
        }

        public void ClearErrors()
        {
            _errors.Clear();
        }

        public void ClearRuntimeErrors()
        {
            _runtimeErrors.Clear();
        }
    }
}