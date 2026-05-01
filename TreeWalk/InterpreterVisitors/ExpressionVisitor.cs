using static TreeWalk.TokenType;

namespace TreeWalk.InterpreterVisitors
{
    public class ExpressionVisitor : IVisitor<object?>
    {
        public object? Visit(IAstNode node)
        {
            return node switch
            {
                Literal l => l.Value,
                Grouping g => Evaluate(g.Expression),
                Unary u => _VisitUnary(u),
                Binary b => _VisitBinary(b),
                Variable v => Environment.GlobalMemory.Get(v.Name),
                Assign a => _VisitAssign(a),
                _ => throw new LoxRuntimeError(null, $"Unknown expression type: {node.GetType().Name}")
            };
        }

        public object? Evaluate(Expr? expr)
        {
            if (expr is null)
            {
                throw new LoxRuntimeError(null, "The expression was nil");
            }

            return expr.Accept(this);
        }

        private object? _VisitUnary(Unary u)
        {
            var right = Evaluate(u.Right);
            if (right is not null)
            {
                return u.Operator.TokenType switch
                {
                    BANG => !_IsTruthy(right),
                    MINUS => _CheckOperandIsNumber(u.Operator, right, d => -d),
                    _ => throw new LoxRuntimeError(u.Operator, $"Unknown unary operator: {u.Operator.TokenType}")
                };
            }

            return null;
        }

        private object? _VisitBinary(Binary b)
        {
            var lhs = Evaluate(b.Left);
            var rhs = Evaluate(b.Right);
            return b.Operator.TokenType switch
            {
                PLUS => VisitBinaryPlus(lhs, rhs, b.Operator),
                MINUS => _CheckOperandsAreNumbers(b.Operator, lhs, rhs, (l, r) => l - r),
                STAR => _CheckOperandsAreNumbers(b.Operator, lhs, rhs, (l, r) => l * r),
                SLASH => _CheckOperandsAreNumbers(b.Operator, lhs, rhs, (l, r) => l / r),
                GREATER => _CheckOperandsAreNumbers(b.Operator, lhs, rhs, (l, r) => l > r),
                GREATER_EQUAL => _CheckOperandsAreNumbers(b.Operator, lhs, rhs, (l, r) => l >= r),
                LESS => _CheckOperandsAreNumbers(b.Operator, lhs, rhs, (l, r) => l < r),
                LESS_EQUAL => _CheckOperandsAreNumbers(b.Operator, lhs, rhs, (l, r) => l <= r),
                BANG_EQUAL => !_IsEqual(lhs, rhs),
                EQUAL_EQUAL => _IsEqual(lhs, rhs),
                _ => null
            };

            static object? VisitBinaryPlus(object? lhs, object? rhs, Token op)
            {
                return lhs switch
                {
                    double l when rhs is double r => l + r,
                    string ls when rhs is string rs => ls + rs,
                    _ => throw new LoxRuntimeError(op, "Operands must be two numbers or two strings.")
                };
            }
        }

        private object? _VisitAssign(Assign expr)
        {
            var val = Evaluate(expr.Value);
            Environment.GlobalMemory.Assign(expr.Name, val);
            return val;
        }

        private static bool _IsTruthy(object? obj)
        {
            // null is false, bool is its own value, and everything else is true
            return obj switch
            {
                null => false,
                bool b => b,
                _ => true
            };
        }

        private static double _CheckOperandIsNumber(Token op, object? operand, Func<double, double> unaryHandler)
        {
            if (operand is double d)
            {
                return unaryHandler(d);
            }

            throw new LoxRuntimeError(op, "Operand must be a number.");
        }

        private static T _CheckOperandsAreNumbers<T>(Token op, object? lhs, object? rhs, Func<double, double, T> binaryHandler)
        {
            if (lhs is double l && rhs is double r)
            {
                return binaryHandler(l, r);
            }

            throw new LoxRuntimeError(op, "Operands must be numbers.");
        }

        private static bool _IsEqual(object? lhs, object? rhs)
        {
            return lhs switch
            {
                null when rhs is null => true,
                null => false,
                _ => lhs.Equals(rhs)
            };
        }
    }
}