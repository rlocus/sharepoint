using System;
using Microsoft.SharePoint.Linq;
using SPCore.Linq;

namespace SPCore.Examples
{
    /// <summary>
    /// Company
    /// </summary>
    [ContentType(Name = "Company", Id = "0x010065D24F25BFAD4E9CBAF3BE9BAB46A499")]
    public class Company : Item
    {
        private string _webSite;
        private string _logoType;
        private readonly EntitySet<Branch> _branches;

        public Company()
        {
            _branches = new EntitySet<Branch>();
            _branches.OnSync += OnBranchesSync;
            _branches.OnChanged += OnBranchesChanged;
            _branches.OnChanging += OnBranchesChanging;
        }

        /// <summary>
        /// Company site URL
        /// </summary>
        [Column(Name = "WebSite", Storage = "_webSite", FieldType = "Url")]
        public string WebSite
        {
            get
            {
                return _webSite;
            }
            set
            {
                if ((value == _webSite)) return;
                OnPropertyChanging("WebSite", _webSite);
                _webSite = value;
                OnPropertyChanged("WebSite");
            }
        }

        /// <summary>
        /// Company logo URL
        /// </summary>
        [Column(Name = "LogoType", Storage = "_logoType", FieldType = "Url")]
        public string LogoType
        {
            get
            {
                return _logoType;
            }
            set
            {
                if ((value == _logoType)) return;
                OnPropertyChanging("LogoType", _logoType);
                _logoType = value;
                OnPropertyChanged("LogoType");
            }
        }

        /// <summary>
        /// Branches of company
        /// </summary>
        [Association(Name = "CompanyIn", Storage = "_branches", ReadOnly = true, MultivalueType = AssociationType.Backward, List = "Branches")]
        public EntitySet<Branch> Branches
        {
            get
            {
                return _branches;
            }
            set
            {
                _branches.Assign(value);
            }
        }

        private void OnBranchesChanging(object sender, EventArgs e)
        {
            OnPropertyChanging("Branches", _branches.Clone());
        }

        private void OnBranchesChanged(object sender, EventArgs e)
        {
            OnPropertyChanged("Branches");
        }

        private void OnBranchesSync(object sender, AssociationChangedEventArgs<Branch> e)
        {
            e.Item.Company = (AssociationChangedState.Added == e.State) ? this : null;
        }
    }
}
