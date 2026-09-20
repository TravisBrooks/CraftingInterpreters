namespace LoxTest
{
    public class CommentTest : InterpreterTestBase
    {
        [Fact]
        public void LineAtEof()
        {
            var source = """
                         print "ok"; // expect: ok
                         // comment
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["ok"], outputMessages);
        }

        [Fact]
        public void OnlyLineComment()
        {
            var source = """
                         // comment
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Empty(outputMessages);
        }

        [Fact]
        public void UnicodeComment()
        {
            var source = """
                         // Unicode characters are allowed in comments.
                         //
                         // Latin 1 Supplement: £§¶ÜÞ
                         // Latin Extended-A: ĐĦŋœ
                         // Latin Extended-B: ƂƢƩǁ
                         // Other stuff: ឃᢆ᯽₪ℜ↩⊗┺░
                         // Emoji: ☃☺♣
                         
                         print "ok"; // expect: ok
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["ok"], outputMessages);
        }
    }
}