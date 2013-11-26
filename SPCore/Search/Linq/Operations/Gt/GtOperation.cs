using SPCore.Search.Linq.Interfaces;
using System.Linq.Expressions;

namespace SPCore.Search.Linq.Operations.Gt
{
    internal class GtOperation : BinaryOperationBase
    {
        public GtOperation(IOperationResultBuilder operationResultBuilder,
            IOperand fieldRefOperand, IOperand valueOperand)
            : base(operationResultBuilder, fieldRefOperand, valueOperand)
        {
        }

        public override IOperationResult ToResult()
        {
            string result = string.Format("{0} > {1}", ColumnOperand, ValueOperand);
            return this.OperationResultBuilder.CreateResult(result);
        }

        public override Expression ToExpression()
        {
            var columnExpr = this.GetColumnOperandExpression();
            var valueExpr = this.GetValueOperandExpression();

            if (!valueExpr.Type.IsSubclassOf(typeof(BaseFieldTypeWithOperators)))
            {
                return Expression.GreaterThan(columnExpr, valueExpr);
            }

            var methodInfo = typeof(BaseFieldTypeWithOperators).GetMethod(ReflectionHelper.GreaterThanMethodName);
            return Expression.GreaterThan(columnExpr, valueExpr, false, methodInfo);
        }
    }
}


