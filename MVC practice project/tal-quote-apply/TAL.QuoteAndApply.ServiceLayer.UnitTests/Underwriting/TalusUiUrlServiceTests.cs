using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure.Time;
using TAL.QuoteAndApply.Infrastructure.Url;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Underwriting;
using TAL.QuoteAndApply.ServiceLayer.User;
using TAL.QuoteAndApply.Underwriting.Models.Phoenix;
using TAL.QuoteAndApply.Underwriting.Service;

namespace TAL.QuoteAndApply.ServiceLayer.UnitTests.Underwriting
{
    [TestFixture]
    public class TalusUiUrlServiceTests
    {
        private Mock<IRiskService> _mockRiskService;
        private Mock<ITalusUiTokenService> _mockTalusUiTokenService;
        private Mock<IDateTimeProvider> _mockDateTimeProvider;
        private Mock<IUnderwritingConfiguration> _mockUnderwritingConfiguration;
        private Mock<ICurrentUserProvider> _mockCurrentUserProvider;
        private Mock<IUrlUtilities> _mockUrlUtilities;
        private Mock<IPolicyService> _mockPolicyService;

        [TestFixtureSetUp]
        public void Setup()
        {
            var mockRepo = new MockRepository(MockBehavior.Strict);
            _mockPolicyService = mockRepo.Create<IPolicyService>();
            _mockRiskService = mockRepo.Create<IRiskService>();
            _mockTalusUiTokenService = mockRepo.Create<ITalusUiTokenService>();
            _mockDateTimeProvider = mockRepo.Create<IDateTimeProvider>();
            _mockUnderwritingConfiguration = mockRepo.Create<IUnderwritingConfiguration>();
            _mockCurrentUserProvider = mockRepo.Create<ICurrentUserProvider>();
            _mockUrlUtilities = mockRepo.Create<IUrlUtilities>();
        }

        [TestCase(PolicyStatus.RaisedToPolicyAdminSystem)]
        [TestCase(PolicyStatus.ReadyForInforce)]
        public void GetTalusUiUrlWithPermissionsFor_StatusThatShouldGiveReadonlyAccess_ReadOnlyTokenCreated(PolicyStatus policyStatus)
        {
            var baseUrl = "http://readonly.com";

            var mockPolicy = new PolicyDto
            {
                Id = 888,
                QuoteReference = "quoteRef",
                Status = policyStatus,
                Progress = PolicyProgress.Unknown
            };

            var mockRisk = new RiskDto
            {
                Id = 999,
                InterviewId = "theInterview",
                InterviewConcurrencyToken = "theToken"
            };

            var mockCurrentUser = new CurrentUser("Test.User", new List<Role> { Role.Agent }, "test@test.com", "TestFirstName", "TestSurname");
            var mockAccessPermissions = new[] { AccessPermission.ReadOnly };
            var mockExpiryDate = DateTime.Today;

            var mockAuthTokenDto = new AuthorisationResponseDto(Guid.NewGuid().ToString(), mockCurrentUser.UserName, mockAccessPermissions, mockRisk.InterviewId, mockExpiryDate);

            _mockPolicyService.Setup(call => call.GetByQuoteReferenceNumber(mockPolicy.QuoteReference)).Returns(mockPolicy);
            _mockRiskService.Setup(call => call.GetRisk(mockRisk.Id)).Returns(mockRisk);
            _mockUrlUtilities.Setup(call => call.UrlEncode(mockRisk.InterviewConcurrencyToken))
                .Returns(mockRisk.InterviewConcurrencyToken);
            _mockCurrentUserProvider.Setup(call => call.GetForApplication()).Returns(mockCurrentUser);
            _mockUnderwritingConfiguration.Setup(call => call.TalusUiBaseUrl).Returns(baseUrl);

            _mockDateTimeProvider.Setup(call => call.GetCurrentDate()).Returns(mockExpiryDate);
            _mockTalusUiTokenService.Setup(call => call.GetReadOnlyPermissions()).Returns(mockAccessPermissions);
            _mockTalusUiTokenService.Setup(
                call =>
                    call.GetAuthorisationToken(mockRisk.InterviewId, mockCurrentUser.UserName, mockAccessPermissions, It.IsAny<DateTime>()))
                .Returns(mockAuthTokenDto);

            var svc = GetService();
            var url = svc.GetTalusUiUrlWithPermissionsFor(mockPolicy.QuoteReference, mockRisk.Id);

            Assert.That(url, Is.EqualTo($"{baseUrl}/#/token/{mockAuthTokenDto.Token}?etag={mockRisk.InterviewConcurrencyToken}"));
        }

        [Test]
        public void GetTalusUiUrlWithPermissionsFor_InprogressStatus_AgentRole_AgentTokenCreated()
        {
            var baseUrl = "http://readonly.com";

            var mockPolicy = new PolicyDto
            {
                Id = 888,
                QuoteReference = "quoteRef",
                Status = PolicyStatus.Incomplete,
                Progress = PolicyProgress.Unknown
            };

            var mockRisk = new RiskDto
            {
                Id = 999,
                InterviewId = "theInterview",
                InterviewConcurrencyToken = "theToken"
            };

            var mockCurrentUser = new CurrentUser("Test.User", new List<Role> {Role.Agent}, "test@test.com", "TestFirstName", "TestSurname");
            var mockAccessPermissions = new[] {AccessPermission.AnswerQuestions};
            var mockExpiryDate = DateTime.Today;

            var mockAuthTokenDto = new AuthorisationResponseDto(Guid.NewGuid().ToString(), mockCurrentUser.UserName, mockAccessPermissions, mockRisk.InterviewId, mockExpiryDate);

            _mockPolicyService.Setup(call => call.GetByQuoteReferenceNumber(mockPolicy.QuoteReference)).Returns(mockPolicy);
            _mockRiskService.Setup(call => call.GetRisk(mockRisk.Id)).Returns(mockRisk);
            _mockUrlUtilities.Setup(call => call.UrlEncode(mockRisk.InterviewConcurrencyToken))
                .Returns(mockRisk.InterviewConcurrencyToken);
            _mockCurrentUserProvider.Setup(call => call.GetForApplication()).Returns(mockCurrentUser);
            _mockUnderwritingConfiguration.Setup(call => call.TalusUiBaseUrl).Returns(baseUrl);

            _mockDateTimeProvider.Setup(call => call.GetCurrentDate()).Returns(mockExpiryDate);
            _mockTalusUiTokenService.Setup(call => call.GetAgentPermissions()).Returns(mockAccessPermissions);
            _mockTalusUiTokenService.Setup(
                call =>
                    call.GetAuthorisationToken(mockRisk.InterviewId, mockCurrentUser.UserName, mockAccessPermissions, It.IsAny<DateTime>()))
                .Returns(mockAuthTokenDto);

            var svc = GetService();
            var url = svc.GetTalusUiUrlWithPermissionsFor(mockPolicy.QuoteReference, mockRisk.Id);
            
            Assert.That(url, Is.EqualTo($"{baseUrl}/#/token/{mockAuthTokenDto.Token}?etag={mockRisk.InterviewConcurrencyToken}"));
        }

        [TestCase(Role.Underwriter)]
        [TestCase(Role.ReadOnly)]
        public void GetTalusUiUrlWithPermissionsFor_InprogressStatus_NotAgentRole_ReadOnlyTokenCreated(Role role)
        {
            var baseUrl = "http://readonly.com";

            var mockPolicy = new PolicyDto
            {
                Id = 888,
                QuoteReference = "quoteRef",
                Status = PolicyStatus.Incomplete,
                Progress = PolicyProgress.Unknown
            };

            var mockRisk = new RiskDto
            {
                Id = 999,
                InterviewId = "theInterview",
                InterviewConcurrencyToken = "theToken"
            };

            var mockCurrentUser = new CurrentUser("Test.User", new List<Role> { role }, "test@test.com", "TestFirstName", "TestSurname");
            var mockAccessPermissions = new[] { AccessPermission.ReadOnly };
            var mockExpiryDate = DateTime.Today;

            var mockAuthTokenDto = new AuthorisationResponseDto(Guid.NewGuid().ToString(), mockCurrentUser.UserName, mockAccessPermissions, mockRisk.InterviewId, mockExpiryDate);

            _mockPolicyService.Setup(call => call.GetByQuoteReferenceNumber(mockPolicy.QuoteReference)).Returns(mockPolicy);
            _mockRiskService.Setup(call => call.GetRisk(mockRisk.Id)).Returns(mockRisk);
            _mockUrlUtilities.Setup(call => call.UrlEncode(mockRisk.InterviewConcurrencyToken))
                .Returns(mockRisk.InterviewConcurrencyToken);
            _mockCurrentUserProvider.Setup(call => call.GetForApplication()).Returns(mockCurrentUser);
            _mockUnderwritingConfiguration.Setup(call => call.TalusUiBaseUrl).Returns(baseUrl);

            _mockDateTimeProvider.Setup(call => call.GetCurrentDate()).Returns(mockExpiryDate);
            _mockTalusUiTokenService.Setup(call => call.GetUnderwriterReadOnlyPermissions()).Returns(mockAccessPermissions);
            _mockTalusUiTokenService.Setup(call => call.GetReadOnlyPermissions()).Returns(mockAccessPermissions);
            _mockTalusUiTokenService.Setup(
                call =>
                    call.GetAuthorisationToken(mockRisk.InterviewId, mockCurrentUser.UserName, mockAccessPermissions, It.IsAny<DateTime>()))
                .Returns(mockAuthTokenDto);

            var svc = GetService();
            var url = svc.GetTalusUiUrlWithPermissionsFor(mockPolicy.QuoteReference, mockRisk.Id);

            Assert.That(url, Is.EqualTo($"{baseUrl}/#/token/{mockAuthTokenDto.Token}?etag={mockRisk.InterviewConcurrencyToken}"));
        }






        [Test]
        public void GetTalusUiUrlWithPermissionsFor_ReferredToUnderwriterStatus_UnderwriterRole_UnderwriterTokenCreated()
        {
            var baseUrl = "http://readonly.com";

            var mockPolicy = new PolicyDto
            {
                Id = 888,
                QuoteReference = "quoteRef",
                Status = PolicyStatus.ReferredToUnderwriter,
                Progress = PolicyProgress.Unknown
            };

            var mockRisk = new RiskDto
            {
                Id = 999,
                InterviewId = "theInterview",
                InterviewConcurrencyToken = "theToken"
            };

            var mockCurrentUser = new CurrentUser("Test.User", new List<Role> { Role.Underwriter }, "test@test.com", "TestFirstName", "TestSurname");
            var mockAccessPermissions = new[] { AccessPermission.Override };
            var mockExpiryDate = DateTime.Today;

            var mockAuthTokenDto = new AuthorisationResponseDto(Guid.NewGuid().ToString(), mockCurrentUser.UserName, mockAccessPermissions, mockRisk.InterviewId, mockExpiryDate);

            _mockPolicyService.Setup(call => call.GetByQuoteReferenceNumber(mockPolicy.QuoteReference)).Returns(mockPolicy);
            _mockRiskService.Setup(call => call.GetRisk(mockRisk.Id)).Returns(mockRisk);
            _mockUrlUtilities.Setup(call => call.UrlEncode(mockRisk.InterviewConcurrencyToken))
                .Returns(mockRisk.InterviewConcurrencyToken);
            _mockCurrentUserProvider.Setup(call => call.GetForApplication()).Returns(mockCurrentUser);
            _mockUnderwritingConfiguration.Setup(call => call.TalusUiBaseUrl).Returns(baseUrl);

            _mockDateTimeProvider.Setup(call => call.GetCurrentDate()).Returns(mockExpiryDate);
            _mockTalusUiTokenService.Setup(call => call.GetUnderwriterPermissions()).Returns(mockAccessPermissions);
            _mockTalusUiTokenService.Setup(
                call =>
                    call.GetAuthorisationToken(mockRisk.InterviewId, mockCurrentUser.UserName, mockAccessPermissions, It.IsAny<DateTime>()))
                .Returns(mockAuthTokenDto);

            var svc = GetService();
            var url = svc.GetTalusUiUrlWithPermissionsFor(mockPolicy.QuoteReference, mockRisk.Id);

            Assert.That(url, Is.EqualTo($"{baseUrl}/#/token/{mockAuthTokenDto.Token}?etag={mockRisk.InterviewConcurrencyToken}"));
        }

        [TestCase(Role.Agent)]
        [TestCase(Role.ReadOnly)]
        public void GetTalusUiUrlWithPermissionsFor_ReferredToUnderwriterStatus_NotUnderwriterRole_ReadOnlyTokenCreated(Role role)
        {
            var baseUrl = "http://readonly.com";

            var mockPolicy = new PolicyDto
            {
                Id = 888,
                QuoteReference = "quoteRef",
                Status = PolicyStatus.ReferredToUnderwriter,
                Progress = PolicyProgress.Unknown
            };

            var mockRisk = new RiskDto
            {
                Id = 999,
                InterviewId = "theInterview",
                InterviewConcurrencyToken = "theToken"
            };

            var mockCurrentUser = new CurrentUser("Test.User", new List<Role> { role }, "test@test.com", "TestFirstName", "TestSurname");
            var mockAccessPermissions = new[] { AccessPermission.ReadOnly };
            var mockExpiryDate = DateTime.Today;

            var mockAuthTokenDto = new AuthorisationResponseDto(Guid.NewGuid().ToString(), mockCurrentUser.UserName, mockAccessPermissions, mockRisk.InterviewId, mockExpiryDate);

            _mockPolicyService.Setup(call => call.GetByQuoteReferenceNumber(mockPolicy.QuoteReference)).Returns(mockPolicy);
            _mockRiskService.Setup(call => call.GetRisk(mockRisk.Id)).Returns(mockRisk);
            _mockUrlUtilities.Setup(call => call.UrlEncode(mockRisk.InterviewConcurrencyToken))
                .Returns(mockRisk.InterviewConcurrencyToken);
            _mockCurrentUserProvider.Setup(call => call.GetForApplication()).Returns(mockCurrentUser);
            _mockUnderwritingConfiguration.Setup(call => call.TalusUiBaseUrl).Returns(baseUrl);

            _mockDateTimeProvider.Setup(call => call.GetCurrentDate()).Returns(mockExpiryDate);
            _mockTalusUiTokenService.Setup(call => call.GetReadOnlyPermissions()).Returns(mockAccessPermissions);
            _mockTalusUiTokenService.Setup(
                call =>
                    call.GetAuthorisationToken(mockRisk.InterviewId, mockCurrentUser.UserName, mockAccessPermissions, It.IsAny<DateTime>()))
                .Returns(mockAuthTokenDto);

            var svc = GetService();
            var url = svc.GetTalusUiUrlWithPermissionsFor(mockPolicy.QuoteReference, mockRisk.Id);

            Assert.That(url, Is.EqualTo($"{baseUrl}/#/token/{mockAuthTokenDto.Token}?etag={mockRisk.InterviewConcurrencyToken}"));
        }

        private TalusUiUrlService GetService()
        {
            return new TalusUiUrlService(_mockPolicyService.Object, _mockRiskService.Object, _mockTalusUiTokenService.Object, _mockDateTimeProvider.Object, 
                _mockUnderwritingConfiguration.Object, _mockCurrentUserProvider.Object, _mockUrlUtilities.Object);
        }
    }
}
