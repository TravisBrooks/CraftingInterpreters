using TreeWalk.InterpreterVisitors;

namespace TreeWalk
{
    public class Interpreter
    {
        private readonly StatementVisitor _statementVisitor = new();

        public void Interpret(IList<Stmt> statements, LoxMode loxMode)
        {
            _statementVisitor.LoxMode = loxMode;
            try
            {
                foreach (var stmt in statements)
                {
                    Execute(stmt);
                }
            }
            catch (LoxRuntimeError e)
            {
                Lox.RuntimeError(e);
            }
        }

        private void Execute(Stmt stmt)
        {
            _ = stmt.Accept(_statementVisitor);
        }
    }
}