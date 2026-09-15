namespace LoxTest
{
    public class IfTest : InterpreterTestBase
    {
        [Fact(Skip = "Haven't implemented classes yet")]
        public void ClassInElse()
        {
            var source = """
                         // [line 2] Error at 'class': Expect expression.
                         if (true) "ok"; else class Foo {}
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["[line 2] Error at 'class': Expect expression."], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void ClassInThen()
        {
            var source = """
                         // [line 2] Error at 'class': Expect expression.
                         if (true) class Foo {}
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["[line 2] Error at 'class': Expect expression."], outputMessages);
        }

        [Fact]
        public void DanglingElse()
        {
            var source = """
                         // A dangling else binds to the right-most if.
                         if (true) if (false) print "bad"; else print "good"; // expect: good
                         if (false) if (true) print "bad"; else print "bad";
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["good"], outputMessages);
        }

        [Fact]
        public void ElseBranch()
        {
            var source = """
                         // Evaluate the 'else' expression if the condition is false.
                         if (true) print "good"; else print "bad"; // expect: good
                         if (false) print "bad"; else print "good"; // expect: good
                         
                         // Allow block body.
                         if (false) nil; else { print "block"; } // expect: block
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["good", "good", "block"], outputMessages);
        }

        [Fact]
        public void FunctionInElse()
        {
            var source = """
                         // [line 2] Error at 'fun': Expect expression.
                         if (true) "ok"; else fun foo() {}
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.First();
            Assert.Contains("[line 2] Error at 'fun': Only anonymous lambda expressions are expected here.", msg);
        }

        [Fact]
        public void FunctionInThen()
        {
            var source = """
                         // [line 2] Error at 'fun': Expect expression.
                         if (true) fun foo() {}
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.First();
            Assert.Contains("[line 2] Error at 'fun': Only anonymous lambda expressions are expected here.", msg);
        }

        [Fact]
        public void IfStatement()
        {
            var source = """
                         // Evaluate the 'then' expression if the condition is true.
                         if (true) print "good"; // expect: good
                         if (false) print "bad";
                         
                         // Allow block body.
                         if (true) { print "block"; } // expect: block
                         
                         // Assignment in if condition.
                         var a = false;
                         if (a = true) print a; // expect: true
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["good", "block", "true"], outputMessages);
        }

        [Fact]
        public void Truth()
        {
            var source = """
                         // False and nil are false.
                         if (false) print "bad"; else print "false"; // expect: false
                         if (nil) print "bad"; else print "nil"; // expect: nil
                         
                         // Everything else is true.
                         if (true) print true; // expect: true
                         if (0) print 0; // expect: 0
                         if ("") print "empty"; // expect: empty
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["false", "nil", "true", "0", "empty"], outputMessages);
        }

        [Fact]
        public void VarInElse()
        {
            var source = """
                         // [line 2] Error at 'var': Expect expression.
                         if (true) "ok"; else var foo;
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.First();
            Assert.Contains("[line 2] Error at 'var': Expect expression.", msg);
        }

        [Fact]
        public void VarInThen()
        {
            var source = """
                         // [line 2] Error at 'var': Expect expression.
                         if (true) var foo;
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.First();
            Assert.Contains("[line 2] Error at 'var': Expect expression.", msg);
        }
    }
}
