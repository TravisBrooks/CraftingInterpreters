using Lox.Callable;
using Lox.Exception;
using static Lox.TokenType;

namespace Lox.InterpreterVisitors
{
    public class ExpressionVisitor : IVisitor<Expr, object?>
    {
        private readonly Lazy<StatementVisitor> _statementVisitor = new(() => new StatementVisitor());

        public object? Visit(Expr expr)
        {
            return expr switch
            {
                Literal l => l.Value,
                Grouping g => Evaluate(g.Expression),
                Unary u => VisitUnary(u),
                Binary b => VisitBinary(b),
                Variable v => Lox.Environment.Get(v.Name),
                Assign a => VisitAssign(a),
                Logical l => VisitLogical(l),
                Call c => VisitCall(c),
                FuncExpr f => VisitFuncExpr(f),
                _ => throw new RuntimeException(null, $"Unknown expression type: {expr.GetType().Name}")
            };
        }

        public object? Evaluate(Expr? expr)
        {
            if (expr is null)
            {
                throw new RuntimeException(null, "The expression was nil");
            }

            return expr.Accept(this);
        }

        internal static bool IsTruthy(object? obj)
        {
            // null is false, bool is its own value, and everything else is true
            return obj switch
            {
                null => false,
                bool b => b,
                _ => true
            };
        }

        private object? VisitUnary(Unary u)
        {
            var right = Evaluate(u.Right);
            if (right is not null)
            {
                return u.Operator.TokenType switch
                {
                    BANG => !IsTruthy(right),
                    MINUS => CheckOperandIsNumber(u.Operator, right, d => -d),
                    _ => throw new RuntimeException(u.Operator, $"Unknown unary operator: {u.Operator.TokenType}")
                };
            }

            return null;
        }

        private object? VisitBinary(Binary b)
        {
            var lhs = Evaluate(b.Left);
            var rhs = Evaluate(b.Right);
            return b.Operator.TokenType switch
            {
                PLUS => VisitBinaryPlus(lhs, rhs, b.Operator),
                MINUS => CheckOperandsAreNumbers(b.Operator, lhs, rhs, (l, r) => l - r),
                STAR => CheckOperandsAreNumbers(b.Operator, lhs, rhs, (l, r) => l * r),
                MODULO => CheckOperandsAreNumbers(b.Operator, lhs, rhs, (l, r) => l % r),
                SLASH => CheckOperandsAreNumbers(b.Operator, lhs, rhs, (l, r) => l / r),
                GREATER => CheckOperandsAreNumbers(b.Operator, lhs, rhs, (l, r) => l > r),
                GREATER_EQUAL => CheckOperandsAreNumbers(b.Operator, lhs, rhs, (l, r) => l >= r),
                LESS => CheckOperandsAreNumbers(b.Operator, lhs, rhs, (l, r) => l < r),
                LESS_EQUAL => CheckOperandsAreNumbers(b.Operator, lhs, rhs, (l, r) => l <= r),
                BANG_EQUAL => !IsEqual(lhs, rhs),
                EQUAL_EQUAL => IsEqual(lhs, rhs),
                _ => null
            };

            static object? VisitBinaryPlus(object? lhs, object? rhs, Token op)
            {
                return lhs switch
                {
                    double ls when rhs is double rs => ls + rs,
                    string ls when rhs is string rs => ls + rs,
                    double ls when rhs is string rs => ls + rs,
                    string ls when rhs is double rs => ls + rs,
                    _ => throw new RuntimeException(op, "Operands must be numbers or strings.")
                };
            }
        }

        private object? VisitAssign(Assign expr)
        {
            var val = Evaluate(expr.Value);
            Lox.Environment.Assign(expr.Name, val);
            return val;
        }

        private object? VisitLogical(Logical logical)
        {
            var left = Evaluate(logical.Left);
            if (logical.Operator.TokenType == OR)
            {
                if (IsTruthy(left))
                {
                    return left;
                }
            }
            else
            {
                if (!IsTruthy(left))
                {
                    return left;
                }
            }

            return Evaluate(logical.Right);
        }

        private object? VisitCall(Call call)
        {
            var callee = Evaluate(call.Callee);
            var arguments = call.Arguments.Select(Evaluate).ToList();
            if (callee is ICallable fn)
            {
                if (arguments.Count != fn.Arity())
                {
                    throw new RuntimeException(call.Paren, $"Expected {fn.Arity()} arguments but got {arguments.Count}.");
                }
                return fn.Call(_statementVisitor.Value, arguments);
            }
            throw new RuntimeException(call.Paren, "Can only call functions and classes.");
        }

        private object? VisitFuncExpr(FuncExpr funcExpr)
        {
            return new LoxFunction(null, funcExpr);
        }

        private static double CheckOperandIsNumber(Token op, object? operand, Func<double, double> unaryHandler)
        {
            if (operand is double d)
            {
                return unaryHandler(d);
            }

            throw new RuntimeException(op, "Operand must be a number.");
        }

        private static T CheckOperandsAreNumbers<T>(Token op, object? lhs, object? rhs, Func<double, double, T> binaryHandler)
        {
            if (lhs is double l && rhs is double r)
            {
                return binaryHandler(l, r);
            }

            throw new RuntimeException(op, "Operands must be numbers.");
        }

        private static bool IsEqual(object? lhs, object? rhs)
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