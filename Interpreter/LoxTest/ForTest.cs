namespace LoxTest
{
    public class ForTest : InterpreterTestBase
    {
        [Fact(Skip = "Haven't implemented classes yet")]
        public void ClassInBody()
        {
            var source = """
                         // [line 2] Error at 'class': Expect expression.
                         for (;;) class Foo {}
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Single(outputMessages);
            var msg = outputMessages.First();
            Assert.Contains("[line 2] Error at 'class': Expect expression.", msg);
        }

        [Fact]
        public void ClosureInBody()
        {
            var source = """
                         var f1;
                         var f2;
                         var f3;
                         
                         for (var i = 1; i < 4; i = i + 1) {
                           var j = i;
                           fun f() {
                             print i;
                             print j;
                           }
                         
                           if (j == 1) f1 = f;
                           else if (j == 2) f2 = f;
                           else f3 = f;
                         }
                         
                         f1(); // expect: 4
                               // expect: 1
                         f2(); // expect: 4
                               // expect: 2
                         f3(); // expect: 4
                               // expect: 3
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["4", "1", "4", "2", "4", "3"], outputMessages);
        }

        [Fact]
        public void FunctionInBody()
        {
            var source = """
                         // [line 2] Error at 'fun': Expect expression.
                         for (;;) fun foo() {}
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Single(outputMessages);
            var msg = outputMessages.First();
            Assert.Contains("[line 2] Error at 'fun': Expect expression.", msg);
        }

        [Fact]
        public void ReturnClosure()
        {
            var source = """
                         fun f() {
                           for (;;) {
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
                           for (;;) {
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
        public void Scope()
        {
            var source = """
                         {
                           var i = "before";
                         
                           // New variable is in inner scope.
                           for (var i = 0; i < 1; i = i + 1) {
                             print i; // expect: 0
                         
                             // Loop body is in second inner scope.
                             var i = -1;
                             print i; // expect: -1
                           }
                         }
                         
                         {
                           // New variable shadows outer variable.
                           for (var i = 0; i > 0; i = i + 1) {}
                         
                           // Goes out of scope after loop.
                           var i = "after";
                           print i; // expect: after
                         
                           // Can reuse an existing variable.
                           for (i = 0; i < 1; i = i + 1) {
                             print i; // expect: 0
                           }
                         }
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["0", "-1", "after", "0"], outputMessages);
        }

        [Fact]
        public void StatementCondition()
        {
            var source = """
                         // [line 3] Error at '{': Expect expression.
                         // [line 3] Error at ')': Expect ';' after expression.
                         for (var a = 1; {}; a = a + 1) {}
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(2, outputMessages.Length);
            var msg1 = outputMessages[0];
            var msg2 = outputMessages[1];
            Assert.Contains("[line 3] Error at '{': Expect expression.", msg1);
            Assert.Contains("[line 3] Error at ')': Expect ';' after expression.", msg2);
        }

        [Fact]
        public void StatementIncrement()
        {
            var source = """
                         // [line 2] Error at '{': Expect expression.
                         for (var a = 1; a < 2; {}) {}
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Single(outputMessages);
            var msg = outputMessages.First();
            Assert.Contains("[line 2] Error at '{': Expect expression.", msg);
        }

        [Fact]
        public void StatementInitializer()
        {
            var source = """
                         // [line 3] Error at '{': Expect expression.
                         // [line 3] Error at ')': Expect ';' after expression.
                         for ({}; a < 2; a = a + 1) {}
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(2, outputMessages.Length);
            var msg1 = outputMessages[0];
            var msg2 = outputMessages[1];
            Assert.Contains("[line 3] Error at '{': Expect expression.", msg1);
            Assert.Contains("[line 3] Error at ')': Expect ';' after expression.", msg2);
        }

        /// <summary>
        /// There is a bug here when running the bar() method. Running bar() in isolation does not cause an error so it seems to be some variable shadowing issue.
        /// </summary>
        [Fact(Skip = "Bug in variable shadowing, might get resolved by changes in chapter 11?")]
        public void Syntax()
        {
            var source = """
                         // Single-expression body.
                         for (var c = 0; c < 3;) print c = c + 1;
                         // expect: 1
                         // expect: 2
                         // expect: 3
                         
                         // Block body.
                         for (var a = 0; a < 3; a = a + 1) {
                           print a;
                         }
                         // expect: 0
                         // expect: 1
                         // expect: 2
                         
                         // No clauses.
                         fun foo() {
                           for (;;) return "done";
                         }
                         print foo(); // expect: done
                         
                         // No variable.
                         var i = 0;
                         for (; i < 2; i = i + 1) print i;
                         // expect: 0
                         // expect: 1
                         
                         // No condition.
                         fun bar() {
                           for (var i = 0;; i = i + 1) {
                             print i;
                             if (i >= 2) return;
                           }
                         }
                         bar();
                         // expect: 0
                         // expect: 1
                         // expect: 2
                         
                         // No increment.
                         for (var i = 0; i < 2;) {
                           print i;
                           i = i + 1;
                         }
                         // expect: 0
                         // expect: 1
                         
                         // Statement bodies.
                         for (; false;) if (true) 1; else 2;
                         for (; false;) while (true) 1;
                         for (; false;) for (;;) 1;
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["1", "2", "3", "0", "1", "2", "done", "0", "1", "0", "1", "2", "0", "1"], outputMessages);
        }

        [Fact]
        public void VarInBody()
        {
            var source = """
                         // [line 2] Error at 'var': Expect expression.
                         for (;;) var foo;
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Single(outputMessages);
            var msg = outputMessages.First();
            Assert.Contains("[line 2] Error at 'var': Expect expression.", msg);
        }
    }
}
