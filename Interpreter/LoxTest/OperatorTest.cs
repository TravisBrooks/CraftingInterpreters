namespace LoxTest
{
    public class OperatorTest : InterpreterTestBase
    {
        [Fact]
        public void Add()
        {
            var source = """
                         print 123 + 456; // expect: 579
                         print "str" + "ing"; // expect: string
                         print "str " + 123; // expect: "str 123"
                         print 123 + " str"; // expect: "123 str"
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["579", "string", "str 123", "123 str"], outputMessages);
        }

        [Fact]
        public void AddBoolNil()
        {
            var source = """
                         true + nil; // expect runtime error: Operands must be numbers or strings.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Operands must be numbers or strings.", msg);
        }

        [Fact]
        public void AddBoolNum()
        {
            var source = """
                         true + 123; // expect runtime error: Operands must be numbers or strings.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Operands must be numbers or strings.", msg);
        }

        [Fact]
        public void AddBoolString()
        {
            var source = """
                         true + "s"; // expect runtime error: Operands must be numbers or strings.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Operands must be numbers or strings.", msg);
        }

        [Fact]
        public void AddNilNil()
        {
            var source = """
                         nil + nil; // expect runtime error: Operands must be numbers or strings.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Operands must be numbers or strings.", msg);
        }

        [Fact]
        public void AddNumNil()
        {
            var source = """
                         1 + nil; // expect runtime error: Operands must be numbers or strings.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Operands must be numbers or strings.", msg);
        }

        [Fact]
        public void AddStringNil()
        {
            var source = """
                         "s" + nil; // expect runtime error: Operands must be numbers or strings.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Operands must be numbers or strings.", msg);
        }

        [Fact]
        public void Comparison()
        {
            var source = """
                         print 1 < 2;    // expect: true
                         print 2 < 2;    // expect: false
                         print 2 < 1;    // expect: false
                         
                         print 1 <= 2;    // expect: true
                         print 2 <= 2;    // expect: true
                         print 2 <= 1;    // expect: false
                         
                         print 1 > 2;    // expect: false
                         print 2 > 2;    // expect: false
                         print 2 > 1;    // expect: true
                         
                         print 1 >= 2;    // expect: false
                         print 2 >= 2;    // expect: true
                         print 2 >= 1;    // expect: true
                         
                         // Zero and negative zero compare the same.
                         print 0 < -0; // expect: false
                         print -0 < 0; // expect: false
                         print 0 > -0; // expect: false
                         print -0 > 0; // expect: false
                         print 0 <= -0; // expect: true
                         print -0 <= 0; // expect: true
                         print 0 >= -0; // expect: true
                         print -0 >= 0; // expect: true
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal([
                "true",
                "false",
                "false",
                "true",
                "true",
                "false",
                "false",
                "false",
                "true",
                "false",
                "true",
                "true",
                "false",
                "false",
                "false",
                "false",
                "true",
                "true",
                "true",
                "true"
            ], outputMessages);
        }

        [Fact]
        public void Divide()
        {
            var source = """
                         print 8 / 2;         // expect: 4
                         print 12.34 / 12.34;  // expect: 1
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["4", "1"], outputMessages);
        }

        [Fact]
        public void DivideNonNumThenNum()
        {
            var source = """
                         "1" / 1; // expect runtime error: Operands must be numbers.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Operands must be numbers.", msg);
        }

        [Fact]
        public void DivideNumThenNonNum()
        {
            var source = """
                         1 / "1"; // expect runtime error: Operands must be numbers.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Operands must be numbers.", msg);
        }

        [Fact]
        public void Equality()
        {
            var source = """
                         print nil == nil; // expect: true
                         
                         print true == true; // expect: true
                         print true == false; // expect: false
                         
                         print 1 == 1; // expect: true
                         print 1 == 2; // expect: false
                         
                         print "str" == "str"; // expect: true
                         print "str" == "ing"; // expect: false
                         
                         print nil == false; // expect: false
                         print false == 0; // expect: false
                         print 0 == "0"; // expect: false
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["true", "true", "false", "true", "false", "true", "false", "false", "false", "false"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void EqualityClasses()
        {
            var source = """
                         // Bound methods have identity equality.
                         class Foo {}
                         class Bar {}
                         
                         print Foo == Foo; // expect: true
                         print Foo == Bar; // expect: false
                         print Bar == Foo; // expect: false
                         print Bar == Bar; // expect: true
                         
                         print Foo == "Foo"; // expect: false
                         print Foo == nil;   // expect: false
                         print Foo == 123;   // expect: false
                         print Foo == true;  // expect: false
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["true", "false", "false", "true", "false", "false", "false", "false"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void EqualityMethod()
        {
            var source = """
                         // Bound methods have identity equality.
                         class Foo {
                           method() {}
                         }
                         
                         var foo = Foo();
                         var fooMethod = foo.method;
                         
                         // Same bound method.
                         print fooMethod == fooMethod; // expect: true
                         
                         // Different closurizations.
                         print foo.method == foo.method; // expect: false
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["true", "false"], outputMessages);
        }

        [Fact]
        public void GreaterNonNumThenNum()
        {
            var source = """
                         "1" > 1; // expect runtime error: Operands must be numbers.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Operands must be numbers.", msg);
        }

        [Fact]
        public void GreaterNumThenNonNum()
        {
            var source = """
                         1 > "1"; // expect runtime error: Operands must be numbers.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Operands must be numbers.", msg);
        }

        [Fact]
        public void GreaterOrEqualNonNumThenNum()
        {
            var source = """
                         "1" >= 1; // expect runtime error: Operands must be numbers.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Operands must be numbers.", msg);
        }

        [Fact]
        public void GreaterOrEqualNumThenNonNum()
        {
            var source = """
                         1 >= "1"; // expect runtime error: Operands must be numbers.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Operands must be numbers.", msg);
        }

        [Fact]
        public void LesserNonNumThenNum()
        {
            var source = """
                         "1" < 1; // expect runtime error: Operands must be numbers.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Operands must be numbers.", msg);
        }

        [Fact]
        public void LesserNumThenNonNum()
        {
            var source = """
                         1 < "1"; // expect runtime error: Operands must be numbers.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Operands must be numbers.", msg);
        }

        [Fact]
        public void LesserOrEqualNonNumThenNum()
        {
            var source = """
                         "1" <= 1; // expect runtime error: Operands must be numbers.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Operands must be numbers.", msg);
        }

        [Fact]
        public void LesserOrEqualNumThenNonNum()
        {
            var source = """
                         1 <= "1"; // expect runtime error: Operands must be numbers.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Operands must be numbers.", msg);
        }

        [Fact]
        public void Multiply()
        {
            var source = """
                         print 5 * 3; // expect: 15
                         print 12.34 * 0.3; // expect: 3.702
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["15", "3.702"], outputMessages);
        }

        [Fact]
        public void MultiplyNonNumThenNum()
        {
            var source = """
                         "1" * 1; // expect runtime error: Operands must be numbers.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Operands must be numbers.", msg);
        }

        [Fact]
        public void MultiplyNumThenNonNum()
        {
            var source = """
                         1 * "1"; // expect runtime error: Operands must be numbers.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Operands must be numbers.", msg);
        }

        [Fact]
        public void Negate()
        {
            var source = """
                         print -(3); // expect: -3
                         print --(3); // expect: 3
                         print ---(3); // expect: -3
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["-3", "3", "-3"], outputMessages);
        }

        [Fact]
        public void NegateNonNum()
        {
            var source = """
                         -"s"; // expect runtime error: Operand must be a number.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Operand must be a number.", msg);
        }

        [Fact]
        public void Not()
        {
            var source = """
                         print !true;     // expect: false
                         print !false;    // expect: true
                         print !!true;    // expect: true
                         
                         print !123;      // expect: false
                         print !0;        // expect: false
                         
                         print !nil;     // expect: true
                         
                         print !"";       // expect: false
                         
                         fun foo() {}
                         print !foo;      // expect: false
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["false", "true", "true", "false", "false", "true", "false", "false"], outputMessages);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void NotClasses()
        {
            var source = """
                         class Bar {}
                         print !Bar;      // expect: false
                         print !Bar();    // expect: false
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["false", "false"], outputMessages);
        }

        [Fact]
        public void NotEquality()
        {
            var source = """
                         print nil != nil; // expect: false
                         
                         print true != true; // expect: false
                         print true != false; // expect: true
                         
                         print 1 != 1; // expect: false
                         print 1 != 2; // expect: true
                         
                         print "str" != "str"; // expect: false
                         print "str" != "ing"; // expect: true
                         
                         print nil != false; // expect: true
                         print false != 0; // expect: true
                         print 0 != "0"; // expect: true
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["false", "false", "true", "false", "true", "false", "true", "true", "true", "true"], outputMessages);
        }

        [Fact]
        public void Subtract()
        {
            var source = """
                         print 4 - 3; // expect: 1
                         print 1.2 - 1.2; // expect: 0
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["1", "0"], outputMessages);
        }

        [Fact]
        public void SubtractNonNumThenNum()
        {
            var source = """
                         "1" - 1; // expect runtime error: Operands must be numbers.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Operands must be numbers.", msg);
        }

        [Fact]
        public void SubtractNumThenNonNum()
        {
            var source = """
                         1 - "1"; // expect runtime error: Operands must be numbers.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Operands must be numbers.", msg);
        }

        /// <summary>
        /// The author put this in as a standalone file not grouped by a directory, but it seems to go along with operator testing
        /// </summary>
        [Fact]
        public void Precedence()
        {
            var source = """
                         // * has higher precedence than +.
                         print 2 + 3 * 4; // expect: 14
                         
                         // * has higher precedence than -.
                         print 20 - 3 * 4; // expect: 8
                         
                         // / has higher precedence than +.
                         print 2 + 6 / 3; // expect: 4
                         
                         // / has higher precedence than -.
                         print 2 - 6 / 3; // expect: 0
                         
                         // < has higher precedence than ==.
                         print false == 2 < 1; // expect: true
                         
                         // > has higher precedence than ==.
                         print false == 1 > 2; // expect: true
                         
                         // <= has higher precedence than ==.
                         print false == 2 <= 1; // expect: true
                         
                         // >= has higher precedence than ==.
                         print false == 1 >= 2; // expect: true
                         
                         // 1 - 1 is not space-sensitive.
                         print 1 - 1; // expect: 0
                         print 1 -1;  // expect: 0
                         print 1- 1;  // expect: 0
                         print 1-1;   // expect: 0
                         
                         // Using () for grouping.
                         print (2 * (6 - (2 + 2))); // expect: 4
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["14", "8", "4", "0", "true", "true", "true", "true", "0", "0", "0", "0", "4"], outputMessages);
        }
    }
}