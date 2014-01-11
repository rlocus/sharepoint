using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Microsoft.Office.Server.Utilities;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Linq;
using EntityState = Microsoft.SharePoint.Linq.EntityState;

namespace SPCore.Linq
{
    /// <summary>
    /// Base repository class
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    public abstract class BaseRepository<TEntity, TContext> : IDisposable
        where TEntity : EntityItem, new()
        where TContext : EntityDataContext
    {
        #region [ Fields ]
        protected readonly string WebUrl;
        protected readonly string ListName;
        protected readonly bool ReadOnly;
        protected readonly bool CrossSite;
        protected readonly bool IsAnonymous;
        private SPList _spList;

        #endregion

        #region [ Properties ]

        public TContext Context { get; private set; }

        private EntityList<TEntity> _list;

        protected EntityList<TEntity> List
        {
            get
            {
                if (Context == null)
                {
                    throw new ArgumentNullException("Context");
                }

                return _list ?? (_list = Context.GetList<TEntity>(ListName));
            }
        }

        protected SPList SPList
        {
            get { return _spList ?? (_spList = this.Context.GetSPList<TEntity>(ListName)); }
        }

        protected TextWriter Log { get { return Context.Log; } set { Context.Log = value; } }

        #endregion

        #region [ Constructors ]

        protected BaseRepository(string listName)
            : this(listName, true)
        { }

        protected BaseRepository(string listName, bool readOnly)
            : this(listName,
                SPContext.Current.Web.Url, readOnly)
        { }

        protected BaseRepository(string listName, string webUrl)
            : this(listName, webUrl, true)
        { }

        protected BaseRepository(string listName, string webUrl, bool readOnly)
            : this(listName, webUrl, readOnly, false)
        { }

        protected BaseRepository(string listName, string webUrl, bool readOnly, bool crossSite)
        {
            if (listName == null) throw new ArgumentNullException("listName");
            if (webUrl == null) throw new ArgumentNullException("webUrl");

            ReadOnly = readOnly;
            CrossSite = crossSite;
            ListName = listName;
            WebUrl = webUrl;

            IsAnonymous = SPContext.Current != null && SPContext.Current.Web.CurrentUser == null;
            Context = CreateContext(crossSite);
        }

        #endregion

        #region [ Destructor ]

        ~BaseRepository()
        {
            Dispose(true);
        }

        #endregion

        #region [ Private Methods ]

        private TContext GetContext()
        {
            TContext context = null;

            if (IsAnonymous)
            {
                RunAsAdmin(() =>
                {
                    context =
                        (TContext)Activator.CreateInstance(typeof(TContext),
                        new object[] { WebUrl });
                    context.ObjectTrackingEnabled = !ReadOnly;
                });
            }
            else
            {
                context =
                    (TContext)Activator.CreateInstance(typeof(TContext),
                    new object[] { WebUrl });

                context.ObjectTrackingEnabled = !ReadOnly;
            }

            return context;
        }

        private void RunAsAdmin(SPSecurity.CodeToRunElevated secureCode)
        {
            bool nullUserFlag = (SPContext.Current != null && IsAnonymous);

            if (nullUserFlag)
            {
                HttpContext backupCtx = HttpContext.Current;

                try
                {
                    HttpContext.Current = null;
                    SPSecurity.RunWithElevatedPrivileges(secureCode);
                }
                finally
                {
                    HttpContext.Current = backupCtx;
                }
            }
            else
            {
                SPSecurity.RunWithElevatedPrivileges(secureCode);
            }
        }

        private void Commit(TContext context, ConflictMode conflictMode, bool systemUpdate)
        {
            if (conflictMode == ConflictMode.ContinueOnConflict)
            {
                try
                {
                    context.SubmitChanges(conflictMode);
                }
                catch (ChangeConflictException)
                {
                    OnChangeConflict(context.ChangeConflicts);
                }
            }
            else
            {
                context.SubmitChanges(conflictMode, systemUpdate);
            }
        }

        #endregion

        #region [ Protected Methods ]

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this.Context != null)
            {
                this.Context.Dispose();
                this.Context = null;
            }
        }

        protected virtual void OnChangeConflict(ChangeConflictCollection changeConflicts)
        {
            foreach (ObjectChangeConflict changeConflict in changeConflicts)
            {
                changeConflict.Resolve(RefreshMode.KeepChanges);
            }
        }

        protected virtual void OnSaveEntity(TContext context, EntityList<TEntity> list, TEntity entity)
        {
            if (!entity.Id.HasValue)
                entity.EntityState = EntityState.ToBeInserted;

            if (entity.EntityState == EntityState.Unchanged)
                return;

            if (context == this.Context)
            {
                if (!context.IsAttached(entity, SPList.Title))
                {
                    list.Attach(entity);
                }
            }
            else
            {
                list.Attach(entity);
            }
        }

        protected void DeleteEntityVersion(int entityId, int versionId)
        {
            SPListItem item = SPList.GetItemById(entityId);
            SPListItemVersion version = item.Versions.GetVersionFromLabel(versionId.ToString(CultureInfo.InvariantCulture));
            version.Delete();
        }

        protected TContext CreateContext()
        {
            return CreateContext(false);
        }

        protected TContext CreateContext(bool crossSite)
        {
            TContext context;

            if (crossSite)
            {
                HttpContext backupCtx = HttpContext.Current;
                HttpContext.Current = null;

                try
                {
                    context = GetContext();
                }
                finally
                {
                    HttpContext.Current = backupCtx;
                }
            }
            else
            {
                context = GetContext();
            }

            return context;
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Retrieving collection of entities
        /// </summary>
        public List<TEntity> GetAll()
        {
            return GetEntityCollection(null).ToList();
        }

        public List<TEntity> GetAll(Expression<Func<TEntity, object>> orderBy, bool descending = false)
        {
            return GetEntityCollection(null, string.Empty, true, orderBy, descending).ToList();
        }

        /// <summary>
        /// Retrieving collection of entities
        /// </summary>
        /// <param name="where">Predicate</param>
        public IQueryable<TEntity> GetEntityCollection(
            Expression<Func<TEntity, bool>> where)
        {
            return GetEntityCollection<object>(where, string.Empty, true, null, false, 0);
        }

        /// <summary>
        /// Retrieving collection of entities
        /// </summary>
        /// <param name="where">Predicate</param>
        /// <param name="path">Folder path</param>
        public IQueryable<TEntity> GetEntityCollection(
            Expression<Func<TEntity, bool>> where, string path)
        {
            return GetEntityCollection<object>(where, path, true, null, false, 0);
        }

        /// <summary>
        /// Retrieving collection of entities
        /// </summary>
        /// <param name="where">Predicate</param>
        /// <param name="path">Folder path</param>
        /// <param name="recursive">Recoursive</param>
        /// <param name="orderBy"></param>
        /// <param name="descending"></param>
        public IQueryable<TEntity> GetEntityCollection<TKey>(Expression<Func<TEntity, bool>> where,
            string path, bool recursive,
            Expression<Func<TEntity, TKey>> orderBy, bool descending = false)
        {
            return GetEntityCollection(where, path, recursive, orderBy, descending, 0);
        }

        /// <summary>
        /// Retrieving collection of entities
        /// </summary>
        /// <param name="descending"></param>
        /// <param name="path">Folder path</param>
        /// <param name="recursive">Recoursive</param>
        /// <param name="maxRows">Max items</param>
        /// <param name="where"></param>
        /// <param name="orderBy"></param>
        public IQueryable<TEntity> GetEntityCollection<TKey>(
           Expression<Func<TEntity, bool>> where, string path, bool recursive, Expression<Func<TEntity, TKey>> orderBy, bool descending, int maxRows)
        {
            IQueryable<TEntity> query = (where == null)
                                            ? List.ScopeToFolder(path, recursive)
                                            : List.ScopeToFolder(path, recursive).Where(where);

            if (orderBy != null)
            {
                query = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
            }

            if (maxRows > 0)
                query = query.Take(maxRows);
            return query;
        }

        /// <summary>
        /// Get entity
        /// </summary>
        /// <param name="id">Id</param>
        public TEntity GetEntity(int id)
        {
            return List.ScopeToFolder(string.Empty, true).SingleOrDefault(entry => entry.Id == id);
        }

        public TEntity GetEntity(Expression<Func<TEntity, bool>> where, string path = "", bool recursive = true)
        {
            return List.ScopeToFolder(path, recursive).FirstOrDefault(where);
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">entity</param>
        public void DeleteEntity(TEntity entity)
        {
            if (!entity.Id.HasValue)
                throw new ArgumentException("Entity has no identifier.");

            if (entity.IsCurrentVersion == true)
            {
                List.DeleteOnSubmit(entity);
            }
            else
            {
                if (!entity.Version.HasValue)
                    throw new ArgumentException("Entity version has no identifier.");

                DeleteEntityVersion(entity.Id.Value, entity.Version.Value);
            }
        }

        public void DeleteEntity(int id)
        {
            TEntity entity = List
                .ScopeToFolder(string.Empty, true)
                .FirstOrDefault(entry => entry.Id == id);

            if (entity != null)
            {
                DeleteEntity(entity);
            }
        }

        public void DeleteEntityCollection(IEnumerable<TEntity> entities)
        {
            List.DeleteAllOnSubmit(entities);
        }

        public void DeleteAll()
        {
            IQueryable<TEntity> entities = GetEntityCollection(entry => true);
            DeleteEntityCollection(entities);
            Commit();
        }

        /// <summary>
        /// Save entity
        /// </summary>
        /// <param name="entity">entity</param>
        public void SaveEntity(TEntity entity)
        {
            if (entity == null) { return; }

            OnSaveEntity(Context, List, entity);
        }

        public void SaveEntityNow(TEntity entity, ConflictMode conflictMode = ConflictMode.FailOnFirstConflict, bool systemUpdate = false)
        {
            if (entity == null) { return; }

            using (TContext context = CreateContext(CrossSite))
            {
                EntityList<TEntity> list = context.GetList<TEntity>(ListName);
                OnSaveEntity(context, list, entity);
                Commit(context, conflictMode, systemUpdate);
            }

            if (!this.Context.IsAttached(entity, ListName))
            {
                this.List.Attach(entity);
            }
        }

        public void Commit(ConflictMode conflictMode = ConflictMode.FailOnFirstConflict, bool systemUpdate = false)
        {
            Commit(Context, conflictMode, systemUpdate);
        }

        /// <summary>
        /// Lock entity
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="lockType">Lock type</param>
        /// <param name="timeSpan">Timeout</param>
        public void LockEntity(int id, SPFile.SPLockType lockType, TimeSpan timeSpan)
        {
            SPFile file = GetFile(id);

            if (file == null) { return; }

            if (file.InDocumentLibrary/*file.ListItemAllFields.ParentList is SPDocumentLibrary*/
                && file.LockType == SPFile.SPLockType.None)
            {
                file.Lock(lockType, Guid.NewGuid().ToString("N"), timeSpan);
            }
        }

        /// <summary>
        /// Release lock
        /// </summary>
        /// <param name="id">Id</param>
        public void ReleaseLockEntity(int id)
        {
            SPFile file = GetFile(id);

            if (file == null) { return; }

            if (file.LockType != SPFile.SPLockType.None)
            {
                file.ReleaseLock(file.LockId);
            }
        }

        /// <summary>
        /// Refresh lock (prologation)
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="timeSpan">Timeout</param>
        public void RefreshLockEntity(int id, TimeSpan timeSpan)
        {
            SPFile file = GetFile(id);

            if (file == null) { return; }

            if (file.LockType != SPFile.SPLockType.None)
            {
                file.RefreshLock(file.LockId, timeSpan);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Id</param>
        public EntityLockInfo GetLockInfo(int id)
        {
            SPFile file = GetFile(id);

            if (file == null) { return null; }

            EntityLockInfo info = new EntityLockInfo(file);
            return info;
        }

        public SPQuery GetSPQuery(IQueryable<TEntity> query)
        {
            if (Context == null) { throw new ArgumentException("Context"); }

            SPQuery res = null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (TextWriter writer = new StreamWriter(ms))
                {
                    Log = writer;
                    var items = query.Take(1).ToList();
                    Log = null;

                    string caml = Encoding.UTF8.GetString(ms.ToArray());

                    if (!string.IsNullOrEmpty(caml))
                    {
                        var doc = XDocument.Parse(caml);
                        var view = doc.Document.Element("View");
                        var spQuery = view.Element("Query");
                        var where = spQuery.Element("Where");
                        var orderBy = spQuery.Element("OrderBy");
                        var fields = view.Element("ViewFields");

                        res = new SPQuery
                            {
                                Query = string.Format("{0}{1}{2}",
                                                      (where == null ? string.Empty : where.ToString()),
                                                      Environment.NewLine,
                                                      (orderBy == null ? string.Empty : orderBy.ToString())),
                                ViewFields = fields.ToString(),
                                RowLimit = int.MaxValue
                            };
                    }
                }
            }
            return res;
        }

        public void ProcessListItem(IQueryable<TEntity> query, ContentIterator.ItemProcessor itemProcessor)
        {
            var caml = GetSPQuery(query);
            var iterator = new ContentIterator();
            iterator.ProcessListItems(SPList, caml, itemProcessor, (e, i) => true);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public DataTable GetDataTable(IEnumerable<TEntity> entities)
        {
            DataTable table = new DataTable(SPList.Title);

            PropertyInfo[] properties = typeof(TEntity).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var dicProperties = new Dictionary<PropertyInfo, ColumnAttribute>();

            foreach (PropertyInfo property in properties)
            {
                ColumnAttribute columnAttribute = property.GetCustomAttributes(typeof(ColumnAttribute), false).FirstOrDefault() as ColumnAttribute;

                if (columnAttribute != null)
                {
                    if (table.Columns.Contains(columnAttribute.Name))
                    {
                        table.Columns.Add(property.Name,
                            Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
                        dicProperties.Add(property, null);
                    }
                    else
                    {
                        table.Columns.Add(columnAttribute.Name,
                            Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
                        dicProperties.Add(property, columnAttribute);
                    }
                }
            }

            foreach (TEntity entity in entities)
            {
                DataRow row = table.NewRow();

                foreach (var dicProperty in dicProperties)
                {
                    PropertyInfo property = dicProperty.Key;
                    ColumnAttribute columnAttribute = dicProperty.Value;

                    if (columnAttribute == null)
                    {
                        row[property.Name] = property.GetValue(entity, null) ?? DBNull.Value;
                    }
                    else
                    {
                        row[columnAttribute.Name] = property.GetValue(entity, null) ?? DBNull.Value;
                    }
                }

                table.Rows.Add(row);
            }

            return table;
        }

        public SPFile GetFile(int id)
        {
            TEntity entity = GetEntity(id);
            return GetFile(entity);
        }

        public SPFile GetFile(TEntity entity)
        {
            if (entity == null)
            {
                return null;
            }

            if (!entity.Id.HasValue)
            {
                throw new ArgumentException("Entity has no id property value.");
            }

            SPWeb web = SPList.ParentWeb;
            SPFile file = web.GetFile(entity.ServerUrl);
            return file;
        }

        public SPListItem GetItem(int id)
        {
            TEntity entity = GetEntity(id);
            return GetItem(entity);
        }

        public SPListItem GetItem(TEntity entity)
        {
            if (entity == null)
            {
                return null;
            }

            if (!entity.Id.HasValue)
            {
                throw new ArgumentException("Entity has no id property value.");
            }

            SPListItem item = SPList.GetItemById(entity.Id.Value);
            return item;
        }

        public SPListItemVersionCollection GetVersions(int id)
        {
            SPListItem item = GetItem(id);

            if (item != null)
            {
                var versions = item.Versions;
                return versions;
            }

            return null;
        }

        public SPListItemVersionCollection GetVersions(TEntity entity)
        {
            if (entity.Id.HasValue)
            {
                return GetVersions(entity.Id.Value);
            }

            throw new ArgumentException("Entity has no id property value.");
        }

        #endregion
    }
}
