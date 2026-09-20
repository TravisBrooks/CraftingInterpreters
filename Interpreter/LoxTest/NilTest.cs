namespace LoxTest
{
    public class NilTest : InterpreterTestBase
    {
        [Fact]
        public void Literal()
        {
            var source = """
                         print nil; // expect: nil
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["nil"], outputMessages);
        }
    }
}