using Microsoft.SharePoint.Linq;
using SPCore.Linq;

namespace SPCore.Examples
{
    public class EmployeeRepository<TEntity, TContext> : BaseRepository<TEntity, TContext>
        where TEntity : Employee, new()
        where TContext : EmployeeDataContext
    {
        public EmployeeRepository(string listName)
            : base(listName)
        {
        }

        public EmployeeRepository(string listName, bool readOnly)
            : base(listName, readOnly)
        {
        }

        public EmployeeRepository(string listName, string webUrl)
            : base(listName, webUrl)
        {
        }

        public EmployeeRepository(string listName, string webUrl, bool readOnly)
            : base(listName, webUrl, readOnly)
        {
        }

        public EmployeeRepository(string listName, string webUrl, bool readOnly, bool crossSite)
            : base(listName, webUrl, readOnly, crossSite)
        {
        }

        protected override void OnSaveEntity(TContext context, EntityList<TEntity> list, TEntity entity)
        {
            if (entity.Manager != null)
            {
                if (entity.Manager.Id == null)
                {
                    context.Employees.InsertOnSubmit(entity.Manager);
                }
                else if (!context.IsAttached(entity.Manager, "Lists/Employees"))
                {
                    context.Employees.Attach(entity.Manager);
                }
            }

            if (entity.Department != null)
            {
                if (entity.Department.Id == null)
                {
                    context.Departments.InsertOnSubmit(entity.Department);
                }
                else if (!context.IsAttached(entity.Department, "Lists/Department"))
                {
                    context.Departments.Attach(entity.Department);
                }
            }

            base.OnSaveEntity(context, list, entity);
        }
    }
}
