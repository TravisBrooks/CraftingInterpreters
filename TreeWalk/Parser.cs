using static Lox.TokenType;

namespace Lox
{
    // This is the first Lox grammar:
    // expression     → equality ;
    // equality       → comparison(( "!=" | "==" ) comparison )* ;
    // comparison     → term(( ">" | ">=" | "<" | "<=" ) term )* ;
    // term           → factor(( "-" | "+" ) factor )* ;
    // factor         → unary(( "/" | "*" ) unary )* ;
    // unary          → ( "!" | "-" ) unary | primary ;
    // primary        → NUMBER | STRING | "true" | "false" | "nil | "(" expression ")" ;
    //
    // In chpt 8 (Statements and State) new grammar rules are introduced:
    // program        → statement* EOF;
    // declaration    → varDecl | statement ;
    // statement      → exprStmt | printStmt | block | ifStmt | whileStmt | forStmt;
    // exprStmt       → expression ";" ;
    // printStmt      → "print" expression ";" ;
    // block          → "{" declaration* "}" ;
    // ifStmt         → "if" "(" expression ")" statement ( "else" statement )? ;
    // whileStmt      → "while" "(" expression ")" statement ;
    // forStmt        → "for" "(" ( varDecl | exprStmt | ";" ) expression? ";" expression? ")" statement ;
    public class Parser
    {
        private readonly List<Token> _tokens;
        private int _current;

        public Parser(IEnumerable<Token> tokens)
        {
            _tokens = tokens.ToList();
            _current = 0;
        }

        public IEnumerable<Stmt> Parse()
        {
            var statements = new List<Stmt>();
            while (!IsAtEnd())
            {
                var decl = Declaration();
                if (decl is not null)
                {
                    statements.Add(decl);
                }
            }

            return statements;
        }

        #region Lox grammar implementation

        // declaration → varDecl | statement ;
        private Stmt? Declaration()
        {
            try
            {
                return Match(VAR) ? VarDeclaration() : Statement();
            }
            catch (ParseError)
            {
                Synchronize();
                return null;
            }
        }

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
                if (expr is Variable variable)
                {
                    var name = variable.Name;
                    return new Assign(name, value);
                }

                Error(equals, "Invalid assignment target.");
            }

            return expr;
        }

        // equality → comparison(( "!=" | "==" ) comparison )* ;
        private Expr Equality()
        {
            return RecursiveBinaryExprBuilder(Comparison, BANG_EQUAL, EQUAL_EQUAL);
        }

        // logic_or → logic_and ( "or" logic_and )* ;
        private Expr LogicOr()
        {
            var expr = LogicAnd();
            while (Match(OR))
            {
                var op = Previous();
                var rightExpr = LogicAnd();
                expr = new Logical(expr, op, rightExpr);
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
                expr = new Logical(expr, op, rightExpr);
            }
            return expr;
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

        // factor → unary(( "/" | "*" ) unary )* ;
        private Expr Factor()
        {
            return RecursiveBinaryExprBuilder(Unary, SLASH, STAR);
        }

        // unary → ( "!" | "-" ) unary | primary ;
        private Expr Unary()
        {
            if (Match(BANG, MINUS))
            {
                var op = Previous();
                var rightExpr = Unary();
                return new Unary(op, rightExpr);
            }

            return Primary();
        }

        // primary → NUMBER | STRING | "true" | "false" | "nil | "(" expression ")" | IDENTIFIER ;
        private Expr Primary()
        {
            if (Match(FALSE))
            {
                return new Literal(false);
            }

            if (Match(TRUE))
            {
                return new Literal(true);
            }

            if (Match(NIL))
            {
                return new Literal(null);
            }

            if (Match(NUMBER, STRING))
            {
                return new Literal(Previous().Literal);
            }

            if (Match(IDENTIFIER))
            {
                return new Variable(Previous());
            }

            if (Match(LEFT_PAREN))
            {
                var expr = Expression();
                _ = Consume(RIGHT_PAREN, "Expect ')' after expression.");
                return new Grouping(expr);
            }

            throw Error(Peek(), "Expect expression.");
        }

        // varDecl → "var" IDENTIFIER ( "=" expression )? ";" ;
        private Stmt VarDeclaration()
        {
            var name = Consume(IDENTIFIER, "Expect variable name.");
            Expr? initializer = null;
            if (Match(EQUAL))
            {
                initializer = Expression();
            }

            _ = Consume(SEMICOLON, "Expect ';' after variable declaration.");
            return new VarStatement(name, initializer);
        }

        // statement → exprStmt | printStmt | block ;
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

            return Match(LEFT_BRACE) ? BlockStatement() : ExpressionStatement();
        }

        // forStmt → "for" "(" ( varDecl | exprStmt | ";" ) expression? ";" expression? ")" statement ;        
        private Stmt ForStatement()
        {
            Consume(LEFT_PAREN, "Expect '(' after 'for'.");
            Stmt? initializer;
            if(Match(SEMICOLON))
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

            Expr? condition = null;
            if (!Check(RIGHT_PAREN))
            {
                condition = Expression();
            }
            Consume(SEMICOLON, "Expect ';' after loop condition.");
            Expr? increment = null;
            if (!Check(RIGHT_PAREN))
            {
                increment = Expression();
            }
            Consume(RIGHT_PAREN, "Expect ')' after for clauses.");
            var body = Statement();
            if (increment is not null)
            {
                body = new BlockStatement(new List<Stmt> { body, new ExprStatement(increment) });
            }

            condition ??= new Literal(true);
            body = new WhileStatement(condition, body);

            if (initializer is not null)
            {
                body = new BlockStatement(new List<Stmt> { initializer, body });
            }

            return body;
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

        // printStmt → "print" expression ";" ;
        private PrintStatement PrintStatement()
        {
            var val = Expression();
            _ = Consume(SEMICOLON, "Expect ';' after value.");
            return new PrintStatement(val);
        }

        // whileStmt → "while" "(" expression ")" statement ;
        private WhileStatement WhileStatement()
        {
            Consume(LEFT_PAREN, "Expect '(' after 'while'.");
            var condition = Expression();
            Consume(RIGHT_PAREN, "Expect ')' after condition.");
            var body = Statement();
            return new WhileStatement(condition, body);
        }

        // block → "{" declaration* "}" ;
        private BlockStatement BlockStatement()
        {
            var statements = new List<Stmt>();
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

        // exprStmt → expression ";" ;
        private ExprStatement ExpressionStatement()
        {
            var val = Expression();
            _ = Consume(SEMICOLON, "Expect ';' after value.");
            return new ExprStatement(val);
        }

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
                expr = new Binary(expr, op, rightExpr);
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

        private static ParseError Error(Token token, string errorMessage)
        {
            Lox.Error(token, errorMessage);
            return new ParseError();
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