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
                ExprStatement exprStatement => Visit(exprStatement),
                PrintStatement printStatement => Visit(printStatement),
                BlockStatement blockStatement => Visit(blockStatement),
                IfStatement ifStatement => Visit(ifStatement),
                WhileStatement whileStatement => Visit(whileStatement),
                ForStmt forStmt => Visit(forStmt),
                BreakStatement breakStatement => Visit(breakStatement),
                ContinueStatement continueStatement => Visit(continueStatement),
                ReturnStatement returnStatement => Visit(returnStatement),
                _ => throw new RuntimeException(null, $"Unknown statement type: {stmt.GetType().Name}")
            };
        }

        #region Vistor implementations

        private Unit Visit(ExprStatement es)
        {
            var exprVal = _expressionVisitor.Evaluate(es.Expression);
            if (_environmentContext.LoxMode == LoxMode.INTERACTIVE_MODE)
            {
                _console.WriteLine(Stringify(exprVal));
            }

            return Unit.Value;
        }

        private Unit Visit(PrintStatement ps)
        {
            var val = _expressionVisitor.Evaluate(ps.Expression);
            _console.WriteLine(Stringify(val));
            return Unit.Value;
        }

        private Unit Visit(BlockStatement bs)
        {
            ExecuteBlock(bs.Declarations);
            return Unit.Value;
        }

        private Unit Visit(IfStatement ifStmt)
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

        private Unit Visit(WhileStatement whileStatement)
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

        private Unit Visit(ForStmt forStmt)
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

        private Unit Visit(BreakStatement breakStatement)
        {
            throw new BreakException();
        }

        private Unit Visit(ContinueStatement continueStatement)
        {
            throw new ContinueException();
        }

        private Unit Visit(ReturnStatement returnStatement)
        {
            throw new ReturnException(_expressionVisitor.Evaluate(returnStatement.Value));
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