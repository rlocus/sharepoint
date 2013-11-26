using SPCore.Search.Linq.Interfaces;
using System.Linq.Expressions;

namespace SPCore.Search.Linq.Operations.Leq
{
    internal class LeqOperation : BinaryOperationBase
    {
        public LeqOperation(IOperationResultBuilder operationResultBuilder,
            IOperand fieldRefOperand, IOperand valueOperand)
            : base(operationResultBuilder, fieldRefOperand, valueOperand)
        {
        }

        public override IOperationResult ToResult()
        {
            string result = string.Format("{0} <= {1}", ColumnOperand, ValueOperand);

            return this.OperationResultBuilder.CreateResult(result);
        }

        public override Expression ToExpression()
        {
            var columnExpr = this.GetColumnOperandExpression();
            var valueExpr = this.GetValueOperandExpression();

            if (!valueExpr.Type.IsSubclassOf(typeof(BaseFieldTypeWithOperators)))
            {
                return Expression.LessThanOrEqual(columnExpr, valueExpr);
            }

            var methodInfo = typeof(BaseFieldTypeWithOperators).GetMethod(ReflectionHelper.LessThanOrEqualMethodName);
            return Expression.LessThanOrEqual(columnExpr, valueExpr, false, methodInfo);
        }
    }
}


