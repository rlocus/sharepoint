using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Linq;

namespace SPCore.Linq
{
    public class EntityDataContext : DataContext
    {
        private readonly SPSite _site;
        private readonly SPWeb _web;

        public EntityDataContext(string requestUrl) :
            base(requestUrl)
        {
            _site = new SPSite(this.Web, SPUserToken.SystemAccount);
            _web = _site.OpenWeb();
        }

        public override EntityList<T> GetList<T>(string listName)
        {
            SPList list = _web.GetListByName(listName);

            return list != null ? base.GetList<T>(list.Title) : null;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                try
                {
                    if (_web != null)
                    {
                        _web.Dispose();
                    }
                    if (_site != null)
                    {
                        _site.Dispose();
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Use the Tasks list to keep track of work that you or your team needs to complete.
        /// </summary>
        [List(Name = "Lists/Tasks")]
        public EntityList<EntityTask> Tasks
        {
            get
            {
                return this.GetList<EntityTask>("Lists/Tasks");
            }
        }

        /// <summary>
        /// Use this list to track upcoming events, status updates or other team news.
        /// </summary>
        [List(Name = "Lists/Announcements")]
        public EntityList<EntityAnnouncement> Announcements
        {
            get
            {
                return this.GetList<EntityAnnouncement>("Lists/Announcements");
            }
        }

        /// <summary>
        /// Use the Calendar list to keep informed of upcoming meetings, deadlines, and other important events.
        /// </summary>
        [List(Name = "Lists/Calendar")]
        public EntityList<EntityEvent> Calendar
        {
            get
            {
                return this.GetList<EntityEvent>("Lists/Calendar");
            }
        }

        [List(Name = "AnalyticsReports")]
        public EntityList<EntityDocument> CustomizedReports
        {
            get
            {
                return this.GetList<EntityDocument>("AnalyticsReports");
            }
        }

        [List(Name = "FormServerTemplates")]
        public EntityList<EntityDocument> FormTemplates
        {
            get
            {
                return this.GetList<EntityDocument>("FormServerTemplates");
            }
        }

        [List(Name = "Lists/Links")]
        public EntityList<EntityItem> Links
        {
            get
            {
                return this.GetList<EntityItem>("Lists/Links");
            }
        }

        /// <summary>
        /// Share a document with the team by adding it to this document library.
        /// </summary>
        [List(Name = "Shared Documents")]
        public EntityList<EntityDocument> SharedDocuments
        {
            get
            {
                return this.GetList<EntityDocument>("Shared Documents");
            }
        }

        /// <summary>
        /// Use this library to store files which are included on pages within this site, such as images on Wiki pages.
        /// </summary>
        [List(Name = "SiteAssets")]
        public EntityList<EntityDocument> SiteAssets
        {
            get
            {
                return this.GetList<EntityDocument>("SiteAssets");
            }
        }

        /// <summary>
        /// Use this library to create and store pages on this site.
        /// </summary>
        [List(Name = "SitePages")]
        public EntityList<EntityWikiPage> SitePages
        {
            get
            {
                return this.GetList<EntityWikiPage>("SitePages");
            }
        }

        /// <summary>
        /// Use the style library to store style sheets, such as CSS or XSL files. The style sheets in this gallery can be used by this site or any of its subsites.
        /// </summary>
        [List(Name = "Style Library")]
        public EntityList<EntityDocument> StyleLibrary
        {
            get
            {
                return this.GetList<EntityDocument>("Style Library");
            }
        }

        /// <summary>
        /// Use the Team Discussion list to hold newsgroup-style discussions on topics relevant to your team.
        /// </summary>
        [List(Name = "Lists/Team Discussion")]
        public EntityList<EntityItem> TeamDiscussion
        {
            get
            {
                return this.GetList<EntityItem>("Lists/Team Discussion");
            }
        }

        /// <summary>
        /// Retrieving metadata for list
        /// </summary>
        /// <typeparam name="TEntity">Entity</typeparam>
        /// <param name="listName">List Name</param>
        /// <returns>Metadata</returns>
        public EntityListMetaData GetListMetaData<TEntity>(string listName) where TEntity : EntityItem
        {
            return EntityListMetaData.GetMetaData(GetList<TEntity>(listName));
        }
    }
}
