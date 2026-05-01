namespace TreeWalk.InterpreterVisitors
{
    public class StatementVisitor : IVisitor<Unit>
    {
        private readonly ExpressionVisitor _expressionVisitor = new();

        public Unit Visit(IAstNode node)
        {
            return node switch
            {
                ExprStatement es => _ExprStmtVisitor(es.Expression),
                PrintStatement ps => _PrintStmtVisitor(ps.Expression),
                VarStatement vs => _VarStmtVisitor(vs.Name, vs.Initializer),
                BlockStatement bs => _BlockStmtVisitor(bs),
                _ => throw new LoxRuntimeError(null, $"Unknown statement type: {node.GetType().Name}")
            };
        }

        private Unit _ExprStmtVisitor(Expr expr)
        {
            _ = _expressionVisitor.Evaluate(expr);
            return Unit.Value;
        }

        private Unit _PrintStmtVisitor(Expr expr)
        {
            var val = _expressionVisitor.Evaluate(expr);
            Console.WriteLine(_Stringify(val));
            return Unit.Value;
        }

        private Unit _VarStmtVisitor(Token name, Expr? initializer)
        {
            object? val = null;
            if (initializer is not null)
            {
                val = _expressionVisitor.Evaluate(initializer);
            }

            Environment.GlobalMemory.Define(name.Lexeme, val);
            return Unit.Value;
        }

        private Unit _BlockStmtVisitor(BlockStatement bs)
        {
            ExecuteBlock(bs.Statements, new Environment(Environment.GlobalMemory));
            return Unit.Value;
        }

        private void ExecuteBlock(IEnumerable<Stmt> statements, Environment environment)
        {
            // This hack caches the environment on the call stack, then pops them off again as the scope of the statement gets unwound.
            var prevEnv = Environment.GlobalMemory;
            try
            {
                Environment.GlobalMemory = environment;
                foreach (var stmt in statements)
                {
                    stmt.Accept(this);
                }
            }
            finally
            {
                Environment.GlobalMemory = prevEnv;
            }
        }

        private static string _Stringify(object? o)
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