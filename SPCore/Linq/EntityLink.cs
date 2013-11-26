using System.ComponentModel;
using Microsoft.SharePoint.Linq;
using System;

namespace SPCore.Linq
{
    /// <summary>
    /// Create a new link to a Web page or other resource.
    /// </summary>
    [ContentType(Name = "Link", Id = "0x0105")]
    [Serializable]
    public class EntityLink : EntityItem
    {
        private string _url;

        private string _comments;

        private string _url0;

        private string _url1;

        [Column(Name = "URL", Storage = "_url", Required = true, FieldType = "Url")]
        public string URL
        {
            get
            {
                return this._url;
            }
            set
            {
                if (value == this._url) return;

                this.OnPropertyChanging("URL", this._url);
                this._url = value;
                this.OnPropertyChanged("URL");
            }
        }

        [Column(Name = "Comments", Storage = "_comments", FieldType = "Note")]
        public string Comments
        {
            get
            {
                return this._comments;
            }
            set
            {
                if (value == this._comments) return;

                this.OnPropertyChanging("Comments", this._comments);
                this._comments = value;
                this.OnPropertyChanged("Comments");
            }
        }

        [Column(Name = "URLwMenu", Storage = "_url0", ReadOnly = true, FieldType = "Computed")]
        public string URL0
        {
            get
            {
                return this._url0;
            }
            protected set
            {
                if (value == this._url0) return;

                this.OnPropertyChanging("URL0", this._url0);
                this._url0 = value;
                this.OnPropertyChanged("URL0");
            }
        }

        [Column(Name = "URLNoMenu", Storage = "_url1", ReadOnly = true, FieldType = "Computed")]
        public string URL1
        {
            get
            {
                return this._url1;
            }
            protected set
            {
                if (value == this._url1) return;

                this.OnPropertyChanging("URL1", this._url1);
                this._url1 = value;
                this.OnPropertyChanged("URL1");
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [RemovedColumn()]
        public override string Title
        {
            get
            {
                throw new InvalidOperationException("Field Title was removed from content type Link.");
            }
            set
            {
                throw new InvalidOperationException("Field Title was removed from content type Link.");
            }
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(URL) ? base.ToString() : URL;
        }
    }
}

