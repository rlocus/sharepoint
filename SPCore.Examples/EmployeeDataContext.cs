using Microsoft.SharePoint.Linq;
using SPCore.Linq;

namespace SPCore.Examples
{
    public class EmployeeDataContext : EntityDataContext
    {
        public EmployeeDataContext(string requestUrl) : base(requestUrl)
        {
        }

        [List(Name = "Lists/Employees")]
        public EntityList<Employee> Employees
        {
            get
            {
                return this.GetList<Employee>("Lists/Employees");
            }
        }

        [List(Name = "Lists/Department")]
        public EntityList<Department> Departments
        {
            get
            {
                return this.GetList<Department>("Lists/Department");
            }
        }
    }
}
