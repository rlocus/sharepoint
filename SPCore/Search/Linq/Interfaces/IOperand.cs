using System.Linq.Expressions;

namespace SPCore.Search.Linq.Interfaces
{
    internal interface IOperand
    {
        Expression ToExpression();
        object GetValue();
    }
}
