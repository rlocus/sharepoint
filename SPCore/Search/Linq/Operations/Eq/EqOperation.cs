using SPCore.Search.Linq.Interfaces;
using System.Linq.Expressions;

namespace SPCore.Search.Linq.Operations.Eq
{
    internal class EqOperation : BinaryOperationBase
    {
        public EqOperation(IOperationResultBuilder operationResultBuilder,
            IOperand fieldRefOperand, IOperand valueOperand)
            : base(operationResultBuilder, fieldRefOperand, valueOperand)
        {
        }

        public override IOperationResult ToResult()
        {
            string result = string.Format("{0} = {1}", ColumnOperand, ValueOperand);
            return this.OperationResultBuilder.CreateResult(result);
        }

        public override Expression ToExpression()
        {
            // in the field ref operand we don't know what type of the value it has. So perform
            // conversion here
            var columnExpr = this.GetColumnOperandExpression();
            var valueExpr = this.GetValueOperandExpression();

            return Expression.Equal(columnExpr, valueExpr);
        }
    }
}


