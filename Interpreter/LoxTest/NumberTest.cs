namespace LoxTest
{
    public class NumberTest : InterpreterTestBase
    {
        /// <summary>
        /// Its not super obvious but I believe the expected error will only show up after changes to the parser for classes. Currently
        /// the line fails with the message: ERROR [line 2] Error at '.': Expect ';' after expression.
        /// </summary>
        [Fact(Skip = "Haven't implemented classes yet")]
        public void DecimalPointAtEndOfFile()
        {
            var source = """
                         // [line 2] Error at end: Expect property name after '.'.
                         123.
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Expect property name after '.'.", msg);
        }

        [Fact]
        public void LeadingDot()
        {
            var source = """
                         // [line 2] Error at '.': Expect expression.
                         .123;
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Error at '.': Expect expression.", msg);
        }

        [Fact]
        public void Literals()
        {
            var source = """
                         print 123;     // expect: 123
                         print 987654;  // expect: 987654
                         print 0;       // expect: 0
                         print -0;      // expect: -0
                         
                         print 123.456; // expect: 123.456
                         print -0.001;  // expect: -0.001
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["123", "987654", "0", "-0", "123.456", "-0.001"], outputMessages);
        }

        [Fact]
        public void NanEquality()
        {
            var source = """
                         var nan = 0/0;
                         
                         print nan == 0; // expect: false
                         print nan != 1; // expect: true
                         
                         // NaN is not equal to self.
                         print nan == nan; // expect: false
                         print nan != nan; // expect: true
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["false", "true", "false", "true"], outputMessages);
        }

        /// <summary>
        /// Its not super obvious but I believe the expected error will only show up after changes to the parser for classes. Currently
        /// the line fails with the message: ERROR [line 2] Error at '.': Expect ';' after expression.
        /// </summary>
        [Fact(Skip = "Haven't implemented classes yet")]
        public void TrailingDot()
        {
            var source = """
                         // [line 2] Error at ';': Expect property name after '.'.
                         123.;
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            var msg = outputMessages.Single();
            Assert.Contains("Error at ';': Expect property name after '.'.", msg);
        }
    }
}
