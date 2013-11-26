
using System.Linq.Expressions;

namespace SPCore.Search.Linq.Interfaces
{
    internal interface ITranslatorFactory
    {
        ITranslator Create(LambdaExpression expr);
    }
}