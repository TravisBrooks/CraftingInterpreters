namespace LoxTest
{
    public class ThisTest : InterpreterTestBase
    {
        [Fact(Skip = "Haven't implemented classes yet")]
        public void Closure()
        {
            var source = """
                         class Foo {
                           getClosure() {
                             fun closure() {
                               return this.toString();
                             }
                             return closure;
                           }
                         
                           toString() { return "Foo"; }
                         }
                         
                         var closure = Foo().getClosure();
                         print closure(); // expect: Foo
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["Foo"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void NestedClass()
        {
            var source = """
                         class Outer {
                           method() {
                             print this; // expect: Outer instance
                         
                             fun f() {
                               print this; // expect: Outer instance
                         
                               class Inner {
                                 method() {
                                   print this; // expect: Inner instance
                                 }
                               }
                         
                               Inner().method();
                             }
                             f();
                           }
                         }
                         
                         Outer().method();
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["Outer instance", "Outer instance", "Inner instance"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void NestedClosure()
        {
            var source = """
                         class Foo {
                           getClosure() {
                             fun f() {
                               fun g() {
                                 fun h() {
                                   return this.toString();
                                 }
                                 return h;
                               }
                               return g;
                             }
                             return f;
                           }
                         
                           toString() { return "Foo"; }
                         }
                         
                         var closure = Foo().getClosure();
                         print closure()()(); // expect: Foo
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["Foo"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void ThisAtTopLevel()
        {
            var source = """
                         this; // Error at 'this': Can't use 'this' outside of a class.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Error at 'this': Can't use 'this' outside of a class.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void ThisInMethod()
        {
            var source = """
                         class Foo {
                           bar() { return this; }
                           baz() { return "baz"; }
                         }
                         
                         print Foo().bar().baz(); // expect: baz
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["baz"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void ThisInTopLevelFunction()
        {
            var source = """
                         fun foo() {
                           this; // Error at 'this': Can't use 'this' outside of a class.
                         }
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Error at 'this': Can't use 'this' outside of a class.", msg);
        }
    }
}