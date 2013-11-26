using System;
using System.Linq.Expressions;

namespace SPCore.Search.Linq.Interfaces
{
    internal interface ITranslator
    {
        string TranslateWhere(LambdaExpression expr);
        string TranslateOrderBy(LambdaExpression expr);
        string TranslateSelect(Expression<Func<SPManagedPropertyCollection, object[]>> expr);
    }
}
