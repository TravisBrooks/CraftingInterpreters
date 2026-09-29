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
                VarDecl varDecl => VisitVarDecl(varDecl.Name, varDecl.Initializer),
                FunDecl funDecl => VisitFunDecl(funDecl),
                Stmt stmt => _statementVisitor.Visit(stmt),
                _ => throw new RuntimeException(null, $"Unknown declaration type: {decl.GetType().Name}")
            };
        }

        private Unit VisitVarDecl(Token name, Expr? initializer)
        {
            object? val = null;
            if (initializer is not null)
            {
                val = _expressionVisitor.Evaluate(initializer);
            }

            _environmentContext.Environment.Define(name.Lexeme, val);
            return Unit.Value;
        }

        private Unit VisitFunDecl(FunDecl fd)
        {
            var fnName = fd.Name.Lexeme;
            var fn = new LoxFunction(_environmentContext, fnName, fd.FunExpr);
            _environmentContext.Environment.Define(fnName, fn);
            return Unit.Value;
        }
    }
}