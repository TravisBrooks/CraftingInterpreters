namespace LoxTest
{
    public class AssignmentTest : InterpreterTestBase
    {
        [Fact]
        public void Associativity()
        {
            var source = """
                var a = "a";
                var b = "b";
                var c = "c";
            
                // Assignment is right-associative.
                a = b = c;
                print a;
                print b;
                print c; 
            """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["c", "c", "c"], outputMessages);
        }

        [Fact]
        public void GlobalAssignment()
        {
            var source = """
                var a = "before";
                print a; // expect: before
        
                a = "after";
                print a; // expect: after
        
                print a = "arg"; // expect: arg
                print a; // expect: arg
            """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["before", "after", "arg", "arg"], outputMessages);
        }

        [Fact]
        public void Grouping()
        {
            var source = """
                             var a = "a";
                             (a) = "value"; // Error at '=': Invalid assignment target.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Single(outputMessages);
            var msg = outputMessages.First();
            Assert.Contains("Invalid assignment target.", msg);
        }

        [Fact]
        public void InfixOperator()
        {
            var source = """
                             var a = "a";
                             var b = "b";
                             a + b = "value"; // Error at '=': Invalid assignment target.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Single(outputMessages);
            var msg = outputMessages.First();
            Assert.Contains("Invalid assignment target.", msg);
        }

        [Fact]
        public void LocalScope()
        {
            var source = """
                {
                  var a = "before";
                  print a; // expect: before
            
                  a = "after";
                  print a; // expect: after
            
                  print a = "arg"; // expect: arg
                  print a; // expect: arg
                }
            """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["before", "after", "arg", "arg"], outputMessages);
        }

        [Fact]
        public void PrefixOperator()
        {
            var source = """
                            var a = "a";
                            !a = "value"; // Error at '=': Invalid assignment target.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Single(outputMessages);
            var msg = outputMessages.First();
            Assert.Contains("Invalid assignment target.", msg);
        }

        [Fact]
        public void Syntax()
        {
            var source = """
                             // Assignment on RHS of variable.
                             var a = "before";
                             var c = a = "var";
                             print a; // expect: var
                             print c; // expect: var
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
           Assert.Equal(["var", "var"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void ToThis()
        {
            var source = """
                             class Foo {
                               Foo() {
                                 this = "value"; // Error at '=': Invalid assignment target.
                               }
                             }
                         
                             Foo();
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Single(outputMessages);
            var msg = outputMessages.First();
            Assert.Contains("Invalid assignment target.", msg);
        }

        [Fact]
        public void Undefined()
        {
            var source = """
                             unknown = "what"; // expect runtime error: Undefined variable 'unknown'.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Single(outputMessages);
            var msg = outputMessages.First();
            Assert.Contains("Undefined variable 'unknown'.", msg);
        }
    }
}