namespace TreeWalk.InterpreterVisitors
{
    public class StatementVisitor : IVisitor<Stmt, Unit>
    {
        private readonly ExpressionVisitor _expressionVisitor = new();

        public Unit Visit(Stmt stmt)
        {
            return stmt switch
            {
                ExprStatement es => ExprStmtVisitor(es.Expression),
                PrintStatement ps => PrintStmtVisitor(ps.Expression),
                VarStatement vs => VarStmtVisitor(vs.Name, vs.Initializer),
                BlockStatement bs => BlockStmtVisitor(bs),
                IfStatement i => IfStmtVisitor(i),
                WhileStatement w => WhileStmtVisitor(w),
                _ => throw new LoxRuntimeError(null, $"Unknown statement type: {stmt.GetType().Name}")
            };
        }

        #region Vistor implementations

        private Unit ExprStmtVisitor(Expr expr)
        {
            var exprVal = _expressionVisitor.Evaluate(expr);
            if (Lox.GlobalEnvironment.LoxMode == LoxMode.INTERACTIVE_MODE)
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

            Lox.GlobalEnvironment.Define(name.Lexeme, val);
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
                whileStatement.Body.Accept(this);
            }
            return Unit.Value;
        }

        #endregion

        private void ExecuteBlock(IEnumerable<Stmt> statements)
        {
            try
            {
                Lox.GlobalEnvironment.EnterInnerScope();
                foreach (var stmt in statements)
                {
                    stmt.Accept(this);
                }
            }
            finally
            {
                Lox.GlobalEnvironment.ExitInnerScope();
            }
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