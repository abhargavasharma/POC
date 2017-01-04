using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataLayer.Encrpytion;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.Infrastructure.Time;
using TAL.QuoteAndApply.Party.Models;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Events;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Policy.Service.AccessControl;
using TAL.QuoteAndApply.Policy.Service.MarketingStatus;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.ServiceLayer.Plan.Models.Mappers;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.MarketingStatus;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.User;
using TAL.QuoteAndApply.Tests.Shared.Mocks;
using TAL.QuoteAndApply.Underwriting.Service;

namespace TAL.QuoteAndApply.Tests.Shared.Helpers
{
    public class ApplicationCurrentUser : IApplicationCurrentUser
    {
        public string CurrentUser { get; set; }
        public string CurrentUserEmailAddress { get; set; }
        public string CurrentUserGivenName { get; set; }
        public string CurrentUserSurname { get; set; }

        public IEnumerable<Role> CurrentUserRoles
        {
            get { return new List<Role>(); }
        }

        public ApplicationCurrentUser(string user = "IntegrationTest")
        {
            CurrentUser = user;
        }
    }

    public class CreatePolicyResult
    {
        public IPolicy Policy { get; set; }
        public IRisk Risk { get; set; }
        public IParty Party { get; set; }
        public List<IPlan> Plans { get; set; }
    }

    public static class PolicyHelper
    {
        private static readonly CreatePolicyService _createPolicyService;
        private static readonly CreatePlanService _CreatePlanService;
        private static readonly ProductDefinitionProvider _productDefinitionProvider;
        private static readonly CreateQuoteService _CreateQuoteService;
        private static UnderwritingTemplateService _underwritingTemplateService;
        private static CreateUnderwritingInterview _createUnderwritingInterview;
        private static PolicyOwnerDtoRepository _policyOwnerDtoRepository;
        private static DataLayerExceptionFactory _dataLayerExceptionFactory;
        private static DbItemEncryptionService _dbItemEncryptionService;
        private static CurrentUserProvider _currentUserProvider;
        private static PolicyConfigurationProvider _policyConfigurationProvider;
        private static CachingWrapper _cachingWrapper;
        private static CoverChangeSubject _coverChangeSubject;

        static PolicyHelper()
        {
            _dataLayerExceptionFactory = new DataLayerExceptionFactory();
            _dbItemEncryptionService = new DbItemEncryptionService(new SimpleEncryptionService());
            _currentUserProvider = new CurrentUserProvider(new ApplicationCurrentUser());
            _policyConfigurationProvider = new PolicyConfigurationProvider();
            _cachingWrapper = new CachingWrapper(new MockHttpProvider());
            _coverChangeSubject = new CoverChangeSubject();

             _createPolicyService = new CreatePolicyService(
                    GetPolicyRepository(),
                    new QuoteReferenceGenerationService(),
                    new MockLoggingService(),
                    GetPolicyAccessControlCreationService());

            var productDefinitionBuilder = new ProductDefinitionBuilder(new MockProductBrandSettingsProvider());
            _productDefinitionProvider = new ProductDefinitionProvider(productDefinitionBuilder,
                new ProductDtoConverter(), new PlanDefinitionProvider(productDefinitionBuilder));

            _CreatePlanService =
                new CreatePlanService(
                    new PlanService(new PlanDtoRepository(_policyConfigurationProvider,
                        _currentUserProvider, new DataLayerExceptionFactory(),
                        _dbItemEncryptionService, _cachingWrapper)),
                    new PlanDtoConverter(), new CoverDtoConverter(), _productDefinitionProvider,
                    new OptionService(new OptionDtoRepository(_policyConfigurationProvider,
                        _currentUserProvider, new DataLayerExceptionFactory(),
                        _dbItemEncryptionService, _cachingWrapper)),
                    new OptionDtoConverter(),
                    new CoverService(new CoverDtoRepository(_policyConfigurationProvider,
                        _currentUserProvider, new DataLayerExceptionFactory(),
                        _dbItemEncryptionService, _cachingWrapper, _coverChangeSubject)),
                    new PlanMarketingStatusUpdater(
                        new PlanMarketingStatusService(new PlanMarketingStatusDtoRepository(_policyConfigurationProvider,
                            _currentUserProvider,
                            _dataLayerExceptionFactory,
                            _dbItemEncryptionService, _cachingWrapper)), new PlanMarketingStatusProvider()), 
                    new CoverMarketingStatusUpdater(
                        new CoverMarketingStatusService(new CoverMarketingStatusDtoRepository(_policyConfigurationProvider,
                            _currentUserProvider,
                            _dataLayerExceptionFactory,
                            _dbItemEncryptionService, _cachingWrapper)), new CoverMarketingStatusProvider()));

            _policyOwnerDtoRepository = new PolicyOwnerDtoRepository(
                _policyConfigurationProvider,
                _currentUserProvider,
                _dataLayerExceptionFactory,
                _dbItemEncryptionService);
        }

        public static IPolicyDtoRepository GetPolicyRepository()
        {
            var policyConfigurationProvider = new PolicyConfigurationProvider();
            var mockCurrentUserProvider = new MockCurrentUserProvider();
            var dataLayerExceptionFactory = new DataLayerExceptionFactory();
            var encryptionService = new DbItemEncryptionService(new SimpleEncryptionService());
            
            return new PolicyDtoRepository(policyConfigurationProvider, mockCurrentUserProvider,
                dataLayerExceptionFactory, encryptionService, _cachingWrapper);
        }

        public static IBrandDtoRepository GetBrandRepository()
        {
            var policyConfigurationProvider = new PolicyConfigurationProvider();
            var mockCurrentUserProvider = new MockCurrentUserProvider();
            var dataLayerExceptionFactory = new DataLayerExceptionFactory();
            var encryptionService = new DbItemEncryptionService(new SimpleEncryptionService());

            return new BrandDtoRepository(policyConfigurationProvider, mockCurrentUserProvider,
                dataLayerExceptionFactory, encryptionService, new CachingWrapper(new MockHttpProvider()));
        }

        public static IPolicyAccessControlCreationService GetPolicyAccessControlCreationService()
        {
            var policyConfigurationProvider = new PolicyConfigurationProvider();
            var mockCurrentUserProvider = new MockCurrentUserProvider();
            var repo = new PolicyAccessControlDtoRepository(policyConfigurationProvider, mockCurrentUserProvider,
                new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()),
                new CachingWrapper(new MockHttpProvider()), new EndRequestOperationCollection(new MockHttpProvider()));
            return new PolicyAccessControlCreationService(new DateTimeProvider(), repo, mockCurrentUserProvider, new AccessControlTypeProvider());
        }

        public static IPolicySearchDtoRepository GetPolicySearchDtoRepository()
        {
            var policyConfigurationProvider = new PolicyConfigurationProvider();
            var mockCurrentUserProvider = new MockCurrentUserProvider();
            return new PolicySearchDtoRepository(policyConfigurationProvider, mockCurrentUserProvider,
                new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()));
        }

        public static IPolicyOwnerDtoRepository GetPolicyOwnerRepository()
        {
            return _policyOwnerDtoRepository;
        }

        public static IPolicyAnalyticsIdentifierDtoRepository GetPolicyAnalyticsIdentifierDtoRepository()
        {
            var policyConfigurationProvider = new PolicyConfigurationProvider();
            var mockCurrentUserProvider = new MockCurrentUserProvider();
            return new PolicyAnalyticsIdentifierDtoRepository(policyConfigurationProvider, mockCurrentUserProvider,
                new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()));
        }


        public static PolicyDto CreatePolicy()
        {
            return _createPolicyService.CreatePolicy(new PolicyDefaultsProvider(new ProductDefinitionBuilder(new MockProductBrandSettingsProvider())).GetPolicyDefaults(1, "tal", 1), PolicySource.CustomerPortalHelpMeChoose);
        }

        public static PolicyOwnerDto CreatePolicyOwner(int partyId, int policyId)
        {
            return _policyOwnerDtoRepository.Insert(new PolicyOwnerDto()
            {
                PartyId = partyId,
                PolicyId = policyId
            });
        }

        public static CreatePolicyResult CreatePolicy(IRisk risk, IParty party)
        {
            var policy = CreatePolicy();
            party = PartyHelper.CreateParty(party);
            var partyConsent = new PartyConsentDto()
            {
                PartyId = party.Id
            };
            PartyHelper.CreatePartyConsent(partyConsent, party);
            
            risk.PartyId = party.Id;
            risk.PolicyId = policy.Id;

            var underwritingInterview = UnderwritingHelper.CreateUnderwritingInterview();
            risk.InterviewId = underwritingInterview.InterviewIdentifier;
            risk.InterviewConcurrencyToken = underwritingInterview.ConcurrencyToken;

            RiskHelper.CreateRisk(risk);

            var productDefinitionPlans =
                _productDefinitionProvider.GetProductDefinition("tal").Plans;

            var plans =
                productDefinitionPlans.Select(
                    plan =>
                        _CreatePlanService.CreatePlan(PlanStateParam.BuildBasicPlanStateParam(plan.Code, "tal", plan.Selected, policy.Id, risk.Id,
                            plan.LinkedToCpi, plan.CoverAmount, plan.PremiumHoliday, plan.PremiumType, plan.PlanId, plan.WaitingPeriod, plan.BenefitPeriod, plan.OccupationDefinition))).ToList();

            //create riders
            foreach (var planResponse in productDefinitionPlans)
            {
                foreach (var rider in planResponse.Riders)
                {
                    _CreatePlanService.CreateRider(PlanStateParam.BuildBasicPlanStateParam(rider.Code, "tal", rider.Selected, policy.Id, risk.Id,
                            rider.LinkedToCpi, rider.CoverAmount, rider.PremiumHoliday, rider.PremiumType, rider.PlanId
                            , null, null, rider.OccupationDefinition), plans.First(x => x.Code == planResponse.Code).Id);
                }
            }

            return new CreatePolicyResult
            {
                Party = party,
                Policy = policy,
                Risk = risk,
                Plans = plans
            };
        }
    }
}
