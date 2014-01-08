using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Caching;
using Microsoft.SharePoint;

namespace SPCore.Cache
{
    public static class SPCache
    {
        public static ICachedObject<T> Cache<T>(Func<T> cachedObjectLoader)
        {
            return new CachedObjectImpl<T>(cachedObjectLoader);
        }

        private class CachedObjectImpl<T> : ICachedObject<T>
        {
            readonly Func<T> _loader;
            readonly List<string> _keys = new List<string>();
            CacheItemPriority _priority = CacheItemPriority.Normal;
            DateTime _absoluteExpiration = System.Web.Caching.Cache.NoAbsoluteExpiration;
            TimeSpan _slidingExpiration = System.Web.Caching.Cache.NoSlidingExpiration;

            public T CachedObject
            {
                get { return GetObject(); }
            }

            private T GetObject()
            {
                string key = this.CacheKey;

                // try to get the query result from the cache
                T result;

                if (!Get(key, out result))
                {
                    if (_loader != null)
                    {
                        result = _loader();
                        Add(result, key);
                    }
                }

                return result;
            }

            public CachedObjectImpl(Func<T> cached)
            {
                _loader = cached;
                By(typeof(T).FullName);
            }

            public ICachedObject<T> By(params string[] cacheKeys)
            {
                _keys.AddRange(cacheKeys);
                return this;
            }

            public ICachedObject<T> ByCurrentUser()
            {
                if (SPContext.Current == null) return this;
                return By(SPContext.Current.Web.CurrentUser.Name);
            }

            public ICachedObject<T> ByCurrentItem()
            {
                return SPContext.Current == null ? this : By(SPContext.Current.Item.ID.ToString(CultureInfo.InvariantCulture));
            }

            public ICachedObject<T> ByCurrentList()
            {
                return SPContext.Current == null ? this : By(SPContext.Current.List.ID.ToString());
            }

            public ICachedObject<T> ByCurrentWeb()
            {
                return SPContext.Current == null ? this : By(SPContext.Current.Web.Url);
            }

            public ICachedObject<T> For(TimeSpan cacheTime)
            {
                _absoluteExpiration = DateTime.UtcNow.Add(cacheTime);
                _slidingExpiration = System.Web.Caching.Cache.NoSlidingExpiration;
                return this;
            }
            public ICachedObject<T> ForSliding(TimeSpan cacheTime)
            {
                _slidingExpiration = cacheTime;
                _absoluteExpiration = System.Web.Caching.Cache.NoAbsoluteExpiration;
                return this;
            }

            public ICachedObject<T> Priority(CacheItemPriority cachePriority)
            {
                _priority = cachePriority;
                return this;
            }

            /// <summary>
            /// Remove item from cache
            /// </summary>
            public void Clear()
            {
                HttpRuntime.Cache.Remove(CacheKey);
            }

            /// <summary>
            /// Check for item in cache
            /// </summary>
            /// <returns></returns>
            public bool Exists()
            {
                return HttpRuntime.Cache[CacheKey] != null;
            }

            public string CacheKey
            {
                get { return string.Join("_", _keys.OrderBy(x => x).ToArray()); }
            }

            private void Add(T o, string key)
            {
                HttpRuntime.Cache.Insert(
                    key,
                    o,
                      null, // no cache dependency
                    _absoluteExpiration,
                    _slidingExpiration,
                    _priority,
                    null); // no removal notification
            }

            //private void Add(T o, string key, uint cacheExpirationMinutes)
            //{
            //    HttpRuntime.Cache.Insert(
            //      key,
            //      o,
            //        null, // no cache dependency
            //      DateTime.Now.AddMinutes(cacheExpirationMinutes),
            //      _slidingExpiration,
            //      _priority,
            //      null); // no removal notification
            //}


            /// <summary>
            /// Retrieve cached item
            /// </summary>
            /// <typeparam name="T">Type of cached item</typeparam>
            /// <param name="key">Name of cached item</param>
            /// <param name="value">Cached value. Default(T) if item doesn't exist.</param>
            /// <returns>Cached item as type</returns>
            private static bool Get(string key, out T value)
            {
                try
                {
                    object cachedObject = HttpRuntime.Cache[key];

                    if (cachedObject == null)
                    {
                        value = default(T);
                        return false;
                    }

                    value = (T)cachedObject;
                }
                catch
                {
                    value = default(T);
                    return false;
                }

                return true;
            }
        }
    }
}
