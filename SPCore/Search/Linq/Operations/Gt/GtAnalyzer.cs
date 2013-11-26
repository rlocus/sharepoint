using System.Linq.Expressions;
using SPCore.Search.Linq.Interfaces;

namespace SPCore.Search.Linq.Operations.Gt
{
    internal class GtAnalyzer : BinaryExpressionBaseAnalyzer
    {
        public GtAnalyzer(IOperationResultBuilder operationResultBuilder, IOperandBuilder operandBuilder)
            : base(operationResultBuilder, operandBuilder)
        {
        }

        public override bool IsValid(LambdaExpression expr)
        {
            if (!base.IsValid(expr))
            {
                return false;
            }
            return (expr.Body.NodeType == ExpressionType.GreaterThan);
        }

        public override IOperation GetOperation(LambdaExpression expr)
        {
            return this.getOperation(expr,
                (operationResultBuilder, fieldRefOperand, valueOperand) => new GtOperation(this.OperationResultBuilder, fieldRefOperand, valueOperand));
        }
    }
}


