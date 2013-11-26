
using System.Linq.Expressions;

namespace SPCore.Search.Linq.Interfaces
{
    internal interface IOperation
    {
        IOperationResult ToResult();
        Expression ToExpression();
    }
}
