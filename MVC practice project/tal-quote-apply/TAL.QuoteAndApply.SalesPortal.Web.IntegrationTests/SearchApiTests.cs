using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TAL.Performance.Infrastructure.Core;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.Party.Models;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests.Clients;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Search.Clients.Models;
using TAL.QuoteAndApply.Tests.Shared.Builders;
using TAL.QuoteAndApply.Tests.Shared.Helpers;
using TAL.QuoteAndApply.Web.Shared.HttpResults;
using Task = System.Threading.Tasks.Task;

namespace TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests
{
    [TestFixture]
    public class SearchApiTests : BaseTestClass<SearchClient>
    {
        public SearchApiTests() : base(SimplePerformanceTool.CreateConsoleTimestamped())
        {
        }

        [Test]
        public async Task SearchClients_SearchOnQuoteReference_QuoteReferenceNotProvided_ModelStateErrorsReturned_Async()
        {
            var searchClientsRequest = new SearchClientsRequest { SearchOnQuoteReference = true, QuoteReferenceNumber = null };

            var result = await Client.SearchClientsAsync<Dictionary<string, IEnumerable<string>>>(searchClientsRequest, throwOnFailure: false);

            Assert.That(result.ContainsKey("searchClientsRequest.quoteReferenceNumber"), Is.True);
            Assert.That(result.First(x => x.Key.Equals("searchClientsRequest.quoteReferenceNumber")).Value.First(), Is.EqualTo("Quote Reference is required"));
        }

        [TestCase("123456789")]
        [TestCase("12345678901")]
        public async Task SearchClients_QuoteReferenceNot10Characters_ModelStateErrorsReturned_Async(string quoteReference)
        {
            var searchClientsRequest = new SearchClientsRequest {SearchOnQuoteReference = true, QuoteReferenceNumber = quoteReference};

            var result = await Client.SearchClientsAsync<Dictionary<string, IEnumerable<string>>>(searchClientsRequest, throwOnFailure:false);

            Assert.That(result.ContainsKey("searchClientsRequest.quoteReferenceNumber"), Is.True);
            Assert.That(result.First(x => x.Key.Equals("searchClientsRequest.quoteReferenceNumber")).Value.First(), Is.EqualTo("Quote reference must be 10 characters long"));
        }

        [Test]
        public async Task SearchClients_SearchOnLeadId_LeadIdNotProvided_ModelStateErrorsReturned_Async()
        {
            var searchClientsRequest = new SearchClientsRequest() { SearchOnLeadId = true, LeadId = null };

            var result = await Client.SearchClientsAsync<Dictionary<string, IEnumerable<string>>>(searchClientsRequest, throwOnFailure: false);

            Assert.That(result.ContainsKey("searchClientsRequest.leadId"), Is.True);
            Assert.That(result.First(x => x.Key.Equals("searchClientsRequest.leadId")).Value.First(), Is.EqualTo("Adobe Id is required"));
        }

        [Test]
        public async Task SearchClients_LeadIdIdNotNumber_ModelStateErrorsReturned_Async()
        {
            var searchClientsRequest = new SearchClientsRequest() {SearchOnLeadId = true, LeadId = "ABC123"};

            var result = await Client.SearchClientsAsync<Dictionary<string, IEnumerable<string>>>(searchClientsRequest, throwOnFailure: false);

            Assert.That(result.ContainsKey("searchClientsRequest.leadId"), Is.True);
            Assert.That(result.First(x => x.Key.Equals("searchClientsRequest.leadId")).Value.First(), Is.EqualTo("Adobe Id must be a number"));
        }

        [Test]
        public async Task SearchClients_ExternalReferenceTooLong_ModelStateErrorsReturned_Async()
        {
            var searchClientsRequest = new SearchClientsRequest() { ExternalCustomerReference = "123456789012345678901", LeadId = "ABC123" };

            var result = await Client.SearchClientsAsync<Dictionary<string, IEnumerable<string>>>(searchClientsRequest, throwOnFailure: false);

            Assert.That(result.ContainsKey("searchClientsRequest.externalCustomerReference"), Is.True);
            Assert.That(result.First(x => x.Key.Equals("searchClientsRequest.externalCustomerReference")).Value.First(), Is.EqualTo("External customer reference cannot be longer than 20 characters."));
        }

        [Test]
        public async Task SearchClients_QuoteReferenceDoesNotExist_NoQuotesReturned_Async()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var searchClientsRequest = new SearchClientsRequest
            {
                QuoteReferenceNumber = "NOT EXISTS",
                SearchOnQuoteReference = true
            };

            var result = await Client.SearchClientsAsync<SearchClientsResponse>(searchClientsRequest);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Quotes, Is.Not.Null);
            Assert.That(result.Quotes.Any(), Is.False);

        }

        [Test]
        public async Task Search_QuoteReferenceNumberExists_PolicyReturned_Async()
        {
            await LogOutAndLoginWithBrand("TAL");

            var policy = CreatePolicy();
            
            var searchClientsRequest = new SearchClientsRequest
            {
                QuoteReferenceNumber = policy.QuoteReference,
                SearchOnQuoteReference = true
            };

            var result = await Client.SearchClientsAsync<SearchClientsResponse>(searchClientsRequest);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Quotes, Is.Not.Null);
            Assert.That(result.Quotes.Any(), Is.True);
            Assert.That(result.Quotes.ToList()[0].QuoteReferenceNumber, Is.EqualTo(policy.QuoteReference));
        }

        [Test]
        public async Task Search_AdobeIdExists_PolicyReturned_Async()
        {
            var adobeId = new Random(DateTime.Now.Millisecond).Next();
            var policy = CreatePolicy(adobeId);

            var searchClientsRequest = new SearchClientsRequest
            {
                LeadId = adobeId.ToString(),
                SearchOnLeadId = true
            };

            var result = await Client.SearchClientsAsync<SearchClientsResponse>(searchClientsRequest);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Quotes, Is.Not.Null);
            Assert.That(result.Quotes.Any(), Is.True);
            Assert.That(result.Quotes.ToList()[0].QuoteReferenceNumber, Is.EqualTo(policy.QuoteReference));
            Assert.That(result.Quotes.ToList()[0].LeadId, Is.EqualTo(adobeId));
        }

        [Test]
        public async Task Search_CreateTalPolicyLogoutThenLoginAsYbAgent_AdobeIdExistsButIsWrongBrand_NoPoliciesReturned_Async()
        {
            var adobeId = new Random(DateTime.Now.Millisecond).Next();
            var policy = CreatePolicy(adobeId);

            await LogOutAndLoginWithBrand("YB");

            var searchClientsRequest = new SearchClientsRequest
            {
                LeadId = adobeId.ToString(),
                SearchOnLeadId = true
            };

            var result = await Client.SearchClientsAsync<SearchClientsResponse>(searchClientsRequest);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Quotes, Is.Not.Null);
            Assert.That(result.Quotes.Any(), Is.False);
            Assert.That(result.Quotes.ToList().Count, Is.EqualTo(0));
            Assert.That(result.ResultType, Is.EqualTo(SearchResultType.Quotes));
        }

        [Test]
        public async Task Search_SearchByPartyWithNoFields_AtLeastOneFieldIsRequiredErrorMsgsReturned_Async()
        {
            var errorMsg = "At least one field is required";

            CreatePolicy();

            var searchClientsRequest = new SearchClientsRequest
            {
                SearchOnLeadId = false,
                SearchOnQuoteReference = false,
                SearchOnParty = true,
                LeadId = null,
                QuoteReferenceNumber = null
            };

            var result = await Client.SearchClientsAsync<Dictionary<string, IEnumerable<string>>>(searchClientsRequest, throwOnFailure: false);

            Assert.That(result, Is.Not.Null);

            Assert.That(result.ContainsKey("searchClientsRequest.dateOfBirth"), Is.True);
            Assert.That(result.ContainsKey("searchClientsRequest.emailAddress"), Is.True);
            Assert.That(result.ContainsKey("searchClientsRequest.externalCustomerReference"), Is.True);
            Assert.That(result.ContainsKey("searchClientsRequest.firstName"), Is.True);
            Assert.That(result.ContainsKey("searchClientsRequest.homeNumber"), Is.True);
            Assert.That(result.ContainsKey("searchClientsRequest.mobileNumber"), Is.True);
            Assert.That(result.ContainsKey("searchClientsRequest.surname"), Is.True);
            Assert.That(result.First(x => x.Key.Equals("searchClientsRequest.dateOfBirth")).Value.First(), Is.EqualTo(errorMsg));
            Assert.That(result.First(x => x.Key.Equals("searchClientsRequest.emailAddress")).Value.First(), Is.EqualTo(errorMsg));
            Assert.That(result.First(x => x.Key.Equals("searchClientsRequest.externalCustomerReference")).Value.First(), Is.EqualTo(errorMsg));
            Assert.That(result.First(x => x.Key.Equals("searchClientsRequest.firstName")).Value.First(), Is.EqualTo(errorMsg));
            Assert.That(result.First(x => x.Key.Equals("searchClientsRequest.homeNumber")).Value.First(), Is.EqualTo(errorMsg));
            Assert.That(result.First(x => x.Key.Equals("searchClientsRequest.mobileNumber")).Value.First(), Is.EqualTo(errorMsg));
            Assert.That(result.First(x => x.Key.Equals("searchClientsRequest.surname")).Value.First(), Is.EqualTo(errorMsg));
        }

        [Test]
        public async Task Search_SearchByPartyFirstName_QuoteReturned_Async()
        {
            await LogOutAndLoginWithBrand("TAL");

            var policy = CreatePolicyWithoutParty();

            PolicyHelper.CreatePolicyOwner(policy.Party.Id, policy.Policy.Id);

            var searchClientsRequest = new SearchClientsRequest
            {
                SearchOnParty = true,
                FirstName = policy.Party.FirstName
            };

            var result = await Client.SearchClientsAsync<SearchClientsResponse>(searchClientsRequest);
            AssertQuoteResultIsAsExpected(result, policy);
        }

        [Test]
        public async Task Search_SearchByPartySurname_QuoteReturned_Async()
        {
            await LogOutAndLoginWithBrand("TAL");

            var policy = CreatePolicyWithoutParty();

            PolicyHelper.CreatePolicyOwner(policy.Party.Id, policy.Policy.Id);

            var searchClientsRequest = new SearchClientsRequest
            {
                SearchOnParty = true,
                Surname = policy.Party.Surname
            };

            var result = await Client.SearchClientsAsync<SearchClientsResponse>(searchClientsRequest);
            AssertQuoteResultIsAsExpected(result, policy);
        }

        [Test]
        public async Task Search_SearchByPartyDob_QuoteReturned_Async()
        {
            await LogOutAndLoginWithBrand("TAL");

            var policy = CreatePolicyWithoutParty();

            PolicyHelper.CreatePolicyOwner(policy.Party.Id, policy.Policy.Id);

            var searchClientsRequest = new SearchClientsRequest
            {
                SearchOnParty = true,
                DateOfBirth = policy.Party.DateOfBirth.ToString("dd/MM/yyyy")
            };

            var result = await Client.SearchClientsAsync<SearchClientsResponse>(searchClientsRequest);
            AssertQuoteResultIsAsExpected(result, policy);
        }

        [Test]
        public async Task Search_SearchByPartyMobileNumber_QuoteReturned_Async()
        {
            await LogOutAndLoginWithBrand("TAL");

            var policy = CreatePolicyWithoutParty();

            PolicyHelper.CreatePolicyOwner(policy.Party.Id, policy.Policy.Id);

            var searchClientsRequest = new SearchClientsRequest
            {
                SearchOnParty = true,
                MobileNumber = policy.Party.MobileNumber
            };

            var result = await Client.SearchClientsAsync<SearchClientsResponse>(searchClientsRequest);
            AssertQuoteResultIsAsExpected(result, policy);
        }

        [Test]
        public async Task Search_SearchByPartyHomeNumber_QuoteReturned_Async()
        {
            await LogOutAndLoginWithBrand("TAL");

            var policy = CreatePolicyWithoutParty();

            PolicyHelper.CreatePolicyOwner(policy.Party.Id, policy.Policy.Id);

            var searchClientsRequest = new SearchClientsRequest
            {
                SearchOnParty = true,
                HomeNumber = policy.Party.HomeNumber
            };

            var result = await Client.SearchClientsAsync<SearchClientsResponse>(searchClientsRequest);
            AssertQuoteResultIsAsExpected(result, policy);
        }

        [Test]
        public async Task Search_SearchByPartyEmailAddress_QuoteReturned_Async()
        {
            await LogOutAndLoginWithBrand("TAL");

            var policy = CreatePolicyWithoutParty();

            PolicyHelper.CreatePolicyOwner(policy.Party.Id, policy.Policy.Id);

            var searchClientsRequest = new SearchClientsRequest
            {
                SearchOnParty = true,
                EmailAddress = policy.Party.EmailAddress
            };

            var result = await Client.SearchClientsAsync<SearchClientsResponse>(searchClientsRequest);
            AssertQuoteResultIsAsExpected(result, policy);
        }

        [Test]
        public async Task Search_SearchByPartyExternalCustomerReference_QuoteReturned_Async()
        {
            await LogOutAndLoginWithBrand("TAL");

            var policy = CreatePolicyWithoutParty();

            PolicyHelper.CreatePolicyOwner(policy.Party.Id, policy.Policy.Id);

            var searchClientsRequest = new SearchClientsRequest
            {
                SearchOnParty = true,
                ExternalCustomerReference = policy.Party.ExternalCustomerReference
            };

            var result = await Client.SearchClientsAsync<SearchClientsResponse>(searchClientsRequest, throwOnFailure: false);
            AssertQuoteResultIsAsExpected(result, policy);
        }

        private void AssertQuoteResultIsAsExpected(SearchClientsResponse result, CreatePolicyResult policy)
        {
            Assert.That(result, Is.Not.Null);
            var quote = result.Quotes.First(x => x.QuoteReferenceNumber == policy.Policy.QuoteReference);
            Assert.That(quote.Brand, Is.EqualTo("TAL"));
            Assert.That(quote.OwnerName, Is.EqualTo(policy.Party.FirstName + " " + policy.Party.Surname));
            Assert.That(quote.OwnerDateOfBirth, Is.EqualTo(DateTime.Today.AddYears(-30).ToString("dd/MM/yyyy")));
            Assert.That(quote.OwnerState, Is.EqualTo("ACT"));
            Assert.That(quote.OwnerPhoneNumber, Is.EqualTo(policy.Party.MobileNumber));
            Assert.That(quote.OwnerEmailAddress, Is.EqualTo(policy.Party.EmailAddress));
            Assert.That(quote.Premium, Is.Null);
            Assert.That(quote.LeadId, Is.EqualTo(policy.Party.LeadId));
        }

        private PolicyDto CreatePolicy(long? leadId = null)
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var policy = PolicyHelper.CreatePolicy();
            var party = PartyHelper.CreateParty(new PartyDto { DateOfBirth = DateTime.Today.AddYears(-30), LeadId = leadId });
            PolicyHelper.CreatePolicyOwner(party.Id, policy.Id);

            return policy;
        }

        private CreatePolicyResult CreatePolicyWithoutParty()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var party = PartyHelper.CreateParty(new PartyDto
            {
                FirstName = "Firstest",
                DateOfBirth = DateTime.Today.AddYears(-30),
                Surname = "Lastest",
                MobileNumber = "0411111111",
                HomeNumber = "0211111111",
                EmailAddress = "test@tal.com.au",
                ExternalCustomerReference = "Test123",
                State = State.ACT,
                LeadId = 123456
            });

            var risk = new RiskBuilder()
                .Default()
                .WithDateOfBirth(party.DateOfBirth)
                .WithGender(party.Gender)
                .Build();

            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var policy = PolicyHelper.CreatePolicy(risk, party);

            PolicyHelper.CreatePolicyOwner(party.Id, policy.Policy.Id);

            return policy;
        }
    }
}
