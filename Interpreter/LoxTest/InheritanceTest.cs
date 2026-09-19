namespace LoxTest
{
    public class InheritanceTest : InterpreterTestBase
    {
        [Fact(Skip = "Haven't implemented classes yet")]
        public void Constructor()
        {
            var source = """
                         class A {
                           init(param) {
                             this.field = param;
                           }
                         
                           test() {
                             print this.field;
                           }
                         }
                         
                         class B < A {}
                         
                         var b = B("value");
                         b.test(); // expect: value
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["value"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void InheritFromFunction()
        {
            var source = """
                         fun foo() {}
                         class Subclass < foo {} // expect runtime error: Superclass must be a class.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Superclass must be a class.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void InheritFromNil()
        {
            var source = """
                         var Nil = nil;
                         class Foo < Nil {} // expect runtime error: Superclass must be a class.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Superclass must be a class.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void InheritFromNumber()
        {
            var source = """
                         var Number = 123;
                         class Foo < Number {} // expect runtime error: Superclass must be a class.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Superclass must be a class.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void InheritMethods()
        {
            var source = """
                         class Foo {
                           methodOnFoo() { print "foo"; }
                           override() { print "foo"; }
                         }
                         
                         class Bar < Foo {
                           methodOnBar() { print "bar"; }
                           override() { print "bar"; }
                         }
                         
                         var bar = Bar();
                         bar.methodOnFoo(); // expect: foo
                         bar.methodOnBar(); // expect: bar
                         bar.override(); // expect: bar
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["foo", "bar", "bar"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void ParenthesizeSuperclass()
        {
            var source = """
                         class Foo {}
                         
                         // [line 4] Error at '(': Expect superclass name.
                         class Bar < (Foo) {}
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Error at '(': Expect superclass name.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void SetFieldsFromBaseClass()
        {
            var source = """
                         class Foo {
                           foo(a, b) {
                             this.field1 = a;
                             this.field2 = b;
                           }
                         
                           fooPrint() {
                             print this.field1;
                             print this.field2;
                           }
                         }
                         
                         class Bar < Foo {
                           bar(a, b) {
                             this.field1 = a;
                             this.field2 = b;
                           }
                         
                           barPrint() {
                             print this.field1;
                             print this.field2;
                           }
                         }
                         
                         var bar = Bar();
                         bar.foo("foo 1", "foo 2");
                         bar.fooPrint();
                         // expect: foo 1
                         // expect: foo 2
                         
                         bar.bar("bar 1", "bar 2");
                         bar.barPrint();
                         // expect: bar 1
                         // expect: bar 2
                         
                         bar.fooPrint();
                         // expect: bar 1
                         // expect: bar 2
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["foo 1", "foo 2", "bar 1", "bar 2", "bar 1", "bar 2"], outputMessages);
        }
    }
}
