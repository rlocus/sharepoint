using System;
using System.Data;
using System.Globalization;
using Microsoft.SharePoint;

namespace SPCore.Cache
{
    public class SPListQueryCache
    {
        private SPListCachedData _cachedData;

        public SPListQueryCache(Guid listId)
        {
            _cachedData = new SPListCachedData(SPContext.Current.Site.ID, SPContext.Current.Web.ID, listId);
        }

        public bool HasListChanged()
        {
            SPList list = SPContext.Current.Web.Lists[_cachedData.ListId];

            DateTime lastItemModifiedDate = list.LastItemModifiedDate;

            if (list.LastItemDeletedDate > lastItemModifiedDate)
            {
                lastItemModifiedDate = list.LastItemDeletedDate;
            }

            return _cachedData.LastItemModifiedDate != lastItemModifiedDate || _cachedData.ItemCount != list.ItemCount;
        }

        private SPListCachedData LoadData(SPQuery query)
        {
            if (SPContext.Current.Site.ID == _cachedData.SiteId && SPContext.Current.Web.ID == _cachedData.WebId)
            {
                SPList list = SPContext.Current.Web.Lists[_cachedData.ListId];

                DateTime lastItemModifiedDate = list.LastItemModifiedDate;

                if (list.LastItemDeletedDate > lastItemModifiedDate)
                {
                    lastItemModifiedDate = list.LastItemDeletedDate;
                }
                _cachedData.LastItemModifiedDate = lastItemModifiedDate;
                _cachedData.ItemCount = list.ItemCount;
                _cachedData.Data = list.GetItems(query).GetDataTable();
            }

            return _cachedData;
        }

        private ICachedObject<SPListCachedData> GetCache(SPQuery query)
        {
            ICachedObject<SPListCachedData> cache =
               SPCache.Cache(() => LoadData(query))
                   .By(query.ViewXml.GetHashCode().ToString(CultureInfo.InvariantCulture),
                       _cachedData.ListId.ToString(),
                       _cachedData.WebId.ToString(),
                       _cachedData.SiteId.ToString())
                   .ByCurrentUser();
            _cachedData = cache.CachedObject;
            return cache;
        }

        public DataTable GetByQuery(SPQuery query)
        {
            ICachedObject<SPListCachedData> cache = GetCache(query);

            if (HasListChanged())
            {
                cache.Clear();
                GetCache(query);
            }

            return _cachedData.Data;
        }
    }

    /// <summary>
    /// Information holding object for a SharePoint List cache dependency object.
    /// </summary>
    public class SPListCachedData
    {
        public SPListCachedData(Guid siteId, Guid webId, Guid listId)
        {
            SiteId = siteId;
            WebId = webId;
            ListId = listId;
        }

        public Guid SiteId { get; private set; }
        public Guid WebId { get; private set; }
        public Guid ListId { get; private set; }

        public DateTime LastItemModifiedDate { get; set; }
        public int ItemCount { get; set; }

        public DataTable Data { get; set; }
    }

}
