using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Linq;

namespace SPCore.Linq
{
    public class EntityDataContext : DataContext
    {
        #region [ Fields ]

        private SPSite _site;
        private SPWeb _web;

        private readonly Dictionary<string, SPList> _loadedLists;

        #endregion

        #region [ Constructror ]

        public EntityDataContext(string requestUrl) :
            base(requestUrl)
        {
            _loadedLists = new Dictionary<string, SPList>();

            if (SPContext.Current != null)
            {
                this._site = SPContext.Current.Site;
                this._web = ((SPContext.Current.Web.Url == requestUrl) ? SPContext.Current.Web : this._site.OpenWeb(/*new Uri(requestUrl).PathAndQuery*/));
            }
            else
            {
                this._site = new SPSite(requestUrl);
                this._web = this._site.OpenWeb(/*new Uri(requestUrl).PathAndQuery*/);
            }
        }

        #endregion

        #region [ Properties ]

        internal SPList LatestList { get; private set; }

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

        #endregion

        #region [ Private methods ]

        /// <summary>
        /// Check entity is attached to context
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="listName"></param>
        /// <returns>true - attached, false - is not attached</returns>
        private bool EntityExistsInContext<TEntity>(TEntity entity, string listName) where TEntity : EntityItem
        {
            Type type = typeof(DataContext);
            PropertyInfo pi = type.GetProperty("EntityTracker", BindingFlags.NonPublic | BindingFlags.Instance);
            var val = pi.GetValue(this, null);
            Type trackerType = val.GetType();
            Type eidType = Type.GetType("Microsoft.SharePoint.Linq.EntityId, " + typeof(DataContext).Assembly.FullName);

            if (eidType != null)
            {
                var eid = Activator.CreateInstance(eidType, this.Web, listName);
                MethodInfo mi = trackerType.GetMethod("TryGetId", BindingFlags.Public | BindingFlags.Instance);
                var res = mi.Invoke(val, new[] { entity, eid });
                return (bool)res;
            }

            return false;
        }

        #endregion

        #region [ Overrides ]

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            if (_web != null && (SPContext.Current == null || _web != SPContext.Current.Web))
            {
                try
                {
                    _web.Dispose();
                }
                catch { }
                _web = null;
            }
            if (_site != null && (SPContext.Current == null || _site != SPContext.Current.Site))
            {
                try
                {
                    _site.Dispose();
                }
                catch { }
                _site = null;
            }
        }

        public override EntityList<TEntity> GetList<TEntity>(string listName)
        {
            if (_loadedLists.ContainsKey(listName))
            {
                LatestList = _loadedLists[listName];
            }
            else
            {
                LatestList = _web.GetListByName(listName);
                _loadedLists.Add(listName, LatestList);
            }

            return base.GetList<TEntity>(LatestList.Title);
        }

        #endregion

        #region [Public methods ]

        public SPList GetSPList<TEntity>(string listName) where TEntity : EntityItem
        {
            return GetListMetaData<TEntity>(listName).List ?? LatestList;
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

        public bool IsAttached<TEntity>(TEntity entity, string listName) where TEntity : EntityItem
        {

            return EntityExistsInContext(entity, listName);
        }

        #endregion
    }
}
