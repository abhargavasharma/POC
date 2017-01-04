using System;
using System.Collections;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Moq;
using NUnit.Framework;
using TAL.Performance.Infrastructure.Core;
using TAL.Performance.Infrastructure.Core.ApiClients;
using TAL.Performance.Infrastructure.Core.WebServers;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Infrastructure.Http;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests.Clients;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.SalesPortal.Web.User;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.Tests.Shared.Mocks;
using TAL.QuoteAndApply.Tests.Shared.WebIntegration;
using TAL.QuoteAndApply.Web.Shared.Cookie;
using TAL.QuoteAndApply.Web.Shared.HttpResults;
using TAL.QuoteAndApply.Web.Shared.Ioc;
using Task = System.Threading.Tasks.Task;

namespace TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests
{
    public abstract class BaseTestClass<T> where T : SalesPortalApiClient, new()
    {
        protected WebApiClient WebApiClient;
        protected IWebServer WebServer;
        protected T Client;
        private readonly IPerformanceTestTool _performanceTestTool;
        protected IContainer Container;
        protected ISalesPortalSessionContext SalesPortalContext;

        protected BaseTestClass(IPerformanceTestTool performanceTestTool)
        {
            _performanceTestTool = performanceTestTool;
            SalesPortalContext = new SalesPortalIntegrationTestSessionContext();
            SalesPortalContext.Set(new SalesPortalSession("IntegrationTests", new[] {Role.Agent}, "mmd@tal.com.au", "TestFirstName", "Test", "TAL"));
        }

        protected void AddUnderwriterPermission()
        {
            SalesPortalContext.Set(new SalesPortalSession("IntegrationTests", new[] { Role.Underwriter }, "mmd@tal.com.au", "TestFirstName", "Test", "TAL"));
        }

        protected void SetupTestClient<T>(T testClient) where T : SalesPortalApiClient
        {
            testClient.AssignDependencies(WebApiClient, _performanceTestTool);
        }

        protected string GetDefaultBrandInSession()
        {
            return SalesPortalContext.SalesPortalSession.SelectedBrand;
        }

        protected async Task LogOutAndLoginWithBrand(string brandKey)
        {
            SalesPortalContext.Clear();

            var loginClient = GetSalesPortalLoginClient();

            var loginRequest = new LoginRequest
            {
                UserName = "QaTalYbAgent_QA",
                Password = "Transform2001!",
                UseWindowsAuth = false
            };

            await loginClient.AttemptLoginAsync<RedirectToResponse>(loginRequest, false);

            await loginClient.SaveSelectedBrand(new SaveBrandRequest() {Brand = brandKey }, false);
        }

        protected bool PolicyHasOwnerType(string quoteRef, PolicyOwnerType policyOwnerType)
        {

            var policyOverviewProvider = Container.Resolve<IPolicyOverviewProvider>();
            return policyOverviewProvider.GetFor(quoteRef).OwnerType == policyOwnerType;
        }

        protected bool GetLifeInsured(string quoteRef, PolicyOwnerType policyOwnerType)
        {

            var policyOverviewProvider = Container.Resolve<IPolicyOverviewProvider>();
            return policyOverviewProvider.GetFor(quoteRef).OwnerType == policyOwnerType;
        }

        protected async Task<PolicyWithRisks> CreatePolicyFromRatingFactorsAsync(RatingFactorsRequest ratingFactors)
        {
            var createClientRequest = new CreateClientRequest
            {
                PolicyOwner = new ClientRequest {RatingFactors = ratingFactors}
            };

            var policyClient = new PolicyClient();
            SetupTestClient(policyClient);
            var createResult = await policyClient.CreateAsync<RedirectToResponse>(createClientRequest);

            //Grab end of redirect url to get Quote Reference
            var quoteReference = createResult.RedirectTo.Split('/').Last();

            if (quoteReference.Contains("?"))
            {
                var parts = quoteReference.Split('?');
                quoteReference = parts.First();
            }

            //No endpoint that gets policy with risks so hack in and grab service from container
            var policyWithRisksSvc = Container.Resolve<IPolicyWithRisksService>();
            var policyWithRisks = policyWithRisksSvc.GetFrom(quoteReference);

            return policyWithRisks;
        }

        [TestFixtureSetUp]
        public void SetupWebApi()
        {
            try
            {
                DbItemClassMapper<DbItem>.RegisterClassMaps();

                Action<HttpConfiguration> action = (config) =>
                {
                    //call sales portal register web api 
                    WebApiConfig.Register(config);

                    //InMemoryWebServer doesn't allow use of MVC, so mock out the MVC default route
                    //This is so if we call the Url helper (Url.Link) on controllers a URL can be generated
                    AddMockMvcRoute(config);

                    var baseIocConfig = new BaseIocConfig();

                    //call sales portal Ioc Setup. This adds dependencied to container builder
                    var containerBuilder = baseIocConfig.SetupContainerBuilder(IocConfig.RegisterSalesPortal);

                    //Override some dependencies already in container with Mocks
                    //Mocking out HttpContextBase
                    RegisterMocks(containerBuilder);

                    //Create the container
                    Container = baseIocConfig.CreateContainer(containerBuilder);

                    //Register the container with WebApi
                    baseIocConfig.RegisterContainerWithWebApi(config, Container, containerBuilder);

                    //Register the observers
                    baseIocConfig.RegisterObservers(config);

                    //We cannot set the DependencyResolver with the AutoFac MVC resolver as InMemoryWebServer does not support MVC funtcions
                    //This needs to be set as the ValidationAttributes in the sales portal use DependencyResolver
                    AddMockMvcDependencyResolver(Container);

                    //configurare log4Net
                    log4net.Config.XmlConfigurator.Configure();

                };

                WebServer = new InMemoryWebServer(action);
                WebApiClient = new WebApiClient(WebServer, new Json());

                Client = new T();
                SetupTestClient(Client);

                //todo: need to work how to simulate a windows user for this to work
                //WebServer = new ExternalWebServer("http://local-talsalesportal.delivery.lan/");
            }
            catch (Exception err)
            {
                Console.WriteLine("ERROR IN TEST FIXTURE SETUP:" + err);
            }
        }

        private void AddMockMvcDependencyResolver(IContainer container)
        {
            DependencyResolver.SetResolver(new SimpleResolver(container));
        }

        private void AddMockMvcRoute(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "Default",
                routeTemplate: "{controller}/{action}/{id}"
                );
        }

        private void RegisterMocks(ContainerBuilder containerBuilder)
        {
            var mockRepository = new MockRepository(MockBehavior.Strict);

            var context = mockRepository.Create<HttpContextBase>();
            var request = mockRepository.Create<HttpRequestBase>();
            var response = mockRepository.Create<HttpResponseBase>();
            var session = mockRepository.Create<HttpSessionStateBase>();
            var server = mockRepository.Create<HttpServerUtilityBase>();
            var user = mockRepository.Create<IPrincipal>();
            var identity = mockRepository.Create<IIdentity>();
            var items = new Hashtable();

            context.Setup(h => h.Request).Returns(request.Object);
            context.Setup(h => h.Response).Returns(response.Object);

            context.Setup(ctx => ctx.Request).Returns(request.Object);
            context.Setup(ctx => ctx.Response).Returns(response.Object);
            context.Setup(ctx => ctx.Session).Returns(session.Object);
            context.Setup(ctx => ctx.Server).Returns(server.Object);
            context.Setup(ctx => ctx.Items).Returns(items);

            request.Setup(r => r.Url).Returns(new Uri("http://tal.com.au"));
            
            identity.Setup(x => x.Name).Returns("TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests");
            user.Setup(h => h.Identity).Returns(identity.Object);
            context.Setup(h => h.User).Returns(user.Object);

            var contextProvider = new MockHttpProvider(context.Object);

            containerBuilder.RegisterInstance(contextProvider).As<IHttpContextProvider>().SingleInstance();
            containerBuilder.RegisterInstance(context.Object).As<HttpContextBase>().SingleInstance();
            containerBuilder.RegisterType<MockCookieService>().As<ICookieService>().SingleInstance();

            containerBuilder.RegisterInstance(SalesPortalContext).AsImplementedInterfaces().SingleInstance();
        }

        public PlanClient GetSalesPortalPlanClient()
        {
            return GetSalesPortalClient<PlanClient>(WebServer, _performanceTestTool);
        }

        public ReferralClient GetSalesPortalReferralClient()
        {
            return GetSalesPortalClient<ReferralClient>(WebServer, _performanceTestTool);
        }

        public PolicyClient GetSalesPortalPolicyClient()
        {
            return GetSalesPortalClient<PolicyClient>(WebServer, _performanceTestTool);
        }

        public RiskClient GetSalesPortalRiskClient()
        {
            return GetSalesPortalClient<RiskClient>(WebServer, _performanceTestTool);
        }

        public LoginClient GetSalesPortalLoginClient()
        {
            return GetSalesPortalClient<LoginClient>(WebServer, _performanceTestTool);
        }

        private void AssignClientDependencies(SalesPortalApiClient client, IWebServer webServer, IPerformanceTestTool performanceTestTool)
        {
            client.AssignDependencies(new WebApiClient(webServer, new Json()), performanceTestTool);
        }

        public T GetSalesPortalClient<T>(IWebServer webServer, IPerformanceTestTool performanceTestTool) where T : SalesPortalApiClient
        {
            var retVal = Activator.CreateInstance<T>();
            AssignClientDependencies(retVal, webServer, performanceTestTool);
            return retVal;
        }
    }
}