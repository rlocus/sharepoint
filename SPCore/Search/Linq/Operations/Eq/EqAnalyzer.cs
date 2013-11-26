using System.Linq.Expressions;
using SPCore.Search.Linq.Interfaces;

namespace SPCore.Search.Linq.Operations.Eq
{
    internal class EqAnalyzer : BinaryExpressionBaseAnalyzer
    {
        public EqAnalyzer(IOperationResultBuilder operationResultBuilder, IOperandBuilder operandBuilder)
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
            return this.getOperation(expr,
                (operationResultBuilder, fieldRefOperand, valueOperand) => new EqOperation(this.OperationResultBuilder, fieldRefOperand, valueOperand));
        }
    }
}


