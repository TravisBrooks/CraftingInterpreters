using Lox.Exception;
using System.Collections.Immutable;
using static Lox.TokenType;

namespace Lox
{
    // Lox grammar (I altered the definition from the book of funDecl and added fnExpression when I added lambdas):
    //
    // Rule that matches an entire Lox program:
    // program        → declaration* EOF ;
    //
    // DECLARATIONS:
    // declaration    → funDecl | varDecl | statement ;
    // funDecl        → "fun" IDENTIFIER fnExpression ;
    // varDecl        → "var" IDENTIFIER ( "=" expression )? ";" ;
    //
    // STATEMENTS:
    // statement      → exprStmt | forStmt | ifStmt | printStmt | returnStmt | whileStmt | breakStmt | continueStmt | block ;
    // exprStmt       → expression ";" ;
    // forStmt        → "for" "(" ( varDecl | exprStmt | ";" ) expression? ";" expression? ")" statement ;
    // ifStmt         → "if" "(" expression ")" statement ( "else" statement )? ;
    // printStmt      → "print" expression ";" ;
    // returnStmt     → "return" expression? ";" ;
    // whileStmt      → "while" "(" expression ")" statement ;
    // breakStmt      → "break" ";" ;
    // continueStmt   → "continue" ";" ;
    // block          → "{" declaration* "}" ;
    //
    // EXPRESSIONS:
    // expression     → equality ;
    // assignment     → ( call "." )? IDENTIFIER "=" assignment | logic_or ;
    // equality       → comparison(( "!=" | "==" ) comparison )* ;
    // comparison     → term(( ">" | ">=" | "<" | "<=" ) term )* ;
    // term           → factor(( "-" | "+" ) factor )* ;
    // factor         → unary(( "/" | "*" | "%" ) unary )* ;
    // unary          → ( "!" | "-" ) unary | call ;
    // call           → primary ( "(" arguments? ")" )* ;
    // primary        → NUMBER | STRING | "true" | "false" | "nil | "(" expression ")" | fnExpression ;
    // arguments      → expression ( "," expression )* ;
    // fnExpression   → "(" parameters? ")" block ;
    // parameters     → IDENTIFIER ( "," IDENTIFIER )* ;
    public class Parser
    {
        public const int MaxFuncParameterCount = 255;
        private readonly List<Token> _tokens;
        private int _current;
        private readonly ErrorLogger _errorLogger;
        private int _loopDepth = 0;
        private int _functionDepth = 0;

        public Parser(ErrorLogger errorLogger, IEnumerable<Token> tokens)
        {
            _tokens = tokens.ToList();
            _current = 0;
            _errorLogger = errorLogger;
        }

        public ImmutableList<Decl> Parse()
        {
            return Program().ToImmutableList();
        }

        #region Lox grammar implementation

        // program → declaration* EOF ;
        private IEnumerable<Decl> Program()
        {
            while (!IsAtEnd())
            {
                var decl = Declaration();
                if (decl is not null)
                {
                    yield return decl;
                }
            }
        }

        #region Declarations

        // declaration → funDecl | varDecl | statement ;
        private Decl? Declaration()
        {
            try
            {
                // the match on FUN is true it advances _current so to check if the next thing is an identifier we need to peek
                if (Match(FUN) && Peek().TokenType == IDENTIFIER)
                {
                    return FunDeclaration("function");
                }
                if (Match(VAR))
                {
                    return VarDeclaration();
                }
                return Statement();
            }
            catch (ParseException)
            {
                Synchronize();
                return null;
            }
        }

        // funDecl → "fun" IDENTIFIER fnExpression ;
        private FunDecl FunDeclaration(string kind)
        {
            var name = Consume(IDENTIFIER, $"Expect {kind} name.");
            var fnExpr = FnExpression(kind);
            return new FunDecl(name, fnExpr);
        }

        // varDecl → "var" IDENTIFIER ( "=" expression )? ";" ;
        private VarDecl VarDeclaration()
        {
            var name = Consume(IDENTIFIER, "Expect variable name.");
            Expr? initializer = null;
            if (Match(EQUAL))
            {
                initializer = Expression();
            }

            _ = Consume(SEMICOLON, "Expect ';' after variable declaration.");
            return new VarDecl(name, initializer);
        }

        #endregion

        #region Statements

        // statement → exprStmt | printStmt | block | ifStmt | whileStmt | forStmt | breakStmt | continueStmt| returnStmt ;
        private Stmt Statement()
        {
            if (Match(FOR))
            {
                return ForStatement();
            }
            if (Match(IF))
            {
                return IfStatement();
            }
            if (Match(PRINT))
            {
                return PrintStatement();
            }
            if (Match(WHILE))
            {
                return WhileStatement();
            }
            if (Match(BREAK))
            {
                return BreakStatement();
            }
            if (Match(CONTINUE))
            {
                return ContinueStatement();
            }
            if (Match(RETURN))
            {
                return ReturnStatement();
            }

            return Match(LEFT_BRACE) ? BlockStatement() : ExpressionStatement();
        }

        // exprStmt → expression ";" ;
        private ExprStatement ExpressionStatement()
        {
            var val = Expression();
            _ = Consume(SEMICOLON, "Expect ';' after expression.");
            return new ExprStatement(val);
        }

        // printStmt → "print" expression ";" ;
        private PrintStatement PrintStatement()
        {
            var val = Expression();
            _ = Consume(SEMICOLON, "Expect ';' after expression.");
            return new PrintStatement(val);
        }

        // block → "{" declaration* "}" ;
        private BlockStatement BlockStatement()
        {
            var statements = new List<Decl>();
            while (!Check(RIGHT_BRACE) && !IsAtEnd())
            {
                var declaration = Declaration();
                if (declaration is not null)
                {
                    statements.Add(declaration);
                }
            }

            Consume(RIGHT_BRACE, "Expect '}' after block.");
            return new BlockStatement(statements);
        }

        // ifStmt → "if" "(" expression ")" statement ( "else" statement )? ;
        private IfStatement IfStatement()
        {
            Consume(LEFT_PAREN, "Expect '(' after 'if'.");
            var condition = Expression();
            Consume(RIGHT_PAREN, "Expect ')' after if condition.");
            var thenBranch = Statement();
            Stmt? elseBranch = null;
            if (Match(ELSE))
            {
                elseBranch = Statement();
            }
            return new IfStatement(condition, thenBranch, elseBranch);
        }

        // whileStmt → "while" "(" expression ")" statement ;
        private WhileStatement WhileStatement()
        {
            Consume(LEFT_PAREN, "Expect '(' after 'while'.");
            var condition = Expression();
            Consume(RIGHT_PAREN, "Expect ')' after condition.");
            _loopDepth++;
            try
            {
                var body = Statement();
                return new WhileStatement(condition, body);
            }
            finally
            {
                _loopDepth--;
            }
        }

        // forStmt → "for" "(" ( varDecl | exprStmt | ";" ) expression? ";" expression? ")" statement ;        
        private ForStmt ForStatement()
        {
            Consume(LEFT_PAREN, "Expect '(' after 'for'.");
            Decl? initializer;
            if (Match(SEMICOLON))
            {
                initializer = null;
            }
            else if (Match(VAR))
            {
                initializer = VarDeclaration();
            }
            else
            {
                initializer = ExpressionStatement();
            }

            Expr? condition;
            if (Match(SEMICOLON))
            {
                condition = null;
            }
            else
            {
                condition = Expression();
                Consume(SEMICOLON, "Expect ';' after loop condition.");
            }

            Expr? increment = null;
            if (!Check(RIGHT_PAREN))
            {
                increment = Expression();
            }
            Consume(RIGHT_PAREN, "Expect ')' after for clauses.");

            _loopDepth++;
            try
            {
                var body = Statement();
                return new ForStmt(initializer, condition, increment, body);
            }
            finally
            {
                _loopDepth--;
            }
        }

        // breakStmt → "break" ";" ;
        private BreakStatement BreakStatement()
        {
            var breakToken = Previous();
            if (_loopDepth == 0 || _functionDepth > 0)
            {
                throw Error(breakToken, "Cannot use 'break' outside of a loop.");
            }
            Consume(SEMICOLON, "Expect ';' after 'break'.");
            return new BreakStatement(breakToken);
        }

        // continueStmt → "continue" ";" ;
        private ContinueStatement ContinueStatement()
        {
            var continueToken = Previous();
            if (_loopDepth == 0 || _functionDepth > 0)
            {
                throw Error(continueToken, "Cannot use 'continue' outside of a loop.");
            }
            Consume(SEMICOLON, "Expect ';' after 'continue'.");
            return new ContinueStatement(continueToken);
        }

        // returnStmt → "return" expression? ";" ;
        private ReturnStatement ReturnStatement()
        {
            var keyword = Previous();
            Expr? value = null;
            if (!Check(SEMICOLON))
            {
                value = Expression();
            }
            _ = Consume(SEMICOLON, "Expect ';' after return value.");
            return new ReturnStatement(keyword, value);
        }

        #endregion

        #region Expressions

        // expression → assignment ;
        private Expr Expression()
        {
            return Assignment();
        }

        // assignment → IDENTIFIER "=" assignment | logic_or ;
        private Expr Assignment()
        {
            var expr = LogicOr();
            if (Match(EQUAL))
            {
                var equals = Previous();
                var value = Assignment();
                if (expr is VariableExpr variable)
                {
                    var name = variable.Name;
                    return new AssignExpr(name, value);
                }

                Error(equals, "Invalid assignment target.");
            }

            return expr;
        }

        // logic_or → logic_and ( "or" logic_and )* ;
        private Expr LogicOr()
        {
            var expr = LogicAnd();
            while (Match(OR))
            {
                var op = Previous();
                var rightExpr = LogicAnd();
                expr = new LogicalExpr(expr, op, rightExpr);
            }
            return expr;
        }

        // logic_and → equality ( "and" equality )* ;
        private Expr LogicAnd()
        {
            var expr = Equality();
            while (Match(AND))
            {
                var op = Previous();
                var rightExpr = Equality();
                expr = new LogicalExpr(expr, op, rightExpr);
            }
            return expr;
        }

        // equality → comparison(( "!=" | "==" ) comparison )* ;
        private Expr Equality()
        {
            return RecursiveBinaryExprBuilder(Comparison, BANG_EQUAL, EQUAL_EQUAL);
        }

        // comparison → term(( ">" | ">=" | "<" | "<=" ) term )* ;
        private Expr Comparison()
        {
            return RecursiveBinaryExprBuilder(Term, GREATER, GREATER_EQUAL, LESS, LESS_EQUAL);
        }

        // term → factor(( "-" | "+" ) factor )* ;
        private Expr Term()
        {
            return RecursiveBinaryExprBuilder(Factor, MINUS, PLUS);
        }

        // factor → unary(( "/" | "*" | "%" ) unary )* ;
        private Expr Factor()
        {
            return RecursiveBinaryExprBuilder(Unary, SLASH, STAR, MODULO);
        }

        // unary → ( "!" | "-" ) unary | primary ;
        private Expr Unary()
        {
            if (Match(BANG, MINUS))
            {
                var op = Previous();
                var rightExpr = Unary();
                return new UnaryExpr(op, rightExpr);
            }

            return Call();
        }

        // call → primary ( "(" arguments? ")" )* ;
        private Expr Call()
        {
            var expr = Primary();
            while (true)
            {
                if (Match(LEFT_PAREN))
                {
                    expr = FinishCall(expr);
                }
                else
                {
                    break;
                }
            }
            return expr;
        }

        private CallExpr FinishCall(Expr callee)
        {
            var arguments = new List<Expr>();
            if(!Check(RIGHT_PAREN))
            {
                do
                {
                    if (arguments.Count >= MaxFuncParameterCount)
                    {
                        _errorLogger.ReportError(Peek(), $"Can't have more than {MaxFuncParameterCount} arguments.");
                    }
                    arguments.Add(Expression());
                }
                while (Match(COMMA));
            }
            var paren = Consume(RIGHT_PAREN, "Expect ')' after arguments.");
            return new CallExpr(callee, paren, arguments);
        }

        // primary → NUMBER | STRING | "true" | "false" | "nil" | "(" expression ")" | IDENTIFIER | fnExpression ;
        private Expr Primary()
        {
            if (Match(FUN))
            {
                if (Check(IDENTIFIER))
                {
                    throw Error(Previous(), "Only anonymous lambda expressions are expected here.");
                }
                return FnExpression("function");
            }

            if (Match(FALSE))
            {
                return new LiteralExpr(false);
            }

            if (Match(TRUE))
            {
                return new LiteralExpr(true);
            }

            if (Match(NIL))
            {
                return new LiteralExpr(null);
            }

            if (Match(NUMBER, STRING))
            {
                return new LiteralExpr(Previous().Literal);
            }

            if (Match(IDENTIFIER))
            {
                return new VariableExpr(Previous());
            }

            if (Match(LEFT_PAREN))
            {
                var expr = Expression();
                _ = Consume(RIGHT_PAREN, "Expect ')' after expression.");
                return new GroupingExpr(expr);
            }

            throw Error(Peek(), "Expect expression.");
        }

        // fnExpression → "(" parameters? ")" block ;
        private FuncExpr FnExpression(string kind)
        {
            _ = Consume(LEFT_PAREN, $"Expect '(' after {kind} name.");
            var parameters = new List<Token>();
            if (!Check(RIGHT_PAREN))
            {
                do
                {
                    if (parameters.Count >= MaxFuncParameterCount)
                    {
                        _errorLogger.ReportError(Peek(), $"Can't have more than {MaxFuncParameterCount} parameters.");
                    }
                    parameters.Add(Consume(IDENTIFIER, "Expect parameter name."));
                }
                while (Match(COMMA));
            }
            _ = Consume(RIGHT_PAREN, $"Expect ')' after parameters.");
            _ = Consume(LEFT_BRACE, $"Expect '{{' before {kind} body.");

            _functionDepth++;
            try
            {
                var body = BlockStatement();
                return new FuncExpr(parameters, body);
            }
            finally
            {
                _functionDepth--;
            }
        }

        #endregion

        #endregion

        #region Parser utilities

        /// <summary>
        ///     All the grammar rules besides Expression, Unary, Primary follow a repetitive pattern that is captured here.
        /// </summary>
        /// <returns></returns>
        private Expr RecursiveBinaryExprBuilder(Func<Expr> childFunc, params TokenType[] matchingTokenTypes)
        {
            var expr = childFunc();
            while (Match(matchingTokenTypes))
            {
                var op = Previous();
                var rightExpr = childFunc();
                expr = new BinaryExpr(expr, op, rightExpr);
            }

            return expr;
        }

        private bool Match(params TokenType[] types)
        {
            if (types.Any(Check))
            {
                Advance();
                return true;
            }

            return false;
        }

        private bool Check(TokenType tokenType)
        {
            if (IsAtEnd())
            {
                return false;
            }

            return Peek().TokenType == tokenType;
        }

        private bool IsAtEnd()
        {
            return Peek().TokenType == EOF;
        }

        private Token Peek()
        {
            return _tokens[_current];
        }

        private Token Advance()
        {
            if (!IsAtEnd())
            {
                _current++;
            }

            return Previous();
        }

        private Token Previous()
        {
            return _tokens[_current - 1];
        }

        private Token Consume(TokenType expectedTokenType, string errorMessage)
        {
            if (Check(expectedTokenType))
            {
                return Advance();
            }

            throw Error(Peek(), errorMessage);
        }

        private ParseException Error(Token token, string errorMessage)
        {
            _errorLogger.ReportError(token, errorMessage);
            return new ParseException();
        }

        /// <summary>
        /// If there was a grammatical error throw away tokens until we think we're at the start of the next statement.
        /// </summary>
        private void Synchronize()
        {
            Advance();
            while (!IsAtEnd())
            {
                if (Previous().TokenType == SEMICOLON)
                {
                    return;
                }

                // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                switch (Peek().TokenType)
                {
                    case CLASS:
                    case FUN:
                    case VAR:
                    case FOR:
                    case IF:
                    case WHILE:
                    case PRINT:
                    case RETURN:
                        return;
                }

                Advance();
            }
        }

        #endregion
    }
}