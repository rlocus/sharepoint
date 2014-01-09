using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SPCore.Cache
{
    public class SPListQueryCachedData
    {
        public SPListQueryCachedData(Guid siteId, Guid webId, Guid listId, string userId)
        {
            SiteId = siteId;
            WebId = webId;
            ListId = listId;
            UserId = userId;
            Queries = new Dictionary<string, string>();
        }

        public Guid SiteId { get; private set; }
        public Guid WebId { get; private set; }
        public Guid ListId { get; private set; }
        public string UserId { get; private set; }

        public DateTime LastItemModifiedDate { get; set; }
        public int ItemCount { get; set; }

        private Dictionary<string, string> Queries { get; set; }

        public void Add(string key, string viewXml)
        {
            if (!string.IsNullOrEmpty(viewXml))
            {
                if (Queries.ContainsKey(key))
                {
                    Queries[key] = viewXml;
                }
                else
                {
                    Queries.Add(key, viewXml);
                }
            }
        }

        public string FindKey(string viewXml)
        {
            return Queries.Where(q => q.Value.Equals(viewXml)).Select(q => q.Key).SingleOrDefault();
        }

        public void Clear()
        {
            foreach (var cache in this.Queries.Keys.Select(key => SPCache.Cache<DataTable>(null)
                   .By(key,
                       this.ListId.ToString(),
                       this.WebId.ToString(),
                       this.SiteId.ToString())
                   .By(UserId)).Where(cache => cache != null))
            {
                cache.Clear();
            }

            this.Queries.Clear();
        }

        public ICachedObject<DataTable> GetDataCache(string keyQuery, Func<DataTable> loader)
        {
            if (Queries.ContainsKey(keyQuery))
            {
                return SPCache.Cache(loader)
                    .By(keyQuery,
                        this.ListId.ToString(),
                        this.WebId.ToString(),
                        this.SiteId.ToString(),
                        this.UserId);
            }

            return null;
        }
    }

}
