namespace LoxTest
{
    public class ReturnTest : InterpreterTestBase
    {
        [Fact]
        public void AfterElse()
        {
            var source = """
                         fun f() {
                           if (false) "no"; else return "ok";
                         }
                         
                         print f(); // expect: ok
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["ok"], outputMessages);
        }

        [Fact]
        public void AfterIf()
        {
            var source = """
                         fun f() {
                           if (true) return "ok";
                         }
                         
                         print f(); // expect: ok
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["ok"], outputMessages);
        }

        [Fact]
        public void AfterWhile()
        {
            var source = """
                         fun f() {
                           while (true) return "ok";
                         }
                         
                         print f(); // expect: ok
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["ok"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented Resolver yet")]
        public void AtTopLevel()
        {
            var source = """
                         return "wat"; // Error at 'return': Can't return from top-level code.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Error at 'return': Can't return from top-level code.", msg);
        }

        [Fact]
        public void InFunction()
        {
            var source = """
                         fun f() {
                           return "ok";
                           print "bad";
                         }
                         
                         print f(); // expect: ok
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["ok"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void InMethod()
        {
            var source = """
                         class Foo {
                           method() {
                             return "ok";
                             print "bad";
                           }
                         }
                         
                         print Foo().method(); // expect: ok
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["ok"], outputMessages);
        }

        [Fact]
        public void ReturnNilIfNoValue()
        {
            var source = """
                         fun f() {
                           return;
                           print "bad";
                         }
                         
                         print f(); // expect: nil
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["nil"], outputMessages);
        }
    }
}