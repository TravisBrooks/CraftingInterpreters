using System.Diagnostics;

namespace TreeWalk
{
    public static class Lox
    {
        private static readonly List<string> Errors = [];
        private static readonly List<string> RuntimeErrors = [];
        private static readonly Interpreter Interpreter = new();

        public static Environment GlobalEnvironment { get; } = new();

        #region Public interface

        public static int Main(string[] args)
        {
            var timer = new Stopwatch();
            timer.Start();
            if (args.Length > 1)
            {
                Console.Error.WriteLine("Usage: CLox [script]");
                return 64;
            }

            if (args.Length == 1)
            {
                RunFile(args[0]);
                timer.Stop();
                Console.WriteLine();
                Console.WriteLine($"Execution time: {timer.Elapsed}");
            }
            else
            {
                RunPrompt();
            }

            if (HadError())
            {
                PrintErrors();
                return 65;
            }

            if (HadRuntimeError())
            {
                PrintRuntimeErrors();
                return 70;
            }

            return 0;
        }

        public static void Error(int line, string message)
        {
            ReportError(line, string.Empty, message);
        }

        public static void Error(Token token, string errorMessage)
        {
            var whereStr = token.TokenType == TokenType.EOF ? " at end" : $" at '{token.Lexeme}'";
            ReportError(token.Line, whereStr, errorMessage);
        }

        public static void RuntimeError(LoxRuntimeError lre)
        {
            ReportRuntimeError(lre);
        }

        #endregion

        #region Private Lox grammar processors

        private static void RunFile(string pathToScript)
        {
            var source = File.ReadAllText(pathToScript);
            GlobalEnvironment.LoxMode = LoxMode.SCRIPT_MODE;
            Run(source);
        }

        private static void Run(string source)
        {
            var scanner = new Scanner(source);
            var tokens = scanner.ScanTokens();
            var parser = new Parser(tokens);
            var statements = parser.Parse();
            // RuntimeError only gets called by Interpreter.Interpret so no need to check for that here.
            if (HadError())
            {
                return;
            }

            Interpreter.Interpret(statements);
        }

        private static void RunPrompt()
        {
            GlobalEnvironment.LoxMode = LoxMode.INTERACTIVE_MODE;
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
                if (HadError())
                {
                    PrintErrors();
                    Errors.Clear();
                }

                if (HadRuntimeError())
                {
                    PrintRuntimeErrors();
                    RuntimeErrors.Clear();
                }
            }
        }

        #endregion

        #region Private Lox grammar error handling

        private static void ReportError(int line, string where, string message)
        {
            Errors.Add($"ERROR [line {line}] Error{where}: {message}");
        }

        private static void ReportRuntimeError(LoxRuntimeError lre)
        {
            var lineNumber = lre.Token is null ? "(Unknown)" : lre.Token.Line.ToString();
            RuntimeErrors.Add($"RUNTIME ERROR [line {lineNumber}]: {lre.Message}");
        }

        private static bool HadError()
        {
            return Errors.Count > 0;
        }

        private static bool HadRuntimeError()
        {
            return RuntimeErrors.Count > 0;
        }

        private static void PrintErrors()
        {
            foreach (var error in Errors)
            {
                Console.Error.WriteLine(error);
            }
        }

        private static void PrintRuntimeErrors()
        {
            foreach (var error in RuntimeErrors)
            {
                Console.Error.WriteLine(error);
            }
        }

        #endregion
    }
}