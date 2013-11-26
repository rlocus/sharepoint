using System.Linq.Expressions;
using SPCore.Search.Linq.Interfaces;

namespace SPCore.Search.Linq.Operations.OrElse
{
    internal class OrElseAnalyzer : CompositeExpressionBaseAnalyzer
    {
        public OrElseAnalyzer(IOperationResultBuilder operationResultBuilder, IAnalyzerFactory analyzerFactory) :
            base(operationResultBuilder, analyzerFactory)
        {
        }

        public override bool IsValid(LambdaExpression expr)
        {
            if (!base.IsValid(expr))
            {
                return false;
            }
            return (expr.Body.NodeType == ExpressionType.OrElse);
        }

        public override IOperation GetOperation(LambdaExpression expr)
        {
            if (!this.IsValid(expr))
            {
                throw new NonSupportedExpressionException(expr);
            }
            var leftOperation = this.GetLeftOperation(expr);
            var rightOperation = this.GetRightOperation(expr);
            var operation = new OrElseOperation(this.OperationResultBuilder, leftOperation, rightOperation);
            return operation;
        }
    }
}


