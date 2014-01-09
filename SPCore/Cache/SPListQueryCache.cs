using System;
using System.Data;
using System.Xml.Linq;
using Microsoft.SharePoint;

namespace SPCore.Cache
{
    public class SPListQueryCache
    {
        private readonly SPList _list;

        public SPListQueryCache(SPWeb web, Guid listId)
        {
            if (web == null) throw new ArgumentNullException("web");

            _list = web.Lists[listId];
        }

        public SPListQueryCache(SPList list)
        {
            if (list == null) throw new ArgumentNullException("list");

            _list = list;
        }

        public TimeSpan CacheTime
        {
            get;
            set;
        }

        private static bool HasListChanged(SPList list, SPListQueryCachedData listQueryCachedData)
        {
            DateTime lastItemModifiedDate = list.LastItemModifiedDate;

            if (list.LastItemDeletedDate > lastItemModifiedDate)
            {
                lastItemModifiedDate = list.LastItemDeletedDate;
            }

            return listQueryCachedData.LastItemModifiedDate != lastItemModifiedDate ||
                   listQueryCachedData.ItemCount != list.ItemCount;
        }

        private static SPListQueryCachedData GetListQueryCachedData(SPList list, string viewXml)
        {
            SPListQueryCachedData listQueryCachedData = GetListQueryCachedData(list);

            if (!string.IsNullOrEmpty(viewXml))
            {
                listQueryCachedData.Add(Guid.NewGuid().ToString(), viewXml);
            }

            return listQueryCachedData;
        }

        private static SPListQueryCachedData GetListQueryCachedData(SPList list)
        {
            SPListQueryCachedData listQueryCachedData = new SPListQueryCachedData(list.ParentWeb.Site.ID, list.ParentWeb.ID, list.ID, list.ParentWeb.CurrentUser.Sid);

            DateTime lastModifiedDate = list.LastItemModifiedDate;

            if (list.LastItemDeletedDate > lastModifiedDate)
            {
                lastModifiedDate = list.LastItemDeletedDate;
            }

            listQueryCachedData.LastItemModifiedDate = lastModifiedDate;
            listQueryCachedData.ItemCount = list.ItemCount;

            return listQueryCachedData;
        }

        private static DataTable LoadDataTable(SPList list, SPQuery query)
        {
            return list.GetItems(query).GetDataTable();
        }

        private DataTable GetData(SPList list, SPQuery query)
        {
            if (string.IsNullOrEmpty(query.ViewFields))
            {
                throw new ArgumentException("The ViewFields property for SPQuery hasn't been set.");
            }

            string viewXml = XElement.Parse(query.ViewXml, LoadOptions.None).ToString(SaveOptions.DisableFormatting);

            ICachedObject<SPListQueryCachedData> queryCache =
                SPCache.Cache(() => GetListQueryCachedData(list, viewXml))
                    .By(list.ID.ToString(),
                        list.ParentWeb.ID.ToString(),
                        list.ParentWeb.Site.ID.ToString(),
                        list.ParentWeb.CurrentUser.Sid);

            if (CacheTime != default(TimeSpan))
            {
                queryCache.ForSliding(CacheTime);
            }

            SPListQueryCachedData listQueryCachedData = queryCache.CachedObject;

            string keyQuery;

            if (HasListChanged(list, listQueryCachedData))
            {
                listQueryCachedData.Clear();
                listQueryCachedData = GetListQueryCachedData(list);
                keyQuery = Guid.NewGuid().ToString();
                listQueryCachedData.Add(keyQuery, viewXml);
                queryCache.CachedObject = listQueryCachedData;
                queryCache.Update();
            }
            else
            {
                keyQuery = listQueryCachedData.FindKey(viewXml);
            }

            ICachedObject<DataTable> dataCache = listQueryCachedData.GetDataCache(keyQuery, () => LoadDataTable(list, query));

            if (CacheTime != default(TimeSpan))
            {
                dataCache.ForSliding(CacheTime);
            }

            return dataCache.CachedObject;
        }

        public DataTable GetByQuery(SPQuery query)
        {
            using (SPWeb web = _list.ParentWeb.Site.OpenWeb(_list.ParentWeb.ID))
            {
                SPList list = web.Lists.GetList(_list.ID, true, true);
                return GetData(list, query);
            }
        }
    }
}
