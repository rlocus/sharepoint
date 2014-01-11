using System;
using Microsoft.SharePoint.Linq;
using SPCore.Linq;

namespace SPCore.Examples
{
    /// <summary>
    /// Department of branch
    /// </summary>
    [ContentType(Name = "Department", Id = "0x01007236633166654687ABCECFED51DCA642")]
    public class Department : Item
    {
        //private int? _headOfId;
        //private string _headOfTitle;
        //private int? _branchId;
        //private string _branchTitle;
        //private readonly EntityRef<Employee> _headOf;
        //private readonly EntityRef<Branch> _branch;
        private readonly EntitySet<Employee> _employees;

        public Department()
            : base()
        {
            _employees = new EntitySet<Employee>();
            //_employees.OnSync += OnEmployeesSync;
            _employees.OnChanged += OnEmployeesChanged;
            _employees.OnChanging += OnEmployeesChanging;
            //_headOf = new EntityRef<Employee>();
            //_headOf.OnSync += OnHeadOfSync;
            //_headOf.OnChanged += OnHeadOfChanged;
            //_headOf.OnChanging += OnHeadOfChanging;
            //_branch = new EntityRef<Branch>();
            //_branch.OnSync += OnBranchSync;
            //_branch.OnChanged += OnBranchChanged;
            //_branch.OnChanging += OnBranchChanging;
        }

        /// <summary>
        /// Employees
        /// </summary>
        [Association(Name = "ExDepartment", Storage = "_employees", ReadOnly = true, MultivalueType = AssociationType.Backward, List = "Lists/Employees")]
        public EntitySet<Employee> Employees
        {
            get
            {
                return _employees;
            }
            protected set
            {
                _employees.Assign(value);
            }
        }

        ///// <summary>
        ///// Branch head-of Id
        ///// </summary>
        //[Column(Name = "HeadOf", Storage = "_headOfId", FieldType = "Lookup", IsLookupId = true)]
        //public int? HeadOfId
        //{
        //    get
        //    {
        //        return _headOfId;
        //    }
        //    set
        //    {
        //        if ((value == _headOfId)) return;
        //        OnPropertyChanging("HeadOfId", _headOfId);
        //        _headOfId = value;
        //        OnPropertyChanged("HeadOfId");
        //    }
        //}

        ///// <summary>
        ///// Branch head-of name
        ///// </summary>
        //[Column(Name = "HeadOf", Storage = "_headOfTitle", ReadOnly = true, FieldType = "Lookup", IsLookupValue = true)]
        //public string HeadOfTitle
        //{
        //    get
        //    {
        //        return _headOfTitle;
        //    }
        //    set
        //    {
        //        if ((value == _headOfTitle)) return;
        //        OnPropertyChanging("HeadOfTitle", _headOfTitle);
        //        _headOfTitle = value;
        //        OnPropertyChanged("HeadOfTitle");
        //    }
        //}

        ///// <summary>
        ///// Branch head-of
        ///// </summary>
        //[Association(Name = "HeadOf", Storage = "_headOf", MultivalueType = AssociationType.Single, List = "Lists/Employees")]
        //public Employee HeadOf
        //{
        //    get
        //    {
        //        return _headOf.GetEntity();
        //    }
        //    set
        //    {
        //        _headOf.SetEntity(value);
        //    }
        //}

        ///// <summary>
        ///// Parent branch Id
        ///// </summary>
        //[Column(Name = "Branch", Storage = "_branchId", FieldType = "Lookup", IsLookupId = true)]
        //public int? BranchId
        //{
        //    get
        //    {
        //        return _branchId;
        //    }
        //    set
        //    {
        //        if ((value == _branchId)) return;
        //        OnPropertyChanging("BranchId", _branchId);
        //        _branchId = value;
        //        OnPropertyChanged("BranchId");
        //    }
        //}

        ///// <summary>
        ///// Parent branch Title
        ///// </summary>
        //[Column(Name = "Branch", Storage = "_branchTitle", ReadOnly = true, FieldType = "Lookup", IsLookupValue = true)]
        //public string BranchTitle
        //{
        //    get
        //    {
        //        return _branchTitle;
        //    }
        //    set
        //    {
        //        if ((value != _branchTitle))
        //        {
        //            OnPropertyChanging("BranchTitle", _branchTitle);
        //            _branchTitle = value;
        //            OnPropertyChanged("BranchTitle");
        //        }
        //    }
        //}

        ///// <summary>
        ///// Parent Branch
        ///// </summary>
        //[Association(Name = "Branch", Storage = "_branch", MultivalueType = AssociationType.Single, List = "Lists/Branches")]
        //public Branch Branch
        //{
        //    get
        //    {
        //        return _branch.GetEntity();
        //    }
        //    set
        //    {
        //        _branch.SetEntity(value);
        //    }
        //}

        private void OnEmployeesChanging(object sender, EventArgs e)
        {
            OnPropertyChanging("Employees", _employees.Clone());
        }

        private void OnEmployeesChanged(object sender, EventArgs e)
        {
            OnPropertyChanged("Employees");
        }

        //private void OnEmployeesSync(object sender, AssociationChangedEventArgs<Employee> e)
        //{
        //    e.Item.Department = (AssociationChangedState.Added == e.State) ? this : null;
        //}

        //private void OnHeadOfChanging(object sender, EventArgs e)
        //{
        //    OnPropertyChanging("HeadOf", _headOf.Clone());
        //}

        //private void OnHeadOfChanged(object sender, EventArgs e)
        //{
        //    OnPropertyChanged("HeadOf");
        //}

        //private void OnHeadOfSync(object sender, AssociationChangedEventArgs<Employee> e)
        //{
        //    if ((AssociationChangedState.Added == e.State))
        //    {
        //        e.Item.Departments.Add(this);
        //    }
        //    else
        //    {
        //        e.Item.Departments.Remove(this);
        //    }
        //}

        //private void OnBranchChanging(object sender, EventArgs e)
        //{
        //    OnPropertyChanging("Branch", _headOf.Clone());
        //}

        //private void OnBranchChanged(object sender, EventArgs e)
        //{
        //    OnPropertyChanged("Branch");
        //}

        //private void OnBranchSync(object sender, AssociationChangedEventArgs<Branch> e)
        //{
        //    if ((AssociationChangedState.Added == e.State))
        //    {
        //        e.Item.Departments.Add(this);
        //    }
        //    else
        //    {
        //        e.Item.Departments.Remove(this);
        //    }
        //}
    }
}
