namespace LoxTest
{
    public class WhileTest : InterpreterTestBase
    {
        [Fact]
        public void ClassInBody()
        {
            var source = """
                         // [line 2] Error at 'class': Expect expression.
                         while (true) class Foo {}
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Error at 'class': Expect expression.", msg);
        }

        [Fact]
        public void ClosureInBody()
        {
            var source = """
                         var f1;
                         var f2;
                         var f3;
                         
                         var i = 1;
                         while (i < 4) {
                           var j = i;
                           fun f() { print j; }
                         
                           if (j == 1) f1 = f;
                           else if (j == 2) f2 = f;
                           else f3 = f;
                         
                           i = i + 1;
                         }
                         
                         f1(); // expect: 1
                         f2(); // expect: 2
                         f3(); // expect: 3
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["1", "2", "3"], outputMessages);
        }

        [Fact]
        public void FunctionInBody()
        {
            var source = """
                         // [line 2] Error at 'fun': Expect expression.
                         while (true) fun foo() {}
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            // My error message differs slightly because I added lambda expressions
            Assert.Contains("[line 2] Error at 'fun': Only anonymous lambda expressions are expected here.", msg);
        }

        [Fact]
        public void ReturnClosure()
        {
            var source = """
                         fun f() {
                           while (true) {
                             var i = "i";
                             fun g() { print i; }
                             return g;
                           }
                         }
                         
                         var h = f();
                         h(); // expect: i
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["i"], outputMessages);
        }

        [Fact]
        public void ReturnInside()
        {
            var source = """
                         fun f() {
                           while (true) {
                             var i = "i";
                             return i;
                           }
                         }
                         
                         print f();
                         // expect: i
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["i"], outputMessages);
        }

        [Fact]
        public void Syntax()
        {
            var source = """
                         // Single-expression body.
                         var c = 0;
                         while (c < 3) print c = c + 1;
                         // expect: 1
                         // expect: 2
                         // expect: 3
                         
                         // Block body.
                         var a = 0;
                         while (a < 3) {
                           print a;
                           a = a + 1;
                         }
                         // expect: 0
                         // expect: 1
                         // expect: 2
                         
                         // Statement bodies.
                         while (false) if (true) 1; else 2;
                         while (false) while (true) 1;
                         while (false) for (;;) 1;
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["1", "2", "3", "0", "1", "2"], outputMessages);
        }

        [Fact]
        public void VarInBody()
        {
            var source = """
                         // [line 2] Error at 'var': Expect expression.
                         while (true) var foo;
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("[line 2] Error at 'var': Expect expression.", msg);
        }
    }
}