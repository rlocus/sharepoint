using System.Linq.Expressions;
using SPCore.Search.Linq.Interfaces;

namespace SPCore.Search.Linq.Operations.Results
{
    public class ExpressionOperationResult : IOperationResult
    {
        protected Expression Expression;

        public ExpressionOperationResult()
        {}

        public ExpressionOperationResult(Expression expression)
        {
            this.Expression = expression;
        }

        public object Value
        {
            get { return this.Expression; }
        }
    }
}
