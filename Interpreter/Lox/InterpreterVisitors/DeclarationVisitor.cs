using Lox.Callable;
using Lox.Exception;

namespace Lox.InterpreterVisitors
{
    public class DeclarationVisitor : IVisitor<Decl, Unit>
    {
        private readonly EnvironmentContext _environmentContext;
        private readonly StatementVisitor _statementVisitor;
        private readonly ExpressionVisitor _expressionVisitor;

        public DeclarationVisitor(
            EnvironmentContext environmentContext,
            StatementVisitor statementVisitor,
            ExpressionVisitor expressionVisitor)
        {
            _environmentContext = environmentContext;
            _statementVisitor = statementVisitor;
            _expressionVisitor = expressionVisitor;
        }

        public Unit Visit(IEnumerable<Decl> declarations)
        {
            foreach (var d in declarations)
            {
                Visit(d);
            }
            return Unit.Value;
        }

        public Unit Visit(Decl decl)
        {
            return decl switch
            {
                ClassDecl classDecl => Visit(classDecl),
                VarDecl varDecl => Visit(varDecl),
                FunDecl funDecl => Visit(funDecl),
                Stmt stmt => Visit(stmt),
                _ => throw new RuntimeException(null, $"Unknown declaration type: {decl.GetType().Name}")
            };
        }

        private Unit Visit(ClassDecl classDecl)
        {
            _environmentContext.Environment.Define(classDecl.Name.Lexeme, classDecl);
            var methods = new Dictionary<string, LoxFunction>();
            foreach (var method in classDecl.Methods)
            {
                var isInitializer = method.Name.Lexeme == "init";
                var loxFunction = new LoxFunction(_environmentContext, method.Name.Lexeme, method.FunExpr, isInitializer);
                methods[method.Name.Lexeme] = loxFunction;
            }
            var klass = new LoxClass(classDecl.Name.Lexeme, methods);
            _environmentContext.Environment.Assign(classDecl.Name, klass);
            return Unit.Value;
        }

        private Unit Visit(VarDecl varDecl)
        {
            object? val = null;
            if (varDecl.Initializer is not null)
            {
                val = _expressionVisitor.Evaluate(varDecl.Initializer);
            }

            _environmentContext.Environment.Define(varDecl.Name.Lexeme, val);
            return Unit.Value;
        }

        private Unit Visit(FunDecl funDecl)
        {
            var fnName = funDecl.Name.Lexeme;
            var fn = new LoxFunction(_environmentContext, fnName, funDecl.FunExpr, isInitializer: false);
            _environmentContext.Environment.Define(fnName, fn);
            return Unit.Value;
        }

        private Unit Visit(Stmt stmt)
        {
            return _statementVisitor.Visit(stmt);
        }
    }
}