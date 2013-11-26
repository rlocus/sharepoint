using SPCore.Search.Linq.Interfaces;
using System.Linq.Expressions;

namespace SPCore.Search.Linq.Operations.AndAlso
{
    internal class AndAlsoOperation : CompositeOperationBase
    {
        public AndAlsoOperation(IOperationResultBuilder operationResultBuilder,
            IOperation leftOperation, IOperation rightOperation)
            : base(operationResultBuilder, leftOperation, rightOperation)
        {
        }

        public override IOperationResult ToResult()
        {
            //var result = new XElement(Tags.And,
            //                 this.LeftOperation.ToResult().Value,
            //                 this.RightOperation.ToResult().Value);
            string result = string.Format("{0} AND {1}", this.LeftOperation.ToResult().Value,
                                          this.RightOperation.ToResult().Value);
            
            return this.OperationResultBuilder.CreateResult(result);
        }

        public override Expression ToExpression()
        {
            var leftOperationExpr = this.GetLeftOperationExpression();
            var rightOperationExpr = this.GetRightOperationExpression();
            return Expression.AndAlso(leftOperationExpr, rightOperationExpr);
        }
    }
}


