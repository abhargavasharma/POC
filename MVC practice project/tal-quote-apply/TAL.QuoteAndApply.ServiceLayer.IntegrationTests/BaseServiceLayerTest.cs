using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Autofac;
using log4net;
using Moq;
using TAL.QuoteAndApply.Infrastructure.Http;
using TAL.QuoteAndApply.Infrastructure.Ioc;
using TAL.QuoteAndApply.Infrastructure.Logging;
using TAL.QuoteAndApply.Infrastructure.Observer;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Ioc;
using TAL.QuoteAndApply.ServiceLayer.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Product.Rules;
using TAL.QuoteAndApply.ServiceLayer.Search.AgentDashboard;
using TAL.QuoteAndApply.ServiceLayer.Search.AgentDashboard.Models;
using TAL.QuoteAndApply.ServiceLayer.User;
using TAL.QuoteAndApply.Tests.Shared.Builders;
using TAL.QuoteAndApply.Tests.Shared.Helpers;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.ServiceLayer.IntegrationTests
{
    public abstract class BaseServiceLayerTest
    {
        public IContainer _container;
        public ApplicationCurrentUser _user;
        protected Mock<IProductErrorMessageService> MockProductErrorMessageService;
        protected Mock<IPlanErrorMessageService> MockPlanErrorMessageService;
        protected Mock<ILog> MockLog;
        protected Mock<IPolicyInitialisationMetadataSessionStorageService> MockPolicyInitialisationMetadataSessionStorageService;
        protected Mock<ILoggingService> MockLoggingService;
        protected TestCurrentProductBrandProvider CurrentProductBrandProvider;

        public BaseServiceLayerTest()
        {
            var mockRepository = new MockRepository(MockBehavior.Strict);
            _user = new ApplicationCurrentUser();
            MockProductErrorMessageService = mockRepository.Create<IProductErrorMessageService>();
            MockPlanErrorMessageService = mockRepository.Create<IPlanErrorMessageService>();
            MockLog = mockRepository.Create<log4net.ILog>();
            MockLoggingService = mockRepository.Create<ILoggingService>();
            MockPolicyInitialisationMetadataSessionStorageService = mockRepository.Create<IPolicyInitialisationMetadataSessionStorageService>();
            MockPolicyInitialisationMetadataSessionStorageService.Setup(call => call.GetMetaData()).Returns(() => null);
            CurrentProductBrandProvider = new TestCurrentProductBrandProvider();
        }

        public void EnsureContainer()
        {
            if (_container == null)
            {
                var builder = new ContainerBuilder();
                RegisterModuleMappings(builder, typeof(ServiceLayer.Ioc.RegistrationModule), "TAL.QuoteAndApply.");

                

                var mockRequest = new Mock<HttpRequestBase>();
                mockRequest.Setup(call => call.Url)
                    .Returns(new Uri("http://test-talsalesportal.delivery.lan/abc/def?abc=123"));

                var mockHttpContext = new Mock<HttpContextBase>();
                mockHttpContext.Setup(call => call.Request).Returns(mockRequest.Object);

                var items = new Hashtable();
                mockHttpContext.Setup(ctx => ctx.Items).Returns(items);
                var contextProvider = new MockHttpProvider(mockHttpContext.Object);
                builder.RegisterInstance(contextProvider).As<IHttpContextProvider>().SingleInstance();

                //add some mocks
                builder.RegisterInstance(CurrentProductBrandProvider).As<ICurrentProductBrandProvider>();
                builder.RegisterInstance(MockProductErrorMessageService.Object).As<IProductErrorMessageService>();
                builder.RegisterInstance(MockPlanErrorMessageService.Object).As<IPlanErrorMessageService>();
                builder.RegisterInstance(MockLog.Object).As<log4net.ILog>();
                builder.RegisterInstance(MockLoggingService.Object).As<ILoggingService>();
                builder.RegisterInstance(MockPolicyInitialisationMetadataSessionStorageService.Object).As<IPolicyInitialisationMetadataSessionStorageService>();
                builder.RegisterInstance(_user).As<IApplicationCurrentUser>();

                _container = builder.Build();

                var eventRegistrations = _container.Resolve<IEnumerable<IEventsRegistration>>();
                foreach (var eventReg in eventRegistrations)
                {
                    eventReg.RegisterAllObserversWithSubjects();
                }
            }
        }

        public string GetApplicationCurrentUser()
        {
            return _user.CurrentUser;
        }

        public string SetApplicationCurrentUser(string userName)
        {
            return _user.CurrentUser = userName;
        }

        public T GetServiceInstance<T>()
        {
            EnsureContainer();

            return _container.Resolve<T>();
        }

        public T GetServiceInstanceWithUser<T>(string user)
        {
            EnsureContainer();

            return _container.Resolve<T>();
        }

        public void RegisterModuleMappings(ContainerBuilder builder, Type type, string assemblyStartsWith)
        {
            var resolver = new IocRegistrationResolver();
            resolver.LoadAssembly(Assembly.GetAssembly(type), assemblyStartsWith);
            var mappings = resolver.GetMappings();

            foreach (var typeMapItem in mappings.Where(m => m.WithMatchInterface && !m.IsSingletonScope))
            {
                builder.RegisterType(typeMapItem.ResolveToType).InstancePerLifetimeScope().AsImplementedInterfaces();
            }

            foreach (var typeMapItem in mappings.Where(m => !m.WithMatchInterface && !m.IsSingletonScope))
            {
                builder.RegisterType(typeMapItem.ResolveToType).InstancePerLifetimeScope().As(typeMapItem.RegisteredType);
            }

            foreach (var typeMapItem in mappings.Where(m => m.WithMatchInterface && m.IsSingletonScope))
            {
                builder.RegisterType(typeMapItem.ResolveToType).SingleInstance().AsImplementedInterfaces();
            }

            foreach (var typeMapItem in mappings.Where(m => !m.WithMatchInterface && m.IsSingletonScope))
            {
                builder.RegisterType(typeMapItem.ResolveToType).SingleInstance().As(typeMapItem.RegisteredType);
            }
        }


        protected PolicyWithRisks GetPolicy(string quoteRef)
        {
            var svc = GetServiceInstance<IPolicyWithRisksService>();

            return svc.GetFrom(quoteRef);
        }

        protected IRisk GetRisk(int riskid)
        {
            var svc = GetServiceInstance<IRiskService>();

            return svc.GetRisk(riskid);
        }

        protected void UpdatePolicy(PolicyWithRisks policyWithRisks)
        {
            var svc = GetServiceInstance<IPolicyWithRisksService>();
            svc.SaveAll(policyWithRisks);
        }

        protected CreateQuoteResult CreateQuoteWithDefaults()
        {
            var defaultCreateQuote = new CreateQuoteBuilder().Build();
            return CreateQuote(defaultCreateQuote);
        }

        protected CreateQuoteResult CreateQuote(CreateQuoteParam createQuoteParam)
        {
            var createQuoteService = GetServiceInstance<ICreateQuoteService>();
            return createQuoteService.CreateQuote(createQuoteParam);
        }


        protected AgentDashboardQuotesResult FindQuoteInAgentDashboard(AgentDashboardRequest request, string quoteReference)
        {
            //Beacause of paging, keep looking through pages of quotes until we find the one we're looking for
            const int resultsPerPage = 5;

            AgentDashboardQuotesResult foundQuoteResult = null;
            var agentDaschboardSvc = GetServiceInstance<IGetAgentDashboardQuotesService>();

            request.PageNumber = 1;
            while (foundQuoteResult == null)
            {
                var quotes = agentDaschboardSvc.GetQuotes(request, resultsPerPage);
                foundQuoteResult = quotes.FirstOrDefault(x => x.QuoteReference == quoteReference);

                if (foundQuoteResult == null)
                {
                    if (quotes.Count() < resultsPerPage)
                    {
                        //If exhausted search results return null (because we also test that quotes are not found);
                        return null;
                    }

                    request.PageNumber += 1;
                }

            }

            return foundQuoteResult;
        }


    }
}