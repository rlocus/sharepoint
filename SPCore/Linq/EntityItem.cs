using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Linq;

namespace SPCore.Linq
{
    [ContentType(Name = "Item", Id = "0x01")]
    [Serializable]
    [DerivedEntityClass(Type = typeof(EntityAnnouncement))]
    [DerivedEntityClass(Type = typeof(EntityFolder))]
    [DerivedEntityClass(Type = typeof(EntityDocument))]
    [DerivedEntityClass(Type = typeof(EntityLink))]
    [DerivedEntityClass(Type = typeof(EntityEvent))]
    [DerivedEntityClass(Type = typeof(EntityTask))]
    [DerivedEntityClass(Type = typeof(EntityMessage))]
    public class EntityItem : ITrackEntityState, ITrackOriginalValues, INotifyPropertyChanged, INotifyPropertyChanging, ICustomMapping
    {
        #region Private Fields

        private int? _id;
        private int? _modifiedById;
        private string _modifiedBy;
        private Guid _uniqueId;
        private int? _version;
        private string _path;
        private EntityState _entityState;
        private IDictionary<string, object> _originalValues;
        private string _title;
        private DateTime? _createdDate;
        private string _createdBy;
        private int? _createdById;
        private DateTime? _modifiedDate;
        private string _serverUrl;
        private string _baseName;
        private string _contentType;
        private bool? _hasAttachments;
        private SPAttachmentCollection _contentAttachments;
        private string _moderationComments;
        private bool? _isCurrentVersion;

        #endregion

        ///// <summary>
        ///// Create a blank instance
        ///// </summary>
        //public DataItem() { }

        ///// <summary>
        ///// Create an instance of class from SPitem
        ///// </summary>
        ///// <param name="item">SPItem instance</param>
        //public DataItem(SPItem item)
        //{
        //    if (item == null) return;

        //    var objType = GetType();
        //    var properties = objType.GetProperties();

        //    foreach (var property in properties)
        //    {
        //        var attributes = property.GetCustomAttributes(typeof(ColumnAttribute), false);

        //        foreach (ColumnAttribute att in attributes)
        //        {
        //            FieldInfo field = objType.GetField(att.Storage,
        //                BindingFlags.NonPublic | BindingFlags.Instance);

        //            while (field == null)
        //            {
        //                objType = objType.BaseType;

        //                if (objType == null) break;

        //                field = objType.GetField(att.Storage,
        //                    BindingFlags.NonPublic | BindingFlags.Instance);
        //            }

        //            if (field != null)
        //            {
        //                switch (att.FieldType)
        //                {
        //                    case "Lookup":
        //                        try
        //                        {
        //                            SPFieldLookupValue fv = new SPFieldLookupValue(
        //                                (item[att.Name] ?? string.Empty).ToString());

        //                            if (att.IsLookupId)
        //                            {
        //                                field.SetValue(this, fv.LookupId);
        //                            }
        //                            else
        //                            {
        //                                field.SetValue(this, fv.LookupValue);
        //                            }
        //                        }
        //                        catch (ArgumentException)
        //                        {
        //                            field.SetValue(this, item[att.Name]);
        //                        }
        //                        break;
        //                    case "User":
        //                        try
        //                        {
        //                            SPFieldUserValue fv = new SPFieldUserValue(item.Fields.List.ParentWeb,
        //                                                          (item[att.Name] ?? string.Empty).ToString());
        //                            if (att.IsLookupId)
        //                            {
        //                                field.SetValue(this, fv.LookupId);
        //                            }
        //                            else
        //                            {
        //                                field.SetValue(this, fv.LookupValue);
        //                            }
        //                        }
        //                        catch (ArgumentException)
        //                        {
        //                            field.SetValue(this, item[att.Name]);
        //                        }
        //                        break;
        //                    case "Guid":
        //                        field.SetValue(this, new Guid(item[att.Name].ToString()));
        //                        break;
        //                    default:
        //                        field.SetValue(this, item[att.Name]);
        //                        break;
        //                }
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// Create an instance of class from SPitem
        ///// </summary>
        ///// <param name="item">SPListItemVersion instance</param>
        //public DataItem(SPListItemVersion item)
        //{
        //    if (item == null) return;

        //    var objType = GetType();
        //    var properties = objType.GetProperties();

        //    foreach (PropertyInfo property in properties)
        //    {
        //        var attributes = property.GetCustomAttributes(typeof(ColumnAttribute), false);

        //        foreach (ColumnAttribute att in attributes)
        //        {
        //            var field = objType.GetField(att.Storage,
        //                BindingFlags.NonPublic | BindingFlags.Instance);

        //            while (field == null)
        //            {
        //                objType = objType.BaseType;
        //                if (objType == null) break;
        //                field = objType.GetField(att.Storage,
        //                    BindingFlags.NonPublic | BindingFlags.Instance);
        //            }

        //            if (field != null)
        //            {
        //                switch (att.FieldType)
        //                {
        //                    case "Lookup":
        //                        try
        //                        {
        //                            var fv = new SPFieldLookupValue(
        //                                (item[att.Name] ?? string.Empty).ToString());

        //                            if (att.IsLookupId)
        //                            {
        //                                field.SetValue(this, fv.LookupId);
        //                            }
        //                            else
        //                            {
        //                                field.SetValue(this, fv.LookupValue);
        //                            }
        //                        }
        //                        catch (ArgumentException)
        //                        {
        //                            field.SetValue(this, item[att.Name]);
        //                        }
        //                        break;
        //                    case "User":
        //                        try
        //                        {
        //                            var fv = new SPFieldUserValue(item.Fields.List.ParentWeb,
        //                                                (item[att.Name] ?? string.Empty).ToString());
        //                            if (att.IsLookupId)
        //                            {
        //                                field.SetValue(this, fv.LookupId);
        //                            }
        //                            else
        //                            {
        //                                field.SetValue(this, fv.LookupValue);
        //                            }
        //                        }
        //                        catch (ArgumentException)
        //                        {
        //                            field.SetValue(this, item[att.Name]);
        //                        }
        //                        break;
        //                    case "Guid":
        //                        field.SetValue(this, new Guid(item[att.Name].ToString()));
        //                        break;
        //                    default:
        //                        field.SetValue(this, item[att.Name]);
        //                        break;
        //                }
        //            }
        //        }
        //    }
        //    IsCurrentVersion = false;
        //}

        ///// <summary>
        ///// Generate batch-command for creating/changing element
        ///// </summary>
        ///// <param name="listId">List Id</param>
        ///// <returns>Batch-command</returns>
        //public string GetBatchSaveCommand(Guid listId)
        //{
        //    var objType = GetType();
        //    var properties = objType.GetProperties();
        //    var sb = new StringBuilder(10);
        //    sb.AppendFormat(@"<Method ID=""{0}, Save"">{1}",
        //        Id.HasValue ? UniqueId : Guid.NewGuid(),
        //        Environment.NewLine);
        //    sb.AppendFormat(@"<SetList>{0}</SetList>{1}",
        //        listId,
        //        Environment.NewLine);
        //    sb.AppendFormat(@"<SetVar Name=""ID"">{0}</SetVar>{1}",
        //        Id.HasValue ? Id.Value.ToString() : "New",
        //        Environment.NewLine);
        //    sb.AppendLine(@"<SetVar Name=""Cmd"">Save</SetVar>");

        //    sb.AppendFormat(@"<SetVar Name=""RootFolder"">{0}</SetVar>{1}",
        //        Path,
        //        Environment.NewLine);
        //    foreach (var property in properties)
        //    {
        //        var attributes = property.GetCustomAttributes(typeof(ColumnAttribute), false);
        //        foreach (ColumnAttribute att in attributes)
        //        {
        //            if (att.ReadOnly)
        //                continue;
        //            var field = objType.GetField(att.Storage,
        //                                            BindingFlags.NonPublic | BindingFlags.Instance);
        //            while (field == null)
        //            {
        //                objType = objType.BaseType;
        //                if (objType == null) break;
        //                field = objType.GetField(att.Storage,
        //                                            BindingFlags.NonPublic | BindingFlags.Instance);
        //            }
        //            if (field != null)
        //            {
        //                sb.AppendFormat(@"<SetVar Name=""{0}{1}"">{2}</SetVar>{3}",
        //                    BatchFieldPrefix,
        //                    att.Name,
        //                    field.GetValue(this),
        //                    Environment.NewLine);
        //            }
        //        }
        //    }
        //    sb.AppendLine(@"</Method>");
        //    return sb.ToString();
        //}

        ///// <summary>
        ///// Generate batch-command for deleting element
        ///// </summary>
        ///// <param name="listId">List Id</param>
        ///// <returns>Batch-command</returns>
        //public string GetBatchDeleteCommand(Guid listId)
        //{
        //    var sb = new StringBuilder(10);
        //    sb.AppendFormat(@"<Method ID=""{0}, Delete"">{1}",
        //        UniqueId,
        //        Environment.NewLine);
        //    sb.AppendFormat(@"<SetList>{0}</SetList>{1}",
        //        listId,
        //        Environment.NewLine);
        //    sb.AppendFormat(@"<SetVar Name=""ID"">{0}</SetVar>{1}",
        //        Id,
        //        Environment.NewLine);
        //    sb.AppendLine(@"<SetVar Name=""Cmd"">Delete</SetVar>");
        //    sb.AppendLine(@"</Method>");
        //    return sb.ToString();
        //}

        ///// <summary>
        ///// Field names prefix from batch-command
        ///// </summary>
        //private const string BatchFieldPrefix = "urn:schemas-microsoft-com:office:office#";

        /// <summary>
        /// State of current instance
        /// </summary>
        public EntityState EntityState
        {
            get
            {
                return _entityState;
            }
            set
            {
                if ((value != _entityState))
                {
                    _entityState = value;
                }
            }
        }

        IDictionary<string, object> ITrackOriginalValues.OriginalValues
        {
            get { return _originalValues ?? (_originalValues = new Dictionary<string, object>()); }
        }

        #region LINQ Columns

        /// <summary>
        /// Boolean flag meaning is this entity has attachments
        /// </summary>
        [Column(Name = "Attachments", Storage = "_hasAttachments", ReadOnly = true, FieldType = "Attachments")]
        public bool? HasAttachments
        {
            get
            {
                return _hasAttachments;
            }
            protected set
            {
                if (value == _hasAttachments) return;

                OnPropertyChanging("HasAttachments", _hasAttachments);
                _hasAttachments = value;
                OnPropertyChanged("HasAttachments");
            }
        }

        /// <summary>
        /// Is this entity is current version flag
        /// </summary>
        [Column(Name = "_IsCurrentVersion", Storage = "_isCurrentVersion", ReadOnly = true, FieldType = "Boolean")]
        public bool? IsCurrentVersion
        {
            get
            {
                return _isCurrentVersion;
            }
            protected set
            {
                if (value == _isCurrentVersion) return;

                OnPropertyChanging("IsCurrentVersion", _isCurrentVersion);
                _isCurrentVersion = value;
                OnPropertyChanged("IsCurrentVersion");
            }
        }

        /// <summary>
        /// ID 
        /// </summary>
        [Column(Name = "ID", Storage = "_id", ReadOnly = true, FieldType = "Counter")]
        public int? Id
        {
            get
            {
                return _id;
            }
            protected set
            {
                if (value == _id) return;

                OnPropertyChanging("Id", _id);
                _id = value;
                OnPropertyChanged("Id");
            }
        }

        /// <summary>
        /// Version
        /// </summary>
        [Column(Name = "owshiddenversion", Storage = "_version", ReadOnly = true, FieldType = "Integer")]
        public int? Version
        {
            get
            {
                return _version;
            }
            protected set
            {
                if (value == _version) return;

                OnPropertyChanging("Version", _version);
                _version = value;
                OnPropertyChanged("Version");
            }
        }

        /// <summary>
        /// Path to folder containing this entity
        /// </summary>
        [Column(Name = "FileDirRef", Storage = "_path", ReadOnly = true, FieldType = "Lookup", IsLookupValue = true)]
        public string Path
        {
            get
            {
                return _path;
            }
            protected set
            {
                if (value == _path) return;

                OnPropertyChanging("Path", _path);
                _path = value;
                OnPropertyChanged("Path");
            }
        }

        /// <summary>
        /// Title
        /// </summary>
        [Column(Name = "Title", Storage = "_title", Required = true, FieldType = "Text")]
        public virtual string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (value == _title) return;

                OnPropertyChanging("Title", _title);
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        /// <summary>
        /// Author' name
        /// </summary>
        [Column(Name = "Author", Storage = "_createdBy", ReadOnly = true, FieldType = "User", IsLookupValue = true)]
        public string CreatedBy
        {
            get
            {
                return _createdBy;
            }
            protected set
            {
                if (value == _createdBy) return;

                OnPropertyChanging("CreatedBy", _createdBy);
                _createdBy = value;
                OnPropertyChanged("CreatedBy");
            }
        }

        /// <summary>
        /// Author' Id
        /// </summary>
        [Column(Name = "Author", Storage = "_createdById", ReadOnly = true, FieldType = "User", IsLookupId = true)]
        public int? CreatedById
        {
            get
            {
                return _createdById;
            }
            protected set
            {
                if (value == _createdById) return;

                OnPropertyChanging("CreatedById", _createdById);
                _createdById = value;
                OnPropertyChanged("CreatedById");
            }
        }

        /// <summary>
        /// Created date
        /// </summary>
        [Column(Name = "Created", Storage = "_createdDate", ReadOnly = true, FieldType = "DateTime")]
        public DateTime? CreatedDate
        {
            get
            {
                return _createdDate;
            }
            protected set
            {
                if (value == _createdDate) return;

                OnPropertyChanging("CreatedDate", _createdDate);
                _createdDate = value;
                OnPropertyChanged("CreatedDate");
            }
        }

        /// <summary>
        /// Editor' name
        /// </summary>
        [Column(Name = "Editor", Storage = "_modifiedBy", ReadOnly = true, FieldType = "User", IsLookupValue = true)]
        public string ModifiedBy
        {
            get
            {
                return _modifiedBy;
            }
            protected set
            {
                if (value == _modifiedBy) return;

                OnPropertyChanging("ModifiedBy", _modifiedBy);
                _modifiedBy = value;
                OnPropertyChanged("ModifiedBy");
            }
        }

        /// <summary>
        /// Editor' Id
        /// </summary>
        [Column(Name = "Editor", Storage = "_modifiedById", ReadOnly = true, FieldType = "User", IsLookupId = true)]
        public int? ModifiedById
        {
            get
            {
                return _modifiedById;
            }
            protected set
            {
                if (value == _modifiedById) return;

                OnPropertyChanging("ModifiedById", _modifiedById);
                _modifiedById = value;
                OnPropertyChanged("ModifiedById");
            }
        }

        /// <summary>
        /// Last modified date
        /// </summary>
        [Column(Name = "Modified", Storage = "_modifiedDate", ReadOnly = true, FieldType = "DateTime")]
        public DateTime? ModifiedDate
        {
            get
            {
                return _modifiedDate;
            }
            protected set
            {
                if ((value == _modifiedDate)) return;

                OnPropertyChanging("ModifiedDate", _modifiedDate);
                _modifiedDate = value;
                OnPropertyChanged("ModifiedDate");
            }
        }

        /// <summary>
        /// Unique identifier
        /// </summary>
        [Column(Name = "GUID", Storage = "_uniqueId", ReadOnly = true, FieldType = "Guid")]
        public Guid UniqueId
        {
            get
            {
                return _uniqueId;
            }
            protected set
            {
                if (value == _uniqueId) return;

                OnPropertyChanging("UniqueId", _uniqueId);
                _uniqueId = value;
                OnPropertyChanged("UniqueId");
            }
        }

        /// <summary>
        /// Server-relative URL
        /// </summary>
        [Column(Name = "ServerUrl", Storage = "_serverUrl",
         ReadOnly = true, FieldType = "Computed")]
        public string ServerUrl
        {
            get
            {
                return _serverUrl;
            }
            protected set
            {
                if (value == _serverUrl) return;

                OnPropertyChanging("ServerUrl", _serverUrl);
                _serverUrl = value;
                OnPropertyChanged("ServerUrl");
            }
        }

        /// <summary>
        /// Moderation comments
        /// </summary>
        [Column(Name = "_ModerationComments", Storage = "_moderationComments",
         ReadOnly = true, FieldType = "Note")]
        public string ModerationComments
        {
            get
            {
                return _moderationComments;
            }
            protected set
            {
                if (value == _moderationComments) return;

                OnPropertyChanging("ModerationComments", _moderationComments);
                _moderationComments = value;
                OnPropertyChanged("ModerationComments");
            }
        }

        /// <summary>
        /// Base name. Without path and extension
        /// </summary>
        [Column(Name = "BaseName", Storage = "_baseName",
         ReadOnly = true, FieldType = "Computed")]
        public string BaseName
        {
            get
            {
                return _baseName;
            }
            protected set
            {
                if (value == _baseName) return;

                OnPropertyChanging("BaseName", _baseName);
                _baseName = value;
                OnPropertyChanged("BaseName");
            }
        }

        [Column(Name = "ContentType", Storage = "_contentType",
        ReadOnly = true, FieldType = "Computed")]
        public string ContentType
        {
            get
            {
                return _contentType;
            }
            protected set
            {
                if (value == _contentType) return;

                OnPropertyChanging("ContentType", _contentType);
                _contentType = value;
                OnPropertyChanged("ContentType");
            }
        }

        #endregion

        ///<summary>
        ///</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        ///<summary>
        ///</summary>
        public event PropertyChangingEventHandler PropertyChanging;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanging(string propertyName, object value)
        {
            if (null == _originalValues)
            {
                _originalValues = new Dictionary<string, object>();
            }
            if (false == _originalValues.ContainsKey(propertyName))
            {
                _originalValues.Add(propertyName, value);
            }
            if (null != PropertyChanging)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Attachments of item
        /// </summary>
        public SPAttachmentCollection Attachments
        {
            get
            {
                return _contentAttachments;
            }
            set
            {
                if (value == _contentAttachments) return;

                if (_contentAttachments == null)
                {
                    _contentAttachments = value;
                    return;
                }

                OnPropertyChanging("Attachments", _contentAttachments);
                _contentAttachments = value;
                OnPropertyChanged("Attachments");
            }
        }

        #region ICustomMapping

        [CustomMapping(Columns = new[] { "Attachments" })]
        public void MapFrom(object listItem)
        {
            SPListItem item = (SPListItem)listItem;
            SPList list = item.ParentList;

            if (list.EnableAttachments && (bool) item["Attachments"])
            {
                Attachments = item.Attachments;
            }
        }

        public void MapTo(object listItem)
        {

        }

        public void Resolve(RefreshMode mode, object originalListItem, object databaseListItem)
        {
            SPListItem originalItem = originalListItem as SPListItem;
            SPListItem databaseItem = databaseListItem as SPListItem;

            if (databaseItem == null || originalItem == null) return;

            //SPAttachmentCollection originalValue = originalItem["Attachments"] as SPAttachmentCollection;
            //SPAttachmentCollection databaseValue = databaseItem["Attachments"] as SPAttachmentCollection;

            //switch (mode)
            //{
            //    case RefreshMode.KeepChanges:
            //        if (_contentAttachments != originalValue)
            //        {
            //            databaseItem["Attachments"] = _contentAttachments;
            //        }
            //        else if (_contentAttachments == originalValue &&
            //            _contentAttachments != databaseValue)
            //        {
            //            _contentAttachments = databaseValue;
            //        }
            //        break;
            //    case RefreshMode.KeepCurrentValues:
            //        databaseItem["Attachments"] = _contentAttachments;
            //        break;
            //    case RefreshMode.OverwriteCurrentValues:
            //        _contentAttachments = databaseValue;
            //        break;
            //    default:
            //        break;
            //}
        }

        #endregion

        public override string ToString()
        {
            return string.IsNullOrEmpty(Title) ? Id.ToString() : Title;
        }
    }
}

