using System;
using System.IO;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Linq;
using SPCore.Linq;

namespace SPCore.Examples
{
    /// <summary>
    /// Employee
    /// </summary>
    [ContentType(Name = "Employee", Id = "0x010078B0DD38574940478CF9E129FCD65E9B")]
    public class Employee : Item
    {
        //private string _sexValue;
        private EmployeeSex _sex;
        private string _cellPhone;
        private int? _accessLevel;
        private readonly EntityRef<Employee> _manager;
        private int? _managerId;
        private string _managerTitle;
        private EmployeeHobby? _hobbies;
        private readonly EntityRef<Department> _department;
        private int? _departmentId;
        private string _departmentTitle;
        private readonly EntitySet<Employee> _managers;
        private readonly EntitySet<Department> _departments;
        //private string _barCodeValue;
        //private string _barCodeUrl;

        /// <summary>
        /// Blank employee
        /// </summary>
        public Employee()
            : base()
        {
            _managers = new EntitySet<Employee>();
            _managers.OnSync += OnManagersSync;
            _managers.OnChanged += OnManagersChanged;
            _managers.OnChanging += OnManagersChanging;
            _departments = new EntitySet<Department>();
            _departments.OnSync += OnDepartmentsSync;
            _departments.OnChanged += OnDepartmentsChanged;
            _departments.OnChanging += OnDepartmentsChanging;
            _manager = new EntityRef<Employee>();
            _manager.OnSync += OnManagerSync;
            _manager.OnChanged += OnManagerChanged;
            _manager.OnChanging += OnManagerChanging;
            _department = new EntityRef<Department>();
            _department.OnSync += OnDepartmentSync;
            _department.OnChanged += OnDepartmentChanged;
            _department.OnChanging += OnDepartmentChanging;
        }

        [Column(Name = "Sex", Storage = "_sex", FieldType = "Choice")]
        public EmployeeSex? Sex
        {
            get { return this._sex; }
            set
            {
                if (value == this._sex) return;

                this.OnPropertyChanging("Sex", this._sex);

                if (value != null) this._sex = (EmployeeSex)value;

                this.OnPropertyChanged("Sex");
            }
        }

        [Column(Name = "ExHobbies", Storage = "_hobbies", FieldType = "MultiChoice")]
        public EmployeeHobby? Hobbies
        {
            get { return this._hobbies; }
            set
            {
                if ((value == this._hobbies)) return;

                this.OnPropertyChanging("Hobbies", this._hobbies);
                this._hobbies = value;
                this.OnPropertyChanged("Hobbies");
            }
        }

        ///// <summary>
        ///// Sex of employee
        ///// </summary>
        //public EmployeeSex? Sex
        //{
        //    get
        //    {
        //        var res = Enum.Parse(typeof(EmployeeSex), _sexValue);
        //        return res is EmployeeSex ? (EmployeeSex)res : EmployeeSex.Invalid;
        //    }
        //}

        ///// <summary>
        ///// Sex of employee
        ///// </summary>
        //[Column(Name = "Sex", Storage = "_sexValue", FieldType = "Choice")]
        //public string SexValue
        //{
        //    get
        //    {
        //        return _sexValue;
        //    }
        //    set
        //    {
        //        if ((value == _sexValue)) return;
        //        var vals = Enum.GetValues(typeof(EmployeeSex));
        //        foreach (EmployeeSex val in vals)
        //        {
        //            if (!string.Equals(Enum.GetName(typeof(EmployeeSex), val), value,
        //                                StringComparison.InvariantCultureIgnoreCase)) continue;
        //            OnPropertyChanging("SexValue", _sexValue);
        //            _sexValue = value;
        //            OnPropertyChanged("SexValue");
        //        }
        //    }
        //}

        /// <summary>
        /// CellPhone
        /// </summary>
        [Column(Name = "ExCellPhone", Storage = "_cellPhone", FieldType = "Text")]
        public string CellPhone
        {
            get
            {
                return _cellPhone;
            }
            set
            {
                if (value == _cellPhone) return;

                OnPropertyChanging("CellPhone", _cellPhone);
                _cellPhone = value;
                OnPropertyChanged("CellPhone");
            }
        }

        /// <summary>
        /// AccessLevel
        /// </summary>
        [Column(Name = "AccessLevel", Storage = "_accessLevel", FieldType = "Integer")]
        public int? AccessLevel
        {
            get
            {
                return _accessLevel;
            }
            set
            {
                if (value == _accessLevel) return;

                OnPropertyChanging("AccessLevel", _accessLevel);
                _accessLevel = value;
                OnPropertyChanged("AccessLevel");
            }
        }

        /// <summary>
        /// Employee's manager Id
        /// </summary>
        [Column(Name = "Manager", Storage = "_managerId", FieldType = "Lookup", IsLookupId = true)]
        public int? ManagerId
        {
            get
            {
                return _managerId;
            }
            set
            {
                if (value == _managerId) return;

                OnPropertyChanging("ManagerId", _managerId);
                _managerId = value;
                OnPropertyChanged("ManagerId");
            }
        }

        /// <summary>
        /// Employee's manager Name
        /// </summary>
        [Column(Name = "Manager", Storage = "_managerTitle", ReadOnly = true, FieldType = "Lookup", IsLookupValue = true)]
        public string ManagerTitle
        {
            get
            {
                return _managerTitle;
            }
            protected set
            {
                if (value == _managerTitle) return;

                OnPropertyChanging("ManagerTitle", _managerTitle);
                _managerTitle = value;
                OnPropertyChanged("ManagerTitle");
            }
        }

        /// <summary>
        /// Employee's manager
        /// </summary>
        [Association(Name = "Manager", Storage = "_manager", MultivalueType = AssociationType.Single, List = "Lists/Employees")]
        public Employee Manager
        {
            get
            {
                return _manager.GetEntity();
            }
            set
            {
                _manager.SetEntity(value);
            }
        }

        /// <summary>
        /// Employee's subordinates
        /// </summary>
        [Association(Name = "Manager", Storage = "_managers", ReadOnly = true, MultivalueType = AssociationType.Backward, List = "Lists/Employees")]
        public EntitySet<Employee> Managers
        {
            get
            {
                return _managers;
            }
            protected set
            {
                _managers.Assign(value);
            }
        }

        ///// <summary>
        ///// Departments managed by employee
        ///// </summary>
        //[Association(Name = "HeadOf", Storage = "_departments", ReadOnly = true, MultivalueType = AssociationType.Backward, List = "Lists/Department")]
        //public EntitySet<Department> Departments
        //{
        //    get
        //    {
        //        return _departments;
        //    }
        //    set
        //    {
        //        _departments.Assign(value);
        //    }
        //}

      
        [Column(Name = "ExDepartment", Storage = "_departmentId", FieldType = "Lookup", IsLookupId = true)]
        public int? DepartmentId
        {
            get
            {
                return _departmentId;
            }
            set
            {
                if (value == _departmentId) return;

                OnPropertyChanging("DepartmentId", _departmentId);
                _departmentId = value;
                OnPropertyChanged("DepartmentId");
            }
        }

        /// <summary>
        /// Parent department Title
        /// </summary>
        [Column(Name = "ExDepartment", Storage = "_departmentTitle", ReadOnly = true, FieldType = "Lookup", IsLookupValue = true)]
        public string DepartmentTitle
        {
            get
            {
                return _departmentTitle;
            }
            protected set
            {
                if (value == _departmentTitle) return;

                OnPropertyChanging("DepartmentTitle", _departmentTitle);
                _departmentTitle = value;
                OnPropertyChanged("DepartmentTitle");
            }
        }

        /// <summary>
        /// Parent department
        /// </summary>
        [Association(Name = "ExDepartment", Storage = "_department", MultivalueType = AssociationType.Single, List = "Lists/Department")]
        public Department Department
        {
            get
            {
                return _department.GetEntity();
            }
            set
            {
                _department.SetEntity(value);
            }
        }

        ///// <summary>
        ///// Barcode value
        ///// </summary>
        //[Column(Name = "_dlc_BarcodeValue", Storage = "_barCodeValue", ReadOnly = true, FieldType = "Text")]
        //public string BarCodeValue
        //{
        //    get
        //    {
        //        return _barCodeValue;
        //    }
        //}

        ///// <summary>
        ///// Barcode image URL
        ///// </summary>
        //[Column(Name = "_dlc_BarcodePreview", Storage = "_barCodeUrl", ReadOnly = true, FieldType = "Url")]
        //public string BarCodeURL
        //{
        //    get
        //    {
        //        return _barCodeUrl;
        //    }
        //}

        /// <summary>
        /// Return employee' name
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Title;
        }

        private void OnManagersChanging(object sender, EventArgs e)
        {
            OnPropertyChanging("Managers", _managers.Clone());
        }

        private void OnManagersChanged(object sender, EventArgs e)
        {
            OnPropertyChanged("Managers");
        }

        private void OnManagersSync(object sender, AssociationChangedEventArgs<Employee> e)
        {
            e.Item.Manager = (AssociationChangedState.Added == e.State) ? this : null;
        }

        private void OnDepartmentsChanging(object sender, EventArgs e)
        {
            OnPropertyChanging("Departments", _managers.Clone());
        }

        private void OnDepartmentsChanged(object sender, EventArgs e)
        {
            OnPropertyChanged("Departments");
        }

        private void OnDepartmentsSync(object sender, AssociationChangedEventArgs<Department> e)
        {
            //e.Item.HeadOf = (AssociationChangedState.Added == e.State) ? this : null;
        }

        private void OnManagerChanging(object sender, EventArgs e)
        {
            OnPropertyChanging("Manager", _manager.Clone());
        }

        private void OnManagerChanged(object sender, EventArgs e)
        {
            OnPropertyChanged("Manager");
        }

        private void OnManagerSync(object sender, AssociationChangedEventArgs<Employee> e)
        {
            if ((AssociationChangedState.Added == e.State))
            {
                e.Item.Managers.Add(this);
            }
            else
            {
                e.Item.Managers.Remove(this);
            }
        }

        private void OnDepartmentChanging(object sender, EventArgs e)
        {
            OnPropertyChanging("Department", _department.Clone());
        }

        private void OnDepartmentChanged(object sender, EventArgs e)
        {
            OnPropertyChanged("Department");
        }

        private void OnDepartmentSync(object sender, AssociationChangedEventArgs<Department> e)
        {
            if ((AssociationChangedState.Added == e.State))
            {
                e.Item.Employees.Add(this);
            }
            else
            {
                e.Item.Employees.Remove(this);
            }
        }
    }

    public enum EmployeeSex
    {
        None = 0,
        Invalid = 1,
        [Choice(Value = "Male")]
        Male = 2,
        [Choice(Value = "Female")]
        Female = 4
    }

    [Flags]
    public enum EmployeeHobby
    {
        None = 0,
        Invalid = 1,
        [Choice(Value = "Chess")]
        Chess = 2,
        [Choice(Value = "Football")]
        Football = 4,
        [Choice(Value = "Basketball")]
        Basketball = 8
    }
}