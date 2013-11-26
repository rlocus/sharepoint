using System.Linq.Expressions;
using SPCore.Search.Linq.Interfaces;

namespace SPCore.Search.Linq.Operations.BeginsWith
{
    internal class BeginsWithAnalyzer : UnaryExpressionBaseAnalyzer
    {
        public BeginsWithAnalyzer(IOperationResultBuilder operationResultBuilder, IOperandBuilder operandBuilder)
            : base(operationResultBuilder, operandBuilder)
        {
        }

        public override bool IsValid(LambdaExpression expr)
        {
            if (!base.IsValid(expr))
            {
                return false;
            }
            return ((MethodCallExpression)expr.Body).Method.Name == ReflectionHelper.StartsWithMethodName;
        }

        public override IOperation GetOperation(LambdaExpression expr)
        {
            if (!IsValid(expr))
            {
                throw new NonSupportedExpressionException(expr);
            }
            var fieldRefOperand = GetFieldRefOperand(expr);
            var valueOperand = GetValueOperand(expr);
            return new BeginsWithOperation(OperationResultBuilder, fieldRefOperand, valueOperand);
        }
    }
}