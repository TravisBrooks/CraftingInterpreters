using Lox.Callable;
using Microsoft.Extensions.Logging;

namespace Lox
{
    public class LoxService
    {
        private readonly ILogger<LoxService> _logger;
        private readonly ErrorLogger _errorLogger;
        private readonly EnvironmentContext _environmentContext;
        private readonly Interpreter _interpreter;

        public LoxService(
            ILogger<LoxService> logger,
            ErrorLogger errorLogger,
            EnvironmentContext environmentContext,
            Interpreter interpreter)
        {
            _logger = logger;
            _errorLogger = errorLogger;
            _environmentContext = environmentContext;
            _interpreter = interpreter;
        }

        public int Start(string? fileName)
        {
            _environmentContext.Environment.DefineGlobal("clock", new Clock());
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
                _logger.LogError("Error: File not found: {0}", fileName);
            }
            catch (System.Exception ex)
            {
                _logger.LogError("Error: {0}", ex.Message);
            }
        }

        private void RunPrompt()
        {
            _environmentContext.LoxMode = LoxMode.INTERACTIVE_MODE;
            using var reader = new StreamReader(Console.OpenStandardInput());
            while (true)
            {
                Console.WriteLine(">");
                var line = reader.ReadLine();
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

        private void Run(string loxCode)
        {
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
