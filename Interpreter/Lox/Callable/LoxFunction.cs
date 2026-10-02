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
        private readonly bool _isInitializer;

        public LoxFunction(
            EnvironmentContext environmentContext,
            string? name,
            FunExpr declaration,
            bool isInitializer,
            Environment? closure = null)
        {
            _environmentContext = environmentContext;
            _name = name;
            _declaration = declaration;
            _isInitializer = isInitializer;
            _closure = closure ?? _environmentContext.Environment;
            if (name is not null)
            {
                _environmentContext.Environment.Define(name, this);
            }
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
                // the edge case of a constructor using a return statement to stop evaluating the rest of the init method
                if (_isInitializer)
                {
                    return _closure.GetAt(0, new Token(TokenType.THIS, "this", null, 0));
                }
                return re.Value; 
            }
            finally
            {
                _environmentContext.Environment = originalEnvironment;
            }

            if (_isInitializer)
            {
               return _closure.GetAt(0, new Token(TokenType.THIS, "this", null, 0));
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

        public LoxFunction Bind(LoxInstance loxInstance)
        {
            var environment = new Environment(_closure);
            environment.Define("this", loxInstance);
            return new LoxFunction(_environmentContext, _name, _declaration, isInitializer: _isInitializer, closure: environment);
        }
    }
}