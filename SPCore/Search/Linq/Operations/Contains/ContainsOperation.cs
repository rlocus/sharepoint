using SPCore.Search.Linq.Interfaces;
using SPCore.Search.Linq.Operands;
using System.Linq.Expressions;

namespace SPCore.Search.Linq.Operations.Contains
{
    internal class ContainsOperation : BinaryOperationBase
    {
        public ContainsOperation(IOperationResultBuilder operationResultBuilder,
            IOperand fieldRefOperand, IOperand valueOperand)
            : base(operationResultBuilder, fieldRefOperand, valueOperand)
        {
        }

        public override IOperationResult ToResult()
        {
            string result;

            if (ValueOperand is TextValueOperand)
            {
                string value = (string)ValueOperand.GetValue();
                result = value.Contains("*")
                             ? string.Format("CONTAINS({0},'\"{1}\"')", ColumnOperand, value)
                             : string.Format("CONTAINS({0},{1})", ColumnOperand, ValueOperand);
            }
            else
            {
                result = string.Format("CONTAINS({0},{1})", ColumnOperand, ValueOperand);
            }

            return OperationResultBuilder.CreateResult(result);
        }

        public override Expression ToExpression()
        {
            if (!(this.ColumnOperand is ColumnOperand))
            {
                throw new OperationShouldContainFieldRefOperandException();
            }
            if (!(this.ValueOperand is TextValueOperand))
            {
                throw new OperationShouldContainTextValueOperandException();
            }
            var columnExpr = this.GetColumnOperandExpression();
            var valueExpr = this.GetValueOperandExpression();
            var mi = typeof(string).GetMethod(ReflectionHelper.ContainsMethodName, new[] { typeof(string) });
            return Expression.Call(columnExpr, mi, valueExpr);
        }
    }
}
