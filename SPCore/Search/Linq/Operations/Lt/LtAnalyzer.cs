using System.Linq.Expressions;
using SPCore.Search.Linq.Interfaces;

namespace SPCore.Search.Linq.Operations.Lt
{
    internal class LtAnalyzer : BinaryExpressionBaseAnalyzer
    {
        public LtAnalyzer(IOperationResultBuilder operationResultBuilder, IOperandBuilder operandBuilder)
            : base(operationResultBuilder, operandBuilder)
        {
        }

        public override bool IsValid(LambdaExpression expr)
        {
            if (!base.IsValid(expr))
            {
                return false;
            }
            return (expr.Body.NodeType == ExpressionType.LessThan);
        }

        public override IOperation GetOperation(LambdaExpression expr)
        {
            return this.getOperation(expr,
                (operationResultBuilder, fieldRefOperand, valueOperand) => new LtOperation(this.OperationResultBuilder, fieldRefOperand, valueOperand));
        }
    }
}


