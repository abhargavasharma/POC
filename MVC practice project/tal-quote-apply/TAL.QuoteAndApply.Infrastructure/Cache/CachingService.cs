using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Runtime.Caching;
using System.Web;
using TAL.QuoteAndApply.Infrastructure.Http;

namespace TAL.QuoteAndApply.Infrastructure.Cache
{
    public interface ICachingService
    {
        void RemoveItem(string key);
        Lazy<object> UpdateOrAddCacheItemSliding(string key, TimeSpan slidingDuration, object value);
        Lazy<object> GetOrAddCacheItemSliding(string key, TimeSpan slidingDuration, Func<object> getItemFunc);
        Lazy<object> UpdateOrAddCacheItemAbsolute(string key, DateTime expirationTime, object value);
        Lazy<object> GetOrAddCacheItemAbsolute(string key, DateTime expirationTime, Func<object> getItemFunc);
        CacheItem GetOrAddCacheItemRequestScope(string key, Func<object> getItemFunc);
        CacheItem UpdateOrAddCacheItemRequestScope(string key, object value);
        CacheItem UpdateOrAddCacheItemIndefinite(string key, object value);
        CacheItem GetOrAddCacheItemIndefinite(string key, Func<object> getItemFunc);
    }

    public class CachingService : ICachingService
    {
        private static readonly MemoryCache Cache = new MemoryCache("CachingProvider");
        private static readonly ConcurrentDictionary<string, CacheItem> PermanentCache = new ConcurrentDictionary<string, CacheItem>();
        protected static readonly object Padlock = new object();

        private readonly IHttpContextProvider _httpContext;

        public CachingService(IHttpContextProvider httpContext)
        {
            _httpContext = httpContext;
        }

        public void RemoveItem(string key)
        {
            lock (Padlock)
            {
                Cache.Remove(key);
            }
            var items = _httpContext.GetCurrentContext().Items;
            lock (items.SyncRoot)
            {
                items.Remove(key);
            }
        }

        public Lazy<object> UpdateOrAddCacheItemSliding(string key, TimeSpan slidingDuration, object value)
        {
            var res = new CacheItem(key, new Lazy<object>(() => value));
            Cache.Set(res, new CacheItemPolicy() { SlidingExpiration = slidingDuration });
            return res.Value as Lazy<object>;
        }

        public Lazy<object> GetOrAddCacheItemSliding(string key, TimeSpan slidingDuration, Func<object> getItemFunc)
        {
            var newValue = new Lazy<object>(getItemFunc);
            var cachedValued = (Lazy<object>)Cache.AddOrGetExisting(key, newValue, new CacheItemPolicy() { SlidingExpiration = slidingDuration });
            return cachedValued ?? newValue;
        }

        public Lazy<object> UpdateOrAddCacheItemAbsolute(string key, DateTime expirationTime, object value)
        {
            var res = new CacheItem(key, new Lazy<object>(() => value));
            Cache.Set(res, new CacheItemPolicy() { AbsoluteExpiration = expirationTime });
            return res.Value as Lazy<object>;
        }

        public Lazy<object> GetOrAddCacheItemAbsolute(string key, DateTime expirationTime, Func<object> getItemFunc)
        {
            var newValue = new Lazy<object>(getItemFunc);
            var cachedValued = (Lazy<object>)Cache.AddOrGetExisting(key, newValue, new CacheItemPolicy() { AbsoluteExpiration = expirationTime });
            return cachedValued ?? newValue;
        }

        public CacheItem UpdateOrAddCacheItemIndefinite(string key, object value)
        {
            return PermanentCache.AddOrUpdate(key, new CacheItem(key, value), (k, v) => new CacheItem(k, value));
        }

        public CacheItem GetOrAddCacheItemIndefinite(string key, Func<object> getItemFunc)
        {
            return PermanentCache.GetOrAdd(key, k => new CacheItem(k, getItemFunc.Invoke()));
        }

        public CacheItem GetOrAddCacheItemRequestScope(string key, Func<object> getItemFunc)
        {
            var items = _httpContext.GetCurrentContext().Items;
            lock (items.SyncRoot)
            {
                var cacheItem = items[key] as CacheItem;

                if (cacheItem == null)
                {
                    cacheItem = (CacheItem)(items[key] = new CacheItem(key, getItemFunc.Invoke()));
                }
                else
                {
                    if (!ShouldItemBeCached(cacheItem.Value))
                    {
                        cacheItem = (CacheItem)(items[key] = new CacheItem(key, getItemFunc.Invoke()));
                    }
                }
                return cacheItem;
            }
        }

        public CacheItem UpdateOrAddCacheItemRequestScope(string key, object value)
        {
            var items = _httpContext.GetCurrentContext().Items;
            lock (items.SyncRoot)
            {
                var item = items[key] as CacheItem ?? (CacheItem)(items[key] = new CacheItem(key, value));
                item.Value = value;
                return item;
            }
        }

        private bool ShouldItemBeCached(object value)
        {
            if (value == null)
                return false;

            var enumerable = value as System.Collections.IEnumerable;
            if (enumerable != null && !enumerable.GetEnumerator().MoveNext())
                return false;

            return true;
        }
    }
}