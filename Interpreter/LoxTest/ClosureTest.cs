namespace LoxTest
{
    public class ClosureTest : InterpreterTestBase
    {
        [Fact]
        public void TestClosure()
        {
            var source = """
                var f;
                var g;
            
                {
                  var local = "local";
                  fun f_() {
                    print local;
                    local = "after f";
                    print local;
                  }
                  f = f_;
            
                  fun g_() {
                    print local;
                    local = "after g";
                    print local;
                  }
                  g = g_;
                }
            
                f();
                // expect: local
                // expect: after f
            
                g();
                // expect: after f
                // expect: after g
            """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["local", "after f", "after f", "after g"], outputMessages);
        }

        [Fact(Skip = "This is the closure scope bug described in beginning of chapter 11, need to implement that chapter for test to pass.")]
        public void ShadowedVariable()
        {
            var source = """
                var a = "global";
            
                {
                  fun assign() {
                    a = "assigned";
                  }
            
                  var a = "inner";
                  assign();
                  print a; // expect: inner
                }
            
                print a; // expect: assigned
                """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["inner", "assigned"], outputMessages);
        }

        [Fact]
        public void ClosureOverFunctionParameter()
        {
            var source = """
                var f;
            
                fun foo(param) {
                  fun f_() {
                    print param;
                  }
                  f = f_;
                }
                foo("param");
            
                f(); // expect: param
            """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["param"], outputMessages);
        }

        [Fact]
        public void ClosureOverVariable()
        {
            var source = """
                fun f() {
                  var a = "a";
                  var b = "b";
                  fun g() {
                    print b; // expect: b
                    print a; // expect: a
                  }
                  g();
                }
                f();
            """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["b", "a"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void ClosureOverMethodParameter()
        {
            var source = """
                var f;
            
                class Foo {
                  method(param) {
                    fun f_() {
                      print param;
                    }
                    f = f_;
                  }
                }
            
                Foo().method("param");
                f(); // expect: param
            """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["param"], outputMessages);
        }

        [Fact]
        public void ClosureInFunction()
        {
            var source = """
                         var f;
                         
                         {
                           var local = "local";
                           fun f_() {
                             print local;
                           }
                           f = f_;
                         }
                         
                         f(); // expect: local
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["local"], outputMessages);
        }

        [Fact]
        public void NestedClosure()
        {
            var source = """
                var f;
            
                fun f1() {
                  var a = "a";
                  fun f2() {
                    var b = "b";
                    fun f3() {
                      var c = "c";
                      fun f4() {
                        print a;
                        print b;
                        print c;
                      }
                      f = f4;
                    }
                    f3();
                  }
                  f2();
                }
                f1();
            
                f();
                // expect: a
                // expect: b
                // expect: c
            """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["a", "b", "c"], outputMessages);
        }

        [Fact]
        public void OpenClosureInFunction()
        {
            var source = """
                         {
                           var local = "local";
                           fun f() {
                             print local; // expect: local
                           }
                           f();
                         }
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["local"], outputMessages);
        }

        [Fact]
        public void ReferenceClosureMultipleTimes()
        {
            var source = """
                var f;
            
                {
                  var a = "a";
                  fun f_() {
                    print a;
                    print a;
                  }
                  f = f_;
                }
            
                f();
                // expect: a
                // expect: a
            """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["a", "a"], outputMessages);
        }

        [Fact]
        public void ReuseClosureSlot()
        {
            var source = """
                {
                  var f;
            
                  {
                    var a = "a";
                    fun f_() { print a; }
                    f = f_;
                  }
            
                  {
                    // Since a is out of scope, the local slot will be reused by b. Make sure
                    // that f still closes over a.
                    var b = "b";
                    f(); // expect: a
                  }
                }
            """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["a"], outputMessages);
        }

        [Fact]
        public void ShadowClosureWithLocal()
        {
            var source = """
                {
                  var foo = "closure";
                  fun f() {
                    {
                      print foo; // expect: closure
                      var foo = "shadow";
                      print foo; // expect: shadow
                    }
                    print foo; // expect: closure
                  }
                  f();
                }
            """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["closure", "shadow", "closure"], outputMessages);
        }

        [Fact]
        public void UnusedClosure()
        {
            // This test is weird, think the book author had some weird bug he was tracking down and this test stuck around.
            // Added it just to make sure I didn't have a similar bug.
            var source = """
                {
                  var a = "a";
                  if (false) {
                    fun foo() { a; }
                  }
                }
            
                // If we get here, we didn't segfault when a went out of scope.
                print "ok"; // expect: ok
            """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["ok"], outputMessages);
        }

        [Fact]
        public void UnusedLaterClosure()
        {
            // This test is weird, think the book author had some weird bug he was tracking down and this test stuck around.
            // Added it just to make sure I didn't have a similar bug.
            var source = """
                // Here we create two locals that can be closed over, but only the first one
                // actually is. When "b" goes out of scope, we need to make sure we don't
                // prematurely close "a".
                var closure;
            
                {
                  var a = "a";
            
                  {
                    var b = "b";
                    fun returnA() {
                      return a;
                    }
            
                    closure = returnA;
            
                    if (false) {
                      fun returnB() {
                        return b;
                      }
                    }
                  }
            
                  print closure(); // expect: a
                }
            """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["a"], outputMessages);
        }
    }
}
