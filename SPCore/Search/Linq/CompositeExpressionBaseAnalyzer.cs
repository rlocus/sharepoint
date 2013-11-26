using Microsoft.Office.Server.Search.Administration;
using SPCore.Search.Linq.Interfaces;
using System;
using System.Collections;
using System.Linq.Expressions;

namespace SPCore.Search.Linq
{
    // Base class for AndAlso and OrElse analyzers
    internal abstract class CompositeExpressionBaseAnalyzer : BaseAnalyzer
    {
        protected IAnalyzerFactory AnalyzerFactory;

        protected CompositeExpressionBaseAnalyzer(IOperationResultBuilder operationResultBuilder,
            IAnalyzerFactory analyzerFactory) :
            base(operationResultBuilder)
        {
            this.AnalyzerFactory = analyzerFactory;
        }

        public override bool IsValid(LambdaExpression expr)
        {
            // expression should be binary expresion
            if (!(expr.Body is BinaryExpression))
            {
                return false;
            }
            var body = expr.Body as BinaryExpression;

            var lambdaParam = expr.Parameters[0];
            // check left operand
            return this.IsExpressionValid(body.Left, lambdaParam) && this.IsExpressionValid(body.Right, lambdaParam);

            // check right operand
        }

        private bool IsExpressionValid(Expression subExpr, ParameterExpression lambdaParam)
        {
            // make Expression<Func<SPListItem, bool>> lambda expression from BinaryExpression
            var lambda = CreateLambdaFromExpression(subExpr, lambdaParam);
            var subExpressionAnalyzer = this.AnalyzerFactory.Create(lambda);
            return subExpressionAnalyzer.IsValid(lambda);
        }

        // For composite expressions like x => (string)x["Email"] == "test@example.com" && (int)x["Count1"] == 1
        // it creates 2 lambdas: x => (string)x["Email"] == "test@example.com" ; x => (int)x["Count1"] == 1
        private static Expression<Func<SPManagedPropertyCollection, bool>> CreateLambdaFromExpression(Expression subExpr,
            ParameterExpression lambdaParam)
        {
            return Expression.Lambda<Func<SPManagedPropertyCollection, bool>>(subExpr, lambdaParam);
        }

        private IOperation CreateOperationFromExpression(Expression subExpr, ParameterExpression lambdaParam)
        {
            // make Expression<Func<SPListItem, bool>> lambda expression from BinaryExpression
            var lambda = CreateLambdaFromExpression(subExpr, lambdaParam);
            var subExpressionAnalyzer = this.AnalyzerFactory.Create(lambda);
            return subExpressionAnalyzer.GetOperation(lambda);
        }

        protected IOperation GetLeftOperation(LambdaExpression expr)
        {
            if (!this.IsValid(expr))
            {
                throw new NonSupportedExpressionException(expr);
            }
            var body = expr.Body as BinaryExpression;
            var lambdaParam = expr.Parameters[0];
            var operation = this.CreateOperationFromExpression(body.Left, lambdaParam);
            return operation;
        }

        protected IOperation GetRightOperation(LambdaExpression expr)
        {
            if (!this.IsValid(expr))
            {
                throw new NonSupportedExpressionException(expr);
            }
            var body = expr.Body as BinaryExpression;
            var lambdaParam = expr.Parameters[0];
            var operation = this.CreateOperationFromExpression(body.Right, lambdaParam);
            return operation;
        }
    }
}
