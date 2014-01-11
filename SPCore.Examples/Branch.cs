using System;
using Microsoft.SharePoint.Linq;
using SPCore.Linq;

namespace SPCore.Examples
{
    /// <summary>
    /// Branch of company
    /// </summary>
    [ContentType(Name = "Branch", Id = "0x01007087DA17F20149E3A4602E517E6E8EEF")]
    public class Branch : Item
    {
        private string _postAddress;
        private string _city;
        private int? _companyInId;
        private string _companyInTitle;

        private readonly EntityRef<Company> _company;
        private readonly EntitySet<Department> _departments;

        public Branch()
        {
            _departments = new EntitySet<Department>();
            //_departments.OnSync += OnDepartmentsSync;
            _departments.OnChanged += OnDepartmentsChanged;
            _departments.OnChanging += OnDepartmentsChanging;
            _company = new EntityRef<Company>();
            _company.OnSync += OnCompanySync;
            _company.OnChanged += OnCompanyChanged;
            _company.OnChanging += OnCompanyChanging;
        }

        /// <summary>
        /// Branch Post address
        /// </summary>
        [Column(Name = "PostAddress", Storage = "_postAddress", FieldType = "Note")]
        public string PostAddress
        {
            get
            {
                return _postAddress;
            }
            set
            {
                if ((value == _postAddress)) return;
                OnPropertyChanging("PostAddress", _postAddress);
                _postAddress = value;
                OnPropertyChanged("PostAddress");
            }
        }

        /// <summary>
        /// Branch City
        /// </summary>
        [Column(Name = "City", Storage = "_city", FieldType = "Text")]
        public string City
        {
            get
            {
                return _city;
            }
            set
            {
                if ((value == _city)) return;
                OnPropertyChanging("City", _city);
                _city = value;
                OnPropertyChanged("City");
            }
        }

        /// <summary>
        /// Id of company owning branch
        /// </summary>
        [Column(Name = "CompanyIn", Storage = "_companyInId", FieldType = "Lookup", IsLookupId = true)]
        public int? CompanyInId
        {
            get
            {
                return _companyInId;
            }
            set
            {
                if ((value != _companyInId))
                {
                    OnPropertyChanging("CompanyInId", _companyInId);
                    _companyInId = value;
                    OnPropertyChanged("CompanyInId");
                }
            }
        }

        /// <summary>
        /// Name of company owning branch
        /// </summary>
        [Column(Name = "CompanyIn", Storage = "_companyInTitle", ReadOnly = true, FieldType = "Lookup", IsLookupValue = true)]
        public string CompanyInTitle
        {
            get
            {
                return _companyInTitle;
            }
            set
            {
                if ((value != _companyInTitle))
                {
                    OnPropertyChanging("CompanyInTitle", _companyInTitle);
                    _companyInTitle = value;
                    OnPropertyChanged("CompanyInTitle");
                }
            }
        }

        /// <summary>
        /// Company owning branch
        /// </summary>
        [Association(Name = "Branch", Storage = "_company", MultivalueType = AssociationType.Single, List = "Lists/Companies")]
        public Company Company
        {
            get
            {
                return _company.GetEntity();
            }
            set
            {
                _company.SetEntity(value);
            }
        }

        /// <summary>
        /// Departments of branch
        /// </summary>
        [Association(Name = "Branch", Storage = "_departments", ReadOnly = true, MultivalueType = AssociationType.Backward, List = "Lists/Department")]
        public EntitySet<Department> Departments
        {
            get
            {
                return _departments;
            }
            set
            {
                _departments.Assign(value);
            }
        }

        private void OnDepartmentsChanging(object sender, EventArgs e)
        {
            OnPropertyChanging("Departments", _departments.Clone());
        }

        private void OnDepartmentsChanged(object sender, EventArgs e)
        {
            OnPropertyChanged("Departments");
        }

        //private void OnDepartmentsSync(object sender, AssociationChangedEventArgs<Department> e)
        //{
        //    e.Item.Branch = (AssociationChangedState.Added == e.State) ? this : null;
        //}

        private void OnCompanyChanging(object sender, EventArgs e)
        {
            OnPropertyChanging("Company", _company.Clone());
        }

        private void OnCompanyChanged(object sender, EventArgs e)
        {
            OnPropertyChanged("Company");
        }

        private void OnCompanySync(object sender, AssociationChangedEventArgs<Company> e)
        {
            if ((AssociationChangedState.Added == e.State))
            {
                e.Item.Branches.Add(this);
            }
            else
            {
                e.Item.Branches.Remove(this);
            }
        }
    }

}
