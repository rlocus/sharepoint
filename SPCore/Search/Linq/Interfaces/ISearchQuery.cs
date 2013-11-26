using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SPCore.Search.Linq.Interfaces
{
    public interface ISearchQuery
    {
        ISearchQuery Select(Expression<Func<SPManagedPropertyCollection, object>> expr);
        ISearchQuery Select(Expression<Func<SPManagedPropertyCollection, object[]>> expr);
        ISearchQuery Select(IEnumerable<string> titles);
        ISearchQuery Where(Expression<Func<SPManagedPropertyCollection, bool>> expr);
        ISearchQuery WhereAll(IEnumerable<Expression<Func<SPManagedPropertyCollection, bool>>> expressions);
        ISearchQuery WhereAny(IEnumerable<Expression<Func<SPManagedPropertyCollection, bool>>> expressions);
        ISearchQuery OrderBy(Expression<Func<SPManagedPropertyCollection, object>> expr);
        ISearchQuery OrderBy(Expression<Func<SPManagedPropertyCollection, object[]>> expr);
        ISearchQuery OrderBy(IEnumerable<Expression<Func<SPManagedPropertyCollection, object>>> expressions);
        ISearchQuery From(string scope);
        //string ToString();
    }
}
