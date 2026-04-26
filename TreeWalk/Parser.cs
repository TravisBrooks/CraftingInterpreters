using static TreeWalk.TokenType;

namespace TreeWalk
{
    // This is the Lox grammar:
    // expression     → equality ;
    // equality       → comparison(( "!=" | "==" ) comparison )* ;
    // comparison     → term(( ">" | ">=" | "<" | "<=" ) term )* ;
    // term           → factor(( "-" | "+" ) factor )* ;
    // factor         → unary(( "/" | "*" ) unary )* ;
    // unary          → ( "!" | "-" ) unary | primary ;
    // primary        → NUMBER | STRING | "true" | "false" | "nil | "(" expression ")" ;
    public class Parser
    {
        private readonly List<Token> _tokens;
        private int _current;

        public Parser(IEnumerable<Token> tokens)
        {
            _tokens = tokens.ToList();
            _current = 0;
        }

        public Expr? Parse()
        {
            try
            {
                return Expression();
            }
            catch (ParseError)
            {
                return null;
            }
        }

        #region Lox grammar implementation

        // expression → equality ;
        private Expr Expression()
        {
            return Equality();
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

        // primary → NUMBER | STRING | "true" | "false" | "nil | "(" expression ")" ;
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

            if (Match(LEFT_PAREN))
            {
                var expr = Expression();
                Consume(RIGHT_PAREN, "Expect ')' after expression.");
                return new Grouping(expr);
            }
            throw Error(Peek(), "Expect expression.");
        }

        /// <summary>
        /// All the grammar rules besides Expression, Unary, Primary follow a repetitive pattern that is captured here.
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

        #endregion

        #region Parser utilities

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

        private void Consume(TokenType expectedTokenType, string errorMessage)
        {
            if (Check(expectedTokenType))
            {
                Advance();
                return;
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