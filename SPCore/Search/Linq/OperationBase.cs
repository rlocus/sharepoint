using System.Linq.Expressions;
using SPCore.Search.Linq.Interfaces;

namespace SPCore.Search.Linq
{
    internal abstract class OperationBase : IOperation
    {
        protected IOperationResultBuilder OperationResultBuilder;

        protected OperationBase(IOperationResultBuilder operationResultBuilder)
        {
            this.OperationResultBuilder = operationResultBuilder;
        }

        public abstract IOperationResult ToResult();
        public abstract Expression ToExpression();
    }
}


