using Lox;
using Lox.Exception;
using Environment = Lox.Environment;

namespace LoxTest
{
    public class EnvironmentTest
    {
        [Fact]
        public void EnclosingProperty()
        {
            var parent = new Environment();
            var child = new Environment(parent);

            Assert.Null(parent.Enclosing);
            Assert.Same(parent, child.Enclosing);
        }

        [Fact]
        public void DefineMutatesState()
        {
            var env = new Environment();
            Assert.Empty(env.GetInternalState());
            var someKey = "some key";
            var someValue = "some value";
            env.Define(someKey, someValue);
            var state = env.GetInternalState();
            var found = state[someKey];
            Assert.Same(someValue, found);
        }

        [Fact]
        public void GetSomething()
        {
            var env = new Environment();
            Assert.Empty(env.GetInternalState());
            var someKey = "some key";
            var someValue = "some value";
            env.Define(someKey, someValue);
            Assert.Single(env.GetInternalState());
            var found = env.Get(new Lox.Token(Lox.TokenType.STRING, someKey, null, 0));
            Assert.Same(someValue, found);
        }

        [Fact]
        public void AssignHappyPath()
        {
            var env = new Environment();
            Assert.Empty(env.GetInternalState());
            var someKey = "some key";
            var someValue = "some value";
            env.Define(someKey, someValue);
            
            var internalStateBeforeAssign = env.GetInternalState();
            var originalFoundValue = internalStateBeforeAssign[someKey];
            Assert.Same(someValue, originalFoundValue);

            var assignedValue = "new assigned value";
            var bullshitToken = new Lox.Token(TokenType: (TokenType)(-1), Lexeme: someKey, Literal: null, Line: -1);
            env.Assign(bullshitToken, assignedValue);

            var internalStateAfterAssign = env.GetInternalState();
            var assignedFoundValue = internalStateAfterAssign[someKey];
            Assert.Same(assignedValue, assignedFoundValue);
        }

        [Fact]
        public void AssignSadPath()
        {
            var env = new Environment();
            Assert.Empty(env.GetInternalState());

            var someKey = "some key";
            var bullshitToken = new Lox.Token(TokenType: (TokenType)(-1), Lexeme: someKey, Literal: null, Line: -1);
            var re = Assert.Throws<RuntimeException>(() => env.Assign(bullshitToken, "lets assign something that is not already defined..."));
            Assert.Same(bullshitToken, re.Token);
            Assert.Equal($"Undefined variable '{bullshitToken.Lexeme}'.", re.Message);
        }

        [Fact]
        public void DefineGlobal()
        {
            var global = new Environment();
            var child = new Environment(global);
            var grandChild = new Environment(child);

            var globalKey = "global key";
            var globalValue = "global value";
            // define global should go up the stack of enclosing environments to the global
            grandChild.DefineGlobal(globalKey, globalValue);

            Assert.Empty(grandChild.GetInternalState());
            Assert.Empty(child.GetInternalState());
            var globalValues = global.GetInternalState();
            Assert.True(globalValues.TryGetValue(globalKey, out var foundValue));
            Assert.Equal(globalValue, foundValue);
        }

        [Fact]
        public void GetGlobal()
        {
            var global = new Environment();
            var child = new Environment(global);
            var grandChild = new Environment(child);

            var globalKey = "global key";
            var globalValue = "global value";
            global.Define(globalKey, globalValue);

            Assert.Empty(grandChild.GetInternalState());
            Assert.Empty(child.GetInternalState());
            // verify that GetGlobal goes up the stack of enclosing environments to the global
            var foundValue = grandChild.GetGlobal(globalKey);
            Assert.Equal(globalValue, foundValue);
        }

        [Fact]
        public void ExecuteInScope()
        {
            var ctxt = new EnvironmentContext();
            var global = ctxt.Environment;
            Assert.Null(global.Enclosing);

            Environment? capturedEnv = null;
            Environment.ExecuteInScope(ctxt, () => capturedEnv = ctxt.Environment);

            // this verifies that ExecuteInScope adds an environment to the stack that the initial environment encloses
            Assert.NotNull(capturedEnv?.Enclosing);
            Assert.Same(global, capturedEnv.Enclosing);
        }
    }
}