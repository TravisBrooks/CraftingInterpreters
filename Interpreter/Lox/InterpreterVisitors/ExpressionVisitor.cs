using Lox.Callable;
using Lox.Exception;
using static Lox.TokenType;

namespace Lox.InterpreterVisitors
{
    public class ExpressionVisitor : IVisitor<Expr, object?>
    {
        private readonly EnvironmentContext _environmentContext;
        private readonly ResolvedExpressions _resolvedExpressions;
        private readonly Func<DeclarationVisitor> _declarationVisitorFn;

        public ExpressionVisitor(
            EnvironmentContext environmentContext,
            ResolvedExpressions resolvedExpressions,
            Func<DeclarationVisitor> declarationVisitorFnFn)
        {
            _environmentContext = environmentContext;
            _resolvedExpressions = resolvedExpressions;
            _declarationVisitorFn = declarationVisitorFnFn;
        }

        public object? Visit(Expr expr)
        {
            return expr switch
            {
                LiteralExpr literalExpr => Visit(literalExpr),
                GroupingExpr groupingExpr => Visit(groupingExpr),
                UnaryExpr unaryExpr => Visit(unaryExpr),
                BinaryExpr binaryExpr => Visit(binaryExpr),
                VariableExpr variableExpr => Visit(variableExpr),
                AssignExpr assignExpr => Visit(assignExpr),
                LogicalExpr logicalExpr => Visit(logicalExpr),
                CallExpr callExpr => Visit(callExpr),
                GetExpr getExpr => Visit(getExpr),
                SetExpr setExpr => Visit(setExpr),
                ThisExpr thisExpr => Visit(thisExpr),
                FunExpr funExpr => Visit(funExpr),
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

        private object? Visit(LiteralExpr literalExpr)
        {
            return literalExpr.Value;
        }

        private object? Visit(GroupingExpr groupingExpr)
        {
            return Evaluate(groupingExpr.Expression);
        }

        private object? Visit(UnaryExpr u)
        {
            var right = Evaluate(u.Right);
            return u.Operator.TokenType switch
            {
                BANG => !IsTruthy(right),
                MINUS => CheckOperandIsNumber(u.Operator, right, d => -d),
                _ => throw new RuntimeException(u.Operator, $"Unknown unary operator: {u.Operator.TokenType}")
            };
        }

        private object? Visit(BinaryExpr b)
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

        private object? Visit(VariableExpr ve)
        {
            var distance = _resolvedExpressions.GetDistance(ve);
            if (distance is null)
            {
                return _environmentContext.Environment.GetGlobal(ve.Name);
            }

            return _environmentContext.Environment.GetAt((int)distance!, ve.Name);
        }

        private object? Visit(AssignExpr expr)
        {
            var val = Evaluate(expr.Value);
            var distance = _resolvedExpressions.GetDistance(expr);
            if (distance is not null)
            {
                _environmentContext.Environment.AssignAt((int)distance, expr.Name, val);
            }
            else
            {
                _environmentContext.Environment.AssignGlobal(expr.Name, val);
            }
            return val;
        }

        private object? Visit(LogicalExpr logical)
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

        private object? Visit(CallExpr call)
        {
            var callee = Evaluate(call.Callee);
            var arguments = call.Arguments.Select(Evaluate).ToList();
            if (callee is ICallable fn)
            {
                if (arguments.Count != fn.Arity())
                {
                    throw new RuntimeException(call.Paren, $"Expected {fn.Arity()} arguments but got {arguments.Count}.");
                }
                return fn.Call(_declarationVisitorFn(), arguments);
            }
            throw new RuntimeException(call.Paren, "Can only call functions and classes.");
        }

        private object? Visit(GetExpr getExpr)
        {
            var obj = Evaluate(getExpr.Object);
            if (obj is LoxInstance instance)
            {
                return instance.Get(getExpr.Name);
            }
            throw new RuntimeException(getExpr.Name, "Only instances have properties.");
        }

        private object? Visit(SetExpr setExpr)
        {
            var obj = Evaluate(setExpr.Object);
            if (obj is not LoxInstance instance)
            {
                throw new RuntimeException(setExpr.Name, "Only instances have fields.");
            }
            var value = Evaluate(setExpr.Value);
            instance.Set(setExpr.Name, value);
            return value;
        }

        private object? Visit(ThisExpr thisExpr)
        {
            var distance = _resolvedExpressions.GetDistance(thisExpr);
            if (distance is null)
            {
                throw new RuntimeException(thisExpr.Keyword, "Cannot use 'this' outside of a class.");
            }
            return _environmentContext.Environment.GetAt((int)distance!, thisExpr.Keyword);
        }

        private LoxFunction Visit(FunExpr funExpr)
        {
            return new LoxFunction(_environmentContext, null, funExpr, isInitializer: false);
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