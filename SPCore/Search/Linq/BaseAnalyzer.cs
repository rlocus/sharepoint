using System.Linq.Expressions;
using SPCore.Search.Linq.Interfaces;

namespace SPCore.Search.Linq
{
    // Base class for all analyzers
    internal abstract class BaseAnalyzer : IAnalyzer
    {
        protected IOperationResultBuilder OperationResultBuilder;

        protected BaseAnalyzer(IOperationResultBuilder operationResultBuilder)
        {
            this.OperationResultBuilder = operationResultBuilder;
        }

        public abstract bool IsValid(LambdaExpression expr);
        public abstract IOperation GetOperation(LambdaExpression expr);

        protected bool IsValidEvaluableExpression(Expression expr)
        {
            return (!expr.Type.IsSubclassOf(typeof(BaseFieldType)));
        }
    }
}
