using System;
using System.Linq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Encrpytion;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.Party.Models;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Tests.Shared.Helpers;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.Policy.IntegrationTests.Data
{
    [TestFixture]
    public class PolicySearchDtoRepositoryTests
    {
        [Test]
        public void SearchByQuoteReference_ReturnsCorrectQuote()
        {
            var config = new PolicyConfigurationProvider();
            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var repo = new PolicySearchDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()));

            var po = new PartyDto
            {
                FirstName = "TestFirstName",
                Surname = "TestSurname",
                DateOfBirth = DateTime.Today.AddYears(-30),
                State = State.VIC,
                MobileNumber = "0444444444",
                HomeNumber = "0123456789",
                EmailAddress = "test@emailaddress.com.au",
                LeadId = 99
            };

            var policy = PolicyHelper.CreatePolicy();
            var party = PartyHelper.CreateParty(po);
            PolicyHelper.CreatePolicyOwner(party.Id, policy.Id);

            var results = repo.SearchByQuoteReference(policy.QuoteReference).ToList();

            Assert.That(results.Any(), Is.True);

            var searchResult = results.First();

            Assert.That(searchResult.QuoteReference, Is.EqualTo(policy.QuoteReference));
            Assert.That(searchResult.DateOfBirth, Is.EqualTo(po.DateOfBirth));
            Assert.That(searchResult.FirstName, Is.EqualTo(po.FirstName));
            Assert.That(searchResult.Surname, Is.EqualTo(po.Surname));
            Assert.That(searchResult.EmailAddress, Is.EqualTo(po.EmailAddress));
            Assert.That(searchResult.MobileNumber, Is.EqualTo(po.MobileNumber));
            Assert.That(searchResult.HomeNumber, Is.EqualTo(po.HomeNumber));
            Assert.That(searchResult.State, Is.EqualTo(po.State));
            Assert.That(searchResult.LeadId, Is.EqualTo(po.LeadId));
        }

        [Test]
        public void SearchByQuoteReference_WithDifferentBrandReturnsNoQuoteWithTALBrand()
        {
            var config = new PolicyConfigurationProvider();
            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var repo = new PolicySearchDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()));

            var po = new PartyDto
            {
                FirstName = "TestFirstName",
                Surname = "TestSurname",
                DateOfBirth = DateTime.Today.AddYears(-30),
                State = State.VIC,
                MobileNumber = "0444444444",
                HomeNumber = "0123456789",
                EmailAddress = "test@emailaddress.com.au",
                LeadId = 99
            };

            var policy = PolicyHelper.CreatePolicy();
            var party = PartyHelper.CreateParty(po);
            PolicyHelper.CreatePolicyOwner(party.Id, policy.Id);

            var results = repo.SearchByQuoteReference(policy.QuoteReference).ToList();

            Assert.That(results.Any(), Is.True);
            Assert.That(results.FirstOrDefault().BrandId, Is.EqualTo(1));
        }

        [Test]
        public void SearchByQuoteReference_WithNoBrand_ReturnsQuote()
        {
            var config = new PolicyConfigurationProvider();
            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var repo = new PolicySearchDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()));

            var po = new PartyDto
            {
                FirstName = "TestFirstName",
                Surname = "TestSurname",
                DateOfBirth = DateTime.Today.AddYears(-30),
                State = State.VIC,
                MobileNumber = "0444444444",
                HomeNumber = "0123456789",
                EmailAddress = "test@emailaddress.com.au",
                LeadId = 99
            };

            var policy = PolicyHelper.CreatePolicy();
            var party = PartyHelper.CreateParty(po);
            PolicyHelper.CreatePolicyOwner(party.Id, policy.Id);

            var results = repo.SearchByQuoteReference(policy.QuoteReference).ToList();

            Assert.That(results.Any(), Is.True);

            var searchResult = results.First();

            Assert.That(searchResult.QuoteReference, Is.EqualTo(policy.QuoteReference));
            Assert.That(searchResult.DateOfBirth, Is.EqualTo(po.DateOfBirth));
            Assert.That(searchResult.FirstName, Is.EqualTo(po.FirstName));
            Assert.That(searchResult.Surname, Is.EqualTo(po.Surname));
            Assert.That(searchResult.EmailAddress, Is.EqualTo(po.EmailAddress));
            Assert.That(searchResult.MobileNumber, Is.EqualTo(po.MobileNumber));
            Assert.That(searchResult.HomeNumber, Is.EqualTo(po.HomeNumber));
            Assert.That(searchResult.State, Is.EqualTo(po.State));
            Assert.That(searchResult.LeadId, Is.EqualTo(po.LeadId));
        }
    }
}
