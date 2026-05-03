using System.Text;

namespace TreeWalk
{
    public class AstPrinter : IVisitor<IAstNode, string>
    {
        public string Visit(IAstNode node)
        {
            return node switch
            {
                Binary b => Parenthesize(b.Operator.Lexeme, b.Left, b.Right),
                Grouping g => Parenthesize("group", g.Expression),
                Literal l => l.Value switch
                {
                    null => "nil",
                    _ => l.Value.ToString() ?? string.Empty
                },
                Unary u => Parenthesize(u.Operator.Lexeme, u.Right),
                _ => throw new NotImplementedException($"Unknown expression type: {node.GetType().Name}")
            };
        }

        public string Print(Expr? expr)
        {
            return expr is null ? "nil" : expr.Accept(this);
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