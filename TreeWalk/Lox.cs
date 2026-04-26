namespace TreeWalk
{
    public class Lox
    {
        private static readonly List<string> Errors = [];

        private static int Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.Error.WriteLine("Usage: CLox [script]");
                return 64;
            }

            if (args.Length == 1)
            {
                RunFile(args[0]);
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

            return 0;
        }

        private static void RunFile(string pathToScript)
        {
            var source = File.ReadAllText(pathToScript);
            Run(source);
        }

        private static void Run(string source)
        {
            var scanner = new Scanner(source);
            var tokens = scanner.ScanTokens();
            var parser = new Parser(tokens);
            var expression = parser.Parse();
            if (HadError())
            {
                return;
            }

            // For now, just print the expression tree.
            Console.WriteLine(new AstPrinter().Print(expression));
        }

        private static void RunPrompt()
        {
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
            }
        }

        public static void Error(int line, string message)
        {
            ReportError(line, string.Empty, message);
        }

        private static void ReportError(int line, string where, string message)
        {
            Errors.Add($"[line {line}] Error{where}: {message}");
        }

        public static void Error(Token token, string errorMessage)
        {
            if (token.TokenType == TokenType.EOF)
            {
                ReportError(token.Line, " at end", errorMessage);
            }
            else
            {
                ReportError(token.Line, $" a '{token.Lexeme}'", errorMessage);
            }
        }

        private static bool HadError()
        {
            return Errors.Count > 0;
        }

        private static void PrintErrors()
        {
            foreach (var error in Errors)
            {
                Console.Error.WriteLine(error);
            }
        }
    }
}