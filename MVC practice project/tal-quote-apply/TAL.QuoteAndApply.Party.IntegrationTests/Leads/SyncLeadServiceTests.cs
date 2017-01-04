using System;
using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Encrpytion;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Infrastructure.Features;
using TAL.QuoteAndApply.Infrastructure.Http;
using TAL.QuoteAndApply.Infrastructure.Url;
using TAL.QuoteAndApply.Party.Configuration;
using TAL.QuoteAndApply.Party.Data;
using TAL.QuoteAndApply.Party.Leads;
using TAL.QuoteAndApply.Party.Leads.LeadsService;
using TAL.QuoteAndApply.Party.Rules;
using TAL.QuoteAndApply.Party.Services;
using TAL.QuoteAndApply.Tests.Shared.Builders;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.Party.IntegrationTests.Leads
{
    [TestFixture]
    public class SyncLeadServiceTests
    {
        [Test]
        public void SyncLeadWithParty_PartyNotAlreadyLead_PartyNotReadyToCreatedAsLead_NoActionPerformed()
        {
            var partyBuilder = new PartyBuilder()
                  .WithId(98765)
                  .WithDateOfBirth(DateTime.Today.AddYears(-30))
                  .WithGender(Gender.Male);

            var policySourceType = PolicySource.CustomerPortalBuildMyOwn;

            var svc = GetService();
            var result = svc.SyncLeadWithParty(partyBuilder.Build(), policySourceType);

            Assert.That(result.SyncLeadResult, Is.EqualTo(SyncLeadResult.NoActionPerformed));
        }

        [Test]
        public void SyncLeadWithParty_PartyNotAlreadyLead_PartyReadyToCreatedAsLead_LeadCreated()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var firstName = GenerateName();
            var lastName = GenerateName();

            var partyBuilder = new PartyBuilder()
                  .WithId(98765)
                  .WithDateOfBirth(DateTime.Today.AddYears(-30))
                  .WithGender(Gender.Male)
                  .WithEmailAddress($"{firstName}.{lastName}@test.com.au")
                  .WithFirstName(firstName)
                  .WithSurname(lastName);

            var policySourceType = PolicySource.CustomerPortalBuildMyOwn;

            var svc = GetService();
            var result = svc.SyncLeadWithParty(partyBuilder.Build(), policySourceType);

            Assert.That(result.SyncLeadResult, Is.EqualTo(SyncLeadResult.LeadCreated), "Test failed to create an adobe lead");
            Assert.That(result.LeadId, Is.Not.Null);
        }

        [Test]
        public void SyncLeadWithParty_PartyAlreadyLead_LeadUpdated()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var firstName = GenerateName();
            var lastName = GenerateName();

            var partyBuilder = new PartyBuilder()
                  .WithId(98765)
                  .WithDateOfBirth(DateTime.Today.AddYears(-30))
                  .WithGender(Gender.Male)
                  .WithEmailAddress($"{firstName}.{lastName}@test.com.au")
                  .WithFirstName(firstName)
                  .WithSurname(lastName);

            var party = partyBuilder.Build();

            var policySourceType = PolicySource.CustomerPortalBuildMyOwn;

            var converter = GetPartyToLeadsMessageConverter();
            var leadsService = GetLeadsService();

            var response = leadsService.CreateLead(converter.From(party, policySourceType));
            Assert.That(response, Is.Not.Null, "Test failed to create an adobe lead");

            party.LeadId = response.AdobeId;

            //act
            var svc = GetService();
            var result = svc.SyncLeadWithParty(partyBuilder.Build(), policySourceType);

            Assert.That(result.SyncLeadResult, Is.EqualTo(SyncLeadResult.LeadUpdated));
            Assert.That(result.LeadId.Value, Is.EqualTo(party.LeadId.Value));
        }

        private static string GenerateName()
        {
            return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 30);
        }

        private HttpLeadsService GetLeadsService()
        {
            var httpClientService = new HttpClientService(new HttpResponseMessageSerializer(), new MimeTypeProvider(),
               new HttpRequestMessageSerializer());

            var urlUtilities = new UrlUtilities();

            var leadConfigProvider = new LeadConfigurationProvider();

            var leadsService = new HttpLeadsService(leadConfigProvider, httpClientService, urlUtilities, new MockLoggingService(), new Features());

            return leadsService;
        }

        private PartyConsentService GetPartyConsentService()
        {
            return
                new PartyConsentService(
                    new PartyConsentDtoRepository(new PartyConfigurationProvider(), new MockCurrentUserProvider(),
                        new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService())),
                    new SyncCommPreferenceService(GetLeadsService(), new PartyCommunicationMessageConverter(new LeadConfigurationProvider())));

        }

        private SyncLeadService GetService()
        {
            return new SyncLeadService(new PartyRulesService(new PartyRuleFactory()), GetLeadsService(), GetPartyToLeadsMessageConverter(), GetPartyConsentService(), new SyncCommPreferenceService(GetLeadsService(), new PartyCommunicationMessageConverter(new LeadConfigurationProvider())));
        }

        private PartyToLeadsMessageConverter GetPartyToLeadsMessageConverter()
        {
            return new PartyToLeadsMessageConverter(new LeadConfigurationProvider());
        }
    }
}
