using TAL.QuoteAndApply.DataLayer.Encrpytion;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.Infrastructure.Features;
using TAL.QuoteAndApply.Infrastructure.Http;
using TAL.QuoteAndApply.Infrastructure.Url;
using TAL.QuoteAndApply.Party.Configuration;
using TAL.QuoteAndApply.Party.Data;
using TAL.QuoteAndApply.Party.Leads;
using TAL.QuoteAndApply.Party.Leads.LeadsService;
using TAL.QuoteAndApply.Party.Models;
using TAL.QuoteAndApply.Party.Rules;
using TAL.QuoteAndApply.Party.Services;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.Tests.Shared.Helpers
{
    public static class PartyHelper
    {
        private static readonly PartyService _partyService;
        private static readonly PartyDtoRepository _partyRepo;
        private static readonly PartyConsentService _partyConsentService;
        private static readonly PartyConfigurationProvider _partyConfigurationProvider;
        private static readonly DataLayerExceptionFactory _dataLayerExceptionFactory;
        private static readonly DbItemEncryptionService _dbItemEncryptionService;
        private static readonly SimpleEncryptionService _simpleEncryptionService;
        private static readonly SyncCommPreferenceService _syncCommPreferenceService;
        private static readonly HttpLeadsService _httpLeadsService;

        static PartyHelper()
        {
            var mockCurrentUserProvider = new MockCurrentUserProvider();
            _partyConfigurationProvider = new PartyConfigurationProvider();
            _dataLayerExceptionFactory = new DataLayerExceptionFactory();
            _simpleEncryptionService = new SimpleEncryptionService();
            _dbItemEncryptionService = new DbItemEncryptionService(_simpleEncryptionService);
            _partyRepo = new PartyDtoRepository(_partyConfigurationProvider, mockCurrentUserProvider,
                _dataLayerExceptionFactory, _dbItemEncryptionService,
                new CachingWrapper(new MockHttpProvider()));

            var partyConsentRepo = new PartyConsentDtoRepository(_partyConfigurationProvider, mockCurrentUserProvider,
                _dataLayerExceptionFactory, _dbItemEncryptionService);

            _partyService = new PartyService(new PartyRulesService(new PartyRuleFactory()),
                _partyRepo, new MockSyncLeadService(), partyConsentRepo);

            _httpLeadsService = new HttpLeadsService(new LeadConfigurationProvider(), new HttpClientService(new HttpResponseMessageSerializer(), new MimeTypeProvider(), new HttpRequestMessageSerializer()), new UrlUtilities(), new MockLoggingService(), new Features());

            _syncCommPreferenceService = new SyncCommPreferenceService(_httpLeadsService, new PartyCommunicationMessageConverter(new LeadConfigurationProvider()));

            _partyConsentService = new PartyConsentService(partyConsentRepo, _syncCommPreferenceService);
        }

        public static IPartyService GetPartyService()
        {
            return _partyService;
        }

        public static IParty GetParty(int partyId, bool ignoreCache = false)
        {
            return _partyRepo.GetParty(partyId, ignoreCache);
        }

        public static PartyDto CreateParty(IParty party)
        {
            return _partyService.CreateParty(party, PolicySource.SalesPortal) as PartyDto;
        }

        public static IPartyConsent CreatePartyConsent(IPartyConsent partyConsent, IParty party)
        {
            return _partyConsentService.CreatePartyConsent(partyConsent, party) as PartyConsentDto;
        }
    }
}