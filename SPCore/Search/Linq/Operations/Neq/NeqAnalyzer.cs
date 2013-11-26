using System.Linq.Expressions;
using SPCore.Search.Linq.Interfaces;

namespace SPCore.Search.Linq.Operations.Neq
{
    internal class NeqAnalyzer : BinaryExpressionBaseAnalyzer
    {
        public NeqAnalyzer(IOperationResultBuilder operationResultBuilder, IOperandBuilder operandBuilder)
            : base(operationResultBuilder, operandBuilder)
        {
        }

        public override bool IsValid(LambdaExpression expr)
        {
            if (!base.IsValid(expr))
            {
                return false;
            }
            return (expr.Body.NodeType == ExpressionType.NotEqual);
        }

        public override IOperation GetOperation(LambdaExpression expr)
        {
            return this.getOperation(expr,
                (operationResultBuilder, fieldRefOperand, valueOperand) => new NeqOperation(this.OperationResultBuilder, fieldRefOperand, valueOperand));
        }
    }
}


