using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.SalesPortal.Web.Models;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.SalesPortal.Web.Services;
using TAL.QuoteAndApply.SalesPortal.Web.Services.Converters;
using TAL.QuoteAndApply.SalesPortal.Web.User;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Underwriting;

namespace TAL.QuoteAndApply.SalesPortal.Web.UnitTests.Services
{
    [TestFixture]
    public class SalesPortalPolicyRetrievalServiceTests
    {
        private Mock<IRetrievePolicyViewModelConverter> _mockRetrievePolicyViewModelConverter;
        private Mock<IEditPolicyPermissionsService> _mockEditPolicyPermissionsService;
        private Mock<ISalesPortalSessionContext> _mockSalesPortalSessionContext;
        private Mock<IPolicyOverviewProvider> _mockPolicyOverviewProvider;
        private Mock<IPolicyAutoUpdateService> _mockPolicyAutoUpdateService;
        private Mock<IPolicyInteractionService> _mockInteractionService;
        private Mock<IRiskUnderwritingAnswerSyncService> _mockRiskUnderwritingAnswerSyncService;

        [TestFixtureSetUp]
        public void Setup()
        {
            var mockRepo = new MockRepository(MockBehavior.Strict);
            _mockRetrievePolicyViewModelConverter = mockRepo.Create<IRetrievePolicyViewModelConverter>();
            _mockEditPolicyPermissionsService = mockRepo.Create<IEditPolicyPermissionsService>();
            _mockSalesPortalSessionContext = mockRepo.Create<ISalesPortalSessionContext>();
            _mockPolicyOverviewProvider = mockRepo.Create<IPolicyOverviewProvider>();
            _mockPolicyAutoUpdateService = mockRepo.Create<IPolicyAutoUpdateService>();
            _mockInteractionService = mockRepo.Create<IPolicyInteractionService>();
            _mockRiskUnderwritingAnswerSyncService = mockRepo.Create<IRiskUnderwritingAnswerSyncService>();
        }

        [Test]
        public void RetrieveQuote_QuoteEditSourceIsCreated_InteractionAddedAndViewModelReturned()
        {
            var quoteRef = "M12345678";
            var quoteEditSource = QuoteEditSource.Created;

            var mockPolicyOveriew = new PolicyOverviewResult
            {
                PolicyId = 1234,
                QuoteReferenceNumber = quoteRef,
                Status = PolicyStatus.Incomplete
            };

            var mockSalesPortalSession = new SalesPortalSession("Test.User", new [] {Role.Agent}, "test@test.com", "TestFirstName", "TestSurname", "TAL");

            var mockEditPolicyPermissions = new EditPolicyPermissionsResult(false, Role.Agent);

            _mockPolicyOverviewProvider.Setup(call => call.GetFor(quoteRef)).Returns(mockPolicyOveriew);
            _mockInteractionService.Setup(call => call.PolicyAccessed(mockPolicyOveriew.PolicyId));

            _mockSalesPortalSessionContext.Setup(call => call.SalesPortalSession).Returns(mockSalesPortalSession);
            _mockEditPolicyPermissionsService.Setup(
                call => call.GetPermissionsFor(mockPolicyOveriew.Status, mockSalesPortalSession.Roles))
                .Returns(mockEditPolicyPermissions);

            _mockRetrievePolicyViewModelConverter.Setup(
                call => call.CreateFrom(mockPolicyOveriew, mockEditPolicyPermissions))
                .Returns(new RetrievePolicyViewModel());

            var svc = GetService();
            svc.RetrieveQuote(quoteRef, quoteEditSource);
        }

        [TestCase(PolicyStatus.FailedDuringPolicyAdminSystemLoad)]
        [TestCase(PolicyStatus.FailedToSendToPolicyAdminSystem)]
        [TestCase(PolicyStatus.Inforce)]
        [TestCase(PolicyStatus.RaisedToPolicyAdminSystem)]
        [TestCase(PolicyStatus.ReadyForInforce)]
        public void RetrieveQuote_QuoteEditSourceIsRetrieve_StatusShouldNotRecalculate_InteractionAddedAndViewModelReturned(PolicyStatus status)
        {
            var quoteRef = "M12345678";
            var quoteEditSource = QuoteEditSource.Retrieved;

            var mockPolicyOveriew = new PolicyOverviewResult
            {
                PolicyId = 1234,
                QuoteReferenceNumber = quoteRef,
                Status = status,
                Risks = new List<RiskOverviewResult>
                {
                    new RiskOverviewResult {RiskId = 1}
                }
            };

            var mockSalesPortalSession = new SalesPortalSession("Test.User", new[] { Role.Agent }, "test@test.com", "TestFirstName", "TestSurname", "TAL");

            var mockEditPolicyPermissions = new EditPolicyPermissionsResult(false, Role.Agent);

            _mockPolicyOverviewProvider.Setup(call => call.GetFor(quoteRef)).Returns(mockPolicyOveriew);
            _mockInteractionService.Setup(call => call.PolicyAccessed(mockPolicyOveriew.PolicyId));

            _mockSalesPortalSessionContext.Setup(call => call.SalesPortalSession).Returns(mockSalesPortalSession);
            _mockEditPolicyPermissionsService.Setup(
                call => call.GetPermissionsFor(mockPolicyOveriew.Status, mockSalesPortalSession.Roles))
                .Returns(mockEditPolicyPermissions);

            _mockRetrievePolicyViewModelConverter.Setup(
                call => call.CreateFrom(mockPolicyOveriew, mockEditPolicyPermissions))
                .Returns(new RetrievePolicyViewModel());

            var svc = GetService();
            svc.RetrieveQuote(quoteRef, quoteEditSource);
        }

        [TestCase(PolicyStatus.Incomplete)]
        [TestCase(PolicyStatus.ReferredToUnderwriter)]
        public void RetrieveQuote_QuoteEditSourceIsRetrieve_StatusShouldRecalculate_InteractionAddedAndViewModelReturned(PolicyStatus status)
        {
            var quoteRef = "M12345678";
            var quoteEditSource = QuoteEditSource.Retrieved;

            var mockPolicyOveriew = new PolicyOverviewResult
            {
                PolicyId = 1234,
                QuoteReferenceNumber = quoteRef,
                Status = status,
                Risks = new List<RiskOverviewResult>
                {
                    new RiskOverviewResult {RiskId = 1}
                }
            };

            var mockSalesPortalSession = new SalesPortalSession("Test.User", new[] { Role.Agent }, "test@test.com", "TestFirstName", "TestSurname", "TAL");

            var mockEditPolicyPermissions = new EditPolicyPermissionsResult(false, Role.Agent);

            _mockPolicyOverviewProvider.Setup(call => call.GetFor(quoteRef)).Returns(mockPolicyOveriew);

            _mockRiskUnderwritingAnswerSyncService.Setup(
                call => call.SyncRiskWithFullInterviewAndUpdatePlanEligibility(1));

            _mockPolicyAutoUpdateService.Setup(
                call => call.AutoUpdatePlansForEligibililityAndRecalculatePremium(quoteRef));

            _mockInteractionService.Setup(call => call.PolicyAccessed(mockPolicyOveriew.PolicyId));

            _mockSalesPortalSessionContext.Setup(call => call.SalesPortalSession).Returns(mockSalesPortalSession);
            _mockEditPolicyPermissionsService.Setup(
                call => call.GetPermissionsFor(mockPolicyOveriew.Status, mockSalesPortalSession.Roles))
                .Returns(mockEditPolicyPermissions);

            _mockRetrievePolicyViewModelConverter.Setup(
                call => call.CreateFrom(mockPolicyOveriew, mockEditPolicyPermissions))
                .Returns(new RetrievePolicyViewModel());

            var svc = GetService();
            svc.RetrieveQuote(quoteRef, quoteEditSource);
        }

        public SalesPortalPolicyRetrievalService GetService()
        {
            return new SalesPortalPolicyRetrievalService(_mockRetrievePolicyViewModelConverter.Object, _mockEditPolicyPermissionsService.Object, 
                _mockSalesPortalSessionContext.Object, _mockPolicyOverviewProvider.Object, 
                _mockPolicyAutoUpdateService.Object, _mockInteractionService.Object, _mockRiskUnderwritingAnswerSyncService.Object);
        }
    }
}
