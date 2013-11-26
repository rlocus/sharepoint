using SPCore.Search.Linq.Interfaces;
using System;
using System.Linq.Expressions;

namespace SPCore.Search.Linq
{
    internal abstract class CompositeOperationBase : OperationBase
    {
        protected IOperation LeftOperation;
        protected IOperation RightOperation;

        protected CompositeOperationBase(IOperationResultBuilder operationResultBuilder,
            IOperation leftOperation, IOperation rightOperation) :
            base(operationResultBuilder)
        {
            this.LeftOperation = leftOperation;
            this.RightOperation = rightOperation;
        }

        protected virtual Expression GetLeftOperationExpression()
        {
            if (this.LeftOperation == null)
            {
                throw new NullReferenceException("LeftOperation");
            }
            return this.LeftOperation.ToExpression();
        }

        protected virtual Expression GetRightOperationExpression()
        {
            if (this.RightOperation == null)
            {
                throw new NullReferenceException("RightOperation");
            }
            return this.RightOperation.ToExpression();
        }
    }
}
