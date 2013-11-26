using SPCore.Search.Linq.Interfaces;
using System.Linq.Expressions;

namespace SPCore.Search.Linq.Operations.Neq
{
    internal class NeqOperation : BinaryOperationBase
    {
        public NeqOperation(IOperationResultBuilder operationResultBuilder,
            IOperand fieldRefOperand, IOperand valueOperand)
            : base(operationResultBuilder, fieldRefOperand, valueOperand)
        {
        }

        public override IOperationResult ToResult()
        {
            string result = string.Format("{0} <> {1}", ColumnOperand, ValueOperand);
            return this.OperationResultBuilder.CreateResult(result);
        }

        public override Expression ToExpression()
        {
            var columnExpr = this.GetColumnOperandExpression();
            var valueExpr = this.GetValueOperandExpression();

            return Expression.NotEqual(columnExpr, valueExpr);
        }
    }
}


