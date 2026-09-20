namespace LoxTest
{
    public class MethodTest : InterpreterTestBase
    {
        [Fact(Skip = "Haven't implemented classes yet")]
        public void Arity()
        {
            var source = """
                         class Foo {
                           method0() { return "no args"; }
                           method1(a) { return a; }
                           method2(a, b) { return a + b; }
                           method3(a, b, c) { return a + b + c; }
                           method4(a, b, c, d) { return a + b + c + d; }
                           method5(a, b, c, d, e) { return a + b + c + d + e; }
                           method6(a, b, c, d, e, f) { return a + b + c + d + e + f; }
                           method7(a, b, c, d, e, f, g) { return a + b + c + d + e + f + g; }
                           method8(a, b, c, d, e, f, g, h) { return a + b + c + d + e + f + g + h; }
                         }
                         
                         var foo = Foo();
                         print foo.method0(); // expect: no args
                         print foo.method1(1); // expect: 1
                         print foo.method2(1, 2); // expect: 3
                         print foo.method3(1, 2, 3); // expect: 6
                         print foo.method4(1, 2, 3, 4); // expect: 10
                         print foo.method5(1, 2, 3, 4, 5); // expect: 15
                         print foo.method6(1, 2, 3, 4, 5, 6); // expect: 21
                         print foo.method7(1, 2, 3, 4, 5, 6, 7); // expect: 28
                         print foo.method8(1, 2, 3, 4, 5, 6, 7, 8); // expect: 36
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["no args", "1", "3", "6", "10", "15", "21", "28", "36"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void EmptyBlock()
        {
            var source = """
                         class Foo {
                           bar() {}
                         }
                         
                         print Foo().bar(); // expect: nil
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["nil"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void ExtraArguments()
        {
            var source = """
                         class Foo {
                           method(a, b) {
                             print a;
                             print b;
                           }
                         }
                         
                         Foo().method(1, 2, 3, 4); // expect runtime error: Expected 2 arguments but got 4.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Expected 2 arguments but got 4.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void MissingArguments()
        {
            var source = """
                         class Foo {
                           method(a, b) {}
                         }
                         
                         Foo().method(1); // expect runtime error: Expected 2 arguments but got 1.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Expected 2 arguments but got 1.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void NotFound()
        {
            var source = """
                         class Foo {}
                         
                         Foo().unknown(); // expect runtime error: Undefined property 'unknown'.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Undefined property 'unknown'.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void PrintBoundMethod()
        {
            var source = """
                         class Foo {
                           method() { }
                         }
                         var foo = Foo();
                         print foo.method; // expect: <fn method>
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["<fn method>"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void ReferToName()
        {
            var source = """
                         class Foo {
                           method() {
                             print method; // expect runtime error: Undefined variable 'method'.
                           }
                         }
                         
                         Foo().method();
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Undefined variable 'method'.", msg);
        }
    }
}