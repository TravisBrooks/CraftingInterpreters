using System;
using System.Collections.Generic;
using System.Text;

namespace TreeWalk
{
    public interface IVisitor<T>
    {
        T Visit(Binary expr);
        T Visit(Grouping expr);
        T Visit(Literal expr);
        T Visit(Unary expr);
    }
}
