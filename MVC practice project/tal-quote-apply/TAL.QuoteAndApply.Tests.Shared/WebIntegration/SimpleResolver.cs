using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Autofac;

namespace TAL.QuoteAndApply.Tests.Shared.WebIntegration
{
    /// <summary>
    /// This is needed because setting the Autofac MVC resolver to be the 
    /// DependencyResolver does not work when using an InMemory WebServer
    /// </summary>
    public class SimpleResolver : IDependencyResolver
    {
        private readonly IContainer _container;

        public SimpleResolver(IContainer container)
        {
            _container = container;
        }

        public object GetService(Type serviceType)
        {
            return _container.Resolve(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            throw new NotImplementedException();
        }
    }
}