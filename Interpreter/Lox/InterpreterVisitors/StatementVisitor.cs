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
                ExprStatement es => ExprStmtVisitor(es.Expression),
                PrintStatement ps => PrintStmtVisitor(ps.Expression),
                BlockStatement bs => BlockStmtVisitor(bs),
                IfStatement i => IfStmtVisitor(i),
                WhileStatement ws => WhileStmtVisitor(ws),
                ForStmt fs => ForStmtVisitor(fs),
                BreakStatement => throw new BreakException(),
                ContinueStatement => throw new ContinueException(),
                ReturnStatement rs => throw new ReturnException(_expressionVisitor.Evaluate(rs.Value)),
                _ => throw new RuntimeException(null, $"Unknown statement type: {stmt.GetType().Name}")
            };
        }

        #region Vistor implementations

        private Unit ExprStmtVisitor(Expr expr)
        {
            var exprVal = _expressionVisitor.Evaluate(expr);
            if (_environmentContext.LoxMode == LoxMode.INTERACTIVE_MODE)
            {
                _console.WriteLine(Stringify(exprVal));
            }

            return Unit.Value;
        }

        private Unit PrintStmtVisitor(Expr expr)
        {
            var val = _expressionVisitor.Evaluate(expr);
            _console.WriteLine(Stringify(val));
            return Unit.Value;
        }

        private Unit BlockStmtVisitor(BlockStatement bs)
        {
            ExecuteBlock(bs.Declarations);
            return Unit.Value;
        }

        private Unit IfStmtVisitor(IfStatement ifStmt)
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

        private Unit WhileStmtVisitor(WhileStatement whileStatement)
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

        private Unit ForStmtVisitor(ForStmt forStmt)
        {
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