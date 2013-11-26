using Microsoft.SharePoint.Linq;
using System;
using System.ComponentModel;

namespace SPCore.Linq
{
    /// <summary>
    /// Create a new folder.
    /// </summary>
    [ContentType(Name = "Folder", Id = "0x0120")]
    [Serializable]
    [DerivedEntityClass(Type = typeof(EntityDiscussion))]
    public class EntityFolder : EntityItem
    {
        private string _name;

        private int? _itemChildCountId;

        private string _itemChildCountItemChildCount;

        private int? _folderChildCountId;

        private string _folderChildCountFolderChildCount;

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

        [EditorBrowsable(EditorBrowsableState.Never)]
        [RemovedColumn()]
        public override string Title
        {
            get
            {
                throw new InvalidOperationException("Field Title was removed from content type Folder.");
            }
            set
            {
                throw new InvalidOperationException("Field Title was removed from content type Folder.");
            }
        }

        [Column(Name = "ItemChildCount", Storage = "_itemChildCountId", ReadOnly = true, FieldType = "Lookup", IsLookupId = true)]
        public int? ItemChildCountId
        {
            get
            {
                return this._itemChildCountId;
            }
            protected set
            {
                if (value == this._itemChildCountId) return;

                this.OnPropertyChanging("ItemChildCountId", this._itemChildCountId);
                this._itemChildCountId = value;
                this.OnPropertyChanged("ItemChildCountId");
            }
        }

        [Column(Name = "ItemChildCount", Storage = "_itemChildCountItemChildCount", ReadOnly = true, FieldType = "Lookup", IsLookupValue = true)]
        public string ItemChildCountItemChildCount
        {
            get
            {
                return this._itemChildCountItemChildCount;
            }
            protected set
            {
                if (value == this._itemChildCountItemChildCount) return;

                this.OnPropertyChanging("ItemChildCountItemChildCount", this._itemChildCountItemChildCount);
                this._itemChildCountItemChildCount = value;
                this.OnPropertyChanged("ItemChildCountItemChildCount");
            }
        }

        [Column(Name = "FolderChildCount", Storage = "_folderChildCountId", ReadOnly = true, FieldType = "Lookup", IsLookupId = true)]
        public int? FolderChildCountId
        {
            get
            {
                return this._folderChildCountId;
            }
            protected set
            {
                if (value == this._folderChildCountId) return;

                this.OnPropertyChanging("FolderChildCountId", this._folderChildCountId);
                this._folderChildCountId = value;
                this.OnPropertyChanged("FolderChildCountId");
            }
        }

        [Column(Name = "FolderChildCount", Storage = "_folderChildCountFolderChildCount", ReadOnly = true, FieldType = "Lookup", IsLookupValue = true)]
        public string FolderChildCountFolderChildCount
        {
            get
            {
                return this._folderChildCountFolderChildCount;
            }
            protected set
            {
                if (value == this._folderChildCountFolderChildCount) return;

                this.OnPropertyChanging("FolderChildCountFolderChildCount", this._folderChildCountFolderChildCount);
                this._folderChildCountFolderChildCount = value;
                this.OnPropertyChanged("FolderChildCountFolderChildCount");
            }
        }
    }
}

