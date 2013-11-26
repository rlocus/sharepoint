using SPCore.Search.Linq.Interfaces;
using System;
using System.Linq.Expressions;

namespace SPCore.Search.Linq.Operations.IsNotNull
{
    internal class IsNotNullOperation : UnaryOperationBase
    {
        public IsNotNullOperation(IOperationResultBuilder operationResultBuilder,
            IOperand fieldRefOperand)
            : base(operationResultBuilder, fieldRefOperand)
        {
        }

        public override IOperationResult ToResult()
        {
            string result = string.Format("{0} IS NOT NULL", ColumnOperand);
            return this.OperationResultBuilder.CreateResult(result);
        }

        public override Expression ToExpression()
        {
            if (this.ColumnOperand == null)
            {
                throw new NullReferenceException("ColumnOperand");
            }
            var columnExpr = this.ColumnOperand.ToExpression();
            return Expression.NotEqual(columnExpr, Expression.Constant(null));
        }
    }
}


