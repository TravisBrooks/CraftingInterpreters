using Lox.Callable;
using Lox.Exception;

namespace Lox.InterpreterVisitors
{
    public class StatementVisitor : IVisitor<Stmt, Unit>
    {
        private readonly ExpressionVisitor _expressionVisitor;

        public StatementVisitor()
        {
            _expressionVisitor = new ExpressionVisitor(this);
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
            if (Lox.EnvironmentContext.Environment.LoxMode == LoxMode.INTERACTIVE_MODE)
            {
                Console.WriteLine(Stringify(exprVal));
            }

            return Unit.Value;
        }

        private Unit PrintStmtVisitor(Expr expr)
        {
            var val = _expressionVisitor.Evaluate(expr);
            Console.WriteLine(Stringify(val));
            return Unit.Value;
        }

        private Unit VarStmtVisitor(Token name, Expr? initializer)
        {
            object? val = null;
            if (initializer is not null)
            {
                val = _expressionVisitor.Evaluate(initializer);
            }

            Lox.EnvironmentContext.Environment.Define(name.Lexeme, val);
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

        private static Unit FuncStmtVisitor(FunctionDeclaration fs)
        {
            var fnName = fs.Name.Lexeme;
            var fn = new LoxFunction(fnName, fs.FuncExpr);
            Lox.EnvironmentContext.Environment.Define(fnName, fn);
            return Unit.Value;
        }

        #endregion

        private void ExecuteBlock(IEnumerable<Stmt> statements)
        {
            Environment.ExecuteInScope(Lox.EnvironmentContext, () => 
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