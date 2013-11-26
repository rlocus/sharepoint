
namespace SPCore.Linq
{
    public class ListRepository<TEntity, TContext> :
        BaseRepository<TEntity, TContext>
        where TEntity : EntityItem, new()
        where TContext : EntityDataContext
    {
        public ListRepository(string listName)
            : base(listName) { }
        public ListRepository(string listName, bool readOnly)
            : base(listName, readOnly) { }
        public ListRepository(string listName, string webUrl)
            : base(listName, webUrl) { }
        public ListRepository(string listName, string webUrl, bool readOnly)
            : base(listName, webUrl, readOnly) { }
        public ListRepository(string listName, string webUrl, bool readOnly, bool crossSite)
            : base(listName, webUrl, readOnly, crossSite) { }
    }
}
