namespace LoxTest
{
    public class BoolTest : InterpreterTestBase
    {
        [Theory]
        [InlineData("print true == true;", "true")]
        [InlineData("print true == false;", "false")]
        [InlineData("print false == true;", "false")]
        [InlineData("print false == false;", "true")]
        [InlineData("print true == 1;", "false")]
        [InlineData("print false == 0;", "false")]
        [InlineData("""print true == "true";""", "false")]
        [InlineData("""print false == "false";""", "false")]
        [InlineData("""print false == "";""", "false")]
        [InlineData("print true != true;", "false")]
        [InlineData("print true != false;", "true")]
        [InlineData("print false != true;", "true")]
        [InlineData("print false != false;", "false")]
        [InlineData("print true != 1;", "true")]
        [InlineData("print false != 0;", "true")]
        [InlineData("""print true != "true";""", "true")]
        [InlineData("""print false != "false";""", "true")]
        [InlineData("""print false != "";""", "true")]
        public void Equality(string source, string expected)
        {
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(new[] { expected }, outputMessages);
        }

        [Fact]
        public void NotOperator()
        {
            var source = """
                print !true;    // expect: false
                print !false;   // expect: true
                print !!true;   // expect: true
            """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["false", "true", "true"], outputMessages);
        }
    }
}