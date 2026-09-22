namespace LoxTest
{
    public class LambdaTest : InterpreterTestBase
    {
        #region Happy Path Tests

        [Fact]
        public void ImmediatelyInvokedLambda()
        {
            var source = """
                         var result = (fun(a, b) { return a + b; })(5, 10);
                         print result;
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["15"], outputMessages);
        }

        [Fact]
        public void HigherOrderFunction()
        {
            var source = """
                         fun runTwice(callback, value) {
                           return callback(callback(value));
                         }
                         
                         var finalValue = runTwice(fun(x) { return x * 2; }, 3);
                         print finalValue;
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["12"], outputMessages);
        }

        [Fact]
        public void Closure()
        {
            var source = """
                         fun makeCounter() {
                           var count = 0;
                           return fun() {
                             count = count + 1;
                             return count;
                           };
                         }
                         
                         var counter = makeCounter();
                         print counter();
                         print counter();
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["1", "2"], outputMessages);
        }

        [Fact]
        public void SharedClosure()
        {
            var source = """
                         var sharedCounter = 0;

                         var increment = fun() {
                           sharedCounter = sharedCounter + 1;
                         };

                         var decrement = fun() {
                           sharedCounter = sharedCounter - 1;
                         };

                         increment();
                         increment();
                         print sharedCounter; // expect 2

                         decrement();
                         print sharedCounter; // expect 1
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["2", "1"], outputMessages);
        }

        [Fact]
        public void ClosureInsideLambda()
        {
            var source = """
                         fun createWallet() {
                           var balance = 0;
                         
                           fun alterBalance(amount) {
                             balance = balance + amount;
                             return balance;
                           }
                         
                           return alterBalance;
                         }
                         
                         var wallet = createWallet();
                         print wallet(50); // expect 50
                         print wallet(-20.25); // expect 29.75
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["50", "29.75"], outputMessages);
        }

        [Fact]
        public void StandaloneLambdaWrappedInParens()
        {
            var source = """
                         (fun(){print "allowed";})();
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["allowed"], outputMessages);
        }

        [Fact]
        public void Print()
        {
            var source = """
                         var foo = fun(x) { return x + 1; };
                         var bar = fun(a, b, c) {return a + b + c; };
                         print foo;
                         print bar;
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["<fn [lambda](x)>", "<fn [lambda](a, b, c)>"], outputMessages);
        }

        [Fact]
        public void CanBeReassigned()
        {
            var source = """
                         var foo = fun(x) { return x + 1; };
                         print foo(5); // expect 6
                         foo = fun(x, y){ return x * y; };
                         print foo(5, 2); // expect 10
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["6", "10"], outputMessages);
        }

        [Fact]
        public void EmptyParameters()
        {
            var source = """
                         var sayHi = fun() {
                           print "hi";
                         };
                         
                         sayHi();
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["hi"], outputMessages);
        }

        #endregion

        #region Sad Path Tests

        [Fact]
        public void MissingParameters()
        {
            var source = """
                         var bad = fun { print "no parameters"; };
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());

            Assert.True(output.OutputMessages[0].IsError);
            Assert.Contains("Expect '(' after lambda expression.", output.OutputMessages[0].Message);

            Assert.True(output.OutputMessages[1].IsError);
            Assert.Contains("Error at '}': Expect expression.", output.OutputMessages[1].Message);
        }

        [Fact]
        public void CannotBeStandaloneExpression()
        {
            var source = """
                         fun() { print "not allowed"; }; // a lambda must be assigned, passed as a param, or wrapped in parens 
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());

            Assert.True(output.OutputMessages[0].IsError);
            Assert.Contains(" Error at ')': Expect expression.", output.OutputMessages[0].Message);

            Assert.True(output.OutputMessages[1].IsError);
            Assert.Contains("Error at '}': Expect expression.", output.OutputMessages[1].Message);
        }

        #endregion
    }
}