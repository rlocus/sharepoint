using SPCore.Search.Linq.Interfaces;
using SPCore.Search.Linq.Operations.Results;
using System;
using System.Linq.Expressions;

namespace SPCore.Search.Linq
{
    internal class GenericTranslator : ITranslator
    {
        private readonly IAnalyzer _analyzer;

        public GenericTranslator(IAnalyzer analyzer)
        {
            this._analyzer = analyzer;
        }

        public string TranslateWhere(LambdaExpression expr)
        {
            if (!this._analyzer.IsValid(expr))
            {
                throw new NonSupportedExpressionException(expr);
            }

            var operation = this._analyzer.GetOperation(expr);
            var result = operation.ToResult().Value;
           
            return string.Format("WHERE {0}", result);
        }

        public string TranslateOrderBy(LambdaExpression expr)
        {
            if (!this._analyzer.IsValid(expr))
            {
                throw new NonSupportedExpressionException(expr);
            }

            var operation = this._analyzer.GetOperation(expr);
            var result = (StringArrayOperationResult)operation.ToResult();

            return string.Format("ORDER BY {0}", result);
        }

        public string TranslateSelect(Expression<Func<SPManagedPropertyCollection, object[]>> expr)
        {
            if (!this._analyzer.IsValid(expr))
            {
                throw new NonSupportedExpressionException(expr);
            }

            var operation = this._analyzer.GetOperation(expr);
            var result = (StringArrayOperationResult)operation.ToResult();

            return string.Format("SELECT {0}", result);
        }
    }
}
