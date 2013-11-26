using System.Linq.Expressions;
using SPCore.Search.Linq.Interfaces;

namespace SPCore.Search.Linq
{
    internal abstract class UnaryExpressionBaseAnalyzer : BaseAnalyzer
    {
        protected IOperandBuilder OperandBuilder;

        protected UnaryExpressionBaseAnalyzer(IOperationResultBuilder operationResultBuilder, IOperandBuilder operandBuilder)
            : base(operationResultBuilder)
        {
            this.OperandBuilder = operandBuilder;
        }

        public override bool IsValid(LambdaExpression expr)
        {
            // body should be MethodCallExpression
            if (!(expr.Body is MethodCallExpression))
            {
                return false;
            }
            var body = expr.Body as MethodCallExpression;

            // --- check for object ---

            // left operand for string based syntax should be indexer call
            if (!(body.Object is UnaryExpression))
            {
                return false;
            }
            var methodCallExpression = ((UnaryExpression)body.Object).Operand;
            if (!(methodCallExpression is MethodCallExpression))
            {
                return false;
            }
            var objectExpression = (MethodCallExpression)methodCallExpression;
            if (objectExpression.Method.Name != ReflectionHelper.IndexerMethodName)
            {
                return false;
            }
            if (objectExpression.Arguments.Count != 1)
            {
                return false;
            }
            // currently only constants are supported as indexer's argument
            if (!this.IsValidEvaluableExpression(objectExpression.Arguments[0]))
            {
                return false;
            }

            // --- check for function ---

            // right expression should be constant, variable or method call
            if (body.Arguments.Count != 1)
            {
                return false;
            }

            var parameterExpression = body.Arguments[0];

            return this.IsValidEvaluableExpression(parameterExpression);
        }
        
        protected IOperand GetFieldRefOperand(LambdaExpression expr)
        {
            if (!IsValid(expr))
            {
                throw new NonSupportedExpressionException(expr);
            }

            var body = expr.Body as MethodCallExpression;
           
            return OperandBuilder.CreateColumnOperand(body.Object, null);
        }

        protected IOperand GetValueOperand(LambdaExpression expr)
        {
            if (!IsValid(expr))
            {
                throw new NonSupportedExpressionException(expr);
            }
            var body = expr.Body as MethodCallExpression;
            var valueType = body.Object.Type;
            var parameterExpression = body.Arguments[0];
            return OperandBuilder.CreateValueOperandForNativeSyntax(parameterExpression, valueType);
        }
    }
}
