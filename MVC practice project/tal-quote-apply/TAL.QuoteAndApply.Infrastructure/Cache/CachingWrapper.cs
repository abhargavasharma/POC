using System;
using TAL.QuoteAndApply.Infrastructure.Http;

namespace TAL.QuoteAndApply.Infrastructure.Cache
{
    public interface ICachingWrapper
    {
        void RemoveItem(Type callingClass, string key);
        T UpdateOrAddCacheItemSliding<T>(Type callingClass, string key, TimeSpan slidingDuration, T value);
        T GetOrAddCacheItemSliding<T>(Type callingClass, string key, TimeSpan slidingDuration, Func<T> getItemFunc);
        T UpdateOrAddCacheItemAbsolute<T>(Type callingClass, string key, DateTime expirationTime, T value);
        T GetOrAddCacheItemAbsolute<T>(Type callingClass, string key, DateTime expirationTime, Func<T> getItemFunc);
        T GetOrAddCacheItemRequestScope<T>(Type callingClass, string key, Func<T> getItemFunc);
        T GetOrAddCacheItemIndefinite<T>(Type callingClass, string key, Func<T> getItemFunc);
        T UpdateOrAddCacheItemIndefinite<T>(Type callingClass, string key, T value);
        T UpdateOrAddCacheItemRequestScope<T>(Type callingClass, string key, T value);
    }

    public class CachingWrapper : ICachingWrapper
    {
        private readonly ICachingService _cachingService;

        public CachingWrapper(IHttpContextProvider contextProvider)
        {
            _cachingService = new CachingService(contextProvider);
        }

        private string BuildKey(Type callingClass, string key)
        {
            return $"{callingClass.FullName}.{key}";
        }

        public void RemoveItem(Type callingClass, string key)
        {
            var actualKey = BuildKey(callingClass, key);
            _cachingService.RemoveItem(actualKey);
        }

        public T UpdateOrAddCacheItemSliding<T>(Type callingClass, string key, TimeSpan slidingDuration, T value)
        {
            var actualKey = BuildKey(callingClass, key);
            return (T)_cachingService.UpdateOrAddCacheItemSliding(actualKey, slidingDuration, value).Value;
        }

        public T GetOrAddCacheItemSliding<T>(Type callingClass, string key, TimeSpan slidingDuration, Func<T> getItemFunc)
        {
            var actualKey = BuildKey(callingClass, key);
            return (T)_cachingService.GetOrAddCacheItemSliding(actualKey, slidingDuration, () => getItemFunc.Invoke()).Value;
        }

        public T UpdateOrAddCacheItemAbsolute<T>(Type callingClass, string key, DateTime expirationTime, T value)
        {
            var actualKey = BuildKey(callingClass, key);
            return (T)_cachingService.UpdateOrAddCacheItemAbsolute(actualKey, expirationTime, value).Value;
        }

        public T GetOrAddCacheItemAbsolute<T>(Type callingClass, string key, DateTime expirationTime, Func<T> getItemFunc)
        {
            var actualKey = BuildKey(callingClass, key);
            return (T)_cachingService.GetOrAddCacheItemAbsolute(actualKey, expirationTime, () => getItemFunc.Invoke()).Value;
        }

        public T GetOrAddCacheItemRequestScope<T>(Type callingClass, string key, Func<T> getItemFunc)
        {
            var actualKey = BuildKey(callingClass, key);
            return (T)_cachingService.GetOrAddCacheItemRequestScope(actualKey, () => getItemFunc.Invoke()).Value;
        }

        public T GetOrAddCacheItemIndefinite<T>(Type callingClass, string key, Func<T> getItemFunc)
        {
            var actualKey = BuildKey(callingClass, key);
            return (T)_cachingService.GetOrAddCacheItemIndefinite(actualKey, () => getItemFunc.Invoke()).Value;
        }

        public T UpdateOrAddCacheItemIndefinite<T>(Type callingClass, string key, T value)
        {
            var actualKey = BuildKey(callingClass, key);
            return (T)_cachingService.UpdateOrAddCacheItemIndefinite(actualKey, value).Value;
        }

        public T UpdateOrAddCacheItemRequestScope<T>(Type callingClass, string key, T value)
        {
            var actualKey = BuildKey(callingClass, key);
            return (T)_cachingService.UpdateOrAddCacheItemRequestScope(actualKey, value).Value;
        }
    }
}