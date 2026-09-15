namespace LoxTest
{
    public class BlockTest : InterpreterTestBase
    {
        [Fact]
        public void EmptyBlock()
        {
            var source = """
                {} // By itself.
                
                // In a statement.
                if (true) {}
                if (false) {} else {}
                
                print "ok"; // expect: ok
                """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["ok"], outputMessages);
        }

        [Fact]
        public void BlockScope()
        {
            var source = """
                var a = "outer";
                
                {
                  var a = "inner";
                  print a; // expect: inner
                }
                
                print a; // expect: outer
                """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["inner", "outer"], outputMessages);
        }
    }
}
