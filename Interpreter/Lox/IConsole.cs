namespace Lox
{
    /// <summary>
    /// Abstraction for the Console class.
    /// </summary>
    public interface IConsole
    {
        public void WriteLine(string message);
        public void WriteErrorLine(string message);
        public string? ReadLine();
    }
}