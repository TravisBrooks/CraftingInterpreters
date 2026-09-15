namespace Lox
{
    internal class ConsoleWrapper : IConsole
    {
        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public void WriteErrorLine(string message)
        {
            Console.Error.WriteLine(message);
        }

        public string? ReadLine()
        {
            return Console.ReadLine();
        }
    }
}