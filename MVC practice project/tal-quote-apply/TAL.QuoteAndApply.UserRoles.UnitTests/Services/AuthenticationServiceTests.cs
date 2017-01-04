using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.SalesPortal.Web.Configuration;
using TAL.QuoteAndApply.UserRoles.Configuration;
using TAL.QuoteAndApply.UserRoles.Models;
using TAL.QuoteAndApply.UserRoles.Services;

namespace TAL.QuoteAndApply.UserRoles.UnitTests.Services
{
    [TestFixture]
    public class AuthenticationServiceTests
    {
        private Mock<IUserRolesConfigurationProvider> _mockUserConfigurationProvider;
        private Mock<IUserPrincipalService> _mockUserPrincipalService;
        private Mock<IBrandSettingsProvider> _mockBrandSettingsProvider;
        private BrandSettingsProvider _brandSettingsProvider;

        [TestFixtureSetUp]
        public void Setup()
        {
            var mockRepo = new MockRepository(MockBehavior.Strict);
            _mockUserConfigurationProvider = mockRepo.Create<IUserRolesConfigurationProvider>();
            _mockUserPrincipalService = mockRepo.Create<IUserPrincipalService>();
            _mockBrandSettingsProvider = mockRepo.Create<IBrandSettingsProvider>();
            _brandSettingsProvider = new BrandSettingsProvider();
        }

        [Test]
        public void AuthenticateCurrentWindowsUser_UserInAppropriateRoles_Success()
        {
            string domain = "TestDomain";
            string agentGroup = ".uTalConsumerAgent_QA";
            string underwriterGroup = ".uTalConsumerUnderwriter_QA";
            string readonlyGroup = ".uTalConsumerReadOnly_QA";

            string userName = "TestUser";

            var user = new UserDetails()
            {
                Name = userName,
                EmailAddress = "test@test.com",
                GivenName = "Test",
                Surname = "Testsurname",
                Groups = new List<string>() { agentGroup, readonlyGroup, underwriterGroup }
            };

            _mockUserConfigurationProvider.Setup(call => call.Domain).Returns(domain);
            _mockBrandSettingsProvider.Setup(call => call.GetAllBrands()).Returns(_brandSettingsProvider.GetAllBrands());

            _mockUserPrincipalService.Setup(call => call.GetDetailsForUser(domain, userName))
                .Returns(user);

            var svc = GetService();

            var result = svc.AuthenticateCurrentWindowsUser(userName);

            Assert.That(result.UserName, Is.EqualTo(userName));
            Assert.That(result.Authenticated, Is.True);
            Assert.That(result.Roles.Contains(Role.ReadOnly), Is.True);
            Assert.That(result.Roles.Contains(Role.Agent), Is.True);
            Assert.That(result.Roles.Contains(Role.Underwriter), Is.True);
        }

        [Test]
        public void AuthenticateCurrentWindowsUser_UserInNoRoles_Failure()
        {
            string domain = "TestDomain";
            string agentGroup = ".uTalConsumerAgent_QA";
            string underwriterGroup = ".uTalConsumerUnderwriter_QA";
            string readonlyGroup = ".uTalConsumerReadOnly_QA";

            string userName = "TestUser";

            var user = new UserDetails()
            {
                Name = userName,
                EmailAddress = "test@test.com",
                GivenName = "Test",
                Surname = "Testsurname",
                Groups = new List<string>() {}
            };

            _mockUserConfigurationProvider.Setup(call => call.Domain).Returns(domain);
            _mockBrandSettingsProvider.Setup(call => call.GetAllBrands()).Returns(_brandSettingsProvider.GetAllBrands());

            _mockUserPrincipalService.Setup(call => call.GetDetailsForUser(domain, userName))
                .Returns(user);

            var svc = GetService();

            var result = svc.AuthenticateCurrentWindowsUser(userName);

            Assert.That(result.UserName, Is.EqualTo(userName));
            Assert.That(result.Authenticated, Is.False);
            Assert.That(result.AuthenticationFailureReason, Is.EqualTo(AuthenticationFailureReason.NoRoles));
            Assert.That(result.Roles, Is.Null);
        }


        [Test]
        public void AuthenticateUser_UserCredentialsCorrectAndInAppropriateRoles_Success()
        {
            string domain = "TestDomain";
            string agentGroup = ".uTalConsumerAgent_QA";
            string underwriterGroup = ".uTalConsumerUnderwriter_QA";
            string readonlyGroup = ".uTalConsumerReadOnly_QA";

            string userName = "TestUser";
            string password = "TestPassword";

            var user = new UserDetails()
            {
                Name = userName,
                EmailAddress = "test@test.com",
                GivenName = "Test",
                Surname = "Testsurname",
                Groups = new List<string>() { agentGroup, readonlyGroup, underwriterGroup }
            };

            _mockUserConfigurationProvider.Setup(call => call.Domain).Returns(domain);

            _mockUserPrincipalService.Setup(call => call.ValidateCredentials(domain, userName, password)).Returns(true);
            _mockUserPrincipalService.Setup(call => call.GetDetailsForUser(domain, userName))
                .Returns(user);
            _mockBrandSettingsProvider.Setup(call => call.GetAllBrands()).Returns(_brandSettingsProvider.GetAllBrands());

            var svc = GetService();

            var result = svc.AuthenticateUser(userName, password);

            Assert.That(result.UserName, Is.EqualTo(userName));
            Assert.That(result.Authenticated, Is.True);
            Assert.That(result.Roles.Contains(Role.ReadOnly), Is.True);
            Assert.That(result.Roles.Contains(Role.Agent), Is.True);
            Assert.That(result.Roles.Contains(Role.Underwriter), Is.True);
        }

        [Test]
        public void AuthenticateUser_UserCredentialsCorrectAndInNoRoles_Failure()
        {
            string domain = "TestDomain";
            string agentGroup = ".uTalConsumerAgent_QA";
            string underwriterGroup = ".uTalConsumerUnderwriter_QA";
            string readonlyGroup = ".uTalConsumerReadOnly_QA";

            string userName = "TestUser";
            string password = "TestPassword";

            var user = new UserDetails()
            {
                Name = userName,
                EmailAddress = "test@test.com",
                GivenName = "Test",
                Surname = "Testsurname",
                Groups = new List<string>() {}
            };

            _mockUserConfigurationProvider.Setup(call => call.Domain).Returns(domain);

            _mockUserPrincipalService.Setup(call => call.ValidateCredentials(domain, userName, password)).Returns(true);
            _mockUserPrincipalService.Setup(call => call.GetDetailsForUser(domain, userName))
                .Returns(user);
            _mockBrandSettingsProvider.Setup(call => call.GetAllBrands()).Returns(_brandSettingsProvider.GetAllBrands());

            var svc = GetService();

            var result = svc.AuthenticateUser(userName, password);

            Assert.That(result.UserName, Is.EqualTo(userName));
            Assert.That(result.Authenticated, Is.False);
            Assert.That(result.AuthenticationFailureReason, Is.EqualTo(AuthenticationFailureReason.NoRoles));
            Assert.That(result.Roles, Is.Null);
        }

        [Test]
        public void AuthenticateUser_UserCredentialsIncorrect_Failure()
        {
            string domain = "TestDomain";

            string userName = "TestUser";
            string password = "TestPassword";

            _mockUserConfigurationProvider.Setup(call => call.Domain).Returns(domain);

            _mockUserPrincipalService.Setup(call => call.ValidateCredentials(domain, userName, password)).Returns(false);

            var svc = GetService();

            var result = svc.AuthenticateUser(userName, password);

            Assert.That(result.UserName, Is.EqualTo(userName));
            Assert.That(result.Authenticated, Is.False);
            Assert.That(result.AuthenticationFailureReason, Is.EqualTo(AuthenticationFailureReason.InvalidCredentials));
            Assert.That(result.Roles, Is.Null);
        }

        private AuthenticationService GetService()
        {
            return new AuthenticationService(_mockUserConfigurationProvider.Object, _mockUserPrincipalService.Object, _mockBrandSettingsProvider.Object);
        }
    }
}
