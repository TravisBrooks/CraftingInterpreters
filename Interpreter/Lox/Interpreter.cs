using Lox.Exception;
using Lox.InterpreterVisitors;

namespace Lox
{
    public class Interpreter
    {
        private readonly DeclarationVisitor _declarationVisitor;
        private readonly ErrorLogger _errorLogger;

        public Interpreter(
            ErrorLogger errorLogger,
            DeclarationVisitor declarationVisitor)
        {
            _declarationVisitor = declarationVisitor;
            _errorLogger = errorLogger;
        }

        public void Interpret(IEnumerable<Decl> declarations)
        {
            try
            {
                foreach (var decl in declarations)
                {
                    Execute(decl);
                }
            }
            catch (RuntimeException e)
            {
                _errorLogger.ReportRuntimeError(e);
            }
        }

        private void Execute(Decl decl)
        {
            _ = decl.Accept(_declarationVisitor);
        }
    }
}