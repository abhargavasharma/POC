using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.Infrastructure.Http;

namespace TAL.QuoteAndApply.Infrastructure.Tests
{
    [TestFixture]
    public class CachingProviderTests
    {
        private Mock<IHttpContextProvider> _httpContext;

        [SetUp]
        public void Setup()
        {
            _httpContext = new Mock<IHttpContextProvider>(MockBehavior.Strict);
            _httpContext.Setup(call => call.GetCurrentContext().Items).Returns(new Dictionary<string, object>());
        }

        private void NewRequestScope()
        {
            _httpContext.Object.GetCurrentContext().Items.Clear();
        }

        private CachingService SetupService()
        {
            NewRequestScope();
            return new CachingService(_httpContext.Object);
        }

        #region Request Scope Caching

        [Test]
        public void GetOrAddCacheItemRequestScope_ItemDoesNotExist_ReturnsItemFromFunc()
        {
            // Setup
            var service = SetupService();
            var funcRun = false;
            var value = new object();

            // Run
            var cacheItem = service.GetOrAddCacheItemRequestScope("GetOrAddCacheItemRequestScope_ItemDoesNotExist_ReturnsItemFromFunc", () =>
            {
                funcRun = true;
                return value;
            });

            // Assert
            Assert.That(funcRun, Is.True);
            Assert.That(cacheItem.Value, Is.EqualTo(value));
        }

        [Test]
        public void GetOrAddCacheItemRequestScope_ItemExists_ReturnsItemFromCache()
        {
            // Setup
            var service = SetupService();
            var funcRun1 = false;
            var funcRun2 = false;
            var value1 = new object();
            var value2 = new object();

            // Run
            var cacheItem1 = service.GetOrAddCacheItemRequestScope("GetOrAddCacheItemRequestScope_ItemExists_ReturnsItemFromCache", () =>
            {
                funcRun1 = true;
                return value1;
            });

            var cacheItem2 = service.GetOrAddCacheItemRequestScope("GetOrAddCacheItemRequestScope_ItemExists_ReturnsItemFromCache", () =>
            {
                funcRun2 = true;
                return value2;
            });

            // Assert
            Assert.That(funcRun1, Is.True);
            Assert.That(funcRun2, Is.False);
            Assert.That(cacheItem1.Value, Is.EqualTo(value1));
            Assert.That(cacheItem2.Value, Is.Not.EqualTo(value2));
            Assert.That(cacheItem2.Value, Is.EqualTo(value1));
        }

        [Test]
        public void GetOrAddCacheItemRequestScope_NewRequests_ReturnsItemFromFunc()
        {
            // Setup
            var service = SetupService(); 
            var funcRun = false;
            var value = new object();

            // Run
            service.GetOrAddCacheItemRequestScope("GetOrAddCacheItemRequestScope_NewRequests_ReturnsItemFromFunc", () =>
            {
                funcRun = true;
                return value;
            });

            // Assert
            Assert.That(funcRun, Is.True);

            NewRequestScope();
            funcRun = false;

            service.GetOrAddCacheItemRequestScope("GetOrAddCacheItemRequestScope_NewRequests_ReturnsItemFromFunc", () =>
            {
                funcRun = true;
                return value;
            });

            // Assert
            Assert.That(funcRun, Is.True);
        }

        [Test]
        public void UpdateOrAddCacheItemRequestScope_ItemDoesNotExist_ReturnsItemAdded()
        {
            // Setup
            var service = SetupService();
            var value = new object();

            // Run
            var cacheItem = service.UpdateOrAddCacheItemRequestScope("UpdateOrAddCacheItemRequestScope_ItemDoesNotExist_ReturnsItemAdded", value);

            // Assert
            Assert.That(cacheItem.Value, Is.EqualTo(value));
        }

        [Test]
        public void UpdateOrAddCacheItemRequestScope_ItemAlreadyExists_ReturnsItemAdded()
        {
            // Setup
            var service = SetupService();
            var value1 = new object();
            var value2 = new object();

            // Run
            var cacheItem1 = service.GetOrAddCacheItemRequestScope("UpdateOrAddCacheItemRequestScope_ItemAlreadyExists_ReturnsItemAdded", () => value1);
            var cacheItem2 = service.UpdateOrAddCacheItemRequestScope("UpdateOrAddCacheItemRequestScope_ItemAlreadyExists_ReturnsItemAdded", value2);

            // Assert
            Assert.That(cacheItem2.Value, Is.EqualTo(value2));
        }

        #endregion

        #region Sliding Expiration Cache

        [Test]
        public void GetOrAddCacheItemSliding_ItemDoesNotExist_ReturnsItemFromFunc()
        {
            // Setup
            var service = SetupService();
            var funcRun = false;
            var value = new object();

            // Run
            var cacheItem = service.GetOrAddCacheItemSliding("GetOrAddCacheItemSliding_ItemDoesNotExist_ReturnsItemFromFunc", new TimeSpan(0,0,0,10), () =>
            {
                funcRun = true;
                return value;
            });

            // Assert
            Assert.That(cacheItem.Value, Is.EqualTo(value));
            Assert.That(funcRun, Is.True);
        }

        [Test]
        public void GetOrAddCacheItemSliding_ItemExists_ReturnsItemFromCache()
        {
            // Setup
            var service = SetupService();
            var funcRun1 = false;
            var funcRun2 = false;
            var value1 = new object();
            var value2 = new object();

            // Run
            var cacheItem1 = service.GetOrAddCacheItemSliding("GetOrAddCacheItemSliding_ItemExists_ReturnsItemFromCache", new TimeSpan(0, 0, 0, 10), () =>
            {
                funcRun1 = true;
                return value1;
            });

            var cacheItem2 = service.GetOrAddCacheItemSliding("GetOrAddCacheItemSliding_ItemExists_ReturnsItemFromCache", new TimeSpan(0, 0, 0, 10), () =>
            {
                funcRun2 = true;
                return value2;
            });

            // Assert
            Assert.That(cacheItem1.Value, Is.EqualTo(value1));
            Assert.That(cacheItem2.Value, Is.Not.EqualTo(value2));
            Assert.That(funcRun1, Is.True);
            Assert.That(funcRun2, Is.False);
        }

        [Test]
        public void GetOrAddCacheItemSliding_SlidingExpired_ReturnsItemFromFunc()
        {
            // Setup
            var service = SetupService();
            var funcRun = false;
            var value = new object();

            // Run
            var retVal = service.GetOrAddCacheItemSliding("GetOrAddCacheItemSliding_NewRequests_ReturnsItemFromFunc", new TimeSpan(0, 0, 0, 1), () =>
            {
                funcRun = true;
                return value;
            });

            // Assert
            Assert.That(funcRun, Is.False);
            Assert.That(retVal.Value, Is.Not.Null);
            Assert.That(funcRun, Is.True);

            Thread.Sleep(2000);
            funcRun = false;

            retVal = service.GetOrAddCacheItemSliding("GetOrAddCacheItemSliding_NewRequests_ReturnsItemFromFunc", new TimeSpan(0, 0, 0, 1), () =>
            {
                funcRun = true;
                return value;
            });

            // Assert
            Assert.That(funcRun, Is.False);
            Assert.That(retVal.Value, Is.Not.Null);
            Assert.That(funcRun, Is.True);
        }

        [Test]
        public void UpdateOrAddCacheItemSliding_ItemDoesNotExist_ReturnsItemAdded()
        {
            // Setup
            var service = SetupService();
            var value = new object();

            // Run
            var cacheItem = service.UpdateOrAddCacheItemSliding("UpdateOrAddCacheItemSliding_ItemDoesNotExist_ReturnsItemAdded", new TimeSpan(0, 0, 0, 1), value);

            // Assert
            Assert.That(cacheItem.Value, Is.EqualTo(value));
        }

        [Test]
        public void UpdateOrAddCacheItemSliding_ItemAlreadyExists_ReturnsItemAdded()
        {
            // Setup
            var service = SetupService();
            var value1 = new object();
            var value2 = new object();

            // Run
            var cacheItem1 = service.GetOrAddCacheItemSliding("UpdateOrAddCacheItemSliding_ItemAlreadyExists_ReturnsItemAdded", new TimeSpan(0, 0, 0, 1), () => value1);
            var cacheItem2 = service.UpdateOrAddCacheItemSliding("UpdateOrAddCacheItemSliding_ItemAlreadyExists_ReturnsItemAdded", new TimeSpan(0, 0, 0, 1), value2);

            // Assert
            Assert.That(cacheItem2.Value, Is.EqualTo(value2));
        }

        #endregion

        #region Absolute Expiration Cache

        [Test]
        public void GetOrAddCacheItemAbsolute_ItemDoesNotExist_ReturnsItemFromFunc()
        {
            // Setup
            var service = SetupService();
            var funcRun = false;
            var value = new object();

            // Run
            var cacheItem = service.GetOrAddCacheItemAbsolute("GetOrAddCacheItemAbsolute_ItemDoesNotExist_ReturnsItemFromFunc", DateTime.Now.AddSeconds(1), () =>
            {
                funcRun = true;
                return value;
            });

            // Assert
            Assert.That(cacheItem.Value, Is.EqualTo(value));
            Assert.That(funcRun, Is.True);
        }

        [Test]
        public void GetOrAddCacheItemAbsolute_ItemExists_ReturnsItemFromCache()
        {
            // Setup
            var service = SetupService();
            var funcRun1 = false;
            var funcRun2 = false;
            var value1 = new object();
            var value2 = new object();

            // Run
            var cacheItem1 = service.GetOrAddCacheItemAbsolute("GetOrAddCacheItemAbsolute_ItemExists_ReturnsItemFromCache", DateTime.Now.AddSeconds(10), () =>
            {
                funcRun1 = true;
                return value1;
            });

            var cacheItem2 = service.GetOrAddCacheItemAbsolute("GetOrAddCacheItemAbsolute_ItemExists_ReturnsItemFromCache", DateTime.Now.AddSeconds(10), () =>
            {
                funcRun2 = true;
                return value2;
            });

            // Assert
            Assert.That(cacheItem1.Value, Is.EqualTo(value1));
            Assert.That(cacheItem2.Value, Is.Not.EqualTo(value2));
            Assert.That(funcRun1, Is.True);
            Assert.That(funcRun2, Is.False);
        }

        [Test]
        public void GetOrAddCacheItemAbsolute_SlidingExpired_ReturnsItemFromFunc()
        {
            // Setup
            var service = SetupService();
            var funcRun = false;
            var value = new object();

            // Run
            var retVal = service.GetOrAddCacheItemAbsolute("GetOrAddCacheItemAbsolute_SlidingExpired_ReturnsItemFromFunc", DateTime.Now.AddSeconds(1), () =>
            {
                funcRun = true;
                return value;
            });

            // Assert
            Assert.That(funcRun, Is.False);
            Assert.That(retVal.Value, Is.Not.Null);
            Assert.That(funcRun, Is.True);

            Thread.Sleep(1000);
            funcRun = false;

            retVal = service.GetOrAddCacheItemAbsolute("GetOrAddCacheItemAbsolute_SlidingExpired_ReturnsItemFromFunc", DateTime.Now.AddSeconds(1), () =>
            {
                funcRun = true;
                return value;
            });

            // Assert
            Assert.That(funcRun, Is.False);
            Assert.That(retVal.Value, Is.Not.Null);
            Assert.That(funcRun, Is.True);
        }

        [Test]
        public void UpdateOrAddCacheItemAbsolute_ItemDoesNotExist_ReturnsItemAdded()
        {
            // Setup
            var service = SetupService();
            var value = new object();

            // Run
            var cacheItem = service.UpdateOrAddCacheItemAbsolute("UpdateOrAddCacheItemAbsolute_ItemDoesNotExist_ReturnsItemAdded", DateTime.Now.AddSeconds(1), value);

            // Assert
            Assert.That(cacheItem.Value, Is.EqualTo(value));
        }

        [Test]
        public void UpdateOrAddCacheItemAbsolute_ItemAlreadyExists_ReturnsItemAdded()
        {
            // Setup
            var service = SetupService();
            var value1 = new object();
            var value2 = new object();

            // Run
            var cacheItem1 = service.GetOrAddCacheItemAbsolute("UpdateOrAddCacheItemAbsolute_ItemAlreadyExists_ReturnsItemAdded", DateTime.Now.AddSeconds(1), () => value1);
            var cacheItem2 = service.UpdateOrAddCacheItemAbsolute("UpdateOrAddCacheItemAbsolute_ItemAlreadyExists_ReturnsItemAdded", DateTime.Now.AddSeconds(1), value2);

            // Assert
            Assert.That(cacheItem2.Value, Is.EqualTo(value2));
        }

        #endregion
    }
}
