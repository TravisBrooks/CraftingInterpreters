namespace Lox
{
    public class ResolvedExpressions
    {
        private readonly Dictionary<Expr, int> _locals = new();

        public void Resolve(Expr expr, int depth)
        {
            _locals[expr] = depth;
        }

        public int? GetDistance(Expr expr)
        {
            if (_locals.TryGetValue(expr, out var distance))
            {
                return distance;
            }
            return null;
        }
    }
}