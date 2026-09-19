namespace LoxTest
{
    public class PrintTest : InterpreterTestBase
    {
        [Fact]
        public void MissingArgument()
        {
            var source = """
                         // [line 2] Error at ';': Expect expression.
                         print;
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Error at ';': Expect expression.", msg);
        }
    }
}
