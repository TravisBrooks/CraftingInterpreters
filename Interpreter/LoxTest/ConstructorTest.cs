namespace LoxTest
{
    public class ConstructorTest : InterpreterTestBase
    {
        [Fact(Skip = "Haven't implemented classes yet")]
        public void Arguments()
        {
            var source = """
                         class Foo {
                           init(a, b) {
                             print "init"; // expect: init
                             this.a = a;
                             this.b = b;
                           }
                         }
                         
                         var foo = Foo(1, 2);
                         print foo.a; // expect: 1
                         print foo.b; // expect: 2
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["1", "2"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void CallInitEarlyReturn()
        {
            var source = """
                         class Foo {
                           init() {
                             print "init";
                             return;
                             print "nope";
                           }
                         }
                         
                         var foo = Foo(); // expect: init
                         print foo.init(); // expect: init
                         // expect: Foo instance
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["init", "init", "Foo instance"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void CallInitExplicitly()
        {
            var source = """
                         class Foo {
                           init(arg) {
                             print "Foo.init(" + arg + ")";
                             this.field = "init";
                           }
                         }
                         
                         var foo = Foo("one"); // expect: Foo.init(one)
                         foo.field = "field";
                         
                         var foo2 = foo.init("two"); // expect: Foo.init(two)
                         print foo2; // expect: Foo instance
                         
                         // Make sure init() doesn't create a fresh instance.
                         print foo.field; // expect: init
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["Foo.init(one)", "Foo.init(two)", "Foo instance", "init"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void Default()
        {
            var source = """
                         class Foo {}
                         
                         var foo = Foo();
                         print foo; // expect: Foo instance
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["Foo instance"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void DefaultArguments()
        {
            var source = """
                         class Foo {}
                         
                         var foo = Foo(1, 2, 3); // expect runtime error: Expected 0 arguments but got 3.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Expected 0 arguments but got 3.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void EarlyReturn()
        {
            var source = """
                         class Foo {
                           init() {
                             print "init";
                             return;
                             print "nope";
                           }
                         }
                         
                         var foo = Foo(); // expect: init
                         print foo; // expect: Foo instance
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["init", "Foo instance"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void ExtraArguments()
        {
            var source = """
                         class Foo {
                           init(a, b) {
                             this.a = a;
                             this.b = b;
                           }
                         }
                         
                         var foo = Foo(1, 2, 3, 4); // expect runtime error: Expected 2 arguments but got 4.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Expected 2 arguments but got 4.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void InitNotMethod()
        {
            var source = """
                         class Foo {
                           init(arg) {
                             print "Foo.init(" + arg + ")";
                             this.field = "init";
                           }
                         }
                         
                         fun init() {
                           print "not initializer";
                         }
                         
                         init(); // expect: not initializer
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["not initializer"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void MissingArguments()
        {
            var source = """
                         class Foo {
                           init(a, b) {}
                         }
                         
                         var foo = Foo(1); // expect runtime error: Expected 2 arguments but got 1.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Expected 2 arguments but got 1.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void ReturnInNestedFunction()
        {
            var source = """
                         class Foo {
                           init() {
                             fun init() {
                               return "bar";
                             }
                             print init(); // expect: bar
                           }
                         }
                         
                         print Foo(); // expect: Foo instance
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["bar", "Foo instance"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void ReturnInInit()
        {
            var source = """
                         class Foo {
                           init() {
                             return "result"; // Error at 'return': Can't return a value from an initializer.
                           }
                         }
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Can't return a value from an initializer.", msg);
        }
    }
}
