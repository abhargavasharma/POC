using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Policy.Service.RaisePolicy;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy;
using TAL.QuoteAndApply.ServiceLayer.Validation;
using TAL.QuoteAndApply.Underwriting.Service;
using PolicyStatus = TAL.QuoteAndApply.DataModel.Policy.PolicyStatus;

namespace TAL.QuoteAndApply.ServiceLayer.UnitTests.Policy.RaisePolicy
{
    [TestFixture]
    public class RaisePolicyServiceTests
    {
        private MockRepository _mockRepo = new MockRepository(MockBehavior.Strict);
        private Mock<IHttpRaisePolicyService> _httpRaisePolicyService;
        private Mock<IRaisePolicyConverter> _raisePolicyConverter;
        private Mock<IPolicyService> _policyService;
        private Mock<IRaisePolicyFactory> _raisePolicyFactory;
        private Mock<IPolicySubmissionValidationService> _policySubmissionValidationService;
        private Mock<IRaisePolicySubmissionAuditService> _raisePolicySubmissionAuditService;
        private Mock<ICompleteUnderwritingInterview> _completeUnderwritingInterview;
        private Mock<IPolicyInteractionService> _policyInteractionService;

        private const string QuoteReference = "Q123456789";
        private const int RiskId = 1;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new MockRepository(MockBehavior.Strict);
            _httpRaisePolicyService = _mockRepo.Create<IHttpRaisePolicyService>();
            _raisePolicyConverter = _mockRepo.Create<IRaisePolicyConverter>();
            _policyService = _mockRepo.Create<IPolicyService>();
            _raisePolicyFactory = _mockRepo.Create<IRaisePolicyFactory>();
            _policySubmissionValidationService = _mockRepo.Create<IPolicySubmissionValidationService>();
            _raisePolicySubmissionAuditService = _mockRepo.Create<IRaisePolicySubmissionAuditService>();
            _completeUnderwritingInterview = _mockRepo.Create<ICompleteUnderwritingInterview>();
            _policyInteractionService = _mockRepo.Create<IPolicyInteractionService>();

        }

        public RaisePolicyService GetService()
        {
            return new RaisePolicyService(_httpRaisePolicyService.Object, _raisePolicyConverter.Object, _policyService.Object, _raisePolicyFactory.Object,
                _policySubmissionValidationService.Object, _raisePolicySubmissionAuditService.Object,_completeUnderwritingInterview.Object, _policyInteractionService.Object);
        }


        [TestCase(PolicyStatus.Incomplete, false)]
        [TestCase(PolicyStatus.ReadyForInforce, false)]
        [TestCase(PolicyStatus.ReferredToUnderwriter, false)]
        [TestCase(PolicyStatus.RaisedToPolicyAdminSystem, true)]
        [TestCase(PolicyStatus.Inforce, true)]
        [TestCase(PolicyStatus.FailedDuringPolicyAdminSystemLoad, true)]
        [TestCase(PolicyStatus.FailedToSendToPolicyAdminSystem, true)]
        public void PostPolicy_PolicyAlreadySubmitted_PostPolicyNotCalled_SubmissionAttemptedButNotActioned(PolicyStatus testStatus, bool policySubmitted)
        {
            //Arrange

            var raisePolicyObj = new ServiceLayer.Policy.RaisePolicy.Models.RaisePolicy() { Id = 0, Status = testStatus };
            _raisePolicyFactory.Setup(call => call.GetFromQuoteReference(QuoteReference))
                .Returns(raisePolicyObj);
            var newPolicySubmissionValidationResult = new PolicySubmissionValidationResult(new List<RiskSubmissionResult>
                { new RiskSubmissionResult(RiskId, new List<SectionSubmissionResult>()
                    {
                        new SectionSubmissionResult(PolicySection.PersonalDetails, new ValidationError[0], new ValidationError[0])
                    })
                });
            _policySubmissionValidationService.Setup(call => call.ValidatePolicy(raisePolicyObj))
                .Returns(newPolicySubmissionValidationResult);
            _policyService.Setup(call => call.UpdatePolicyToReadyToInforce(raisePolicyObj)).Returns(raisePolicyObj);
            _policyService.Setup(call => call.Get(raisePolicyObj.Id)).Returns(raisePolicyObj);
            var newPolicyNewBusinessOrderProcessType = new PolicyNewBusinessOrderProcess_Type();
            _raisePolicyConverter.Setup(
                call => call.From(It.IsAny<ServiceLayer.Policy.RaisePolicy.Models.RaisePolicy>()))
                .Returns(newPolicyNewBusinessOrderProcessType);
            _policyService.Setup(call => call.UpdateRaisedPolicyFields(It.IsAny<IPolicy>()));
            _policyInteractionService.Setup(call => call.PolicySubmitted(QuoteReference));
            _raisePolicySubmissionAuditService.Setup(
                call => call.WriteSubmissionAudit(raisePolicyObj.Id, newPolicyNewBusinessOrderProcessType));
            _policyService.Setup(call => call.GetRisksForPolicy(raisePolicyObj)).Returns(new List<RiskDto>());
            _policyService.Setup(call => call.UpdatePolicyToFailedToSendToPolicyAdminSystem(It.IsAny<ServiceLayer.Policy.RaisePolicy.Models.RaisePolicy>())).Returns(raisePolicyObj);
            _httpRaisePolicyService.Setup(call => call.Submit(It.IsAny<string>(), It.IsAny<PolicyNewBusinessOrderProcess_Type>()))
                .Returns(false);

            var svc = GetService();

            //Act
            var result = svc.PostPolicy(QuoteReference);

            //Assert
            if (policySubmitted)
            {
                _raisePolicyConverter.Verify(call => call.From(It.IsAny<ServiceLayer.Policy.RaisePolicy.Models.RaisePolicy>()),
                    Times.Never());
            }
            else
            {
                _raisePolicyConverter.Verify(call => call.From(It.IsAny<ServiceLayer.Policy.RaisePolicy.Models.RaisePolicy>()),
                    Times.AtLeastOnce);
            }

            Assert.That(result.SubmissionAttempted, Is.EqualTo(true));
        }
    }
}
