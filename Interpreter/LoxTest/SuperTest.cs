namespace LoxTest
{
    public class SuperTest : InterpreterTestBase
    {
        [Fact(Skip = "Haven't implemented classes yet")]
        public void BoundMethod()
        {
            var source = """
                         class A {
                           method(arg) {
                             print "A.method(" + arg + ")";
                           }
                         }
                         
                         class B < A {
                           getClosure() {
                             return super.method;
                           }
                         
                           method(arg) {
                             print "B.method(" + arg + ")";
                           }
                         }
                         
                         
                         var closure = B().getClosure();
                         closure("arg"); // expect: A.method(arg)
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["A.method(arg)"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void CallOtherMethod()
        {
            var source = """
                         class Base {
                           foo() {
                             print "Base.foo()";
                           }
                         }
                         
                         class Derived < Base {
                           bar() {
                             print "Derived.bar()";
                             super.foo();
                           }
                         }
                         
                         Derived().bar();
                         // expect: Derived.bar()
                         // expect: Base.foo()
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["Derived.bar()", "Base.foo()"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void CallSameMethod()
        {
            var source = """
                         class Base {
                           foo() {
                             print "Base.foo()";
                           }
                         }
                         
                         class Derived < Base {
                           foo() {
                             print "Derived.foo()";
                             super.foo();
                           }
                         }
                         
                         Derived().foo();
                         // expect: Derived.foo()
                         // expect: Base.foo()
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["Derived.foo()", "Base.foo()"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void Closure()
        {
            var source = """
                         class Base {
                           toString() { return "Base"; }
                         }
                         
                         class Derived < Base {
                           getClosure() {
                             fun closure() {
                               return super.toString();
                             }
                             return closure;
                           }
                         
                           toString() { return "Derived"; }
                         }
                         
                         var closure = Derived().getClosure();
                         print closure(); // expect: Base
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["Base"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void Constructor()
        {
            var source = """
                         class Base {
                           init(a, b) {
                             print "Base.init(" + a + ", " + b + ")";
                           }
                         }
                         
                         class Derived < Base {
                           init() {
                             print "Derived.init()";
                             super.init("a", "b");
                           }
                         }
                         
                         Derived();
                         // expect: Derived.init()
                         // expect: Base.init(a, b)
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["Derived.init()", "Base.init(a, b)"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void ExtraArguments()
        {
            var source = """
                         class Base {
                           foo(a, b) {
                             print "Base.foo(" + a + ", " + b + ")";
                           }
                         }
                         
                         class Derived < Base {
                           foo() {
                             print "Derived.foo()"; // expect: Derived.foo()
                             super.foo("a", "b", "c", "d"); // expect runtime error: Expected 2 arguments but got 4.
                           }
                         }
                         
                         Derived().foo();
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var msgs = output.OutputMessages;
            Assert.Equal(2, msgs.Count);

            Assert.False(msgs[0].IsError);
            Assert.Equal("Derived.foo()", msgs[0].Message);
            
            Assert.True(msgs[1].IsError);
            Assert.Contains("Error: Unterminated string.", msgs[1].Message);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void IndirectlyInherited()
        {
            var source = """
                         class A {
                           foo() {
                             print "A.foo()";
                           }
                         }
                         
                         class B < A {}
                         
                         class C < B {
                           foo() {
                             print "C.foo()";
                             super.foo();
                           }
                         }
                         
                         C().foo();
                         // expect: C.foo()
                         // expect: A.foo()
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["C.foo()", "A.foo()"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void MissingArguments()
        {
            var source = """
                         class Base {
                           foo(a, b) {
                             print "Base.foo(" + a + ", " + b + ")";
                           }
                         }
                         
                         class Derived < Base {
                           foo() {
                             super.foo(1); // expect runtime error: Expected 2 arguments but got 1.
                           }
                         }
                         
                         Derived().foo();
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Error: Expected 2 arguments but got 1.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void NoSuperclassBind()
        {
            var source = """
                         class Base {
                           foo() {
                             super.doesNotExist; // Error at 'super': Can't use 'super' in a class with no superclass.
                           }
                         }
                         
                         Base().foo();
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Error: Can't use 'super' in a class with no superclass.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void NoSuperclassCall()
        {
            var source = """
                         class Base {
                           foo() {
                             super.doesNotExist(1); // Error at 'super': Can't use 'super' in a class with no superclass.
                           }
                         }
                         
                         Base().foo();
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Error: Can't use 'super' in a class with no superclass.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void NoSuperclassMethod()
        {
            var source = """
                         class Base {}
                         
                         class Derived < Base {
                           foo() {
                             super.doesNotExist(1); // expect runtime error: Undefined property 'doesNotExist'.
                           }
                         }
                         
                         Derived().foo();
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Error: Undefined property 'doesNotExist'.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void Parenthized()
        {
            var source = """
                         class A {
                           method() {}
                         }
                         
                         class B < A {
                           method() {
                             // [line 8] Error at ')': Expect '.' after 'super'.
                             (super).method();
                           }
                         }
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Error at ')': Expect '.' after 'super'.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void ReassignSuperclass()
        {
            var source = """
                         class Base {
                           method() {
                             print "Base.method()";
                           }
                         }
                         
                         class Derived < Base {
                           method() {
                             super.method();
                           }
                         }
                         
                         class OtherBase {
                           method() {
                             print "OtherBase.method()";
                           }
                         }
                         
                         var derived = Derived();
                         derived.method(); // expect: Base.method()
                         Base = OtherBase;
                         derived.method(); // expect: Base.method()
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["Base.method()", "Base.method()"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void SuperAtTopLevel()
        {
            var source = """
                         super.foo("bar"); // Error at 'super': Can't use 'super' outside of a class.
                         super.foo; // Error at 'super': Can't use 'super' outside of a class.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var msgs = output.OutputMessages;
            Assert.Equal(2, msgs.Count);

            Assert.False(msgs[0].IsError);
            Assert.Equal("Error at 'super': Can't use 'super' outside of a class.", msgs[0].Message);

            Assert.True(msgs[1].IsError);
            Assert.Contains("Error at 'super': Can't use 'super' outside of a class.", msgs[1].Message);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void SuperInClosureInInheritedMethod()
        {
            var source = """
                         class A {
                           say() {
                             print "A";
                           }
                         }
                         
                         class B < A {
                           getClosure() {
                             fun closure() {
                               super.say();
                             }
                             return closure;
                           }
                         
                           say() {
                             print "B";
                           }
                         }
                         
                         class C < B {
                           say() {
                             print "C";
                           }
                         }
                         
                         C().getClosure()(); // expect: A
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["A"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void SuperInInheritedMethod()
        {
            var source = """
                         class A {
                           say() {
                             print "A";
                           }
                         }
                         
                         class B < A {
                           test() {
                             super.say();
                           }
                         
                           say() {
                             print "B";
                           }
                         }
                         
                         class C < B {
                           say() {
                             print "C";
                           }
                         }
                         
                         C().test(); // expect: A
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["A"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void SuperInTopLevelFunction()
        {
            var source = """
                           super.bar(); // Error at 'super': Can't use 'super' outside of a class.
                         fun foo() {
                         }
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Error at 'super': Can't use 'super' outside of a class.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void SuperWithoutDot()
        {
            var source = """
                         class A {}
                         
                         class B < A {
                           method() {
                             // [line 6] Error at ';': Expect '.' after 'super'.
                             super;
                           }
                         }
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Error at ';': Expect '.' after 'super'.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void SuperWithoutName()
        {
            var source = """
                         class A {}
                         
                         class B < A {
                           method() {
                             super.; // Error at ';': Expect superclass method name.
                           }
                         }
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Error at ';': Expect superclass method name.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void ThisInSuperclassMethod()
        {
            var source = """
                         class Base {
                           init(a) {
                             this.a = a;
                           }
                         }
                         
                         class Derived < Base {
                           init(a, b) {
                             super.init(a);
                             this.b = b;
                           }
                         }
                         
                         var derived = Derived("a", "b");
                         print derived.a; // expect: a
                         print derived.b; // expect: b
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["a", "b"], outputMessages);
        }
    }
}