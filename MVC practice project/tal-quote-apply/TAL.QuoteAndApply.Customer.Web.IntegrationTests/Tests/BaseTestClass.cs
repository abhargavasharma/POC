using System;
using System.Collections;
using System.Collections.Generic;
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
using TAL.QuoteAndApply.Customer.Web.Extensions;
using TAL.QuoteAndApply.Customer.Web.IntegrationTests.Clients;
using TAL.QuoteAndApply.Customer.Web.Models.Api;
using TAL.QuoteAndApply.Customer.Web.Models.View;
using TAL.QuoteAndApply.Customer.Web.Services;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Interactions;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Infrastructure.Http;
using TAL.QuoteAndApply.Party.Models;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.Tests.Shared.Builders;
using TAL.QuoteAndApply.Tests.Shared.Helpers;
using TAL.QuoteAndApply.Tests.Shared.Mocks;
using TAL.QuoteAndApply.Tests.Shared.WebIntegration;
using TAL.QuoteAndApply.Web.Shared.Cookie;
using TAL.QuoteAndApply.Web.Shared.HttpResults;
using TAL.QuoteAndApply.Web.Shared.Ioc;
using TAL.QuoteAndApply.Web.Shared.Session;
using PolicySource = TAL.QuoteAndApply.ServiceLayer.Models.PolicySource;
using Task = System.Threading.Tasks.Task;

namespace TAL.QuoteAndApply.Customer.Web.IntegrationTests.Tests
{

    public abstract class BaseTestClass<T> where T : CustomerPortalApiClient, new()
    {
        protected IWebServer WebServer;
        protected T Client;
        private readonly IPerformanceTestTool _performanceTestTool;
        protected IQuoteSessionContext QuoteSessionContext;
        protected IRiskMarketingStatusService RiskMarketingStatusService;
        protected IContainer Container;

        private PolicyClient _policyClient; //For helping with login
        protected QualificationApiClient _qualificationClient; //For helping with risks/underwriting
        private SaveClient _saveApiClient; //For helping with save
        private CoverSelectionApiClient _coverSelectionClient; //For helping with updating plans
        private ReviewClient _reviewClient; //For helping with updating plans

        protected BaseTestClass(IPerformanceTestTool performanceTestTool)
        {
            _performanceTestTool = performanceTestTool;
            QuoteSessionContext = new CustomerIntegrationTestQuoteSessionContext();
        }

        protected void SetQuoteReference(string quoteReference)
        {
            QuoteSessionContext.Set(quoteReference);
        }

        protected async Task<IList<RiskResponse>> BasicInfoDefaultLoginAsync(string occupationCode = UnderwritingHelper.OccupationCode_AccountExecutive, string industryCode = UnderwritingHelper.IndustryCode_AdvertisingAndMarketing)
        {
            return await BasicInfoLoginAsAsync(new BasicInfoViewModel
            {
                DateOfBirth = DateTime.Now.AddYears(-32).ToFriendlyString(),
                Gender = 'M',
                IsSmoker = false,
                AnnualIncome = 25000,
                Source = PolicySource.CustomerPortalBuildMyOwn,
                IndustryCode = industryCode,
                OccupationCode = occupationCode,
                RecaptchaResponse = "",
                Postcode = "1000"
            });            
        }

        protected async Task<IList<RiskResponse>> BasicInfoLoginAsAsync(BasicInfoViewModel basicInfo)
        {
            await _policyClient.CreateQuoteAsync<RedirectToResponse>(basicInfo);
            return await _qualificationClient.GetUnderwritingRisksAsync<IList<RiskResponse>>();
        }

        protected CreatePolicyResult CreateAndLoginAsDefaultPolicy()
        {
            var party = new PartyBuilder()
                .Default()
                .Build();

            var risk = new RiskBuilder()
                .Default()
                .WithDateOfBirth(party.DateOfBirth)
                .WithGender(party.Gender)
                .WithPartyId(party.Id)
                .WithOccupationCode(UnderwritingHelper.OccupationCode_AccountExecutive)
                .WithIndustryCode(UnderwritingHelper.IndustryCode_AdvertisingAndMarketing)
                .Build();

            return CreateAndLoginAs(party, risk);
        }

        protected CreatePolicyResult CreateAndLoginAs(PartyDto party, IRisk risk)
        {

            var createPolicyResult = PolicyHelper.CreatePolicy(risk, party);

            PolicyHelper.CreatePolicyOwner(party.Id, createPolicyResult.Policy.Id);

            SetQuoteReference(createPolicyResult.Policy.QuoteReference);

            return createPolicyResult;
        }

        protected async Task SavePolicyWithDefaultsAsync(int riskId, string password)
        {
            await _saveApiClient.SaveDetailsAsync(riskId,
                new SaveCustomerRequest
                {
                    EmailAddress = "test@test.com",
                    ExpressConsent = true,
                    FirstName = "Testy",
                    LastName = "Test",
                    PhoneNumber = "0400000000"
                });
            await _saveApiClient.SavePasswordAsync(riskId, new CreateLoginRequest { Password = password });
        }

        protected async Task<PolicyReviewResponse> GetReviewForRisk(int riskId)
        {
            return await _reviewClient.GetReviewForRiskAsync(riskId);
        }
        

        protected bool QuoteHasInteraction(string quoteReference, InteractionType interactionType)
        {
            var interactionService = Container.Resolve<IPolicyInteractionService>();
            var interactions =
                interactionService.GetInteractions(
                    PolicyInteractionsRequest.PolicyInteractionsByQuoteReference(quoteReference));

            return interactions.Interactions.Any(i => i.InteractionType == interactionType);
        }

        protected bool RiskMarketingStatusIsEqualTo(MarketingStatus riskMarketingStatus, int riskId)
        {
            var riskMarketingStatusService = Container.Resolve<IRiskMarketingStatusService>();
            var status = riskMarketingStatusService.GetRiskMarketingStatusByRiskId(riskId).MarketingStatusId;
            return riskMarketingStatus == status;
        }

        protected bool PlanMarketingStatusIsEqualTo(MarketingStatus planMarketingStatus, int planId)
        {
            var planMarketingStatusService = Container.Resolve<IPlanMarketingStatusService>();
            var status = planMarketingStatusService.GetPlanMarketingStatusByPlanId(planId).MarketingStatusId;
            return planMarketingStatus == status;
        }

        protected async Task<IEnumerable<PlanSelectionResponse>> GetPlansForRisk(int riskId)
        {
            var response = await _coverSelectionClient.GetPlanForRiskAsync<GetPlanResponse>(riskId);
            return response.Plans;
        }

        protected async Task<T> UpdatePlans<T>(int riskId, PlanSelectionResponse selectedPlan,
            string [] selectedPlanSelectedCoverCodes, string [] selectedPlanCodes, bool includeReviewInfoInReponse = false)
        {
            var request = new UpdatePlanRequest()
            {
                PlanCode = selectedPlan.PlanCode,
                PlanId = selectedPlan.PlanId,
                SelectedCoverAmount = (int) selectedPlan.SelectedCoverAmount,
                IsSelected = selectedPlanCodes.Contains(selectedPlan.PlanCode),
                PremiumType = selectedPlan.PremiumType,
                SelectedCovers = selectedPlanSelectedCoverCodes.ToList(),
                Riders = new List<PlanRiderRequest>(),
                Options =
                    selectedPlan.Options.Select(
                        o => new OptionConfigurationRequest {Code = o.Code, IsSelected = o.IsSelected}).ToList(),
                Variables =
                    selectedPlan.Variables.Select(
                        v => new UpdatePlanVariableRequest {Code = v.Code, SelectedValue = v.SelectedValue}).ToList(),
                SelectedPlans = selectedPlanCodes.ToList(),
                IncludeReviewInfoInResponse = includeReviewInfoInReponse
            };

            var response = await _coverSelectionClient.UpdatePlanAsync<T>(riskId, request);
            return response;
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
                    var containerBuilder = baseIocConfig.SetupContainerBuilder(IocConfig.RegisterInternalModules);

                    //Override some dependencies already in container with Mocks
                    //Mocking out HttpContextBase
                    RegisterMocks(containerBuilder);

                    //IQuoteSessionContext already registered, but this will overwrite it
                    containerBuilder.RegisterInstance(QuoteSessionContext).AsImplementedInterfaces().SingleInstance();

                    //Create the container
                    Container = baseIocConfig.CreateContainer(containerBuilder);

                    //Register the container with WebApi
                    baseIocConfig.RegisterContainerWithWebApi(config, Container, containerBuilder);

                    //Register the observers
                    baseIocConfig.RegisterObservers(config);

                    //We cannot set the DependencyResolver with the AutoFac MVC resolver as InMemoryWebServer does not support MVC funtcions
                    //This needs to be set as the ValidationAttributes in the customer portal use DependencyResolver
                    AddMockMvcDependencyResolver(Container);

                    //configurare log4Net
                    log4net.Config.XmlConfigurator.Configure();

                };

                WebServer = new InMemoryWebServer(action);

                Client = new T();
                _policyClient = new PolicyClient();
                _qualificationClient = new QualificationApiClient();
                _saveApiClient = new SaveClient();
                _coverSelectionClient = new CoverSelectionApiClient();
                _reviewClient = new ReviewClient();

                var webApiClient = new WebApiClient(WebServer, new Json());
                Client.AssignDependencies(webApiClient, _performanceTestTool);
                _policyClient.AssignDependencies(webApiClient, _performanceTestTool);
                _qualificationClient.AssignDependencies(webApiClient, _performanceTestTool);
                _saveApiClient.AssignDependencies(webApiClient, _performanceTestTool);
                _coverSelectionClient.AssignDependencies(webApiClient, _performanceTestTool);
            }
            catch (Exception err)
            {
                Console.WriteLine("ERROR IN TEST FIXTURE SETUP:" + err);
            }
            
        }
		
        protected void FinaliseCustomerUnderwriting(int riskId)
        {
            Container.Resolve<ICustomerPolicyStateService>()
                .FinaliseCustomerUnderwriting(QuoteSessionContext.QuoteSession.QuoteReference, riskId);
        }

        private void AddMockMvcDependencyResolver(IContainer container)
        {
            DependencyResolver.SetResolver(new SimpleResolver(container));
        }

        private void AddMockMvcRoute(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                   name: "Default",
                   routeTemplate: "{controller}/{action}/{id}",
                   defaults: new { controller = "BasicInfo", action = "Index", id = UrlParameter.Optional }
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

            request.Setup(call => call.Url)
                .Returns(new Uri("http://test-talcustomer.delivery.lan/abc/def?abc=123"));

            context.Setup(h => h.Request).Returns(request.Object);
            context.Setup(h => h.Response).Returns(response.Object);

            context.Setup(ctx => ctx.Request).Returns(request.Object);
            context.Setup(ctx => ctx.Response).Returns(response.Object);
            context.Setup(ctx => ctx.Session).Returns(session.Object);
            context.Setup(ctx => ctx.Server).Returns(server.Object);

            context.Setup(ctx => ctx.Items).Returns(items);

            identity.Setup(x => x.Name).Returns("TAL.QuoteAndApply.Customer.Web.IntegrationTests");
            user.Setup(h => h.Identity).Returns(identity.Object);
            context.Setup(h => h.User).Returns(user.Object);

            var contextProvider = new MockHttpProvider(context.Object);

            containerBuilder.RegisterInstance(contextProvider).As<IHttpContextProvider>().SingleInstance();

            containerBuilder.RegisterInstance(context.Object).As<HttpContextBase>().SingleInstance();
            containerBuilder.RegisterType<MockCookieService>().As<ICookieService>().SingleInstance();
        }
    }
}
