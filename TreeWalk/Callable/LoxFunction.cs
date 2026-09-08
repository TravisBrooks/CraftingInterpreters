using Lox.Exception;
using Lox.InterpreterVisitors;

namespace Lox.Callable
{
    public class LoxFunction : ICallable
    {
        private readonly string? _name;
        private readonly FuncExpr _declaration;
        private readonly Environment _closure;

        public LoxFunction(string? name, FuncExpr declaration)
        {
            _name = name;
            _declaration = declaration;
            if (name is not null)
            {
                Lox.EnvironmentContext.Environment.Define(name, this);
            }
            _closure = Lox.EnvironmentContext.Environment;
        }

        public object? Call(StatementVisitor visitor, List<object?> arguments)
        {
            // we're not doing anything to this Environment so no need to clone it.
            var originalEnvironment = Lox.EnvironmentContext.Environment;
            try
            {
                // variables in the closure might get mutated, and we want that to persist across calls
                // so intentionally not cloning the closure Environment.
                Lox.EnvironmentContext.Environment = _closure;
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
                Lox.EnvironmentContext.Environment = originalEnvironment;
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
            Environment.ExecuteInScope(Lox.EnvironmentContext, () =>
            {

                for (var i = 0; i < _declaration.Parameters.Count; i++)
                {
                    Lox.EnvironmentContext.Environment.Define(_declaration.Parameters[i].Lexeme, arguments[i]);
                }
                // The body will be executed in its own inner scope, possibly with nested inner scopes if there are blocks in the body.
                _ = visitor.Visit(_declaration.Body);
            });
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

        public string? Name => _name;

        public override string ToString()
        {
            var paramStr = string.Join(", ", _declaration.Parameters.Select(tkn => tkn.Lexeme));
            return $"<fn {_name ?? "[lambda]"}({paramStr})>";
        }
    }
}
