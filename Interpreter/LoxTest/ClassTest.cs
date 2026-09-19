namespace LoxTest
{
    public class ClassTest : InterpreterTestBase
    {
        [Fact(Skip = "Haven't implemented classes yet")]
        public void EmptyClass()
        {
            var source = """
                class Foo {}
                
                print Foo; // expect: Foo
                """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["Foo"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void InheritSelf()
        {
            var source = """
                         class Foo < Foo {} // Error at 'Foo': A class can't inherit from itself.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("A class can't inherit from itself.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void InheritedMethod()
        {
            var source = """
                         class Foo {
                           inFoo() {
                             print "in foo";
                           }
                         }
                         
                         class Bar < Foo {
                           inBar() {
                             print "in bar";
                           }
                         }
                         
                         class Baz < Bar {
                           inBaz() {
                             print "in baz";
                           }
                         }
                         
                         var baz = Baz();
                         baz.inFoo(); // expect: in foo
                         baz.inBar(); // expect: in bar
                         baz.inBaz(); // expect: in baz
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["in foo", "in bar", "in baz"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void LocalInheritOther()
        {
            var source = """
                         class A {}
                         
                         fun f() {
                           class B < A {}
                           return B;
                         }
                         
                         print f(); // expect: B
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["B"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void LocalInheritSelf()
        {
            var source = """
                         {
                           class Foo < Foo {} // Error at 'Foo': A class can't inherit from itself.
                         }
                         // [c line 5] Error at end: Expect '}' after block.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(2, outputMessages.Length);
            var msg1 = outputMessages[0];
            var msg2 = outputMessages[1];
            Assert.Contains("A class can't inherit from itself.", msg1);
            Assert.Contains("Expect '}' after block.", msg2);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void LocalReferenceSelf()
        {
            var source = """
                         {
                           class Foo {
                             returnSelf() {
                               return Foo;
                             }
                           }
                         
                           print Foo().returnSelf(); // expect: Foo
                         }
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["Foo"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void ReferenceSelf()
        {
            var source = """
                         class Foo {
                           returnSelf() {
                             return Foo;
                           }
                         }
                         
                         print Foo().returnSelf(); // expect: Foo
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["Foo"], outputMessages);
        }
    }
}
