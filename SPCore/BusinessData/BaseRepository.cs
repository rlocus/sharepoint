using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using RefreshMode = System.Data.Linq.RefreshMode;

namespace SPCore.BusinessData
{
    public abstract class BaseRepository<TEntity, TContext> : IDisposable
        where TEntity : BaseEntity
        where TContext : DataContext
    {
        #region [ Fields ]
        protected readonly bool ReadOnly;
        #endregion

        #region [ Properties ]
        protected TContext Context { get; private set; }
        protected Table<TEntity> Table { get; private set; }
        protected TextWriter Log { get { return Context.Log; } set { Context.Log = value; } }
        #endregion

        #region [ Constructors ]
        protected BaseRepository(IDbConnection connection, bool readOnly = true)
            : this(connection, new AttributeMappingSource(), readOnly)
        { }

        protected BaseRepository(string stringConnection, bool readOnly = true)
            : this(stringConnection, new AttributeMappingSource(), readOnly)
        { }

        protected BaseRepository(IDbConnection connection, MappingSource mapping, bool readOnly = true)
        {
            ReadOnly = readOnly;
            Context = (TContext)Activator.CreateInstance(typeof(TContext),
                        new object[] { connection, mapping });
            Context.DeferredLoadingEnabled = Context.ObjectTrackingEnabled = !ReadOnly;
            Table = GetTable(Context);
        }

        protected BaseRepository(string stringConnection, MappingSource mapping, bool readOnly = true)
        {
            ReadOnly = readOnly;
            Context = (TContext)Activator.CreateInstance(typeof(TContext),
                        new object[] { stringConnection, mapping });
            Context.ObjectTrackingEnabled = !ReadOnly;
            Table = GetTable(Context);
        }
        #endregion

        #region [ Destructor ]

        ~BaseRepository()
        {
            Dispose(true);
        }

        #endregion

        private void Commit(TContext context, ConflictMode conflictMode)
        {
            if (context == null) { throw new ArgumentException("Context"); }

            ChangeSet changeSet = context.GetChangeSet();

            if (changeSet.Inserts.Count > 0 || changeSet.Updates.Count > 0 || changeSet.Deletes.Count > 0)
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
                    using (TransactionScope scope = new TransactionScope())
                    {
                        context.SubmitChanges(conflictMode);
                        scope.Complete();
                    }
                }
            }
        }

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

        //protected virtual TEntity GetByKey<TKey>(TKey key, string qualifiedEntitySetName, string keyName)
        //{
        //    // Build entity key
        //    EntityKey entityKey = new EntityKey(qualifiedEntitySetName, keyName, key);
        //    // Query first current state manager and if entity is not found query database!!!
        //    return (TEntity)Context.GetObjectByKey(entityKey);
        //}

        #endregion

        #region [ Public Methods ]

        public TEntity GetEntity(Expression<Func<TEntity, bool>> where)
        {
            if (Context == null) { throw new ArgumentException("Context"); }

            TEntity entity = Table.FirstOrDefault(where);
            return entity;
        }

        public List<TEntity> GetAll()
        {
            return GetEntityCollection(null).ToList();
        }

        public List<TEntity> GetAll(Expression<Func<TEntity, object>> orderBy, bool descending = false)
        {
            return GetEntityCollection(null, orderBy, descending).ToList();
        }

        public IQueryable<TEntity> GetEntityCollection(Expression<Func<TEntity, bool>> where)
        {
            return GetEntityCollection<object>(where, null, false, 0);
        }

        public IQueryable<TEntity> GetEntityCollection<TKey>(Expression<Func<TEntity, bool>> where,
            Expression<Func<TEntity, TKey>> orderBy, bool descending = false)
        {
            return GetEntityCollection(where, orderBy, descending, 0);
        }

        public IQueryable<TEntity> GetEntityCollection<TKey>(
           Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TKey>> orderBy, bool descending, int maxRows)
        {
            if (Context == null) { throw new ArgumentException("Context"); }

            IQueryable<TEntity> query = where == null ? Table.AsQueryable() : Table.Where(where);

            if (orderBy != null)
            {
                query = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
            }

            if (maxRows > 0)
                query = query.Take(maxRows);
            return query;
        }

        //public bool ExistsEntity(TEntity entity)
        //{
        //    if (Context == null) { throw new ArgumentException("Context"); }

        //    return Table.Contains(entity);
        //}

        private void InsertEntity(TEntity entity, bool safely = false)
        {
            if (safely)
            {
                InsertEntitySafely(entity);
                return;
            }

            if (Context == null) { throw new ArgumentException("Context"); }

            Table.InsertOnSubmit(entity);
        }

        private void InsertEntitySafely(TEntity entity)
        {
            using (TContext ctx = CreateContext())
            {
                Table<TEntity> table = GetTable(ctx);
                table.InsertOnSubmit(entity);
                Commit(ctx, ConflictMode.FailOnFirstConflict);
            }
        }

        public void DeleteAll()
        {
            if (Context == null) { throw new ArgumentException("Context"); }

            IQueryable<TEntity> entities = GetEntityCollection(null);
            DeleteEntityCollection(entities);
            Commit();
        }

        public void DeleteEntity(TEntity entity)
        {
            if (Context == null) { throw new ArgumentException("Context"); }

            Table.DeleteOnSubmit(entity);
        }

        public void DeleteEntityCollection(IEnumerable<TEntity> entities)
        {
            if (Context == null) { throw new ArgumentException("Context"); }

            Table.DeleteAllOnSubmit(entities);
        }

        public string GetQuery(IQueryable<TEntity> query)
        {
            if (Context == null) { throw new ArgumentException("Context"); }

            return Context.GetCommand(query.Take(1)).CommandText;
        }

        public void SaveEntity(TEntity entity, bool safely = false)
        {
            if (safely)
            {
                SaveEntitySafely(entity);
                return;
            }

            if (Context == null) { throw new ArgumentException("Context"); }

            if (Exists(Table, entity))
            {
                if (!IsAttached(Table, entity))
                {
                    Table.Attach(entity);
                    Context.Refresh(RefreshMode.KeepCurrentValues, entity);
                }
                else
                {
                    //TODO:
                }
            }
            else
            {
                InsertEntity(entity);
            }
        }

        private void SaveEntitySafely(TEntity entity)
        {
            using (TContext ctx = CreateContext())
            {
                Table<TEntity> table = GetTable(ctx);

                if (Exists(table, entity))
                {
                    if (!IsAttached(table, entity))
                    {
                        table.Attach(entity);
                        ctx.Refresh(RefreshMode.KeepCurrentValues, entity);
                    }

                    Commit(ctx, ConflictMode.FailOnFirstConflict);
                }
                else
                {
                    InsertEntity(entity, true);
                }
            }
        }

        public void Commit(ConflictMode conflictMode = ConflictMode.FailOnFirstConflict)
        {
            Commit(Context, conflictMode);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public TContext CreateContext()
        {
            if (Context == null) { throw new ArgumentException("Context"); }

            TContext context = (TContext)Activator.CreateInstance(typeof(TContext),
                        new object[] { Context.Connection, Context.Mapping.MappingSource });
            context.DeferredLoadingEnabled = context.ObjectTrackingEnabled = !ReadOnly;
            return context;
        }

        static bool Exists(Table<TEntity> table, TEntity entity)
        {
            return (table.Where(e => e == entity).Count() > 0);
        }

        static bool IsAttached(Table<TEntity> table, TEntity entity)
        {
            return (table.GetOriginalEntityState(entity) != null);
        }

        static Table<TEntity> GetTable(TContext dataContext)
        {
            return dataContext.GetTable<TEntity>();
        }

        #endregion
    }
}
