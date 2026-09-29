using Lox.Exception;

namespace Lox.InterpreterVisitors
{
    public class StatementVisitor : IVisitor<Stmt, Unit>
    {
        private readonly IConsole _console;
        private readonly EnvironmentContext _environmentContext;
        private readonly Func<DeclarationVisitor> _declarationVisitorFn;
        private readonly ExpressionVisitor _expressionVisitor;

        public StatementVisitor(
            IConsole console,
            EnvironmentContext environmentContext,
            Func<DeclarationVisitor> declarationVisitor,
            ExpressionVisitor expressionVisitor)
        {
            _console = console;
            _environmentContext = environmentContext;
            _declarationVisitorFn = declarationVisitor;
            _expressionVisitor = expressionVisitor;
        }

        public Unit Visit(Stmt stmt)
        {
            return stmt switch
            {
                ExprStatement es => VisitExprStatement(es),
                PrintStatement ps => VisitPrintStatement(ps),
                BlockStatement bs => VisitBlockStatement(bs),
                IfStatement i => VisitIfStatement(i),
                WhileStatement ws => VisitWhileStatement(ws),
                ForStmt fs => VisitForStmt(fs),
                BreakStatement => throw new BreakException(),
                ContinueStatement => throw new ContinueException(),
                ReturnStatement rs => throw new ReturnException(_expressionVisitor.Evaluate(rs.Value)),
                _ => throw new RuntimeException(null, $"Unknown statement type: {stmt.GetType().Name}")
            };
        }

        #region Vistor implementations

        private Unit VisitExprStatement(ExprStatement es)
        {
            var exprVal = _expressionVisitor.Evaluate(es.Expression);
            if (_environmentContext.LoxMode == LoxMode.INTERACTIVE_MODE)
            {
                _console.WriteLine(Stringify(exprVal));
            }

            return Unit.Value;
        }

        private Unit VisitPrintStatement(PrintStatement ps)
        {
            var val = _expressionVisitor.Evaluate(ps.Expression);
            _console.WriteLine(Stringify(val));
            return Unit.Value;
        }

        private Unit VisitBlockStatement(BlockStatement bs)
        {
            ExecuteBlock(bs.Declarations);
            return Unit.Value;
        }

        private Unit VisitIfStatement(IfStatement ifStmt)
        {
            if (ExpressionVisitor.IsTruthy(_expressionVisitor.Evaluate(ifStmt.Condition)))
            {
                ifStmt.ThenBranch.Accept(this);
            }
            else if (ifStmt.ElseBranch is not null)
            {
                ifStmt.ElseBranch.Accept(this);
            }
            return Unit.Value;
        }

        private Unit VisitWhileStatement(WhileStatement whileStatement)
        {
            while (ExpressionVisitor.IsTruthy(_expressionVisitor.Evaluate(whileStatement.Condition)))
            {
                try
                {
                    whileStatement.Body.Accept(this);
                }
                catch (BreakException)
                {
                    break;
                }
                catch (ContinueException)
                {
                    // call continue here, its redundant since it's the end of the loop body, but it makes the intention more obvious
                    continue;
                }
            }
            return Unit.Value;
        }

        private Unit VisitForStmt(ForStmt forStmt)
        {
            Environment? previous = null;
            try
            {
                if (forStmt.Initializer is VarDecl)
                {
                    previous = _environmentContext.Environment;
                    _environmentContext.Environment = new Environment(previous);
                }
                var declVisitor = _declarationVisitorFn();
                forStmt.Initializer?.Accept(declVisitor);
                while (true)
                {
                    if (forStmt.Condition is not null && !ExpressionVisitor.IsTruthy(_expressionVisitor.Evaluate(forStmt.Condition)))
                    {
                        break;
                    }
                    try
                    {
                        forStmt.Body.Accept(this);
                    }
                    catch (BreakException)
                    {
                        break;
                    }
                    catch (ContinueException)
                    {
                        // fall through to increment
                    }
                    if (forStmt.Increment is not null)
                    {
                        _expressionVisitor.Evaluate(forStmt.Increment);
                    }
                }
            }
            finally{
                if (previous is not null)
                {
                    _environmentContext.Environment = previous;
                }
            }
            return Unit.Value;
        }

        #endregion

        private void ExecuteBlock(IEnumerable<Decl> declarations)
        {
            Environment.ExecuteInScope(_environmentContext, () => 
            {
                var declVisitor = _declarationVisitorFn();
                foreach (var decl in declarations)
                {
                    decl.Accept(declVisitor);
                }
            });
        }

        private static string Stringify(object? o)
        {
            return o switch
            {
                double d => d.ToString("G"),
                // forcing lowercase for true and false to match Lox's output
                bool b => b ? "true" : "false",
                _ => o?.ToString() ?? "nil"
            };
        }
    }
}