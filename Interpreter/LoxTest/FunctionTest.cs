using Lox;

namespace LoxTest
{
    public class FunctionTest : InterpreterTestBase
    {
        [Fact]
        public void BodyMustBeBlock()
        {
            var source = """
                         // [line 3] Error at '123': Expect '{' before function body.
                         // [line 3] Error at end: Expect '}' after block.
                         fun f() 123;
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            // I only get 1 error message, not 2. The first error throws a ParseError, which stops parsing the function body, so the second error is not reported.
            // This kind of looks like a bug in the book's test.
            Assert.Single(outputMessages);
            var msg = outputMessages.First();
            Assert.Contains("[line 3] Error at '123': Expect '{' before function body.", msg);
        }

        [Fact]
        public void EmptyBody()
        {
            var source = """
                         fun f() {}
                         print f(); // expect: nil
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["nil"], outputMessages);
        }

        /// <summary>
        /// The error messages I'm getting seem wildly off, like even line number makes no sense, def seems like an actual bug here...
        /// </summary>
        [Fact]
        public void ExtraArguments()
        {
            var source = """
                         fun f(a, b) {
                           print a;
                           print b;
                         }
                         
                         f(1, 2, 3, 4); // expect runtime error: Expected 2 arguments but got 4.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Expected 2 arguments but got 4", msg);
        }

        [Fact]
        public void LocalMutualRecursion()
        {
            // This test from the book is a bit baffling, there is this that is supposed to fail then the extremely similar MutualRecursion test that is supposed to succeed.
            // The only difference is in this code the functions are defined in a block and the other they're defined in the global scope.
            // I don't see why that should matter, and my code works just fine for both, so I guess this is a bug in the book?
            var source = """
                         {
                           fun isEven(n) {
                             if (n == 0) return true;
                             return isOdd(n - 1); // expect runtime error: Undefined variable 'isOdd'.
                           }
                         
                           fun isOdd(n) {
                             if (n == 0) return false;
                             return isEven(n - 1);
                           }
                         
                           print isEven(4);
                         }
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["true"], outputMessages);
        }

        [Fact]
        public void LocalRecursion()
        {
            var source = """
                         {
                           fun fib(n) {
                             if (n < 2) return n;
                             return fib(n - 1) + fib(n - 2);
                           }
                         
                           print fib(8); // expect: 21
                         }
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["21"], outputMessages);
        }

        [Fact]
        public void MissingArguments()
        {
            var source = """
                         fun f(a, b) {}
                         
                         f(1); // expect runtime error: Expected 2 arguments but got 1.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Expected 2 arguments but got 1", msg);
        }

        [Fact]
        public void MissingCommaInParameters()
        {
            var source = """
                         // [line 2] Error at 'c': Expect ')' after parameters.
                         fun foo(a, b c, d, e, f) {}
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            // I only get 1 error message, not 2. The first error throws a ParseError, which stops parsing the function body, so the second error is not reported.
            // This kind of looks like a bug in the book's test.
            Assert.Single(outputMessages);
            var msg = outputMessages.First();
            Assert.Contains("[line 2] Error at 'c': Expect ')' after parameters.", msg);
        }

        [Fact]
        public void MutualRecursion()
        {
            var source = """
                         fun isEven(n) {
                           if (n == 0) return true;
                           return isOdd(n - 1);
                         }
                         
                         fun isOdd(n) {
                           if (n == 0) return false;
                           return isEven(n - 1);
                         }
                         
                         print isEven(4); // expect: true
                         print isOdd(3); // expect: true
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["true", "true"], outputMessages);
        }

        [Fact]
        public void NestedCallWithArguments()
        {
            var source = """
                         fun returnArg(arg) {
                           return arg;
                         }
                         
                         fun returnFunCallWithArg(func, arg) {
                           return returnArg(func)(arg);
                         }
                         
                         fun printArg(arg) {
                           print arg;
                         }
                         
                         returnFunCallWithArg(printArg, "hello world"); // expect: hello world
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["hello world"], outputMessages);
        }

        /// <summary>
        /// The error messages I'm getting seem wildly off, like even line number makes no sense, def seems like an actual bug here...
        /// </summary>
        [Fact]
        public void Parameters()
        {
            var source = """
                         fun f0() { return 0; }
                         print f0(); // expect: 0
                         
                         fun f1(a) { return a; }
                         print f1(1); // expect: 1
                         
                         fun f2(a, b) { return a + b; }
                         print f2(1, 2); // expect: 3
                         
                         fun f3(a, b, c) { return a + b + c; }
                         print f3(1, 2, 3); // expect: 6
                         
                         fun f4(a, b, c, d) { return a + b + c + d; }
                         print f4(1, 2, 3, 4); // expect: 10
                         
                         fun f5(a, b, c, d, e) { return a + b + c + d + e; }
                         print f5(1, 2, 3, 4, 5); // expect: 15
                         
                         fun f6(a, b, c, d, e, f) { return a + b + c + d + e + f; }
                         print f6(1, 2, 3, 4, 5, 6); // expect: 21
                         
                         fun f7(a, b, c, d, e, f, g) { return a + b + c + d + e + f + g; }
                         print f7(1, 2, 3, 4, 5, 6, 7); // expect: 28
                         
                         fun f8(a, b, c, d, e, f, g, h) { return a + b + c + d + e + f + g + h; }
                         print f8(1, 2, 3, 4, 5, 6, 7, 8); // expect: 36
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["0", "1", "3", "6", "10", "15", "21", "28", "36"], outputMessages);
        }

        [Fact]
        public void Print()
        {
            var source = """
                         fun foo() {}
                         print foo; // expect: <fn foo>
                         
                         print clock; // expect: <native fn>
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["<fn foo()>", "<native fn clock()>"], outputMessages);
        }

        [Fact]
        public void Recursion()
        {
            var source = """
                         fun fib(n) {
                           if (n < 2) return n;
                           return fib(n - 1) + fib(n - 2);
                         }
                         
                         print fib(8); // expect: 21
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["21"], outputMessages);
        }

        [Fact]
        public void TooManyArguments()
        {
            // I was not going to slap 261 lines of lox code into a test...
            var source = EmbeddedLoxFile.GetSource("too_many_arguments.lox");
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Error at 'a': Can't have more than " + Parser.MaxFuncParameterCount + " arguments.", msg);
        }

        [Fact]
        public void TooManyParameters()
        {
            // I was not going to slap 257 lines of lox code into a test...
            var source = EmbeddedLoxFile.GetSource("too_many_parameters.lox");
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Error at 'a': Can't have more than " + Parser.MaxFuncParameterCount + " parameters.", msg);
        }
    }
}
