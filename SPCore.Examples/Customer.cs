using SPCore.BusinessData;

namespace SPCore.Examples
{
    [global::System.Data.Linq.Mapping.TableAttribute(Name = "dbo.Customers")]
    public partial class Customer : BaseEntity
    {
        private string _customerId;

        private string _companyName;

        private string _contactName;

        private string _contactTitle;

        private string _address;

        private string _city;

        private string _region;

        private string _postalCode;

        private string _country;

        private string _phone;

        private string _fax;

        #region Extensibility Method Definitions
        partial void OnCreated();
        partial void OnCustomerIDChanging(string value);
        partial void OnCustomerIDChanged();
        partial void OnCompanyNameChanging(string value);
        partial void OnCompanyNameChanged();
        partial void OnContactNameChanging(string value);
        partial void OnContactNameChanged();
        partial void OnContactTitleChanging(string value);
        partial void OnContactTitleChanged();
        partial void OnAddressChanging(string value);
        partial void OnAddressChanged();
        partial void OnCityChanging(string value);
        partial void OnCityChanged();
        partial void OnRegionChanging(string value);
        partial void OnRegionChanged();
        partial void OnPostalCodeChanging(string value);
        partial void OnPostalCodeChanged();
        partial void OnCountryChanging(string value);
        partial void OnCountryChanged();
        partial void OnPhoneChanging(string value);
        partial void OnPhoneChanged();
        partial void OnFaxChanging(string value);
        partial void OnFaxChanged();
        #endregion

        [global::System.Data.Linq.Mapping.ColumnAttribute(Name = "CustomerID", Storage = "_CustomerID", DbType = "NChar(5) NOT NULL", CanBeNull = false, IsPrimaryKey = true)]
        public string Id
        {
            get
            {
                return this._customerId;
            }
            set
            {
                if ((this._customerId != value))
                {
                    this.OnCustomerIDChanging(value);
                    this.OnPropertyChanging();
                    this._customerId = value;
                    this.OnPropertyChanged("CustomerID");
                    this.OnCustomerIDChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_CompanyName", DbType = "NVarChar(40) NOT NULL", CanBeNull = false)]
        public string CompanyName
        {
            get
            {
                return this._companyName;
            }
            set
            {
                if ((this._companyName != value))
                {
                    this.OnCompanyNameChanging(value);
                    this.OnPropertyChanging();
                    this._companyName = value;
                    this.OnPropertyChanged("CompanyName");
                    this.OnCompanyNameChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_ContactName", DbType = "NVarChar(30)")]
        public string ContactName
        {
            get
            {
                return this._contactName;
            }
            set
            {
                if ((this._contactName != value))
                {
                    this.OnContactNameChanging(value);
                    this.OnPropertyChanging();
                    this._contactName = value;
                    this.OnPropertyChanged("ContactName");
                    this.OnContactNameChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_ContactTitle", DbType = "NVarChar(30)")]
        public string ContactTitle
        {
            get
            {
                return this._contactTitle;
            }
            set
            {
                if ((this._contactTitle != value))
                {
                    this.OnContactTitleChanging(value);
                    this.OnPropertyChanging();
                    this._contactTitle = value;
                    this.OnPropertyChanged("ContactTitle");
                    this.OnContactTitleChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Address", DbType = "NVarChar(60)")]
        public string Address
        {
            get
            {
                return this._address;
            }
            set
            {
                if ((this._address != value))
                {
                    this.OnAddressChanging(value);
                    this.OnPropertyChanging();
                    this._address = value;
                    this.OnPropertyChanged("Address");
                    this.OnAddressChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_City", DbType = "NVarChar(15)")]
        public string City
        {
            get
            {
                return this._city;
            }
            set
            {
                if ((this._city != value))
                {
                    this.OnCityChanging(value);
                    this.OnPropertyChanging();
                    this._city = value;
                    this.OnPropertyChanged("City");
                    this.OnCityChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Region", DbType = "NVarChar(15)")]
        public string Region
        {
            get
            {
                return this._region;
            }
            set
            {
                if ((this._region != value))
                {
                    this.OnRegionChanging(value);
                    this.OnPropertyChanging();
                    this._region = value;
                    this.OnPropertyChanged("Region");
                    this.OnRegionChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_PostalCode", DbType = "NVarChar(10)")]
        public string PostalCode
        {
            get
            {
                return this._postalCode;
            }
            set
            {
                if ((this._postalCode != value))
                {
                    this.OnPostalCodeChanging(value);
                    this.OnPropertyChanging();
                    this._postalCode = value;
                    this.OnPropertyChanged("PostalCode");
                    this.OnPostalCodeChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Country", DbType = "NVarChar(15)")]
        public string Country
        {
            get
            {
                return this._country;
            }
            set
            {
                if ((this._country != value))
                {
                    this.OnCountryChanging(value);
                    this.OnPropertyChanging();
                    this._country = value;
                    this.OnPropertyChanged("Country");
                    this.OnCountryChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Phone", DbType = "NVarChar(24)")]
        public string Phone
        {
            get
            {
                return this._phone;
            }
            set
            {
                if ((this._phone != value))
                {
                    this.OnPhoneChanging(value);
                    this.OnPropertyChanging();
                    this._phone = value;
                    this.OnPropertyChanged("Phone");
                    this.OnPhoneChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Fax", DbType = "NVarChar(24)")]
        public string Fax
        {
            get
            {
                return this._fax;
            }
            set
            {
                if ((this._fax != value))
                {
                    this.OnFaxChanging(value);
                    this.OnPropertyChanging();
                    this._fax = value;
                    this.OnPropertyChanged("Fax");
                    this.OnFaxChanged();
                }
            }
        }
    }

}
