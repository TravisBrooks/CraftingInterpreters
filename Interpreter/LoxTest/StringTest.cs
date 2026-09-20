namespace LoxTest
{
    public class StringTest : InterpreterTestBase
    {
        [Fact]
        public void ErrorAfterMultiline()
        {
            var source = """
                         // Tests that we correctly track the line info across multiline strings.
                         var a = "1
                         2
                         3
                         ";
                         
                         err; // // expect runtime error: Undefined variable 'err'.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Undefined variable 'err'.", msg);
        }

        [Fact]
        public void Literals()
        {
            var source = """
                         print "(" + "" + ")";   // expect: ()
                         print "a string"; // expect: a string
                         
                         // Non-ASCII.
                         print "A~¶Þॐஃ"; // expect: A~¶Þॐஃ
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["()", "a string", "A~¶Þॐஃ"], outputMessages);
        }

        [Fact]
        public void Multiline()
        {
            var source = """
                         var a = "1
                         2
                         3";
                         print a;
                         // expect: 1
                         // expect: 2
                         // expect: 3
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal([@"1
2
3"], outputMessages);
        }

        [Fact]
        public void Unterminated()
        {
            var source = """
                         // [line 2] Error: Unterminated string.
                         "this string has no close quote
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Error: Unterminated string.", msg);
        }
    }
}