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
                VarDecl varDecl => Visit(varDecl),
                FunDecl funDecl => Visit(funDecl),
                Stmt stmt => Visit(stmt),
                _ => throw new RuntimeException(null, $"Unknown declaration type: {decl.GetType().Name}")
            };
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
            var fn = new LoxFunction(_environmentContext, fnName, funDecl.FunExpr);
            _environmentContext.Environment.Define(fnName, fn);
            return Unit.Value;
        }

        private Unit Visit(Stmt stmt)
        {
            return _statementVisitor.Visit(stmt);
        }
    }
}