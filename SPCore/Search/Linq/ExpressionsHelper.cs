using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SPCore.Search.Linq
{
    public static class ExpressionsHelper
    {
        public static Expression<Func<SPManagedPropertyCollection, bool>> CombineAnd(
            IEnumerable<Expression<Func<SPManagedPropertyCollection, bool>>> expressions)
        {
            return Combine(expressions, ExpressionType.AndAlso);
        }

        public static Expression<Func<SPManagedPropertyCollection, bool>> CombineOr(
            IEnumerable<Expression<Func<SPManagedPropertyCollection, bool>>> expressions)
        {
            return Combine(expressions, ExpressionType.OrElse);
        }

        // ----- Internal methods -----

        private static Expression<Func<SPManagedPropertyCollection, bool>> Combine(
            IEnumerable<Expression<Func<SPManagedPropertyCollection, bool>>> expressions, ExpressionType type)
        {
            if (expressions == null || !expressions.Any())
            {
                throw new EmptyExpressionsListException();
            }

            Expression result;
            if (expressions.Count() == 1)
            {
                result = expressions.First().Body;
            }
            else
            {
                result = JoinExpressions(expressions, type);
            }

            var lambda = Expression.Lambda<Func<SPManagedPropertyCollection, bool>>(result,
                Expression.Parameter(typeof(SPManagedPropertyCollection), ReflectionHelper.CommonParameterName));
            return lambda;
        }

        private static BinaryExpression JoinExpressions(
            IEnumerable<Expression<Func<SPManagedPropertyCollection, bool>>> expressions, ExpressionType type)
        {
            return JoinExpressions(1, expressions, expressions.ElementAt(0).Body, type);
        }

        private static BinaryExpression JoinExpressions(
            int currentIdxToAdd, IEnumerable<Expression<Func<SPManagedPropertyCollection, bool>>> expressions,
            Expression prevExpr, ExpressionType type)
        {
            if (currentIdxToAdd >= expressions.Count())
            {
                return (BinaryExpression)prevExpr;
            }

            var currentExpression = expressions.ElementAt(currentIdxToAdd);

            Expression resultExpr;
            if (type == ExpressionType.OrElse)
            {
                resultExpr = Expression.OrElse(prevExpr, currentExpression.Body);
            }
            else if (type == ExpressionType.AndAlso)
            {
                resultExpr = Expression.AndAlso(prevExpr, currentExpression.Body);
            }
            else
            {
                throw new OnlyOrAndBinaryExpressionsAllowedForJoinsExceptions();
            }
            return JoinExpressions(currentIdxToAdd + 1, expressions, resultExpr, type);
        }

        // ----------- Helper methods working with DateTime ----------

        internal static bool IncludeTimeValue(Expression expression)
        {
            return (expression is MethodCallExpression) && (((MethodCallExpression)expression).Method.Name == ReflectionHelper.IncludeTimeValue);
        }

        internal static Expression RemoveIncludeTimeValueMethodCallIfAny(Expression expression)
        {
            if (!IncludeTimeValue(expression)) return expression;
            var methodCall = (MethodCallExpression)expression;

            if (methodCall.Object != null) return methodCall.Object;
            if (methodCall.Arguments.Count == 1) return methodCall.Arguments[0];

            throw new NonSupportedExpressionException(expression); // it should not happen - either Object or Arguments  is not NULL
        }
    }
}