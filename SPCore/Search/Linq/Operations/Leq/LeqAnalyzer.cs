using System.Linq.Expressions;
using SPCore.Search.Linq.Interfaces;

namespace SPCore.Search.Linq.Operations.Leq
{
    internal class LeqAnalyzer : BinaryExpressionBaseAnalyzer
    {
        public LeqAnalyzer(IOperationResultBuilder operationResultBuilder, IOperandBuilder operandBuilder)
            : base(operationResultBuilder, operandBuilder)
        {
        }

        public override bool IsValid(LambdaExpression expr)
        {
            if (!base.IsValid(expr))
            {
                return false;
            }
            return (expr.Body.NodeType == ExpressionType.LessThanOrEqual);
        }

        public override IOperation GetOperation(LambdaExpression expr)
        {
            return this.getOperation(expr,
                (operationResultBuilder, fieldRefOperand, valueOperand) => new LeqOperation(this.OperationResultBuilder, fieldRefOperand, valueOperand));
        }
    }
}


