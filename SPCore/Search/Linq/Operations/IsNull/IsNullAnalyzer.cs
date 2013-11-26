using System.Linq.Expressions;
using SPCore.Search.Linq.Interfaces;

namespace SPCore.Search.Linq.Operations.IsNull
{
    internal class IsNullAnalyzer : NullabilityBaseAnalyzer
    {
        public IsNullAnalyzer(IOperationResultBuilder operationResultBuilder, IOperandBuilder operandBuilder)
            : base(operationResultBuilder, operandBuilder)
        {
        }

        public override bool IsValid(LambdaExpression expr)
        {
            if (!base.IsValid(expr))
            {
                return false;
            }

            return (expr.Body.NodeType == ExpressionType.Equal);
        }

        public override IOperation GetOperation(LambdaExpression expr)
        {
            if (!this.IsValid(expr))
            {
                throw new NonSupportedExpressionException(expr);
            }
            var fieldRefOperand = this.GetFieldRefOperand(expr, null);
            return new IsNullOperation(this.OperationResultBuilder, fieldRefOperand);
        }
    }
}


