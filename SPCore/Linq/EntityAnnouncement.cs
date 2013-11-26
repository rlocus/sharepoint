using Microsoft.SharePoint.Linq;
using System;

namespace SPCore.Linq
{
    /// <summary>
    /// Create a new news item, status or other short piece of information.
    /// </summary>
    [ContentType(Name = "Announcement", Id = "0x0104")]
    [Serializable]
    public class EntityAnnouncement : EntityItem
    {
        private string _body;
        private DateTime? _expires;

        [Column(Name = "Body", Storage = "_body", FieldType = "Note")]
        public string Body
        {
            get
            {
                return this._body;
            }
            set
            {
                if ((value != this._body))
                {
                    this.OnPropertyChanging("Body", this._body);
                    this._body = value;
                    this.OnPropertyChanged("Body");
                }
            }
        }

        [Column(Name = "Expires", Storage = "_expires", FieldType = "DateTime")]
        public DateTime? Expires
        {
            get
            {
                return this._expires;
            }
            set
            {
                if ((value != this._expires))
                {
                    this.OnPropertyChanging("Expires", this._expires);
                    this._expires = value;
                    this.OnPropertyChanged("Expires");
                }
            }
        }
    }

}
