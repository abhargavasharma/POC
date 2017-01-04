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
    public class PartyServiceTests
    {
        [Test]
        public void IsPartyValidForInforce_ValidParty_ReturnsTrue()
        {
            var party = new PartyDto
            {
                Title = Title.Mr,
                FirstName = "Test",
                Surname = "Test",
                Address = "123 Abc St",
                Suburb = "Melbourne",
                State = State.VIC,
                Postcode = "1234",
                Country = Country.Australia,
                MobileNumber = "0400000000",
                HomeNumber = "0200000000",
                EmailAddress = "test@test.com"
            };

            var svc = GetService();
            var result = svc.IsPartyValidForInforce(party);
            
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsPartyValidForInforce_UnkownTitle_ReturnsFalse()
        {
            var party = new PartyDto
            {
                Title = Title.Unknown,
                FirstName = "Test",
                Surname = "Test",
                Address = "123 Abc St",
                Suburb = "Melbourne",
                State = State.VIC,
                Postcode = "1234",
                Country = Country.Australia,
                MobileNumber = "0400000000",
                HomeNumber = "0200000000",
                EmailAddress = "test@test.com"
            };

            var svc = GetService();
            var result = svc.IsPartyValidForInforce(party);

            Assert.That(result, Is.False);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("11111")]
        [TestCase("aaa111")]
        [TestCase("!!!!")]
        public void IsPartyValidForInforce_InvalidFirstName_ReturnsFalse(string firstName)
        {
            var party = new PartyDto
            {
                Title = Title.Mr,
                FirstName = firstName,
                Surname = "Test",
                Address = "123 Abc St",
                Suburb = "Melbourne",
                State = State.VIC,
                Postcode = "1234",
                Country = Country.Australia,
                MobileNumber = "0400000000",
                HomeNumber = "0200000000",
                EmailAddress = "test@test.com"
            };

            var svc = GetService();
            var result = svc.IsPartyValidForInforce(party);

            Assert.That(result, Is.False);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("11111")]
        [TestCase("aaa111")]
        [TestCase("!!!!")]
        public void IsPartyValidForInforce_InvalidSurname_ReturnsFalse(string surname)
        {
            var party = new PartyDto
            {
                Title = Title.Mr,
                FirstName = "Test",
                Surname = surname,
                Address = "123 Abc St",
                Suburb = "Melbourne",
                State = State.VIC,
                Postcode = "1234",
                Country = Country.Australia,
                MobileNumber = "0400000000",
                HomeNumber = "0200000000",
                EmailAddress = "test@test.com"
            };

            var svc = GetService();
            var result = svc.IsPartyValidForInforce(party);

            Assert.That(result, Is.False);
        }

        [TestCase(null)]
        [TestCase("")]
        public void IsPartyValidForInforce_InvalidAddress_ReturnsFalse(string address)
        {
            var party = new PartyDto
            {
                Title = Title.Mr,
                FirstName = "Test",
                Surname = "Test",
                Address = address,
                Suburb = "Melbourne",
                State = State.VIC,
                Postcode = "1234",
                Country = Country.Australia,
                MobileNumber = "0400000000",
                HomeNumber = "0200000000",
                EmailAddress = "test@test.com"
            };

            var svc = GetService();
            var result = svc.IsPartyValidForInforce(party);

            Assert.That(result, Is.False);
        }

        [TestCase(null)]
        [TestCase("")]
        public void IsPartyValidForInforce_InvalidSuburb_ReturnsFalse(string suburb)
        {
            var party = new PartyDto
            {
                Title = Title.Mr,
                FirstName = "Test",
                Surname = "Test",
                Address = "123 abc st",
                Suburb = suburb,
                State = State.VIC,
                Postcode = "1234",
                Country = Country.Australia,
                MobileNumber = "0400000000",
                HomeNumber = "0200000000",
                EmailAddress = "test@test.com"
            };

            var svc = GetService();
            var result = svc.IsPartyValidForInforce(party);

            Assert.That(result, Is.False);
        }

        [Test]
        public void IsPartyValidForInforce_UnkownState_ReturnsFalse()
        {
            var party = new PartyDto
            {
                Title = Title.Mr,
                FirstName = "Test",
                Surname = "Test",
                Address = "123 abc st",
                Suburb = "Melbourne",
                State = State.Unknown,
                Postcode = "1234",
                Country = Country.Australia,
                MobileNumber = "0400000000",
                HomeNumber = "0200000000",
                EmailAddress = "test@test.com"
            };

            var svc = GetService();
            var result = svc.IsPartyValidForInforce(party);

            Assert.That(result, Is.False);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("asda")]
        [TestCase("111")]
        [TestCase("11111")]
        [TestCase("11aa")]
        public void IsPartyValidForInforce_InvalidPostcode_ReturnsFalse(string postcode)
        {
            var party = new PartyDto
            {
                Title = Title.Mr,
                FirstName = "Test",
                Surname = "Test",
                Address = "123 abc st",
                Suburb = "Melbourne",
                State = State.VIC,
                Postcode = postcode,
                Country = Country.Australia,
                MobileNumber = "0400000000",
                HomeNumber = "0200000000",
                EmailAddress = "test@test.com"
            };

            var svc = GetService();
            var result = svc.IsPartyValidForInforce(party);

            Assert.That(result, Is.False);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("aaaaaaaaaa")]
        [TestCase("123456789")]
        [TestCase("01234567890")]
        [TestCase("0123456abc")]
        public void IsPartyValidForInforce_InvalidPhoneNumber_ReturnsFalse(string phoneNumber)
        {
            var party = new PartyDto
            {
                Title = Title.Mr,
                FirstName = "Test",
                Surname = "Test",
                Address = "123 abc st",
                Suburb = "Melbourne",
                State = State.VIC,
                Postcode = "1234",
                Country = Country.Australia,
                MobileNumber = phoneNumber,
                HomeNumber = phoneNumber,
                EmailAddress = "test@test.com"
            };

            var svc = GetService();
            var result = svc.IsPartyValidForInforce(party);

            Assert.That(result, Is.False);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("aaaa")]
        [TestCase("aaa@aaa")]
        [TestCase("aaa.com")]
        public void IsPartyValidForInforce_InvalidEmailAddress_ReturnsFalse(string emailAddress)
        {
            var party = new PartyDto
            {
                Title = Title.Mr,
                FirstName = "Test",
                Surname = "Test",
                Address = "123 abc st",
                Suburb = "Melbourne",
                State = State.VIC,
                Postcode = "1234",
                Country = Country.Australia,
                MobileNumber = "0400000000",
                HomeNumber = "0200000000",
                EmailAddress = emailAddress
            };

            var svc = GetService();
            var result = svc.IsPartyValidForInforce(party);

            Assert.That(result, Is.False);
        }

        [Test]
        public void CreatePartyWithEnoughToCreate_ContinueToUpdateParty_LeadInSync()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var svc = GetService();

            var partyBuilder = new PartyBuilder()
                  .WithDateOfBirth(DateTime.Today.AddYears(-30))
                  .WithGender(Gender.Male)
                  .WithFirstName(GenerateName())
                  .WithSurname(GenerateName());

            IParty party = partyBuilder.Build();
            party.EmailAddress = $"{party.FirstName}.{party.Surname}@tal.com.au";

            var policySource = PolicySource.CustomerPortalBuildMyOwn;

            party = svc.CreateParty(party, policySource);
            Assert.That(party.LeadId.HasValue, Is.True, "Test failed to create an adobe lead");
            AssertLeadIsEqualToParty(party, policySource);

            //state
            party.State = State.VIC;
            svc.UpdateParty(party, policySource);

            Assert.That(party.LeadId.HasValue, Is.True);
            AssertLeadIsEqualToParty(party, policySource);

            //mobile
            party.MobileNumber = "0400000000";
            svc.UpdateParty(party, policySource);

            Assert.That(party.LeadId.HasValue, Is.True);
            AssertLeadIsEqualToParty(party, policySource);

            //address
            party.Address = "123 Seasme St";
            svc.UpdateParty(party, policySource);

            Assert.That(party.LeadId.HasValue, Is.True);
            AssertLeadIsEqualToParty(party, policySource);

            //suburb
            party.Suburb = "Melbs";
            svc.UpdateParty(party, policySource);

            Assert.That(party.LeadId.HasValue, Is.True);
            AssertLeadIsEqualToParty(party, policySource);

            //postcode
            party.Postcode = "3124";
            svc.UpdateParty(party, policySource);

            Assert.That(party.LeadId.HasValue, Is.True);
            AssertLeadIsEqualToParty(party, policySource);
        }

        private PartyService GetService()
        {
            var leadConfigurationProvider = new LeadConfigurationProvider();
            var syncService = new SyncLeadService(new PartyRulesService(new PartyRuleFactory()), GetHttpLeadsService(), new PartyToLeadsMessageConverter(leadConfigurationProvider), GetConsentService(), new SyncCommPreferenceService(GetHttpLeadsService(), new PartyCommunicationMessageConverter(leadConfigurationProvider)));

            var mockCurrentUserProvider = new MockCurrentUserProvider();

            var partyRepo = new PartyDtoRepository(new PartyConfigurationProvider(), mockCurrentUserProvider,
                new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var partyConsentRepo = new PartyConsentDtoRepository(new PartyConfigurationProvider(),
                mockCurrentUserProvider, new DataLayerExceptionFactory(),
                new DbItemEncryptionService(new SimpleEncryptionService()));

            return new PartyService(new PartyRulesService(new PartyRuleFactory()), partyRepo, syncService, partyConsentRepo);
        }

        private PartyConsentService GetConsentService()
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

        private static string GenerateName()
        {
            return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 30);
        }

        private static void AssertLeadIsEqualToParty(IParty party, PolicySource policySource)
        {
            var leadResponse = GetHttpLeadsService().RetrieveLead(party.LeadId.Value);

            Assert.That(leadResponse.MarketingInquiry.Lead, Is.Not.Null);
            Assert.That(leadResponse.MarketingInquiry.Lead.Length, Is.GreaterThan(0));
            var lead = leadResponse.MarketingInquiry.Lead[0];

            //policy source in remarks
            Assert.That(lead.RemarkInformation[0].RemarkTypeCode.Value.Name, Is.EqualTo("ProspectNotes"));
            Assert.That(lead.RemarkInformation[0].Remark, Is.StringContaining(policySource.ToString()));

            //rating factors
            Assert.That(lead.Person.BirthDate, Is.EqualTo(party.DateOfBirth));
            Assert.That(lead.Person.GenderCode.name, Is.EqualTo(party.Gender.ToString()));

            //name
            Assert.That(lead.Person.PersonName[0].GivenName.ToUpper(), Is.EqualTo(party.FirstName.ToUpper()));
            Assert.That(lead.Person.PersonName[0].Surname.ToUpper(), Is.EqualTo(party.Surname.ToUpper()));
            Assert.That(lead.Person.PersonName[0].PersonTitlePrefix, Is.EqualTo(GetTitleString(party)));

            //comms
            Assert.That(lead.Person.PersonCommunication.Email[0].EmailAddress[0], Is.EqualTo(party.EmailAddress));
            Assert.That(lead.Person.PersonCommunication.Telephone.First(x => x.TypeCode.name == "Wired").PhoneNumberUnformatted.Replace(" ", ""), Is.EqualTo(NullAsEmpty(party.HomeNumber)));
            Assert.That(lead.Person.PersonCommunication.Telephone.First(x => x.TypeCode.name == "Wireless").PhoneNumberUnformatted.Replace(" ", ""), Is.EqualTo(NullAsEmpty(party.MobileNumber)));

            //address
            Assert.That(lead.Address.StateOrProvinceCode.Value, Is.EqualTo(GetStateString(party)));
            Assert.That(lead.Address.LineOne, Is.EqualTo(NullAsEmpty(party.Address)));
            Assert.That(lead.Address.SuburbName.ToUpper(), Is.EqualTo(NullAsEmpty(party.Suburb).ToUpper()));
            Assert.That(lead.Address.PostalCode.Value, Is.EqualTo(party.Postcode));

        }

        private static string NullAsEmpty(string val)
        {
            if (val == null)
                return string.Empty;

            return val;
        }

        private static string GetTitleString(IParty party)
        {
            if (party.Title == Title.Unknown)
                return string.Empty;

            return party.Title.ToString();
        }

        private static string GetStateString(IParty party)
        {
            if (party.State == State.Unknown)
                return null;

            return party.State.ToString();
        }
    }
}
