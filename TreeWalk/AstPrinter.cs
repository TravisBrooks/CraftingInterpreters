using System;
using System.Collections.Generic;
using System.Text;

namespace TreeWalk
{
    public class AstPrinter : IVisitor<string>
    {
        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }

        public string Visit(Binary expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
        }

        public string Visit(Grouping expr)
        {
            return Parenthesize("group", expr.Expression);
        }

        public string Visit(Literal expr)
        {
            if (expr.Value is null)
            {
                return "nil";
            }
            return expr.Value?.ToString() ?? string.Empty;
        }

        public string Visit(Unary expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Right);
        }

        private string Parenthesize(object name, params Expr[] expressions)
        {
            var sb = new StringBuilder();
            sb.Append('(').Append(name);
            foreach (var expr in expressions)
            {
                sb.Append(' ');
                sb.Append(expr.Accept(this));
            }
            sb.Append(')');
            return sb.ToString();
        }
    }
}
