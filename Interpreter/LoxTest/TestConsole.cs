using Lox;

namespace LoxTest
{
    public class TestConsole : IConsole
    {
        public List<ConsoleOutput> OutputMessages { get; } = [];
        public Queue<string> InputMessages { get; } = new();

        public void WriteLine(string message)
        {
            OutputMessages.Add(new ConsoleOutput(false, message));
        }

        public void WriteErrorLine(string message)
        {
            OutputMessages.Add(new ConsoleOutput(true, message));
        }

        public string? ReadLine()
        {
            return InputMessages.Count > 0 ? InputMessages.Dequeue() : null;
        }

        public bool HasOutputError()
        {
            return OutputMessages.Any(output => output.IsError);
        }

        public string[] OutputMessagesNoStatus()
        {
            return OutputMessages.Select(output => output.Message).ToArray();
        }
    }

    public record ConsoleOutput(bool IsError, string Message);
}