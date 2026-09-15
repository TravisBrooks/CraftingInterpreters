using Lox.Callable;
using Lox.Exception;

namespace Lox.InterpreterVisitors
{
    public class StatementVisitor : IVisitor<Stmt, Unit>
    {
        private readonly IConsole _console;
        private readonly EnvironmentContext _environmentContext;
        private readonly ExpressionVisitor _expressionVisitor;

        public StatementVisitor(
            IConsole console,
            EnvironmentContext environmentContext,
            ExpressionVisitor expressionVisitor)
        {
            _console = console;
            _environmentContext = environmentContext;
            _expressionVisitor = expressionVisitor;
        }

        public Unit Visit(Stmt stmt)
        {
            return stmt switch
            {
                ExprStatement es => ExprStmtVisitor(es.Expression),
                PrintStatement ps => PrintStmtVisitor(ps.Expression),
                VarDeclaration vs => VarStmtVisitor(vs.Name, vs.Initializer),
                BlockStatement bs => BlockStmtVisitor(bs),
                IfStatement i => IfStmtVisitor(i),
                WhileStatement ws => WhileStmtVisitor(ws),
                BreakStatement => throw new BreakException(),
                ContinueStatement => throw new ContinueException(),
                FunctionDeclaration fs => FuncStmtVisitor(fs),
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

        private Unit VarStmtVisitor(Token name, Expr? initializer)
        {
            object? val = null;
            if (initializer is not null)
            {
                val = _expressionVisitor.Evaluate(initializer);
            }

            _environmentContext.Environment.Define(name.Lexeme, val);
            return Unit.Value;
        }

        private Unit BlockStmtVisitor(BlockStatement bs)
        {
            ExecuteBlock(bs.Statements);
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
                    // technically i could call continue here, but that would be redundant since it's the end of the loop body
                }
            }
            return Unit.Value;
        }

        private Unit FuncStmtVisitor(FunctionDeclaration fs)
        {
            var fnName = fs.Name.Lexeme;
            var fn = new LoxFunction(_environmentContext, fnName, fs.FuncExpr);
            _environmentContext.Environment.Define(fnName, fn);
            return Unit.Value;
        }

        #endregion

        private void ExecuteBlock(IEnumerable<Stmt> statements)
        {
            Environment.ExecuteInScope(_environmentContext, () => 
            {
                foreach (var stmt in statements)
                {
                    stmt.Accept(this);
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