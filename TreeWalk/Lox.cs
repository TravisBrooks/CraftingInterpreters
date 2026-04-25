namespace TreeWalk
{
    public class Lox
    {
        private static readonly List<string> _errors = [];

        static int Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.Error.WriteLine("Usage: CLox [script]");
                return 64;
            }
            else if (args.Length == 1)
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
            if (HadError())
            {
                return;
            }
            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }
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
                    _errors.Clear();
                }
            }
        }

        public static void Error(int line, string message)
        {
            ReportError(line, string.Empty, message);
        }

        private static void ReportError(int line, string where, string message)
        {
            _errors.Add($"[line {line}] Error{where}: {message}");
        }

        private static bool HadError()
        {
            return _errors.Count > 0;
        }

        private static void PrintErrors()
        {
            foreach (var error in _errors)
            {
                Console.Error.WriteLine(error);
            }
        }
    }
}
