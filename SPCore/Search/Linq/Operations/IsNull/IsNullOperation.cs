using SPCore.Search.Linq.Interfaces;
using System;
using System.Linq.Expressions;

namespace SPCore.Search.Linq.Operations.IsNull
{
    internal class IsNullOperation : UnaryOperationBase
    {
        public IsNullOperation(IOperationResultBuilder operationResultBuilder,
            IOperand fieldRefOperand)
            : base(operationResultBuilder, fieldRefOperand)
        {
        }

        public override IOperationResult ToResult()
        {
            string result = string.Format("{0} IS NULL", ColumnOperand);
            return this.OperationResultBuilder.CreateResult(result);
        }

        public override Expression ToExpression()
        {
            if (this.ColumnOperand == null)
            {
                throw new NullReferenceException("fieldRefOperand");
            }
            var columnExpr = this.ColumnOperand.ToExpression();
            return Expression.Equal(columnExpr, Expression.Constant(null));
        }
    }
}


