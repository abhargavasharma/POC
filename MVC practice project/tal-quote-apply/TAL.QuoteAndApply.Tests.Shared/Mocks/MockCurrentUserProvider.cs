using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.Infrastructure.Http;

namespace TAL.QuoteAndApply.Tests.Shared.Mocks
{
    public class MockCurrentUserProvider : ICurrentUserProvider
    {
        public ICurrentUser GetForApplication()
        {
            return new TestingCurrentUser();
        }
    }

    public class TestingCurrentUser : ICurrentUser
    {
        public string UserName
        {
            get
            {
                return "TAL.QuoteAndApply.Testing";
            }
        }

        public IEnumerable<Role> Roles
        {
            get { return new List<Role>(); }
        }

        public string EmailAddress
        {
            get
            {
                return "TAL.QuoteAndApply.Testing@test.com";
            }
        }

        public string GivenName
        {
            get
            {
                return "TAL.QuoteAndApply.Testing";
            }
        }
        public string Surname
        {
            get
            {
                return "TAL.QuoteAndApply.Testing";
            }
        }
    }

    public class MockHttpProvider : IHttpContextProvider
    {
        private readonly HttpContextBase _httpContextBase;
        public MockHttpProvider()
        {
            _httpContextBase = new HttpContextForCache();
        }

        public MockHttpProvider(HttpContextBase httpContextBase)
        {
            _httpContextBase = httpContextBase;
        }

        public HttpContextBase GetCurrentContext()
        {
            return _httpContextBase;
        }
    }


    public class HttpContextForCache : HttpContextBase
    {
        public HttpContextForCache()
        {
            Items = new Hashtable();
        }

        public override IDictionary Items { get; }
    }


    public class MockCacheWrapper : ICachingWrapper
    {
        public void RemoveItem(Type callingClass, string key)
        {
            
        }

        public T UpdateOrAddCacheItemSliding<T>(Type callingClass, string key, TimeSpan slidingDuration, T value)
        {
            return value;
        }

        public T GetOrAddCacheItemSliding<T>(Type callingClass, string key, TimeSpan slidingDuration, Func<T> getItemFunc)
        {
            return getItemFunc.Invoke();
        }

        public T UpdateOrAddCacheItemAbsolute<T>(Type callingClass, string key, DateTime expirationTime, T value)
        {
            return value;
        }

        public T GetOrAddCacheItemAbsolute<T>(Type callingClass, string key, DateTime expirationTime, Func<T> getItemFunc)
        {
            return getItemFunc.Invoke();
        }

        public T GetOrAddCacheItemRequestScope<T>(Type callingClass, string key, Func<T> getItemFunc)
        {
            return getItemFunc.Invoke();
        }

        public T GetOrAddCacheItemIndefinite<T>(Type callingClass, string key, Func<T> getItemFunc)
        {
            return getItemFunc.Invoke();
        }

        public T UpdateOrAddCacheItemIndefinite<T>(Type callingClass, string key, T value)
        {
            return value;
        }

        public T UpdateOrAddCacheItemRequestScope<T>(Type callingClass, string key, T value)
        {
            return value;
        }
    }
}
