using Lox.Exception;
using Lox.InterpreterVisitors;

namespace Lox.Callable
{
    public class LoxFunction : ICallable
    {
        private readonly EnvironmentContext _environmentContext;
        private readonly string? _name;
        private readonly FunExpr _declaration;
        private readonly Environment _closure;

        public LoxFunction(
            EnvironmentContext environmentContext,
            string? name,
            FunExpr declaration)
        {
            _environmentContext = environmentContext;
            _name = name;
            _declaration = declaration;
            if (name is not null)
            {
                _environmentContext.Environment.Define(name, this);
            }
            _closure = _environmentContext.Environment;
        }

        public object? Call(DeclarationVisitor visitor, List<object?> arguments)
        {
            // we're not doing anything to this Environment so no need to set it aside.
            var originalEnvironment = _environmentContext.Environment;
            try
            {
               _environmentContext.Environment = new Environment(_closure);
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
                _environmentContext.Environment = originalEnvironment;
            }

            return null;
        }

        private void CallImplWithArguments(DeclarationVisitor visitor, List<object?> arguments)
        {
            for (var i = 0; i < _declaration.Parameters.Count; i++)
            {
                _environmentContext.Environment.Define(_declaration.Parameters[i].Lexeme, arguments[i]);
            }
            _ = visitor.Visit(_declaration.Body);
        }

        private void CallImplNoArguments(DeclarationVisitor visitor)
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