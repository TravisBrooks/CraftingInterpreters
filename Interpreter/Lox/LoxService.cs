using Lox.Callable;

namespace Lox
{
    public class LoxService
    {
        private readonly IConsole _console;
        private readonly ErrorLogger _errorLogger;
        private readonly EnvironmentContext _environmentContext;
        private readonly Interpreter _interpreter;

        public LoxService(
            IConsole console,
            ErrorLogger errorLogger,
            EnvironmentContext environmentContext,
            Interpreter interpreter)
        {
            _console = console;
            _errorLogger = errorLogger;
            _environmentContext = environmentContext;
            _interpreter = interpreter;
        }

        public int Start(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                RunPrompt();
            }
            else
            {
                RunFile(fileName);
            }

            if (_errorLogger.HadError())
            {
                _errorLogger.LogAllErrors();
                return 65;
            }

            if (_errorLogger.HadRuntimeError())
            {
                _errorLogger.LogAllRuntimeErrors();
                return 70;
            }

            return 0;
        }

        private void RunFile(string fileName)
        {
            try
            {
                var source = File.ReadAllText(fileName);
                _environmentContext.LoxMode = LoxMode.SCRIPT_MODE;
                Run(source);
            }
            catch (FileNotFoundException)
            {
                _console.WriteErrorLine($"Error: File not found: {fileName}");
            }
            catch (System.Exception ex)
            {
                _console.WriteErrorLine($"Error: {ex.Message}");
            }
        }

        private void RunPrompt()
        {
            _environmentContext.LoxMode = LoxMode.INTERACTIVE_MODE;
            while (true)
            {
                _console.WriteLine(">");
                var line = _console.ReadLine();
                if (line is null)
                {
                    break;
                }

                Run(line);
                if (_errorLogger.HadError())
                {
                    _errorLogger.LogAllErrors();
                    _errorLogger.ClearErrors();
                }

                if (_errorLogger.HadRuntimeError())
                {
                    _errorLogger.LogAllRuntimeErrors();
                    _errorLogger.ClearRuntimeErrors();
                }
            }
        }

        public void Run(string loxCode)
        {
            if (_environmentContext.Environment.GetGlobal("clock") is null)
            {
                _environmentContext.Environment.DefineGlobal("clock", new Clock());
            }
            var scanner = new Scanner(_errorLogger, loxCode);
            var tokens = scanner.ScanTokens();
            var parser = new Parser(_errorLogger, tokens);
            var statements = parser.Parse();
            // RuntimeError only gets called by Interpreter.Interpret so no need to check for that here.
            if (_errorLogger.HadError())
            {
                return;
            }

            _interpreter.Interpret(statements);
        }

    }
}
