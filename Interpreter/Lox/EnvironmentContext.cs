namespace Lox
{
    public class EnvironmentContext
    {
        public Environment Environment { get; set; } = new();
        public LoxMode LoxMode { get; set; } = LoxMode.SCRIPT_MODE;
    }
}