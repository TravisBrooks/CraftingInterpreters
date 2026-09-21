namespace LoxTest
{
    public class ContinueTest : InterpreterTestBase
    {
        #region Happy Path Tests

        [Fact]
        public void WhileLoopContinues()
        {
            var source = """
                         var i = 0;
                         while(i < 5){
                            i = i + 1;
                            if(i == 3){
                                continue;
                            }
                            print i;
                         }
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["1", "2", "4", "5"], outputMessages);
        }

        [Fact]
        public void ForLoopContinues()
        {
            var source = """
                         for(var i = 1; i <= 5; i = i + 1){
                            if(i == 3){
                                continue;
                            }
                            print i;
                         }
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["1", "2", "4", "5"], outputMessages);
        }

        [Fact]
        public void ContinueOnlyContinuesLoopItsCurrentlyIn()
        {
            var source = """
                         for(var i = 0; i < 10; i = i + 1){
                            if(i > 2){
                                continue;
                            }
                            print i;
                            for(var j = 10 + i; j < 100; j = j + 10){
                                if(j > 12){
                                    continue;
                                }
                                print j;
                            }
                         }
                         """;
            var output = Interpret(source);
            Assert.False(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Equal(["0", "10", "1", "11", "2", "12"], outputMessages);
        }

        #endregion

        #region Sad Path Tests

        [Fact]
        public void GlobalContinue()
        {
            var source = """
                         var x = 1;
                         continue;
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Single(outputMessages);
            var msg = outputMessages.First();
            Assert.Contains("Cannot use 'continue' outside of a loop.", msg);
        }

        [Fact]
        public void ContinueInsideFunctionInsideLoop()
        {
            var source = """
                         while(true){
                            fun funcInLoop(){
                                continue; // This continue is in a function that coincidentally is in a loop, forbidden
                            }
                            funcInLoop();
                         }
                         """;
            var output = Interpret(source);
            Assert.True(output.HasOutputError());
            var outputMessages = output.OutputMessagesNoStatus();
            Assert.Single(outputMessages);
            var msg = outputMessages.First();
            Assert.Contains("Cannot use 'continue' outside of a loop.", msg);
        }

        [Fact(Skip = "Haven't implemented classes yet")]
        public void CannotContinueInsideClass()
        {
            var source = """
                         while(true){
                            class Foo{
                                init(){
                                    continue; // Error: Cannot use 'continue' outside of a loop.
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
            Assert.Contains("Cannot use 'continue' outside of a loop.", msg);
        }

        #endregion
    }
}