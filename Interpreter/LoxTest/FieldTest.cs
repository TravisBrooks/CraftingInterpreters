namespace LoxTest
{
    public class FieldTest : InterpreterTestBase
    {
        [Fact(Skip = "Haven't implemented classes yet")]
        public void CallFunctionField()
        {
            var source = """
                         class Foo {}
                         
                         fun bar(a, b) {
                           print "bar";
                           print a;
                           print b;
                         }
                         
                         var foo = Foo();
                         foo.bar = bar;
                         
                         foo.bar(1, 2);
                         // expect: bar
                         // expect: 1
                         // expect: 2
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["bar", "1", "2"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void CallNonfunctionField()
        {
            var source = """
                         class Foo {}
                         
                         var foo = Foo();
                         foo.bar = "not fn";
                         
                         foo.bar(); // expect runtime error: Can only call functions and classes.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Can only call functions and classes.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void GetAndSetMethod()
        {
            var source = """
                         // Bound methods have identity equality.
                         class Foo {
                           method(a) {
                             print "method";
                             print a;
                           }
                           other(a) {
                             print "other";
                             print a;
                           }
                         }
                         
                         var foo = Foo();
                         var method = foo.method;
                         
                         // Setting a property shadows the instance method.
                         foo.method = foo.other;
                         foo.method(1);
                         // expect: other
                         // expect: 1
                         
                         // The old method handle still points to the original method.
                         method(2);
                         // expect: method
                         // expect: 2
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["other", "1", "method", "2"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void GetOnBool()
        {
            var source = """
                         true.foo; // expect runtime error: Only instances have properties.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Only instances have properties.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void GetOnClass()
        {
            var source = """
                         class Foo {}
                         Foo.bar; // expect runtime error: Only instances have properties.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Only instances have properties.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void GetOnFunction()
        {
            var source = """
                         fun foo() {}
                         foo.bar; // expect runtime error: Only instances have properties.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Only instances have properties.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void GetOnNil()
        {
            var source = """
                         nil.foo; // expect runtime error: Only instances have properties.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Only instances have properties.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void GetOnNum()
        {
            var source = """
                         123.foo; // expect runtime error: Only instances have properties.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Only instances have properties.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void GetOnString()
        {
            var source = """
                         "str".foo; // expect runtime error: Only instances have properties.
                         "str".foo; // expect runtime error: Only instances have properties.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Only instances have properties.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void ManyFields()
        {
            var source = EmbeddedLoxFile.GetSource("Many.lox");
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal([
                "apple",
                "apricot",
                "avocado",
                "banana",
                "bilberry",
                "blackberry",
                "blackcurrant",
                "blueberry",
                "boysenberry",
                "cantaloupe",
                "cherimoya",
                "cherry",
                "clementine",
                "cloudberry",
                "coconut",
                "cranberry",
                "currant",
                "damson",
                "date",
                "dragonfruit",
                "durian",
                "elderberry",
                "feijoa",
                "fig",
                "gooseberry",
                "grape",
                "grapefruit",
                "guava",
                "honeydew",
                "huckleberry",
                "jabuticaba",
                "jackfruit",
                "jambul",
                "jujube",
                "juniper",
                "kiwifruit",
                "kumquat",
                "lemon",
                "lime",
                "longan",
                "loquat",
                "lychee",
                "mandarine",
                "mango",
                "marionberry",
                "melon",
                "miracle",
                "mulberry",
                "nance",
                "nectarine",
                "olive",
                "orange",
                "papaya",
                "passionfruit",
                "peach",
                "pear",
                "persimmon",
                "physalis",
                "pineapple",
                "plantain",
                "plum",
                "plumcot",
                "pomegranate",
                "pomelo",
                "quince",
                "raisin",
                "rambutan",
                "raspberry",
                "redcurrant",
                "salak",
                "salmonberry",
                "satsuma",
                "strawberry",
                "tamarillo",
                "tamarind",
                "tangerine",
                "tomato",
                "watermelon",
                "yuzu"
                ], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void Method()
        {
            var source = """
                         class Foo {
                           bar(arg) {
                             print arg;
                           }
                         }
                         
                         var bar = Foo().bar;
                         print "got method"; // expect: got method
                         bar("arg");          // expect: arg
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["got method", "arg"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void MethodBindsThis()
        {
            var source = """
                         class Foo {
                           sayName(a) {
                             print this.name;
                             print a;
                           }
                         }
                         
                         var foo1 = Foo();
                         foo1.name = "foo1";
                         
                         var foo2 = Foo();
                         foo2.name = "foo2";
                         
                         // Store the method reference on another object.
                         foo2.fn = foo1.sayName;
                         // Still retains original receiver.
                         foo2.fn(1);
                         // expect: foo1
                         // expect: 1
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["foo1", "1"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void OnInstance()
        {
            var source = """
                         class Foo {}
                         
                         var foo = Foo();
                         
                         print foo.bar = "bar value"; // expect: bar value
                         print foo.baz = "baz value"; // expect: baz value
                         
                         print foo.bar; // expect: bar value
                         print foo.baz; // expect: baz value
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["bar value", "baz value", "bar value", "baz value"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void SetEvaluationOrder()
        {
            var source = """
                         undefined1.bar // expect runtime error: Undefined variable 'undefined1'.
                         = undefined2;
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Undefined variable 'undefined1'.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void SetOnBool()
        {
            var source = """
                         true.foo = "value"; // expect runtime error: Only instances have fields.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Only instances have fields.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void SetOnClass()
        {
            var source = """
                         class Foo {}
                         Foo.bar = "value"; // expect runtime error: Only instances have fields.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Only instances have fields.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void SetOnFunction()
        {
            var source = """
                         fun foo() {}
                         foo.bar = "value"; // expect runtime error: Only instances have fields.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Only instances have fields.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void SetOnNil()
        {
            var source = """
                         nil.foo = "value"; // expect runtime error: Only instances have fields.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Only instances have fields.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void SetOnNum()
        {
            var source = """
                         123.foo = "value"; // expect runtime error: Only instances have fields.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Only instances have fields.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void SetOnStr()
        {
            var source = """
                         "str".foo = "value"; // expect runtime error: Only instances have fields.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Only instances have fields.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void Undefined()
        {
            var source = """
                         class Foo {}
                         var foo = Foo();
                         foo.bar; // expect runtime error: Undefined property 'bar'.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Undefined property 'bar'.", msg);
        }
    }
}
