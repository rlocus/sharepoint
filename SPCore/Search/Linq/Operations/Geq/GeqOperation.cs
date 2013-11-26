using SPCore.Search.Linq.Interfaces;
using System.Linq.Expressions;

namespace SPCore.Search.Linq.Operations.Geq
{
    internal class GeqOperation : BinaryOperationBase
    {
        public GeqOperation(IOperationResultBuilder operationResultBuilder,
            IOperand fieldRefOperand, IOperand valueOperand)
            : base(operationResultBuilder, fieldRefOperand, valueOperand)
        {
        }

        public override IOperationResult ToResult()
        {
            string result = string.Format("{0} >= {1}", ColumnOperand, ValueOperand);
            return this.OperationResultBuilder.CreateResult(result);
        }

        public override Expression ToExpression()
        {
            var columnExpr = this.GetColumnOperandExpression();
            var value = this.GetValueOperandExpression();

            if (!value.Type.IsSubclassOf(typeof(BaseFieldTypeWithOperators)))
            {
                return Expression.GreaterThanOrEqual(columnExpr, value);
            }

            var methodInfo = typeof(BaseFieldTypeWithOperators).GetMethod(ReflectionHelper.GreaterThanOrEqualMethodName);
            return Expression.GreaterThanOrEqual(columnExpr, value, false, methodInfo);
        }
    }
}


