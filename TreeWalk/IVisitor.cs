using System;
using System.Collections.Generic;
using System.Text;

namespace TreeWalk
{
    public interface IVisitor<T>
    {
        T Visit(Expr e);
    }
}
