using System.Linq.Expressions;
using SPCore.Search.Linq.Interfaces;

namespace SPCore.Search.Linq.Operations.Contains
{
    internal class ContainsAnalyzer : UnaryExpressionBaseAnalyzer
    {
        public ContainsAnalyzer(IOperationResultBuilder operationResultBuilder, IOperandBuilder operandBuilder)
            : base(operationResultBuilder, operandBuilder)
        {
        }

        public override bool IsValid(LambdaExpression expr)
        {
            if (!base.IsValid(expr))
            {
                return false;
            }
            return ((MethodCallExpression)expr.Body).Method.Name == ReflectionHelper.ContainsMethodName;
        }

        public override IOperation GetOperation(LambdaExpression expr)
        {
            if (!IsValid(expr))
            {
                throw new NonSupportedExpressionException(expr);
            }
            var fieldRefOperand = GetFieldRefOperand(expr);
            var valueOperand = GetValueOperand(expr);
            return new ContainsOperation(OperationResultBuilder, fieldRefOperand, valueOperand);
        }
    }
}
