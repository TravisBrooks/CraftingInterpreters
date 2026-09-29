using Lox.Exception;

namespace Lox
{
    public class Resolver : IVisitor<IAstNode, Unit>
    {
        private readonly ErrorLogger _errLogger;
        private readonly ResolvedExpressions _resolvedExpressions;
        private readonly Stack<Dictionary<string, bool>> _scopes = new();
        private FunctionType _currentFunction = FunctionType.NONE;

        public Resolver(ErrorLogger errLogger, ResolvedExpressions resolvedExpressions)
        {
            _errLogger = errLogger;
            _resolvedExpressions = resolvedExpressions;
        }

        public void Resolve(IEnumerable<Decl> declarations)
        {
            foreach (var decl in declarations)
            {
                Resolve(decl);
            }
        }

        public Unit Visit(IAstNode node)
        {
            return node switch
            {
                // Declarations
                VarDecl varDecl => VisitVarDecl(varDecl),
                FunDecl funDecl => VisitFunDecl(funDecl),

                // Statements
                ExprStatement es => VisitExprStatement(es),
                PrintStatement ps => VisitPrintStatement(ps),
                BlockStatement bs => VisitBlockStatement(bs),
                IfStatement i => VisitIfStatement(i),
                WhileStatement ws => VisitWhileStatement(ws),
                ForStmt fs => ForStmtVisitor(fs),
                BreakStatement => Unit.Value,
                ContinueStatement => Unit.Value,
                ReturnStatement rs => VisitReturnStatement(rs),

                // Expressions
                LiteralExpr => Unit.Value,
                GroupingExpr g => VisitGroupingExpr(g),
                UnaryExpr u => VisitUnary(u),
                BinaryExpr b => VisitBinaryExpr(b),
                VariableExpr v => VisitVariableExpr(v),
                AssignExpr a => VisitAssign(a),
                LogicalExpr l => VisitLogical(l),
                CallExpr c => VisitCall(c),
                FunExpr fe => ResolveFunction(fe, FunctionType.FUNCTION),

                _ => throw new RuntimeException(null, $"Unknown AST node type: {node.GetType().Name}")
            };
        }

        private Unit VisitVarDecl(VarDecl varDecl)
        {
            Declare(varDecl.Name);
            if (varDecl.Initializer is not null)
            {
                Resolve(varDecl.Initializer);
            }
            Define(varDecl.Name);
            return Unit.Value;
        }

        private Unit VisitFunDecl(FunDecl funDecl)
        {
            Declare(funDecl.Name);
            Define(funDecl.Name);
            return ResolveFunction(funDecl.FunExpr, FunctionType.FUNCTION);
        }

        private Unit VisitExprStatement(ExprStatement es)
        {
            Resolve(es.Expression);
            return Unit.Value;
        }

        private Unit VisitPrintStatement(PrintStatement ps)
        {
            Resolve(ps.Expression);
            return Unit.Value;
        }

        private Unit VisitBlockStatement(BlockStatement bs)
        {
            BeginScope();
            Resolve(bs.Declarations);
            EndScope();
            return Unit.Value;
        }

        private Unit VisitIfStatement(IfStatement ifStatement)
        {
            Resolve(ifStatement.Condition);
            Resolve(ifStatement.ThenBranch);
            if (ifStatement.ElseBranch is not null)
            {
                Resolve(ifStatement.ElseBranch);
            }
            return Unit.Value;
        }

        private Unit VisitWhileStatement(WhileStatement ws)
        {
            Resolve(ws.Condition);
            Resolve(ws.Body);
            return Unit.Value;
        }

        private Unit ForStmtVisitor(ForStmt fs)
        {
            var createdScope = false;
            if (fs.Initializer is VarDecl)
            {
                BeginScope();
                createdScope = true;
            }
            if (fs.Initializer is not null)
            {
                Resolve(fs.Initializer);
            }
            if (fs.Condition is not null)
            {
                Resolve(fs.Condition);
            }
            if (fs.Increment is not null)
            {
                Resolve(fs.Increment);
            }
            Resolve(fs.Body);
            if (createdScope)
            {
                EndScope();
            }
            return Unit.Value;
        }

        private Unit VisitReturnStatement(ReturnStatement rs)
        {
            if (_currentFunction == FunctionType.NONE)
            {
                _errLogger.ReportError(rs.Keyword, "Can't return from top-level code.");
            }
            if (rs.Value is not null)
            {
                Resolve(rs.Value);
            }
            return Unit.Value;
        }

        private Unit VisitGroupingExpr(GroupingExpr groupingExpr)
        {
            Resolve(groupingExpr.Expression);
            return Unit.Value;
        }

        private Unit VisitUnary(UnaryExpr unaryExpr)
        {
            Resolve(unaryExpr.Right);
            return Unit.Value;
        }

        private Unit VisitBinaryExpr(BinaryExpr binaryExpr)
        {
            Resolve(binaryExpr.Left);
            Resolve(binaryExpr.Right);
            return Unit.Value;
        }

        private Unit VisitVariableExpr(VariableExpr ve)
        {
            if (_scopes.Count > 0)
            {
                var scope = _scopes.Peek();
                if (scope.TryGetValue(ve.Name.Lexeme, out var isVeDefined) && !isVeDefined)
                {
                    _errLogger.ReportError(ve.Name, "Can't read local variable in its own initializer.");
                }
            }

            ResolveLocal(ve, ve.Name);
            return Unit.Value;
        }

        private Unit VisitAssign(AssignExpr ae)
        {
            Resolve(ae.Value);
            ResolveLocal(ae, ae.Name);
            return Unit.Value;
        }

        private Unit VisitLogical(LogicalExpr logicalExpr)
        {
            Resolve(logicalExpr.Left);
            Resolve(logicalExpr.Right);
            return Unit.Value;
        }

        private Unit VisitCall(CallExpr callExpr)
        {
            Resolve(callExpr.Callee);
            foreach (var arg in callExpr.Arguments)
            {
                Resolve(arg);
            }
            return Unit.Value;
        }

        private void Declare(Token name)
        {
            if (_scopes.Count == 0)
            {
                return;
            }
            var scope = _scopes.Peek();
            if (scope.ContainsKey(name.Lexeme))
            {
                _errLogger.ReportError(name, "Already a variable with this name in this scope.");
            }
            scope[name.Lexeme] = false;
        }

        private void Define(Token name)
        {
            if (_scopes.Count == 0)
            {
                return;
            }
            var scope = _scopes.Peek();
            scope[name.Lexeme] = true;
        }

        private void Resolve(Decl decl)
        {
            decl.Accept(this);
        }

        private void Resolve(Stmt stmt)
        {
            stmt.Accept(this);
        }

        private void Resolve(Expr expr)
        {
            expr.Accept(this);
        }

        private void ResolveLocal(Expr expr, Token name)
        {
            var depth = 0;
            foreach (var scope in _scopes)
            {
                if (scope.ContainsKey(name.Lexeme))
                {
                    _resolvedExpressions.Resolve(expr, depth);
                    return;
                }
                depth++;
            }
        }

        private Unit ResolveFunction(FunExpr funExpr, FunctionType functionType)
        {
            var enclosingFunction = _currentFunction;
            _currentFunction = functionType;
            BeginScope();
            foreach (var param in funExpr.Parameters)
            {
                Declare(param);
                Define(param);
            }
            Resolve(funExpr.Body);
            EndScope();
            _currentFunction = enclosingFunction;
            return Unit.Value;
        }

        private void BeginScope()
        {
            _scopes.Push(new Dictionary<string, bool>());
        }

        private void EndScope()
        {
            _scopes.Pop();
        }

        private enum FunctionType
        {
            NONE,
            FUNCTION
        }
    }
}