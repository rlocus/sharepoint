using SPCore.Search.Linq.Interfaces;
using System.Linq.Expressions;

namespace SPCore.Search.Linq.Operations.OrElse
{
    internal class OrElseOperation : CompositeOperationBase
    {
        public OrElseOperation(IOperationResultBuilder operationResultBuilder,
            IOperation leftOperation, IOperation rightOperation)
            : base(operationResultBuilder, leftOperation, rightOperation)
        {
        }

        public override IOperationResult ToResult()
        {
            string result = string.Format("{0} OR {1}", this.LeftOperation.ToResult().Value,
                              this.RightOperation.ToResult().Value);
            return this.OperationResultBuilder.CreateResult(result);
        }

        public override Expression ToExpression()
        {
            var leftOperationExpr = this.GetLeftOperationExpression();
            var rightOperationExpr = this.GetRightOperationExpression();
            return Expression.OrElse(leftOperationExpr, rightOperationExpr);
        }
    }
}


