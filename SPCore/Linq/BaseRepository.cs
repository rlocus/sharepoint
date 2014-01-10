using System;
using System.Collections.Generic;
using System.Data;
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

        protected TContext Context { get; private set; }

        private EntityList<TEntity> _list;

        protected EntityList<TEntity> List
        {
            get
            {
                if (Context == null)
                {
                    throw new ArgumentNullException("Context");
                }

                return _list ?? (_list = GetList(Context, ListName));
            }
        }

        /// <summary>
        /// EntityList meta data
        /// </summary>
        protected EntityListMetaData MetaData
        {
            get
            {
                if (Context == null)
                {
                    throw new ArgumentNullException("Context");
                }

                return GetList(Context, ListName).GetMetaData();
            }
        }

        protected virtual SPList SPList
        {
            get { return _spList ?? (_spList = MetaData.List ?? this.Context.LatestList); }
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
            if (context == null) { throw new ArgumentException("Context"); }

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

        #region [ Helper Methods ]
        /// <summary>
        /// Check entity is attached to context
        /// </summary>
        /// <param name="context"></param>
        /// <param name="entity">entity</param>
        /// <param name="listName"></param>
        /// <returns>true - attached, false - is not attached</returns>
        static bool EntityExistsInContext(TContext context, TEntity entity, string listName)
        {
            //if (context == null) { throw new ArgumentException("Context"); }

            Type type = context.GetType();
            PropertyInfo pi = type.GetProperty("EntityTracker", BindingFlags.NonPublic | BindingFlags.Instance);
            var val = pi.GetValue(context, null);
            Type trackerType = val.GetType();
            Type eidType =
                Type.GetType("Microsoft.SharePoint.Linq.EntityId, " + typeof(DataContext).Assembly.FullName);
            var eid = Activator.CreateInstance(eidType, context.Web, listName);
            MethodInfo mi = trackerType.GetMethod("TryGetId", BindingFlags.Public | BindingFlags.Instance);
            var res = mi.Invoke(val, new[] { entity, eid });
            return (bool)res;
        }

        static bool IsAttached(TContext context, TEntity entity, string listName)
        {
            if (context == null) throw new ArgumentNullException("context");

            return EntityExistsInContext(context, entity, listName);
        }

        static EntityList<TEntity> GetList(TContext context, string listName)
        {
            if (context == null) throw new ArgumentNullException("context");

            return context.GetList<TEntity>(listName);
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

        //protected override void OnSaveEntity(TContext context, EntityList<TEntity> list, TEntity entity)
        //{
        //    if (entity.Manager != null)
        //    {
        //        if (entity.Manager.Id == null)
        //        {
        //            this.SaveEntityNow((TEntity)entity.Manager);

        //            if (list != this.List)
        //            {
        //                list.Attach((TEntity)entity.Manager);
        //            }
        //        }
        //        else
        //        {
        //            if (list != this.List)
        //            {
        //                list.Attach((TEntity)entity.Manager);
        //            }

        //            context.Refresh(RefreshMode.KeepCurrentValues, entity.Manager);
        //        }
        //    }

        //    base.OnSaveEntity(context, list, entity);
        //}

        protected virtual void OnSaveEntity(TContext context, EntityList<TEntity> list, TEntity entity)
        {
            if (!entity.Id.HasValue)
                entity.EntityState = EntityState.ToBeInserted;

            if (entity.EntityState == EntityState.Unchanged)
                return;

            if (context == this.Context)
            {
                if (!IsAttached(context, entity, SPList.Title))
                {
                    list.Attach(entity);
                }
            }
            else
            {
                list.Attach(entity);
            }
        }

        //protected void DeleteEntityVersion(int entityId, int versionId)
        //{
        //    SPListItem item = SPList.GetItemById(entityId);
        //    SPListItemVersion version = item.Versions.GetVersionFromLabel(versionId.ToString(CultureInfo.InvariantCulture));
        //    version.Delete();
        //}

        //protected IEnumerable<TEntity> GetVersions(int id)
        //{
        //    var item = SPList.GetItemById(id);
        //    var versions = item.Versions
        //        .Cast<SPListItemVersion>()
        //        .Select(v => Activator.CreateInstance(typeof(TEntity), v))
        //        .Cast<TEntity>()
        //        .ToList();
        //    return versions;
        //}

        protected bool IsAttached(TContext context, TEntity entity)
        {
            if (context == null) throw new ArgumentNullException("context");

            return EntityExistsInContext(context, entity, SPList.Title);
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
            return List
                .ScopeToFolder(string.Empty, true)
                .SingleOrDefault(entry => entry.Id == id);
        }

        public TEntity GetEntity(Expression<Func<TEntity, bool>> where, string path = "", bool recursive = true)
        {
            return List
                .ScopeToFolder(path, recursive)
                .FirstOrDefault(where);
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">entity</param>
        public void DeleteEntity(TEntity entity)
        {
            //if (Context == null) { throw new ArgumentException("Context"); }

            if (!entity.Id.HasValue)
                throw new ArgumentException("Entity has no identifier.");

            if (entity.IsCurrentVersion == true)
            {
                List.DeleteOnSubmit(entity);
            }
            //else
            //{
            //    if (!entity.Version.HasValue)
            //        throw new ArgumentException("Entity version has no identifier.");

            //    DeleteEntityVersion(entity.Id.Value, entity.Version.Value);
            //}
        }

        public void DeleteEntity(int id)
        {
            //if (Context == null) { throw new ArgumentException("Context"); }

            IQueryable<TEntity> query = List
                .ScopeToFolder(string.Empty, true)
                .Where(entry => entry.Id == id);

            TEntity entity = query.FirstOrDefault();

            if (entity != null)
            {
                DeleteEntity(entity);
            }
        }

        public void DeleteEntityCollection(IEnumerable<TEntity> entities)
        {
            //if (Context == null) { throw new ArgumentException("Context"); }

            List.DeleteAllOnSubmit(entities);
        }

        public void DeleteAll()
        {
            //if (Context == null) { throw new ArgumentException("Context"); }

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
            //if (Context == null) { throw new ArgumentException("Context"); }
            OnSaveEntity(Context, List, entity);
        }

        public void SaveEntityNow(TEntity entity, ConflictMode conflictMode = ConflictMode.FailOnFirstConflict, bool systemUpdate = false)
        {
            using (TContext context = CreateContext(CrossSite))
            {
                EntityList<TEntity> list = GetList(context, ListName);
                OnSaveEntity(context, list, entity);
                Commit(context, conflictMode, systemUpdate);
            }

            if (!IsAttached(this.Context, entity))
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

        public TContext CreateContext()
        {
            return CreateContext(false);
        }

        public TContext CreateContext(bool crossSite)
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
