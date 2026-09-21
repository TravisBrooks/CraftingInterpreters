using Lox.Callable;
using Lox.Exception;
using static Lox.TokenType;

namespace Lox.InterpreterVisitors
{
    public class ExpressionVisitor : IVisitor<Expr, object?>
    {
        private readonly EnvironmentContext _environmentContext;
        private readonly Func<StatementVisitor> _statementVisitorFn;

        public ExpressionVisitor(
            EnvironmentContext environmentContext,
            Func<StatementVisitor> statementVisitorFnFn)
        {
            _environmentContext = environmentContext;
            _statementVisitorFn = statementVisitorFnFn;
        }

        public object? Visit(Expr expr)
        {
            return expr switch
            {
                LiteralExpr l => l.Value,
                GroupingExpr g => Evaluate(g.Expression),
                UnaryExpr u => VisitUnary(u),
                BinaryExpr b => VisitBinary(b),
                VariableExpr v => VisitVariableExpr(v),
                AssignExpr a => VisitAssign(a),
                LogicalExpr l => VisitLogical(l),
                CallExpr c => VisitCall(c),
                FunExpr f => VisitFuncExpr(f),
                _ => throw new RuntimeException(null, $"Unknown expression type: {expr.GetType().Name}")
            };
        }

        public object? Evaluate(Expr? expr)
        {
            return expr?.Accept(this);
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

        private object? VisitUnary(UnaryExpr u)
        {
            var right = Evaluate(u.Right);
            return u.Operator.TokenType switch
            {
                BANG => !IsTruthy(right),
                MINUS => CheckOperandIsNumber(u.Operator, right, d => -d),
                _ => throw new RuntimeException(u.Operator, $"Unknown unary operator: {u.Operator.TokenType}")
            };
        }

        private object? VisitBinary(BinaryExpr b)
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
                // I found it convenient to be able to concatenate strings and numbers, which differs slightly from the book, but
                // I didn't go overboard with it to follow C#'s pattern of silently calling ToString() on anything to force it into a string
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

        private object? VisitVariableExpr(VariableExpr ve)
        {
            return _environmentContext.Environment.Get(ve.Name);
        }

        private object? VisitAssign(AssignExpr expr)
        {
            var val = Evaluate(expr.Value);
            _environmentContext.Environment.Assign(expr.Name, val);
            return val;
        }

        private object? VisitLogical(LogicalExpr logical)
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

        private object? VisitCall(CallExpr call)
        {
            var callee = Evaluate(call.Callee);
            var arguments = call.Arguments.Select(Evaluate).ToList();
            if (callee is ICallable fn)
            {
                if (arguments.Count != fn.Arity())
                {
                    throw new RuntimeException(call.Paren, $"Expected {fn.Arity()} arguments but got {arguments.Count}.");
                }
                return fn.Call(_statementVisitorFn(), arguments);
            }
            throw new RuntimeException(call.Paren, "Can only call functions and classes.");
        }

        private LoxFunction VisitFuncExpr(FunExpr funExpr)
        {
            return new LoxFunction(_environmentContext, null, funExpr);
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

                // Matches if it's a double AND is NaN
                double.NaN => false,
                _ when rhs is double.NaN => false,

                _ => lhs.Equals(rhs)
            };
        }
    }
}