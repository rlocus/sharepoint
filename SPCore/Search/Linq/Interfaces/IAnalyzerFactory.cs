using System.Linq.Expressions;

namespace SPCore.Search.Linq.Interfaces
{
    internal interface IAnalyzerFactory
    {
        IAnalyzer Create(LambdaExpression expr);
    }
}
