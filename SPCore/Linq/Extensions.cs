using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Linq;

namespace SPCore.Linq
{
    public static class Extensions
    {
        public static EntityList<TEntity> GetEntityList<TEntity>(this SPList list, DataContext dataContext)
        {
            return dataContext.GetList<TEntity>(list.Title);
        }

        public static ListRepository<TEntity, TContext> GetRepository<TEntity, TContext>(this SPList list, bool readOnly = true)
            where TEntity : EntityItem, new()
            where TContext : EntityDataContext
        {
            return list.GetRepository<ListRepository<TEntity, TContext>, TEntity, TContext>(readOnly);
        }

        public static TRepository GetRepository<TRepository, TEntity, TContext>(this SPList list, bool readOnly = true)
            where TRepository : BaseRepository<TEntity, TContext>
            where TEntity : EntityItem, new()
            where TContext : DataContext
        {
            bool crossSite = SPContext.Current != null && SPContext.Current.Site.ID != list.ParentWeb.Site.ID;

            return (TRepository)Activator.CreateInstance(typeof(TRepository),
                        new object[] { list.Title, list.ParentWeb.Url, readOnly, crossSite });
        }
    }
}
