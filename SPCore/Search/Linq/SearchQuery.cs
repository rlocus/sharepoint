using Microsoft.SharePoint;
using SPCore.Search.Linq.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SPCore.Search.Linq
{
    internal class SearchQuery : ISearchQuery
    {
        private readonly ITranslatorFactory _translatorFactory;
        private string _select;
        private string _where;
        private string _from;
        private string _orderBy;

        public SearchQuery(ITranslatorFactory translatorFactory)
        {
            this._translatorFactory = translatorFactory;
        }

        public ISearchQuery Where(Expression<Func<SPManagedPropertyCollection, bool>> expr)
        {
            var translator = _translatorFactory.Create(expr);
            this._where = translator.TranslateWhere(expr);
            return this;
        }

        public ISearchQuery WhereAll(IEnumerable<Expression<Func<SPManagedPropertyCollection, bool>>> expressions)
        {
            var combinedExpression = ExpressionsHelper.CombineAnd(expressions);
            return this.Where(combinedExpression);
        }

        public ISearchQuery WhereAny(IEnumerable<Expression<Func<SPManagedPropertyCollection, bool>>> expressions)
        {
            var combinedExpression = ExpressionsHelper.CombineOr(expressions);
            return this.Where(combinedExpression);
        }

        public ISearchQuery OrderBy(Expression<Func<SPManagedPropertyCollection, object>> expr)
        {
            var lambda = CreateArrayExpression(expr);
            return OrderBy(lambda);
        }

        public ISearchQuery OrderBy(Expression<Func<SPManagedPropertyCollection, object[]>> expr)
        {
            var translator = _translatorFactory.Create(expr);
            this._orderBy = translator.TranslateOrderBy(expr);
            return this;
        }

        public ISearchQuery OrderBy(IEnumerable<Expression<Func<SPManagedPropertyCollection, object>>> expressions)
        {
            if (expressions == null || !expressions.Any())
            {
                throw new EmptyExpressionsListException();
            }

            var lambda = CreateArrayExpression(expressions);
            return OrderBy(lambda);
        }

        public ISearchQuery OrderBy(IEnumerable<Expression<Func<SPManagedPropertyCollection, object[]>>> expressions)
        {
            if (expressions == null || !expressions.Any())
            {
                throw new EmptyExpressionsListException();
            }

            var lambda = CreateArrayExpression(expressions);
            return OrderBy(lambda);
        }

        private static Expression<Func<SPManagedPropertyCollection, object[]>> CreateArrayExpression(IEnumerable<Expression<Func<SPManagedPropertyCollection, object>>> expressions)
        {
            var expr = expressions.FirstOrDefault();
            if (expr == null)
            {
                throw new EmptyExpressionsListException();
            }

            return Expression.Lambda<Func<SPManagedPropertyCollection, object[]>>(
                Expression.NewArrayInit(typeof(object), expressions.Select(e => e.Body)), expr.Parameters);
        }

        private static Expression<Func<SPManagedPropertyCollection, object[]>> CreateArrayExpression(IEnumerable<Expression<Func<SPManagedPropertyCollection, object[]>>> expressions)
        {
            var expr = expressions.FirstOrDefault();

            if (expr == null)
            {
                throw new EmptyExpressionsListException();
            }

            var list = expressions.SelectMany(e => ((NewArrayExpression)e.Body).Expressions);

            return Expression.Lambda<Func<SPManagedPropertyCollection, object[]>>(
                Expression.NewArrayInit(typeof(object), list), expr.Parameters);
        }

        //private Expression<Func<SPManagedPropertyCollection, object[]>> EnsureArrayExpression(Expression<Func<SPManagedPropertyCollection, object>> expr)
        //{
        //    Expression<Func<SPManagedPropertyCollection, object[]>> lambda = expr.Body.Type != typeof (object[])
        //                            ? CreateArrayExpression(expr)
        //                            : Expression.Lambda<Func<SPManagedPropertyCollection, object[]>>(
        //                                Expression.NewArrayInit(typeof (object), ((NewArrayExpression) expr.Body).Expressions),
        //                                expr.Parameters);
        //    return lambda;
        //}

        private static Expression<Func<SPManagedPropertyCollection, object[]>> CreateArrayExpression(Expression<Func<SPManagedPropertyCollection, object>> expr)
        {
            return Expression.Lambda<Func<SPManagedPropertyCollection, object[]>>(Expression.NewArrayInit(typeof(object), expr.Body), expr.Parameters);
        }

        public ISearchQuery Select(Expression<Func<SPManagedPropertyCollection, object>> expr)
        {
            var lambda = CreateArrayExpression(expr);
            return Select(lambda);
        }

        public ISearchQuery Select(Expression<Func<SPManagedPropertyCollection, object[]>> expr)
        {
            var translator = _translatorFactory.Create(expr);
            _select = translator.TranslateSelect(expr);
            return this;
        }


        public ISearchQuery Select(IEnumerable<string> titles)
        {
            if (titles == null || titles.Any(t => t == null))
            {
                throw new ArgumentNullException();
            }

            return this.Select(CreateExpressionFromArray(titles));
        }

        private static Expression<Func<SPManagedPropertyCollection, object[]>> CreateExpressionFromArray<T>(IEnumerable<T> items)
        {
            return Expression.Lambda<Func<SPManagedPropertyCollection, object[]>>(
                Expression.NewArrayInit(
                    typeof(object),
                    (IEnumerable<Expression>)items.Select(
                        t => Expression.Call(Expression.Parameter(typeof(SPManagedPropertyCollection), ReflectionHelper.CommonParameterName),
                                             typeof(SPListItem).GetMethod(ReflectionHelper.IndexerMethodName, new[] { typeof(T) }),
                                             new[] { Expression.Constant(t) })).ToArray()),
                Expression.Parameter(typeof(SPListItem), ReflectionHelper.CommonParameterName));
        }

        public override string ToString()
        {
            string where = _where;
            string select = _select;
            string from = _from;
            string orderBy = _orderBy;

            if (select == null)
            {
                throw new ArgumentException("Select");
            }

            if (where == null)
            {
                if (from == null)
                {
                    throw new ArgumentException("Where");
                }

                where = string.Format("WHERE {0}", from);
            }
            else
            {
                if (from != null)
                {
                    where = string.Format("{0} AND {1}", where, from);
                }
            }

            return string.IsNullOrEmpty(orderBy)
                       ? string.Format("{0} FROM Scope() {1}", select, where)
                       : string.Format("{0} FROM Scope() {1} {2}", select, where, orderBy);
        }

        public static implicit operator string(SearchQuery query)
        {
            return query.ToString();
        }

        public ISearchQuery From(string scope)
        {
            _from = string.Format(scope.StartsWith("http://") ? "\"site\" = '{0}'" : "\"scope\" = '{0}'", scope);
            return this;
        }
    }
}
