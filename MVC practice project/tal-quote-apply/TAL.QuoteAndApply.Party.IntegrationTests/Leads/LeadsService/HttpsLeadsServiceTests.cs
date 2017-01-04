using System;
using System.Linq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Infrastructure.Features;
using TAL.QuoteAndApply.Infrastructure.Http;
using TAL.QuoteAndApply.Infrastructure.Url;
using TAL.QuoteAndApply.Party.Configuration;
using TAL.QuoteAndApply.Party.Leads.LeadsService;
using TAL.QuoteAndApply.Party.Models;
using TAL.QuoteAndApply.Tests.Shared.Builders;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.Party.IntegrationTests.Leads.LeadsService
{
    [TestFixture]
    public class HttpsLeadsServiceTests
    {
        private readonly HttpLeadsService _leadsService;

        public HttpsLeadsServiceTests()
        {
            var httpClientService = new HttpClientService(new HttpResponseMessageSerializer(), new MimeTypeProvider(),
                new HttpRequestMessageSerializer());

            var urlUtilities = new UrlUtilities();

            var leadConfigProvider = new LeadConfigurationProvider();

            _leadsService = new HttpLeadsService(leadConfigProvider, httpClientService, urlUtilities, new MockLoggingService(), new Features());
        }

        [Test]
        public void CreateParty_FieldByFieldUpdate()
        {
            var converter = new PartyToLeadsMessageConverter(new LeadConfigurationProvider());

            var partyBuilder = new PartyBuilder()
                  .WithId(98765)
                  .WithDateOfBirth(DateTime.Today.AddYears(-30))
                  .WithGender(Gender.Male)
                  .WithFirstName(GenerateName())
                  .WithSurname(GenerateName());

            var party = partyBuilder.Build();
            party.EmailAddress = $"{party.FirstName}.{party.Surname}@tal.com.au";

            var policySourceType = PolicySource.CustomerPortalBuildMyOwn;

            var createLeadMsg = converter.From(party, policySourceType);

            Console.WriteLine("1");
            Console.WriteLine(createLeadMsg.ToXml());

            var response = _leadsService.CreateLead(createLeadMsg);
            Assert.That(response, Is.Not.Null, "Test failed to create an adobe lead");
            party.LeadId = response.AdobeId;

            var retrievedLead = _leadsService.RetrieveLead(response.AdobeId);
            AssertLeadIsEqualToParty(retrievedLead, party);

            //state
            party.State = State.VIC;
            createLeadMsg = converter.From(party, policySourceType);

            Console.WriteLine("2");
            Console.WriteLine(createLeadMsg.ToXml());

            response = _leadsService.UpdateLead(createLeadMsg);
            retrievedLead = _leadsService.RetrieveLead(response.AdobeId);
            AssertLeadIsEqualToParty(retrievedLead, party);
            
            //mobile
            party.MobileNumber = "0400000000";
            createLeadMsg = converter.From(party, policySourceType);
            response = _leadsService.UpdateLead(createLeadMsg);
            retrievedLead = _leadsService.RetrieveLead(response.AdobeId);
            AssertLeadIsEqualToParty(retrievedLead, party);

            //address
            party.Address = "123 Seasme St";
            createLeadMsg = converter.From(party, policySourceType);
            response = _leadsService.UpdateLead(createLeadMsg);
            retrievedLead = _leadsService.RetrieveLead(response.AdobeId);
            AssertLeadIsEqualToParty(retrievedLead, party);

            //suburb
            party.Suburb = "Melbs";
            createLeadMsg = converter.From(party, policySourceType);
            response = _leadsService.UpdateLead(createLeadMsg);
            retrievedLead = _leadsService.RetrieveLead(response.AdobeId);
            AssertLeadIsEqualToParty(retrievedLead, party);

            //postcode
            party.Postcode = "3124";
            createLeadMsg = converter.From(party, policySourceType);
            response = _leadsService.UpdateLead(createLeadMsg);
            retrievedLead = _leadsService.RetrieveLead(response.AdobeId);
            AssertLeadIsEqualToParty(retrievedLead, party);
        }

        [Test]
        public void CreateFullParty_RetrieveOnAdobeId()
        {
            var party = GenerateParty();
            var policySourceType = PolicySource.CustomerPortalBuildMyOwn;

            var converter = new PartyToLeadsMessageConverter(new LeadConfigurationProvider());

            var createLeadMsg = converter.From(party, policySourceType);

            var response = _leadsService.CreateLead(createLeadMsg);
            Assert.That(response, Is.Not.Null, "Test failed to create an adobe lead");

            var retrievedLead = _leadsService.RetrieveLead(response.AdobeId);
            AssertLeadIsEqualToParty(retrievedLead, party);
        }

        [Test]
        public void CreateFullParty_RetrieveOnAdobeId_UpdateParty_RetrieveOnAdobeId()
        {
            var party = GenerateParty();
            var policySourceType = PolicySource.CustomerPortalBuildMyOwn;

            var converter = new PartyToLeadsMessageConverter(new LeadConfigurationProvider());

            var createLeadMsg = converter.From(party, policySourceType);

            var response = _leadsService.CreateLead(createLeadMsg);
            Assert.That(response, Is.Not.Null, "Test failed to create an adobe lead");

            var retrievedLead = _leadsService.RetrieveLead(response.AdobeId);
            AssertLeadIsEqualToParty(retrievedLead, party);

            party.LeadId = response.AdobeId;
            party.FirstName = GenerateName();

            var updateLeadMsg = converter.From(party, policySourceType);

            response = _leadsService.UpdateLead(updateLeadMsg);
            retrievedLead = _leadsService.RetrieveLead(response.AdobeId);
            AssertLeadIsEqualToParty(retrievedLead, party);
        }

        [Test, Ignore] //HttpsLeadsService doesn't implement a search with parameters other than the adobe ID - endpoint would be /api/Leads/{brandCode}
        public void CreateFullParty_RetrieveOnDateOfBirthAndEmail()
        {
            var party = GenerateParty();
            var policySourceType = PolicySource.CustomerPortalBuildMyOwn;

            var leadConfigurationProvider = new LeadConfigurationProvider();
            var converter = new PartyToLeadsMessageConverter(leadConfigurationProvider);

            var createLeadMsg = converter.From(party, policySourceType);

            var response = _leadsService.CreateLead(createLeadMsg);

            var retrieveLeadRequest = new RetrieveLeadRequest
            {
                BrandCode = leadConfigurationProvider.BrandCode,
                DOB = party.DateOfBirth.ToString("yyyy-MM-dd"),
                Email = party.EmailAddress
            };

            var retrievedLead = _leadsService.RetrieveLeads(retrieveLeadRequest);
            AssertLeadIsEqualToParty(retrievedLead, party);
        }

        [Test, Ignore] //HttpsLeadsService doesn't implement a search with parameters other than the adobe ID - endpoint would be /api/Leads/{brandCode}
        public void CreateFullParty_RetrieveOnFirstNameAndHomePhone()
        {
            var party = GenerateParty();

            var leadConfigurationProvider = new LeadConfigurationProvider();
            var converter = new PartyToLeadsMessageConverter(leadConfigurationProvider);
            var policySourceType = PolicySource.CustomerPortalBuildMyOwn;

            var createLeadMsg = converter.From(party, policySourceType);

            var response = _leadsService.CreateLead(createLeadMsg);

            var retrieveLeadRequest = new RetrieveLeadRequest
            {
                BrandCode = leadConfigurationProvider.BrandCode,
                FirstName = party.FirstName,
                HomePhone = party.HomeNumber
            };

            var retrievedLead = _leadsService.RetrieveLeads(retrieveLeadRequest);
            AssertLeadIsEqualToParty(retrievedLead, party);
        }

        [Test] 
        public void CreateFullParty_GetCommunicationPreferences_UpdateCommunicationPreferences()
        {
            var party = GenerateParty();

            var converter = new PartyToLeadsMessageConverter(new LeadConfigurationProvider());
            var policySourceType = PolicySource.CustomerPortalBuildMyOwn;

            var createLeadMsg = converter.From(party, policySourceType);

            var response = _leadsService.CreateLead(createLeadMsg);
            Assert.That(response, Is.Not.Null, "Test failed to create an adobe lead");

            party.LeadId = response.AdobeId;

            var commPreferencesResponse = _leadsService.GetCommunicationPreferences(response.AdobeId);

            AssertDncOnCommunicationPreferences(commPreferencesResponse, false, false);

            var partyConsent = GeneratePartyConsent(party.Id);
            partyConsent.ExpressConsent = true;
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

            AssertDncOnCommunicationPreferences(commPreferencesResponse, true, true);
        }

        private void AssertDncOnCommunicationPreferences(PartyCommunicationInquiryNotify commPreferencesResponse, 
            bool expectedDnc, 
            bool expectedExpressConsent)
        {
            Assert.That(commPreferencesResponse.PartyCommunication, Is.Not.Null);
            Assert.That(commPreferencesResponse.PartyCommunication.Length, Is.GreaterThan(0));
            Assert.That(commPreferencesResponse.PartyCommunication[0].Person, Is.Not.Null);
            Assert.That(commPreferencesResponse.PartyCommunication[0].Person.Length, Is.GreaterThan(0));

            var personCommunication = commPreferencesResponse.PartyCommunication[0].Person[0].PersonCommunication;
            var expressConsentType =
                commPreferencesResponse.PartyCommunication[0].CommunicationPreferences[0].ExpressConsent;

            //express consent
            Assert.That(expressConsentType.ExpressConsentIndicator, Is.EqualTo(expectedExpressConsent));

            //address
            Assert.That(personCommunication.MailingAddress[0].CommunicationPreferences[0].DoNotContactIndicator,
                Is.EqualTo(expectedDnc));

            //email
            Assert.That(personCommunication.Email[0].CommunicationPreferences[0].DoNotContactIndicator,
                Is.EqualTo(expectedDnc));

            //home phone
            Assert.That(personCommunication.Telephone.First(x=> x.TypeCode.Value.Name == "Wired").CommunicationPreferences[0].DoNotContactIndicator,
                Is.EqualTo(expectedDnc));

            //mobile phone
            Assert.That(personCommunication.Telephone.First(x => x.TypeCode.Value.Name == "Wireless").CommunicationPreferences[0].DoNotContactIndicator,
                Is.EqualTo(expectedDnc));
        }

        private static IParty GenerateParty()
        {
            var firstName = GenerateName();
            var surname = GenerateName();

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

        private static string GenerateName()
        {
            return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 30);
        }

        private static void AssertLeadIsEqualToParty(MarketingInquiryProcessResult leadResponse, IParty party)
        {
            Assert.That(leadResponse.MarketingInquiry.Lead, Is.Not.Null);
            Assert.That(leadResponse.MarketingInquiry.Lead.Length, Is.GreaterThan(0));
            var lead = leadResponse.MarketingInquiry.Lead[0];

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