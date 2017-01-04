using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.Party.Data;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;

namespace TAL.QuoteAndApply.Policy.UnitTests.Services
{
    [TestFixture]
    public class PolicyOwnerServiceTests
    {
        private Mock<IPolicyOwnerDtoRepository> _mockPolicyOwnerDtoRepo;

        private Mock<IPolicyService> _mockPolicyService;

        private Mock<IPolicyOwnerService> _mockPolicyOwnerService;

        [TestFixtureSetUp]
        public void Setup()
        {
            var mockRepository = new MockRepository(MockBehavior.Strict);
            _mockPolicyOwnerDtoRepo = mockRepository.Create<IPolicyOwnerDtoRepository>();
            _mockPolicyService = mockRepository.Create<IPolicyService>();
            _mockPolicyOwnerService = mockRepository.Create<IPolicyOwnerService>();
        }

        [Test]
        public void SetPolicyOwnerForPolicy_NotCurrentlyAssociated_Inserted()
        {
            var policyId = 1;
            var partyId = 1;

            PolicyOwnerDto getCallReturn = null;
            PolicyOwnerDto insertCall = new PolicyOwnerDto {PolicyId = policyId, PartyId = partyId};

            _mockPolicyOwnerDtoRepo.Setup(call => call.GetPolicyOwnerForPolicyId(policyId))
                .Returns(getCallReturn);

            _mockPolicyOwnerDtoRepo.Setup(call => call.Insert(It.IsAny<PolicyOwnerDto>())).Returns(insertCall);

            var svc = new PolicyOwnerService(_mockPolicyOwnerDtoRepo.Object, _mockPolicyService.Object);
            svc.SetPolicyOwnerPartyForPolicy(policyId, partyId);

        }

        [Test]
        public void SetPolicyOwnerForPolicy_CurrentlyAssociated_Updated()
        {
            var policyId = 1;
            var originalPartyId = 1;
            var newPartyId = 1;

            PolicyOwnerDto getCallReturn = new PolicyOwnerDto { PolicyId = policyId, PartyId = originalPartyId };

            _mockPolicyOwnerDtoRepo.Setup(call => call.GetPolicyOwnerForPolicyId(policyId))
                .Returns(getCallReturn);

            _mockPolicyOwnerDtoRepo.Setup(call => call.Update(It.IsAny<PolicyOwnerDto>()));

            var svc = new PolicyOwnerService(_mockPolicyOwnerDtoRepo.Object, _mockPolicyService.Object);
            svc.SetPolicyOwnerPartyForPolicy(policyId, newPartyId);

        }
    }
}
