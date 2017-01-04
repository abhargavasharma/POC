using System;
using System.Configuration;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Moq;
using NUnit.Framework;
using TAL.Performance.Infrastructure.Core;
using TAL.Performance.Infrastructure.Core.Data;
using TAL.Performance.Infrastructure.Core.WebServers;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure.Crypto;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.PerformanceEmulator.Emulators;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.SalesPortal.Web;
using TAL.QuoteAndApply.SalesPortal.Web.User;
using TAL.QuoteAndApply.Tests.Shared.Mocks;
using TAL.QuoteAndApply.Tests.Shared.WebIntegration;
using TAL.QuoteAndApply.Underwriting.Configuration;
using TAL.QuoteAndApply.UserRoles.Models;
using TAL.QuoteAndApply.Web.Shared.Cookie;
using TAL.QuoteAndApply.Web.Shared.Ioc;
using IContainer = Autofac.IContainer;
using Task = System.Threading.Tasks.Task;

namespace TAL.QuoteAndApply.PerformanceEmulator.Tests
{
    [TestFixture]
    class CreatePolicyEmulatorTestRunner
    {
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

            context.Setup(h => h.Request).Returns(request.Object);
            context.Setup(h => h.Response).Returns(response.Object);

            context.Setup(ctx => ctx.Request).Returns(request.Object);
            context.Setup(ctx => ctx.Response).Returns(response.Object);
            context.Setup(ctx => ctx.Session).Returns(session.Object);
            context.Setup(ctx => ctx.Server).Returns(server.Object);

            identity.Setup(x => x.Name).Returns("TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests");
            user.Setup(h => h.Identity).Returns(identity.Object);
            context.Setup(h => h.User).Returns(user.Object);

            containerBuilder.RegisterInstance(context.Object).As<HttpContextBase>().SingleInstance();
            containerBuilder.RegisterType<MockCookieService>().As<ICookieService>().SingleInstance();
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

        public void SetupConfig(HttpConfiguration config)
        {
            try
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
                var container = baseIocConfig.CreateContainer(containerBuilder);

                //Register the container with WebApi
                baseIocConfig.RegisterContainerWithWebApi(config, container, containerBuilder);

                //Register the observers
                baseIocConfig.RegisterObservers(config);

                //We cannot set the DependencyResolver with the AutoFac MVC resolver as InMemoryWebServer does not support MVC funtcions
                //This needs to be set as the ValidationAttributes in the sales portal use DependencyResolver
                AddMockMvcDependencyResolver(container);

                //configurare log4Net
                log4net.Config.XmlConfigurator.Configure();
            }
            catch (Exception err)
            {
                Console.WriteLine("ERROR starting web server: " + err);
            }
        }

        [Test, Ignore]
        public void PolicyEmulatorTasks()
        {
            var t = new System.Threading.Tasks.Task[100];

            for (int i = 0; i < t.Length; i++)
                t[i] = System.Threading.Tasks.Task.Run(() => CreatePolicyEmulator());

            System.Threading.Tasks.Task.WaitAll(t);
        }

        [Test]
        public void CreatePolicyEmulator()
        {
            //this is needed because we query the database
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            //replace this w
            var generator = new Random();
            var gender = generator.NextDouble() < 0.5 ? TestPerson.GenderType.Male : TestPerson.GenderType.Female;
            var smoker = generator.NextDouble() < 0.5;
            var income = generator.Next(70000, 300000);
            var dob = DateTime.Today.AddDays(-generator.Next((int) (25.1*365), (int) (55.1*365)));

            var nameGenerator = NameGenerator.Default;
            var firstName = nameGenerator.GetName(gender == TestPerson.GenderType.Female ? NameGenerator.NameType.FemaleFirstName : NameGenerator.NameType.MaleFirstName);
            var middleName = nameGenerator.GetName(gender == TestPerson.GenderType.Female ? NameGenerator.NameType.FemaleFirstName : NameGenerator.NameType.MaleFirstName);
            var address = nameGenerator.GetAddress();

            var testApplicant = new TestPerson("Mr" + (gender == TestPerson.GenderType.Female ? "s" : ""), firstName, "TAL TEST", middleName, dob,
                gender, new NameGenerator.AddressInfo()
                {
                    PostCode = address.PostCode,
                    State = address.State,
                    StreetAddress = address.StreetAddress,
                    Town = address.Town
                });

            var benFirstName = nameGenerator.GetName(NameGenerator.NameType.MaleFirstName);
            var benLastName = nameGenerator.GetName(NameGenerator.NameType.LastName);
            var belAddress = nameGenerator.GetAddress();
            var testBeneficiary = new TestPerson("Mr", benFirstName, benLastName, "", DateTime.Today.AddYears(-30),
                TestPerson.GenderType.Male, new NameGenerator.AddressInfo()
                {
                    PostCode = belAddress.PostCode,
                    State = belAddress.State,
                    StreetAddress = belAddress.StreetAddress,
                    Town = belAddress.Town
                });


            var baseUrl = ConfigurationManager.AppSettings["PerfTest.SalesPortalUrl"];

            var randomName = generator.Next(1, 5).ToString();

            var salesPortalSession = new SalesPortalSession($"PerformanceTester{randomName}", new [] {Role.Agent}, $"test{randomName}@tal.com.au", $"TestFirstName{randomName}", $"TestSurname{randomName}", "TAL");

            
            var randomString = generator.Next(0, 1000000).ToString("D6");

            var stringToEncrypt = $"{randomString}|{salesPortalSession.ToJson()}";

            var cookieValue = new SecurityService().Encrypt(stringToEncrypt);

            var cookie = new Cookie("tL2Taa1aoSLAt1", cookieValue, "/", baseUrl.Replace("http://", ".").Replace("https://", "."));
            cookie.Expires = DateTime.Today.AddDays(2);

            var webServer = new ExternalWebServer(baseUrl, handler =>
            {
                handler.Credentials = new NetworkCredential("ethilapp", "1N5urance", "TOWER");
                handler.CookieContainer = new CookieContainer();

                handler.CookieContainer.Add(cookie);
            });

            var policyConfigProvider = new PolicyConfigurationProvider();
            var underwritingConfigProvider = new UnderwritingConfigurationProvider();

            var emulator = new CreatePolicyEmulator(webServer, SimplePerformanceTool.CreateConsoleTimestamped(), policyConfigProvider, underwritingConfigProvider);
            try
            {
                Task.Run(() => emulator.CreateLifePolicyWithAllCoversAsync(testApplicant, income, smoker, testBeneficiary)).Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.Flatten().InnerExceptions)
                {
                    Console.WriteLine(e.Message);
                }

                throw;
            }
        }
    }
}