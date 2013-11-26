using Microsoft.SharePoint.Linq;
using System;
using System.ComponentModel;

namespace SPCore.Linq
{
    /// <summary>
    /// Create a new wiki page.
    /// </summary>
    [ContentType(Name = "Wiki Page", Id = "0x010108")]
    [Serializable]
    public class EntityWikiPage : EntityDocument
    {
        private string _wikiContent;

        [Column(Name = "WikiField", Storage = "_wikiContent", FieldType = "Note")]
        public string WikiContent
        {
            get
            {
                return this._wikiContent;
            }
            set
            {
                if (value == this._wikiContent) return;

                this.OnPropertyChanging("WikiContent", this._wikiContent);
                this._wikiContent = value;
                this.OnPropertyChanged("WikiContent");
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [RemovedColumn()]
        public override string Title
        {
            get
            {
                throw new InvalidOperationException("Field Title was removed from content type Wiki Page.");
            }
            set
            {
                throw new InvalidOperationException("Field Title was removed from content type Wiki Page.");
            }
        }

        public override string ToString()
        {
            return WikiContent ?? base.ToString();
        }
    }
}

