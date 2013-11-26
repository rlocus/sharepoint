using System.ComponentModel;
using Microsoft.SharePoint.Linq;
using System;

namespace SPCore.Linq
{
    /// <summary>
    /// Create a new message.
    /// </summary>
    [ContentType(Name = "Message", Id = "0x0107")]
    [Serializable]
    public class EntityMessage : EntityItem
    {
        private string _discussionSubject;

        private string _version0;

        private string _body;

        private string _reply;

        private string _post;

        private string _threading;

        private string _postedBy;

        private string _eMailSender;

        private int? _editorId;

        private string _modifiedByNameWithPicture;

        [Column(Name = "DiscussionTitle", Storage = "_discussionSubject", ReadOnly = true, FieldType = "Computed")]
        public string DiscussionSubject
        {
            get
            {
                return this._discussionSubject;
            }
            protected set
            {
                if (value == this._discussionSubject) return;

                this.OnPropertyChanging("DiscussionSubject", this._discussionSubject);
                this._discussionSubject = value;
                this.OnPropertyChanged("DiscussionSubject");
            }
        }

        [Column(Name = "_UIVersionString", Storage = "_version0", ReadOnly = true, FieldType = "Text")]
        public string Version0
        {
            get
            {
                return this._version0;
            }
            protected set
            {
                if (value == this._version0) return;

                this.OnPropertyChanging("Version0", this._version0);
                this._version0 = value;
                this.OnPropertyChanged("Version0");
            }
        }

        [Column(Name = "Body", Storage = "_body", FieldType = "Note")]
        public string Body
        {
            get
            {
                return this._body;
            }
            set
            {
                if (value == this._body) return;

                this.OnPropertyChanging("Body", this._body);
                this._body = value;
                this.OnPropertyChanged("Body");
            }
        }

        [Column(Name = "ReplyNoGif", Storage = "_reply", ReadOnly = true, FieldType = "Computed")]
        public string Reply
        {
            get
            {
                return this._reply;
            }
            protected set
            {
                if (value == this._reply) return;

                this.OnPropertyChanging("Reply", this._reply);
                this._reply = value;
                this.OnPropertyChanged("Reply");
            }
        }

        [Column(Name = "BodyAndMore", Storage = "_post", ReadOnly = true, FieldType = "Computed")]
        public string Post
        {
            get
            {
                return this._post;
            }
            protected set
            {
                if (value == this._post) return;

                this.OnPropertyChanging("Post", this._post);
                this._post = value;
                this.OnPropertyChanged("Post");
            }
        }

        [Column(Name = "Threading", Storage = "_threading", ReadOnly = true, FieldType = "Computed")]
        public string Threading
        {
            get
            {
                return this._threading;
            }
            protected set
            {
                if (value == this._threading) return;

                this.OnPropertyChanging("Threading", this._threading);
                this._threading = value;
                this.OnPropertyChanged("Threading");
            }
        }

        [Column(Name = "PersonViewMinimal", Storage = "_postedBy", ReadOnly = true, FieldType = "Computed")]
        public string PostedBy
        {
            get
            {
                return this._postedBy;
            }
            protected set
            {
                if (value == this._postedBy) return;

                this.OnPropertyChanging("PostedBy", this._postedBy);
                this._postedBy = value;
                this.OnPropertyChanged("PostedBy");
            }
        }

        [Column(Name = "EmailSender", Storage = "_eMailSender", FieldType = "Note")]
        public string EMailSender
        {
            get
            {
                return this._eMailSender;
            }
            set
            {
                if (value == this._eMailSender) return;

                this.OnPropertyChanging("EMailSender", this._eMailSender);
                this._eMailSender = value;
                this.OnPropertyChanged("EMailSender");
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [RemovedColumn()]
        public override string Title
        {
            get
            {
                throw new InvalidOperationException("Field Title was removed from content type Message.");
            }
            set
            {
                throw new InvalidOperationException("Field Title was removed from content type Message.");
            }
        }

        [Column(Name = "MyEditor", Storage = "_editorId", ReadOnly = true, FieldType = "User", IsLookupId = true)]
        public int? EditorId
        {
            get
            {
                return this._editorId;
            }
            protected set
            {
                if (value == this._editorId) return;
                this.OnPropertyChanging("EditorId", this._editorId);
                this._editorId = value;
                this.OnPropertyChanged("EditorId");
            }
        }

        [Column(Name = "MyEditor", Storage = "_modifiedByNameWithPicture", ReadOnly = true, FieldType = "User", IsLookupValue = true)]
        public string ModifiedByNameWithPicture
        {
            get
            {
                return this._modifiedByNameWithPicture;
            }
            protected set
            {
                if (value == this._modifiedByNameWithPicture) return;

                this.OnPropertyChanging("ModifiedByNameWithPicture", this._modifiedByNameWithPicture);
                this._modifiedByNameWithPicture = value;
                this.OnPropertyChanged("ModifiedByNameWithPicture");
            }
        }

        public override string ToString()
        {
            return DiscussionSubject ?? base.ToString();
        }
    }
}

