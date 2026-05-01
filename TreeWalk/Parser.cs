using static TreeWalk.TokenType;

namespace TreeWalk
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
    // statement      → exprStmt | printStmt | block ;
    // exprStmt       → expression ";" ;
    // printStmt      → "print" expression ";" ;
    // block          → "{" declaration* "}" ;
    public class Parser
    {
        private readonly List<Token> _tokens;
        private int _current;

        public Parser(IEnumerable<Token> tokens)
        {
            _tokens = tokens.ToList();
            _current = 0;
        }

        public IList<Stmt> Parse()
        {
            var statements = new List<Stmt>();
            while (!IsAtEnd())
            {
                statements.Add(Declaration());
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

        // expression → equality ;
        private Expr Expression()
        {
            return Assignment();
        }

        // assignment → IDENTIFIER "=" assignment | equality ;
        private Expr Assignment()
        {
            var expr = Equality();
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
            if (Match(PRINT))
            {
                return PrintStatement();
            }

            if (Match(LEFT_BRACE))
            {
                return BlockStatement();
            }

            return ExpressionStatement();
        }

        // printStmt → "print" expression ";" ;
        private Stmt PrintStatement()
        {
            var val = Expression();
            _ = Consume(SEMICOLON, "Expect ';' after value.");
            return new PrintStatement(val);
        }

        // exprStmt → expression ";" ;
        private Stmt ExpressionStatement()
        {
            var val = Expression();
            _ = Consume(SEMICOLON, "Expect ';' after value.");
            return new ExprStatement(val);
        }

        private Stmt BlockStatement()
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
            foreach (var t in types)
            {
                if (Check(t))
                {
                    Advance();
                    return true;
                }
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
        ///     If there was a grammatical error throw away tokens until we think we're at the start of the next statement.
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