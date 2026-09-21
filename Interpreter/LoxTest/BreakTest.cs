namespace LoxTest
{
    public class BreakTest : InterpreterTestBase
    {
        #region Happy Path Tests

        [Fact]
        public void WhileLoopExits()
        {
            var source = """
                         print "start";
                         // if the break statement doesn't work this will just keep going forever
                         while(true){
                           print "in loop";
                           break;
                           print "unreachable in loop";
                         }
                         print "end";
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["start", "in loop", "end"], outputMessages);
        }

        [Fact]
        public void ForLoopExits()
        {
            var source = """
                         print "start";
                         // if the break statement doesn't work this will just keep going forever
                         for(;;){
                           print "in loop";
                           break;
                           print "unreachable in loop";
                         }
                         print "end";
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["start", "in loop", "end"], outputMessages);
        }

        [Fact]
        public void BreakOnlyExitsLoopItsCurrentlyIn()
        {
            var source = """
                         for(var i = 0; i < 10; i = i + 1){
                            print i;
                            while(true){
                                print 10 + i;
                                break;
                            }
                            if(i == 2){
                                break;
                            }
                         }
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["0", "10", "1", "11", "2", "12"], outputMessages);
        }

        [Fact]
        public void NestedLoopInFunction()
        {
            var source = """
                         for(var i=0; i< 4; i = i + 1){
                            fun funcInLoop(){
                                for(var j = i; j < 3; j = j + 1){
                                    if(j > 1){
                                        break; // this is safe because the break is inside a loop in the func
                                    }
                                    print i + j;
                                }
                            }
                            funcInLoop();
                         }
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["0", "1", "2"], outputMessages);
        }

        #endregion

        #region Sad Path Tests

        [Fact]
        public void GlobalBreak()
        {
            var source = """
                         var x = 1;
                         break;
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Single(outputMessages);
            var msg = outputMessages.First();
            Assert.Contains("Cannot use 'break' outside of a loop.", msg);
        }

        [Fact]
        public void BreakInsideFunctionInsideLoop()
        {
            var source = """
                         while(true){
                            fun funcInLoop(){
                                break; // This break is in a function that coincidentally is in a loop, forbidden
                            }
                            funcInLoop();
                         }
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Single(outputMessages);
            var msg = outputMessages.First();
            Assert.Contains("Cannot use 'break' outside of a loop.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void CannotBreakInsideClass()
        {
            var source = """
                         while(true){
                            class Foo{
                                init(){
                                    break; // Error: Cannot use 'break' outside of a loop.
                                }
                            }
                            var f = Foo();
                         }
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Single(outputMessages);
            var msg = outputMessages.First();
            Assert.Contains("Cannot use 'break' outside of a loop.", msg);
        }

        #endregion
    }
}