using System;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure.Time;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.PremiumCalculation;
using TAL.QuoteAndApply.ServiceLayer.Policy.Referral;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Events;
using TAL.QuoteAndApply.Underwriting.Models.Dto;
using TAL.QuoteAndApply.Underwriting.Models.Talus;
using TAL.QuoteAndApply.Underwriting.Service;

namespace TAL.QuoteAndApply.ServiceLayer.UnitTests.Policy.Referral
{
    [TestFixture]
    public class CompleteReferralServiceTests
    {
        private Mock<IDateTimeProvider> _mockDateTimeProvider;
        private Mock<IReferralService> _mockReferralService;
        private Mock<IPolicyService> _mockPolicyService;
        private Mock<ICurrentUserProvider> _mockCurrentUserProvider;
        private Mock<IPolicyWithRisksService> _mockPolicyWithRisksService;
        private Mock<IPolicyPremiumCalculation> _mockPolicyPremiumCalculation;
        private Mock<IGetUnderwritingInterview> _mockGetUnderwritingInterview;
        private Mock<IUnderwritingBenefitsResponseChangeSubject> _mockUnderwritingBenefitsResponseChangeSubject;
        private Mock<IPolicyInteractionService> _mockPolicyInteractionService;

        private MockRepository _mockRepo;

        [TestFixtureSetUp]
        public void Setup()
        {
            _mockRepo = new MockRepository(MockBehavior.Strict);
            _mockReferralService = _mockRepo.Create<IReferralService>();
            _mockPolicyService = _mockRepo.Create<IPolicyService>();
            _mockCurrentUserProvider = _mockRepo.Create<ICurrentUserProvider>();
            _mockDateTimeProvider = _mockRepo.Create<IDateTimeProvider>();
            _mockPolicyWithRisksService = _mockRepo.Create<IPolicyWithRisksService>();
            _mockPolicyPremiumCalculation = _mockRepo.Create<IPolicyPremiumCalculation>();
            _mockGetUnderwritingInterview = _mockRepo.Create<IGetUnderwritingInterview>();
            _mockUnderwritingBenefitsResponseChangeSubject = _mockRepo.Create<IUnderwritingBenefitsResponseChangeSubject>();
            _mockPolicyInteractionService = _mockRepo.Create<IPolicyInteractionService>();
        }

        [Test]
        public void CompleteReferral_ReferralTaggedWithCurrentUserAndCurrentDate_PolicyStatusUpdated()
        {
            
            var mockUser = "Test.User";
            var mockCurrentUser = _mockRepo.Create<ICurrentUser>();
            mockCurrentUser.Setup(call => call.UserName).Returns(mockUser);

            var mockDate = DateTime.Today;

            var quoteRef = "abc123";

            var mockPolicy = new PolicyDto {Id = 123, QuoteReference = quoteRef, Progress = PolicyProgress.Unknown};
            var mockRisk = new RiskDto {InterviewId = "ID123", InterviewConcurrencyToken = Guid.NewGuid().ToString()};

            var mockPolicyWithRisks = new PolicyWithRisks(mockPolicy, new [] {new RiskWithPlans(mockRisk, null) });

            var mockReadOnlyInterview = new ReadOnlyUnderwritingInterview(new UnderwritingInterview());

            var mockPolicyPremiumSummary = new PolicyPremiumSummary(100, PremiumFrequency.Yearly, null);

            var mockReferral = new ReferralDto()
            {
                PolicyId = 123,
                AssignedTo = mockUser,
                AssignedToTS = mockDate,
                CompletedBy = "Bob",
                CompletedTS = mockDate,
                CreatedBy = mockUser,
                CreatedTS = mockDate,
                Id = 321,
                IsCompleted = true
            };

            _mockDateTimeProvider.Setup(call => call.GetCurrentDateAndTime()).Returns(mockDate);
            _mockCurrentUserProvider.Setup(call => call.GetForApplication()).Returns(mockCurrentUser.Object);

            _mockReferralService.Setup(call => call.CompleteReferral(mockPolicy.Id, mockUser, mockDate)).Returns(mockReferral);

            _mockPolicyWithRisksService.Setup(call => call.GetFrom(quoteRef)).Returns(mockPolicyWithRisks);
            _mockPolicyService.Setup(call => call.UpdatePolicyToIncomplete(mockPolicy)).Returns(mockPolicy);
            _mockPolicyInteractionService.Setup(call => call.PolicyReferralCompletedByUnderwriter(quoteRef));

            _mockGetUnderwritingInterview.Setup(
                call =>
                    call.GetInterview(mockRisk.InterviewId, mockRisk.InterviewConcurrencyToken,
                        _mockUnderwritingBenefitsResponseChangeSubject.Object)).Returns(mockReadOnlyInterview);

            _mockPolicyPremiumCalculation.Setup(call => call.CalculateAndSavePolicy(quoteRef)).Returns(mockPolicyPremiumSummary);

            var svc = new CompleteReferralService(_mockReferralService.Object, 
                _mockPolicyWithRisksService.Object,
                _mockPolicyService.Object, 
                _mockPolicyPremiumCalculation.Object,
                _mockGetUnderwritingInterview.Object,
                _mockUnderwritingBenefitsResponseChangeSubject.Object,
                _mockCurrentUserProvider.Object, 
                _mockDateTimeProvider.Object,
                _mockPolicyInteractionService.Object);


            svc.CompleteReferral(quoteRef);
        }
    }
}
