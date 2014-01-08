using System;
using System.Web.Caching;

namespace SPCore.Cache
{
    /// <summary>
    /// Represents a cached object.
    /// </summary>
    /// <typeparam name="T">Type of the cached object</typeparam>
    public interface ICachedObject<out T>
    {
        /// <summary>
        /// Adds new keys that identify the cached object.
        /// </summary>
        /// <param name="cacheKeys">The cache keys</param>
        /// <returns>Fluent interface for further configuring the cache</returns>
        ICachedObject<T> By(params string[] cacheKeys);
        /// <summary>
        /// Specifies that the cache should be keyed by the current user name.
        /// </summary>
        /// <returns>Fluent interface for further configuring the cache</returns>
        ICachedObject<T> ByCurrentUser();
        /// <summary>
        /// Specifies that the cache should be keyed by SPContext.Current.Item.
        /// </summary>
        /// <returns>Fluent interface for further configuring the cache</returns>
        ICachedObject<T> ByCurrentItem();

        ICachedObject<T> ByCurrentList();

        /// <summary>
        /// Specifies that the cache should be keyed by  SPContext.Current.Web.
        /// </summary>
        /// <returns>Fluent interface for further configuring the cache</returns>
        ICachedObject<T> ByCurrentWeb();
        /// <summary>
        /// Specifies the absolute duration of the cache.
        /// </summary>
        /// <returns>Fluent interface for further configuring the cache</returns>
        ICachedObject<T> For(TimeSpan cacheTime);
        /// <summary>
        /// Specifies the sliding duration of the cache, i.e., the time after which the cache will be removed
        /// if it has not been accessed.
        /// </summary>
        /// <returns>Fluent interface for further configuring the cache</returns>
        ICachedObject<T> ForSliding(TimeSpan cacheTime);
        /// <summary>
        /// Specifies the cache priority.
        /// </summary>
        /// <seealso cref="CacheItemPriority"/>
        /// <returns>Fluent interface for further configuring the cache</returns>
        ICachedObject<T> Priority(CacheItemPriority cachePriority);

        void Clear();

        bool Exists();

        /// <summary>
        /// Retrieves the cached object. If found from cache (by the key) then the cached object is returned.
        /// Otherwise, the cachedObjectLoader is called to load the object, and it is then added to cache.
        /// </summary>
        T CachedObject { get; }

        /// <summary>
        /// Returns the cache key for the current cached object.
        /// </summary>
        string CacheKey { get; }
    }
}
