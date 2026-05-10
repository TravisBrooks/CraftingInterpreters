using Lox.Exception;
using Lox.InterpreterVisitors;

namespace Lox.Callable
{
    public class LoxFunction : ICallable
    {
        private readonly FunctionStatement _declaration;
        private readonly Environment _closure;

        public LoxFunction(FunctionStatement declaration)
        {
            _declaration = declaration;
            _closure = Lox.Environment.Clone();
        }

        public object? Call(StatementVisitor visitor, List<object?> arguments)
        {
            var originalEnvironment = Lox.Environment.Clone();
            try
            {
                Lox.Environment = _closure.Clone();
                if (arguments.Count > 0)
                {
                    CallImplWithArguments(visitor, arguments);
                }
                else
                {
                    CallImplNoArguments(visitor);
                }
            }
            catch(ReturnException re) 
            {
                return re.Value; 
            }
            finally
            {
                Lox.Environment = originalEnvironment;
            }

            return null;
        }

        /// <summary>
        /// If there are arguments, we need to enter a new scope and define the parameters in that scope.
        /// </summary>
        /// <param name="visitor"></param>
        /// <param name="arguments"></param>
        private void CallImplWithArguments(StatementVisitor visitor, List<object?> arguments)
        {
            try
            {
                // We do not want the parameters to be defined in the outer scope, which might be the global scope, so we enter a new inner scope.
                // We could just willy-nilly leave cruft lying around in the global scope, but it seems sloppy.
                Lox.Environment.EnterInnerScope();
                for (var i = 0; i < _declaration.Parameters.Count; i++)
                {
                    Lox.Environment.Define(_declaration.Parameters[i].Lexeme, arguments[i]);
                }
                // The body will be executed in its own inner scope, possibly with nested inner scopes if there are blocks in the body.
                _ = visitor.Visit(_declaration.Body);
            }
            finally
            {
                Lox.Environment.ExitInnerScope();
            }
        }

        /// <summary>
        /// No arguments so no need to create an empty inner scope.
        /// </summary>
        /// <param name="visitor"></param>
        private void CallImplNoArguments(StatementVisitor visitor)
        {
            _ = visitor.Visit(_declaration.Body);
        }

        public int Arity()
        {
            return _declaration.Parameters.Count;
        }

        public override string ToString()
        {
            var paramStr = string.Join(", ", _declaration.Parameters.Select(tkn => tkn.Lexeme));
            return $"<fn {_declaration.Name.Lexeme}({paramStr})>";
        }
    }
}
