namespace LoxTest
{
    public class CallTest : InterpreterTestBase
    {
        [Theory]
        [InlineData("true();", "Can only call functions and classes.")]
        [InlineData("nil();", "Can only call functions and classes.")]
        [InlineData("123();", "Can only call functions and classes.")]
        [InlineData(data: [
            """
            class Foo {}
            
            var foo = Foo();
            foo(); // expect runtime error: Can only call functions and classes.
            """, "Can only call functions and classes."], Skip = "Haven't implemented classes yet")]
        [InlineData("\"str\"();", "Can only call functions and classes.")]
        public void IsItCallable(string source, string expected)
        {
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains(expected, msg);
        }
    }
}
