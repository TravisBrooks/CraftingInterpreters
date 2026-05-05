using Lox.Exception;
using Lox.InterpreterVisitors;

namespace Lox
{
    public class Interpreter
    {
        private readonly StatementVisitor _statementVisitor = new();

        public void Interpret(IEnumerable<Stmt> statements)
        {
            try
            {
                foreach (var stmt in statements)
                {
                    Execute(stmt);
                }
            }
            catch (RuntimeException e)
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