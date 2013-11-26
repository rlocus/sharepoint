using Microsoft.SharePoint.Linq;
using System;

namespace SPCore.Linq
{
    /// <summary>
    /// Create a new document.
    /// </summary>
    [ContentType(Name = "Document", Id = "0x0101")] 
    [Serializable]
    [DerivedEntityClass(Type = typeof(EntityWikiPage))]
    public class EntityDocument : EntityItem
    {
        private string _name;
        private string _documentModifiedBy;
        private string _documentCreatedBy;

        [Column(Name = "FileLeafRef", Storage = "_name", Required = true, FieldType = "File")]
        public virtual string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                if (value == this._name) return;

                this.OnPropertyChanging("Name", this._name);
                this._name = value;
                this.OnPropertyChanged("Name");
            }
        }

        [Column(Name = "Modified_x0020_By", Storage = "_documentModifiedBy", ReadOnly = true, FieldType = "Text")]
        public string DocumentModifiedBy
        {
            get
            {
                return this._documentModifiedBy;
            }
            protected set
            {
                if (value == this._documentModifiedBy) return;

                this.OnPropertyChanging("DocumentModifiedBy", this._documentModifiedBy);
                this._documentModifiedBy = value;
                this.OnPropertyChanged("DocumentModifiedBy");
            }
        }

        [Column(Name = "Created_x0020_By", Storage = "_documentCreatedBy", ReadOnly = true, FieldType = "Text")]
        public string DocumentCreatedBy
        {
            get
            {
                return this._documentCreatedBy;
            }
            protected set
            {
                if (value == this._documentCreatedBy) return;

                this.OnPropertyChanging("DocumentCreatedBy", this._documentCreatedBy);
                this._documentCreatedBy = value;
                this.OnPropertyChanged("DocumentCreatedBy");
            }
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(Name) ? base.ToString() : Name;
        }
    }
}

