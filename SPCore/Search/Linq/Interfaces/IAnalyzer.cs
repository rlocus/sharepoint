using System.Linq.Expressions;

namespace SPCore.Search.Linq.Interfaces
{
    internal interface IAnalyzer
    {
        bool IsValid(LambdaExpression expr);
        IOperation GetOperation(LambdaExpression expr);
    }
}
