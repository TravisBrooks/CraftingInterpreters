namespace LoxTest
{
    public class VariableTest : InterpreterTestBase
    {
        [Fact(Skip = "Haven't implemented Resolver yet")]
        public void CollideWithParameter()
        {
            var source = """
                         fun foo(a) {
                           var a; // Error at 'a': Already a variable with this name in this scope.
                         }
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Error at 'a': Already a variable with this name in this scope.", msg);
        }

        [Fact(Skip = "Haven't implemented Resolver yet")]
        public void DuplicateScope()
        {
            var source = """
                         {
                           var a = "value";
                           var a = "other"; // Error at 'a': Already a variable with this name in this scope.
                         }
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Error at 'a': Already a variable with this name in this scope.", msg);
        }

        [Fact(Skip = "Haven't implemented Resolver yet")]
        public void DuplicateParameter()
        {
            var source = """
                         fun foo(arg,
                                 arg) { // Error at 'arg': Already a variable with this name in this scope.
                           "body";
                         }
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Error at 'arg': Already a variable with this name in this scope.", msg);
        }

        [Fact(Skip = "Haven't implemented Resolver yet")]
        public void EarlyBound()
        {
            var source = """
                         var a = "outer";
                         {
                           fun foo() {
                             print a;
                           }
                         
                           foo(); // expect: outer
                           var a = "inner";
                           foo(); // expect: outer
                         }
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["outer", "outer"], outputMessages);
        }

        [Fact]
        public void InMiddleOfBlock()
        {
            var source = """
                         {
                           var a = "a";
                           print a; // expect: a
                           var b = a + " b";
                           print b; // expect: a b
                           var c = a + " c";
                           print c; // expect: a c
                           var d = b + " d";
                           print d; // expect: a b d
                         }
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["a", "a b", "a c", "a b d"], outputMessages);
        }

        [Fact]
        public void InNestedBlock()
        {
            var source = """
                         {
                           var a = "outer";
                           {
                             print a; // expect: outer
                           }
                         }
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["outer"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void LocalFromMethod()
        {
            var source = """
                         var foo = "variable";
                         
                         class Foo {
                           method() {
                             print foo;
                           }
                         }
                         
                         Foo().method(); // expect: variable
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["variable"], outputMessages);
        }

        [Fact]
        public void RedeclareGlobal()
        {
            var source = """
                         var a = "1";
                         var a;
                         print a; // expect: nil
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["nil"], outputMessages);
        }

        [Fact]
        public void RedefineGlobal()
        {
            var source = """
                         var a = "1";
                         var a = "2";
                         print a; // expect: 2
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["2"], outputMessages);
        }

        [Fact]
        public void ScopeReuseInDifferentBlock()
        {
            var source = """
                         {
                           var a = "first";
                           print a; // expect: first
                         }
                         
                         {
                           var a = "second";
                           print a; // expect: second
                         }
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["first", "second"], outputMessages);
        }

        [Fact]
        public void ShadowAndLocal()
        {
            var source = """
                         {
                           var a = "outer";
                           {
                             print a; // expect: outer
                             var a = "inner";
                             print a; // expect: inner
                           }
                         }
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["outer", "inner"], outputMessages);
        }

        [Fact]
        public void ShadowGlobal()
        {
            var source = """
                         var a = "global";
                         {
                           var a = "shadow";
                           print a; // expect: shadow
                         }
                         print a; // expect: global
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["shadow", "global"], outputMessages);
        }

        [Fact]
        public void ShadowLocal()
        {
            var source = """
                         {
                           var a = "local";
                           {
                             var a = "shadow";
                             print a; // expect: shadow
                           }
                           print a; // expect: local
                         }
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["shadow", "local"], outputMessages);
        }

        [Fact]
        public void UndefinedGlobal()
        {
            var source = """
                         print notDefined;  // expect runtime error: Undefined variable 'notDefined'.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Undefined variable 'notDefined'.", msg);
        }

        [Fact]
        public void UndefinedLocal()
        {
            var source = """
                         {
                           print notDefined;  // expect runtime error: Undefined variable 'notDefined'.
                         }
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Undefined variable 'notDefined'.", msg);
        }

        [Fact]
        public void Uninitialized()
        {
            var source = """
                         var a;
                         print a; // expect: nil
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["nil"], outputMessages);
        }

        [Fact]
        public void UnreachedUndefined()
        {
            var source = """
                         if (false) {
                           print notDefined;
                         }
                         
                         print "ok"; // expect: ok
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["ok"], outputMessages);
        }

        [Fact]
        public void UseFalseAsVar()
        {
            var source = """
                         // [line 2] Error at 'false': Expect variable name.
                         var false = "value";
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Error at 'false': Expect variable name.", msg);
        }

        [Fact]
        public void UseGlobalInInitializer()
        {
            var source = """
                         var a = "value";
                         var a = a;
                         print a; // expect: value
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["value"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented Resolver yet")]
        public void UseLocalInInitializer()
        {
            var source = """
                         var a = "outer";
                         {
                           var a = a; // Error at 'a': Can't read local variable in its own initializer.
                         }
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Error at 'a': Can't read local variable in its own initializer.", msg);
        }

        [Fact]
        public void UseNilAsVar()
        {
            var source = """
                         // [line 2] Error at 'nil': Expect variable name.
                         var nil = "value";
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Error at 'nil': Expect variable name.", msg);
        }

        [Fact]
        public void UseThisAsVar()
        {
            var source = """
                         // [line 2] Error at 'this': Expect variable name.
                         var this = "value";
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Error at 'this': Expect variable name.", msg);
        }
    }
}