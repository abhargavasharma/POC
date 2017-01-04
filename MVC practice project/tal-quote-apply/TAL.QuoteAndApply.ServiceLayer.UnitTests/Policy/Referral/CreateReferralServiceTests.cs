using System;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Referral;

namespace TAL.QuoteAndApply.ServiceLayer.UnitTests.Policy.Referral
{
    [TestFixture]
    public class CreateReferralServiceTests
    {
        private Mock<IPolicyService> _mockPolicyService;
        private Mock<IReferralService> _mockReferralService;
        private Mock<IPolicyInteractionService> _mockPolicyInteractionService;

        [TestFixtureSetUp]
        public void Setup()
        {
            var mockRepo = new MockRepository(MockBehavior.Strict);
            _mockPolicyService = mockRepo.Create<IPolicyService>();
            _mockReferralService = mockRepo.Create<IReferralService>();
            _mockPolicyInteractionService = mockRepo.Create<IPolicyInteractionService>();
        }

        [Test]
        public void CreateReferralFor_ExistingReferral_ReferralAlreadyExistsForPolicyReturned()
        {
            var policy = new PolicyDto {QuoteReference = Guid.NewGuid().ToString(), Id = 111, Progress = PolicyProgress.Unknown};

            _mockPolicyService.Setup(call => call.GetByQuoteReferenceNumber(policy.QuoteReference)).Returns(policy);

            var existingReferral = new ReferralDto {PolicyId = policy.Id};
            _mockReferralService.Setup(call => call.GetInprogressReferralForPolicy(policy.Id)).Returns(existingReferral);

            var svc = new CreateReferralService(_mockPolicyService.Object, _mockReferralService.Object, _mockPolicyInteractionService.Object);
            var result = svc.CreateReferralFor(policy.QuoteReference);

            Assert.That(result, Is.EqualTo(CreateReferralResult.ReferralAlreadyExistsForPolicy));
        }

        [Test]
        public void CreateReferralFor_NoExistingReferral_CreatedReturned()
        {
            var policy = new PolicyDto { QuoteReference = Guid.NewGuid().ToString(), Id = 111, Progress = PolicyProgress.Unknown };

            _mockPolicyService.Setup(call => call.GetByQuoteReferenceNumber(policy.QuoteReference)).Returns(policy);

            var newReferral = new ReferralDto { PolicyId = policy.Id };
            _mockReferralService.Setup(call => call.GetInprogressReferralForPolicy(policy.Id)).Returns(()=> null);

            _mockPolicyService.Setup(call => call.UpdatePolicyToReferredToUnderwriter(policy)).Returns(policy);
            _mockReferralService.Setup(call=> call.CreateReferral(policy.Id)).Returns(newReferral);
            _mockPolicyInteractionService.Setup(call => call.PolicyReferredToUnderwriter(policy.QuoteReference));

            var svc = new CreateReferralService(_mockPolicyService.Object, _mockReferralService.Object, _mockPolicyInteractionService.Object);
            var result = svc.CreateReferralFor(policy.QuoteReference);

            Assert.That(result, Is.EqualTo(CreateReferralResult.Created));
        }
    }
}
