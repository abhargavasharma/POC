using System;
using NUnit.Framework;
using TAL.QuoteAndApply.Infrastructure.Crypto;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Tests.Shared.Mocks;
using TAL.QuoteAndApply.UserRoles.Configuration;
using TAL.QuoteAndApply.UserRoles.Customer;

namespace TAL.QuoteAndApply.UserRoles.IntegrationTests.Customer
{
    [TestFixture]
    public class OktaCustomerAuthenticationServiceTests
    {
        [Test]
        public void OktaCustomerAuthenticationProvider_CreateUser_BadLogin_GoodLogin()
        {
            var quoteReference = new QuoteReferenceGenerationService().RandomQuoteReference();
            var password = "Abcd1234!";
            var firstName = "Bob";
            var lastName = "Test";
            var email = $"{firstName}.{lastName}@user.com";

            var provider = GetService();

            var createResult = provider.CreateCustomerLogin(quoteReference, email, password, firstName, lastName);
            Assert.That(createResult.Status, Is.EqualTo(CustomerResultStatus.Success));

            var badAuthenticateResult = provider.Authenticate(quoteReference, password + "AAAA");
            Assert.That(badAuthenticateResult.Status, Is.EqualTo(CustomerResultStatus.Failure));

            var goodAuthenticateResult = provider.Authenticate(quoteReference, password);
            Assert.That(goodAuthenticateResult.Status, Is.EqualTo(CustomerResultStatus.Success));
        }

        [Test]
        public void CreateCustomerLogin_CreateDuplicateUser_ReturnsUserAlreadyExistsResult()
        {
            var quoteReference = new QuoteReferenceGenerationService().RandomQuoteReference();
            var password = "Abcd1234!";
            var firstName = "Bob";
            var lastName = "Test";
            var email = $"{firstName}.{lastName}@user.com";

            var provider = GetService();

            var createResult = provider.CreateCustomerLogin(quoteReference, email, password, firstName, lastName);
            Assert.That(createResult.Status, Is.EqualTo(CustomerResultStatus.Success));

            createResult = provider.CreateCustomerLogin(quoteReference, email, password, firstName, lastName);
            Assert.That(createResult.Status, Is.EqualTo(CustomerResultStatus.UserAlreadyExists));
        }

        [Test]
        public void AccountExists_AccountExistsForUser_ReturnsTrue()
        {
            var quoteReference = new QuoteReferenceGenerationService().RandomQuoteReference();
            var password = "Abcd1234!";
            var firstName = "Bob";
            var lastName = "Test";
            var email = $"{firstName}.{lastName}@user.com";

            var provider = GetService();

            var createResult = provider.CreateCustomerLogin(quoteReference, email, password, firstName, lastName);
            Assert.That(createResult.Status, Is.EqualTo(CustomerResultStatus.Success));

            var accountExists = provider.AccountExists(quoteReference);
            Assert.That(accountExists);
        }

        [Test]
        public void AccountExists_AccountDoesntExist_ReturnsFalse()
        {
            var quoteReference = new QuoteReferenceGenerationService().RandomQuoteReference();

            var provider = GetService();

            var accountExists = provider.AccountExists(quoteReference);
            Assert.That(accountExists, Is.False);
        }

        private static ICustomerAuthenticationService GetService()
        {
            return new OktaCustomerAuthenticationService(new OktaConfigurationProvider(new SecurityService()),
                new OktaAuthenticationResultFactory(), new MockLoggingService());
        }

    }
}