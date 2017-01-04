using System;
using System.Linq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Encrpytion;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.Infrastructure.Extensions;
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
using TAL.QuoteAndApply.Tests.Shared.Builders;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.Party.IntegrationTests.Services
{
    [TestFixture]
    public class PartyConsentServiceTests
    {
        private readonly HttpLeadsService _leadsService;

        public PartyConsentServiceTests()
        {
            var httpClientService = new HttpClientService(new HttpResponseMessageSerializer(), new MimeTypeProvider(),
                new HttpRequestMessageSerializer());

            var urlUtilities = new UrlUtilities();

            var leadConfigProvider = new LeadConfigurationProvider();

            _leadsService = new HttpLeadsService(leadConfigProvider, httpClientService, urlUtilities, new MockLoggingService(), new Features());
        }

        [Test]
        public void CreatePartyConsent_PartyConsentIdReturned()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var partySvc = GetPartyService();
            var partyConsentSvc = GetService();

            var partyBuilder = new PartyBuilder()
                .WithDateOfBirth(DateTime.Today.AddYears(-30))
                .WithGender(Gender.Male)
                .WithFirstName("Jim")
                .WithSurname("Test");

            IParty party = partyBuilder.Build();
            party = partySvc.CreateParty(party, PolicySource.SalesPortal);

            IPartyConsent partyConsent = partyBuilder.BuildPartyConsent(party.Id);
            partyConsent.PartyId = party.Id;

            partyConsent = partyConsentSvc.CreatePartyConsent(partyConsent, party);
            Assert.That(partyConsent.Id, Is.Not.Null);
        }

        [Test]
        public void UpdatePartyConsent_GetPartyConsentExpressConsentReturned()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var partySvc = GetPartyService();
            var partyConsentSvc = GetService();

            var partyBuilder = new PartyBuilder()
                .WithDateOfBirth(DateTime.Today.AddYears(-30))
                .WithGender(Gender.Male)
                .WithFirstName("Jim")
                .WithSurname("Test").WithEmailAddress("jim.barnes@test.com");

            IParty party = partyBuilder.Build();
            party = partySvc.CreateParty(party, PolicySource.SalesPortal);

            IPartyConsent partyConsent = partyBuilder.BuildPartyConsent(party.Id);
            partyConsent.PartyId = party.Id;

            partyConsent = partyConsentSvc.CreatePartyConsent(partyConsent, party);
            Assert.That(partyConsent.Id, Is.Not.Null);

            partyConsent.ExpressConsent = true;
            partyConsent.ExpressConsentUpdatedTs = DateTime.Now;
            partyConsentSvc.UpdatePartyConsent(partyConsent, party);

            partyConsent = partyConsentSvc.GetPartyConsent(partyConsent.Id);
            Assert.That(partyConsent.ExpressConsent, Is.True);
            Assert.That(party.LeadId.HasValue, Is.True, "Test failed to create an adobe lead");
        }

        [Test]
        public void GetPartyConsentByPartyId_GetPartyConsentExpressConsentReturned()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var partySvc = GetPartyService();
            var partyConsentSvc = GetService();

            var partyBuilder = new PartyBuilder()
                .WithDateOfBirth(DateTime.Today.AddYears(-30))
                .WithGender(Gender.Male)
                .WithFirstName("Jim")
                .WithSurname("Test");

            IParty party = partyBuilder.Build();
            party = partySvc.CreateParty(party, PolicySource.SalesPortal);

            IPartyConsent partyConsent = partyBuilder.BuildPartyConsent(party.Id);
            partyConsent.PartyId = party.Id;

            partyConsent = partyConsentSvc.CreatePartyConsent(partyConsent, party);
            Assert.That(partyConsent.Id, Is.Not.Null);

            partyConsent.ExpressConsent = true;
            partyConsent.ExpressConsentUpdatedTs = DateTime.Now;
            partyConsentSvc.UpdatePartyConsent(partyConsent, party);

            partyConsent = partyConsentSvc.GetPartyConsentByPartyId(party.Id);
            Assert.That(partyConsent.ExpressConsent, Is.True);
        }

        [Test]
        public void CreateFullParty_GetCommunicationPreferences_UpdateCommunicationPreferences_ExpressConsentAndTimestampSet()
        {
            var party = GenerateParty();
            var policySourceType = PolicySource.CustomerPortalBuildMyOwn;

            var converter = new PartyToLeadsMessageConverter(new LeadConfigurationProvider());

            var createLeadMsg = converter.From(party, policySourceType);

            var response = _leadsService.CreateLead(createLeadMsg);
            Assert.That(response, Is.Not.Null, "Test failed to create an adobe lead");

            party.LeadId = response.AdobeId;

            var commPreferencesResponse = _leadsService.GetCommunicationPreferences(response.AdobeId);

            var partyConsent = GeneratePartyConsent(party.Id);
            partyConsent.ExpressConsent = false;

            AssertDncOnCommunicationPreferences(commPreferencesResponse, (PartyConsentDto)partyConsent);
            
            partyConsent.ExpressConsent = true;
            partyConsent.ExpressConsentUpdatedTs = DateTime.Now;

            var commUpdateMessage = new PartyCommunicationMessageConverter(new LeadConfigurationProvider()).From(party, partyConsent);

            Console.WriteLine("UPDATE MESSAGE:");
            Console.WriteLine(commUpdateMessage.ToXml());
            _leadsService.UpdateCommunicationPreferences(commUpdateMessage);

            commPreferencesResponse = _leadsService.GetCommunicationPreferences(response.AdobeId);

            Console.WriteLine("GET MESSAGE:");
            Console.WriteLine(commPreferencesResponse.ToXml());

            AssertDncOnCommunicationPreferences(commPreferencesResponse, (PartyConsentDto)partyConsent);
        }

        [Test]
        public void CreateFullParty_GetCommunicationPreferences_UpdateCommunicationPreferences_AllDncsSetCorrectly()
        {
            var party = GenerateParty();
            var policySourceType = PolicySource.CustomerPortalBuildMyOwn;

            var converter = new PartyToLeadsMessageConverter(new LeadConfigurationProvider());

            var createLeadMsg = converter.From(party, policySourceType);

            var response = _leadsService.CreateLead(createLeadMsg);
            Assert.That(response, Is.Not.Null, "Test failed to create an adobe lead");

            party.LeadId = response.AdobeId;

            var commPreferencesResponse = _leadsService.GetCommunicationPreferences(response.AdobeId);

            var partyConsent = GeneratePartyConsent(party.Id);
            partyConsent.ExpressConsent = false;

            AssertDncOnCommunicationPreferences(commPreferencesResponse, (PartyConsentDto)partyConsent);

            partyConsent.ExpressConsent = true;
            partyConsent.ExpressConsentUpdatedTs = DateTime.Now;
            partyConsent.DncEmail = true;
            partyConsent.DncHomeNumber = true;
            partyConsent.DncMobile = true;
            partyConsent.DncPostalMail = true;

            var commUpdateMessage = new PartyCommunicationMessageConverter(new LeadConfigurationProvider()).From(party, partyConsent);

            Console.WriteLine("UPDATE MESSAGE:");
            Console.WriteLine(commUpdateMessage.ToXml());
            _leadsService.UpdateCommunicationPreferences(commUpdateMessage);

            commPreferencesResponse = _leadsService.GetCommunicationPreferences(response.AdobeId);

            Console.WriteLine("GET MESSAGE:");
            Console.WriteLine(commPreferencesResponse.ToXml());

            AssertDncOnCommunicationPreferences(commPreferencesResponse, (PartyConsentDto)partyConsent);
        }

        [Test]
        public void CreateParty_EnsureCommunicationNotes_UpdateCommunicationPreferences_EnsureCommunicationNotes()
        {
            var party = GenerateParty();
            var policySourceType = PolicySource.CustomerPortalBuildMyOwn;

            var converter = new PartyToLeadsMessageConverter(new LeadConfigurationProvider());

            var createLeadMsg = converter.From(party, policySourceType);

            Console.WriteLine(createLeadMsg.ToXml());

            var response = _leadsService.CreateLead(createLeadMsg);
            Assert.That(response, Is.Not.Null, "Test failed to create an adobe lead");

            AssertProspectNotesOnLead(response.AdobeId, policySourceType);
            
            party.LeadId = response.AdobeId;

            var commPreferencesResponse = _leadsService.GetCommunicationPreferences(response.AdobeId);

            var partyConsent = GeneratePartyConsent(party.Id);
            partyConsent.ExpressConsent = false;

            AssertDncOnCommunicationPreferences(commPreferencesResponse, (PartyConsentDto)partyConsent);

            partyConsent.ExpressConsent = true;
            partyConsent.ExpressConsentUpdatedTs = DateTime.Now;
            partyConsent.DncEmail = true;
            partyConsent.DncHomeNumber = true;
            partyConsent.DncMobile = true;
            partyConsent.DncPostalMail = true;

            var commUpdateMessage = new PartyCommunicationMessageConverter(new LeadConfigurationProvider()).From(party, partyConsent);

            _leadsService.UpdateCommunicationPreferences(commUpdateMessage);

            commPreferencesResponse = _leadsService.GetCommunicationPreferences(response.AdobeId);
            AssertDncOnCommunicationPreferences(commPreferencesResponse, (PartyConsentDto)partyConsent);

            AssertProspectNotesOnLead(response.AdobeId, policySourceType);
        }

        private static void AssertProspectNotesOnLead(int leadId, PolicySource policySource)
        {
            var leadResponse = GetHttpLeadsService().RetrieveLead(leadId);

            Assert.That(leadResponse.MarketingInquiry.Lead, Is.Not.Null);
            Assert.That(leadResponse.MarketingInquiry.Lead.Length, Is.GreaterThan(0));
            var lead = leadResponse.MarketingInquiry.Lead[0];

            //policy source in remarks
            var commnotes =
                lead.RemarkInformation.FirstOrDefault(x => x.RemarkTypeCode.Value.Name == "ProspectNotes");

            Assert.That(commnotes, Is.Not.Null);
            Assert.That(commnotes.Remark, Is.StringContaining(policySource.ToString()));

        }

        private static IParty GenerateParty()
        {
            var firstName = "Jimmy";
            var surname = "Test";

            var partyBuilder = new PartyBuilder()
                .WithId(98765)
                .WithState(State.VIC)
                .WithAddress("123 Abc St")
                .WithPostcode("3000")
                .WithSuburb("Melbourne")
                .WithDateOfBirth(DateTime.Today.AddYears(-30))
                .WithEmailAddress($"{firstName}.{surname}@tal.com.au")
                .WithFirstName(firstName)
                .WithSurname(surname)
                .WithTitle(Title.Mr)
                .WithPhoneNumber("0400000000")
                .WithGender(Gender.Male);

            var party = partyBuilder.Build();

            return party;
        }

        private static IPartyConsent GeneratePartyConsent(int partyId)
        {
            var partyBuilder = new PartyBuilder();
            var partyConsent = partyBuilder.BuildPartyConsent(partyId);

            return partyConsent;
        }

        private void AssertDncOnCommunicationPreferences(PartyCommunicationInquiryNotify commPreferencesResponse, PartyConsentDto dncs)
        {
            var personPartyConfig = commPreferencesResponse.PartyCommunication[0].Person[0];
            Assert.That(commPreferencesResponse.PartyCommunication, Is.Not.Null);
            Assert.That(commPreferencesResponse.PartyCommunication.Length, Is.GreaterThan(0));
            Assert.That(personPartyConfig, Is.Not.Null);
            Assert.That(commPreferencesResponse.PartyCommunication[0].Person.Length, Is.GreaterThan(0));
            Assert.That(commPreferencesResponse.PartyCommunication[0].RemarkInformation.Length, Is.GreaterThan(0));

            var personCommunication = personPartyConfig.PersonCommunication;
            var expressConsentType =
                commPreferencesResponse.PartyCommunication[0].CommunicationPreferences[0].ExpressConsent;

            //express consent
            Assert.That(expressConsentType.ExpressConsentIndicator, Is.EqualTo(dncs.ExpressConsent));

            if (dncs.ExpressConsent)
            {
                Assert.That(expressConsentType.ExpressConsentEndDate, Is.GreaterThan(DateTime.Now.AddYears(49)));
                Assert.That(expressConsentType.ExpressConsentStartDate, Is.LessThan(DateTime.Now));
            }
            //address
            Assert.That(personCommunication.MailingAddress[0].CommunicationPreferences[0].DoNotContactIndicator,
                Is.EqualTo(dncs.DncPostalMail));

            //email
            Assert.That(personCommunication.Email[0].CommunicationPreferences[0].DoNotContactIndicator,
                Is.EqualTo(dncs.DncEmail));

            //home phone
            Assert.That(personCommunication.Telephone.First(x => x.TypeCode.Value.Name == "Wired").CommunicationPreferences[0].DoNotContactIndicator,
                Is.EqualTo(dncs.DncHomeNumber));

            //mobile phone
            Assert.That(personCommunication.Telephone.First(x => x.TypeCode.Value.Name == "Wireless").CommunicationPreferences[0].DoNotContactIndicator,
                Is.EqualTo(dncs.DncMobile));
        }

        private PartyService GetPartyService()
        {
            var syncService = new SyncLeadService(new PartyRulesService(new PartyRuleFactory()), GetHttpLeadsService(), new PartyToLeadsMessageConverter(new LeadConfigurationProvider()), GetService(), new SyncCommPreferenceService(GetHttpLeadsService(), new PartyCommunicationMessageConverter(new LeadConfigurationProvider())));

            var mockCurrentUserProvider = new MockCurrentUserProvider();

            var partyRepo = new PartyDtoRepository(new PartyConfigurationProvider(), mockCurrentUserProvider,
                new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var partyConsentRepo = new PartyConsentDtoRepository(new PartyConfigurationProvider(),
                mockCurrentUserProvider, new DataLayerExceptionFactory(),
                new DbItemEncryptionService(new SimpleEncryptionService()));

            return new PartyService(new PartyRulesService(new PartyRuleFactory()), partyRepo, syncService, partyConsentRepo);
        }

        private PartyConsentService GetService()
        {
            var mockCurrentUserProvider = new MockCurrentUserProvider();

            var repo = new
            PartyConsentDtoRepository(new PartyConfigurationProvider(), mockCurrentUserProvider,
                new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()));
            

            return new PartyConsentService(repo, new SyncCommPreferenceService(GetHttpLeadsService(), new PartyCommunicationMessageConverter(new LeadConfigurationProvider())));
        }

        private static HttpLeadsService GetHttpLeadsService()
        {
            var httpClientService = new HttpClientService(new HttpResponseMessageSerializer(), new MimeTypeProvider(),
                new HttpRequestMessageSerializer());

            var urlUtilities = new UrlUtilities();

            var leadConfigProvider = new LeadConfigurationProvider();

            var leadsService = new HttpLeadsService(leadConfigProvider, httpClientService, urlUtilities, new MockLoggingService(), new Features());
            return leadsService;
        }
    }
}
