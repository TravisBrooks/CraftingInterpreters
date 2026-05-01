namespace TreeWalk
{
    public static class Lox
    {
        private static readonly List<string> Errors = [];
        private static readonly List<string> RuntimeErrors = [];
        private static readonly Interpreter Interpreter = new();
        private static LoxMode _loxMode;

        #region Public interface

        public static int Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.Error.WriteLine("Usage: CLox [script]");
                return 64;
            }

            if (args.Length == 1)
            {
                _RunFile(args[0]);
            }
            else
            {
                _RunPrompt();
            }

            if (_HadError())
            {
                _PrintErrors();
                return 65;
            }

            if (_HadRuntimeError())
            {
                _PrintRuntimeErrors();
                return 70;
            }

            return 0;
        }

        public static void Error(int line, string message)
        {
            _ReportError(line, string.Empty, message);
        }

        public static void Error(Token token, string errorMessage)
        {
            if (token.TokenType == TokenType.EOF)
            {
                _ReportError(token.Line, " at end", errorMessage);
            }
            else
            {
                _ReportError(token.Line, $" a '{token.Lexeme}'", errorMessage);
            }
        }

        public static void RuntimeError(LoxRuntimeError lre)
        {
            _ReportRuntimeError(lre);
        }

        #endregion

        #region Private Lox grammar processors

        private static void _RunFile(string pathToScript)
        {
            var source = File.ReadAllText(pathToScript);
            _loxMode = LoxMode.SCRIPT_MODE;
            _Run(source);
        }

        private static void _Run(string source)
        {
            var scanner = new Scanner(source);
            var tokens = scanner.ScanTokens();
            var parser = new Parser(tokens);
            var statements = parser.Parse();
            // RuntimeError only gets called by Interpreter.Interpret so no need to check for that here.
            if (_HadError())
            {
                return;
            }

            Interpreter.Interpret(statements, _loxMode);
        }

        private static void _RunPrompt()
        {
            _loxMode = LoxMode.INTERACTIVE_MODE;
            using var reader = new StreamReader(Console.OpenStandardInput());
            while (true)
            {
                Console.WriteLine(">");
                var line = reader.ReadLine();
                if (line is null)
                {
                    break;
                }

                _Run(line);
                if (_HadError())
                {
                    _PrintErrors();
                    Errors.Clear();
                }

                if (_HadRuntimeError())
                {
                    _PrintRuntimeErrors();
                    RuntimeErrors.Clear();
                }
            }
        }

        #endregion

        #region Private Lox grammar error handling

        private static void _ReportError(int line, string where, string message)
        {
            Errors.Add($"ERROR [line {line}] Error{where}: {message}");
        }

        private static void _ReportRuntimeError(LoxRuntimeError lre)
        {
            if (lre.Token is null)
            {
                RuntimeErrors.Add($"RUNTIME ERROR [line (Unknown))]: {lre.Message}");
            }
            else
            {
                RuntimeErrors.Add($"RUNTIME ERROR [line {lre.Token.Line}]: {lre.Message}");
            }
        }

        private static bool _HadError()
        {
            return Errors.Count > 0;
        }

        private static bool _HadRuntimeError()
        {
            return RuntimeErrors.Count > 0;
        }

        private static void _PrintErrors()
        {
            foreach (var error in Errors)
            {
                Console.Error.WriteLine(error);
            }
        }

        private static void _PrintRuntimeErrors()
        {
            foreach (var error in RuntimeErrors)
            {
                Console.Error.WriteLine(error);
            }
        }

        #endregion
    }
}