using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Party.Models;
using TAL.QuoteAndApply.Party.Services;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters;

namespace TAL.QuoteAndApply.ServiceLayer.UnitTests.Policy
{
    [TestFixture]
    public class PolicyOwnershipServiceTests
    {
        private Mock<IPartyService> _partyService;
        private Mock<IPolicyService> _policyService;
        private Mock<IPolicyOwnerService> _policyOwnerService;
        private Mock<IPartyConsentService> _partyConsentService;
        private Mock<IPartyConsentDtoUpdater> _partyConsentDtoUpdater;
        private Mock<IRiskService> _riskService;

        [SetUp]
        public void SetUp()
        {
            var mockRepo = new MockRepository(MockBehavior.Strict);

            _partyService = mockRepo.Create<IPartyService>(); 
            _policyService = mockRepo.Create<IPolicyService>();
            _policyOwnerService = mockRepo.Create<IPolicyOwnerService>();
            _partyConsentService = mockRepo.Create<IPartyConsentService>();
            _partyConsentDtoUpdater = mockRepo.Create<IPartyConsentDtoUpdater>();
            _riskService = mockRepo.Create<IRiskService>();
        }

        [Test]
        public void SetOrdinaryOwnership_Test()
        {
            var policy = new PolicyDto
            {
                QuoteReference = "1",
                Id = 10
            };
            var ownerParty = new PartyDto { Id = 77 };
            var primaryRiskParty = new PartyDto {Id = 7};
            var primaryRisk = new RiskDto {PartyId = primaryRiskParty.Id };

            _policyService.Setup(call => call.GetByQuoteReferenceNumber(policy.QuoteReference))
                .Returns(policy);
            _policyService.Setup(call => call.GetRisksForPolicy(policy))
                .Returns(new List<IRisk> { primaryRisk } );
            _policyOwnerService.Setup(call => call.GetPolicyOwnerPartyId(policy.Id))
                .Returns(ownerParty.Id);

            _policyOwnerService.Setup(call => call.SetPolicyOwnerTypeForPolicy(policy.Id, PolicyOwnerType.Ordinary));
            _policyOwnerService.Setup(call => call.SetPolicyOwnerFundName(policy.Id, null));
            _riskService.Setup(call => call.UpdateRisk(primaryRisk));
            _partyService.Setup(call => call.DeleteParty(primaryRiskParty.Id));

            var service = new PolicyOwnershipService(_partyService.Object, _policyService.Object,
                _policyOwnerService.Object, _partyConsentService.Object, _partyConsentDtoUpdater.Object,
                _riskService.Object);

            service.SetOrdinaryOwnership(policy.QuoteReference);

            //VERIFY:
            //ownership was changed
            _policyOwnerService.Verify(x => x.SetPolicyOwnerTypeForPolicy(policy.Id, PolicyOwnerType.Ordinary));
            //Fund name was cleaned
            _policyOwnerService.Verify(x => x.SetPolicyOwnerFundName(policy.Id, null));
            //Primary risk party was deleted
            _partyService.Verify(x => x.DeleteParty(primaryRiskParty.Id));
            //Primary risk now pointing to the owner party
            Assert.That(primaryRisk.PartyId, Is.EqualTo(ownerParty.Id));
        }

        [Test]
        public void SetSmsfOwnership_Test()
        {
            var policy = new PolicyDto
            {
                QuoteReference = "1",
                Id = 10
            };
            var ownerParty = new PartyDto { Id = 77 };
            var primaryRiskParty = new PartyDto { Id = 7 };
            var primaryRisk = new RiskDto { PartyId = ownerParty.Id };

            _policyService.Setup(call => call.GetByQuoteReferenceNumber(policy.QuoteReference))
                .Returns(policy);
            _policyService.Setup(call => call.GetRisksForPolicy(policy))
                .Returns(new List<IRisk> { primaryRisk });
            _policyOwnerService.Setup(call => call.GetPolicyOwnerPartyId(policy.Id))
                .Returns(ownerParty.Id);
            _partyService.Setup(call => call.GetParty(ownerParty.Id))
                .Returns(ownerParty);
            _partyService.Setup(call => call.CreatePartyWithoutLead(It.IsAny<IParty>()))
                .Returns(primaryRiskParty);
            _partyConsentService.Setup(call => call.CreatePartyConsent(It.IsAny<IPartyConsent>(), It.IsAny<IParty>()))
                .Returns((IPartyConsent)null);


            _policyOwnerService.Setup(call => call.SetPolicyOwnerTypeForPolicy(policy.Id, PolicyOwnerType.SelfManagedSuperFund));
            _policyOwnerService.Setup(call => call.SetPolicyOwnerFundName(policy.Id, ""));
            _riskService.Setup(call => call.UpdateRisk(primaryRisk));

            var service = new PolicyOwnershipService(_partyService.Object, _policyService.Object,
                _policyOwnerService.Object, _partyConsentService.Object, _partyConsentDtoUpdater.Object,
                _riskService.Object);

            service.SetSmsfOwnership(policy.QuoteReference);

            //VERIFY:
            //ownership was changed
            _policyOwnerService.Verify(x => x.SetPolicyOwnerTypeForPolicy(policy.Id, PolicyOwnerType.SelfManagedSuperFund));
            
            //Primary risk now pointing to the owner party
            Assert.That(primaryRisk.PartyId, Is.EqualTo(primaryRiskParty.Id));
        }

        [Test]
        public void SetSuperOwnership_Test()
        {
            var policy = new PolicyDto
            {
                QuoteReference = "1",
                Id = 10
            };
            var ownerParty = new PartyDto { Id = 77 };
            var primaryRiskParty = new PartyDto { Id = 7 };
            var primaryRisk = new RiskDto { PartyId = ownerParty.Id };

            _policyService.Setup(call => call.GetByQuoteReferenceNumber(policy.QuoteReference))
                .Returns(policy);
            _policyService.Setup(call => call.GetRisksForPolicy(policy))
                .Returns(new List<IRisk> { primaryRisk });
            _policyOwnerService.Setup(call => call.GetPolicyOwnerPartyId(policy.Id))
                .Returns(ownerParty.Id);
            _partyService.Setup(call => call.GetParty(ownerParty.Id))
                .Returns(ownerParty);
            _partyService.Setup(call => call.CreatePartyWithoutLead(It.IsAny<IParty>()))
                .Returns(primaryRiskParty);
            _partyConsentService.Setup(call => call.CreatePartyConsent(It.IsAny<IPartyConsent>(), It.IsAny<IParty>()))
                .Returns((IPartyConsent)null);


            _policyOwnerService.Setup(call => call.SetPolicyOwnerTypeForPolicy(policy.Id, PolicyOwnerType.SuperannuationFund));
            _policyOwnerService.Setup(call => call.SetPolicyOwnerFundName(policy.Id, "TASL / TAL Superfund"));
            _riskService.Setup(call => call.UpdateRisk(primaryRisk));

            var service = new PolicyOwnershipService(_partyService.Object, _policyService.Object,
                _policyOwnerService.Object, _partyConsentService.Object, _partyConsentDtoUpdater.Object,
                _riskService.Object);

            service.SetSuperOwnership(policy.QuoteReference);

            //VERIFY:
            //ownership was changed
            _policyOwnerService.Verify(x => x.SetPolicyOwnerTypeForPolicy(policy.Id, PolicyOwnerType.SuperannuationFund));
            _policyOwnerService.Verify(x => x.SetPolicyOwnerFundName(policy.Id, "TASL / TAL Superfund"));

            //Primary risk now pointing to the owner party
            Assert.That(primaryRisk.PartyId, Is.EqualTo(primaryRiskParty.Id));
        }
    }
}
