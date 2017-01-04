using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Party.Models;
using TAL.QuoteAndApply.Party.Services;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Rules;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy;
using TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy.Models;
using TAL.QuoteAndApply.ServiceLayer.Search.Clients;
using TAL.QuoteAndApply.ServiceLayer.Search.Clients.Models;
using Assert = NUnit.Framework.Assert;

namespace TAL.QuoteAndApply.ServiceLayer.IntegrationTests.Policy.RaisePolicy
{
    [TestFixture]
    public class RaisePolicyOwnershipValidationServiceTests
    {
        private MockRepository _mockRepo = new MockRepository(MockBehavior.Strict);
        private Mock<IPolicyOwnerService> _policyOwnerService;
        private Mock<IRaisePolicyValidationService> _raisePolicyValidationService;
        private Mock<IRaisePolicyFactory> _raisePolicyFactory;
        private Mock<IPolicyOverviewProvider> _policyOverviewProvider;
        private Mock<IPartyService> _partyService;
        private Mock<IExternalRefDetailsFactory> _externalRefDetailsFactory;


        [SetUp]
        public void Setup()
        {
            _mockRepo = new MockRepository(MockBehavior.Strict);
            _policyOwnerService = _mockRepo.Create<IPolicyOwnerService>();
            _raisePolicyValidationService = _mockRepo.Create<IRaisePolicyValidationService>();
            _raisePolicyFactory = _mockRepo.Create<IRaisePolicyFactory>();
            _policyOverviewProvider = _mockRepo.Create<IPolicyOverviewProvider>();
            _partyService = _mockRepo.Create<IPartyService>();
            _externalRefDetailsFactory = _mockRepo.Create<IExternalRefDetailsFactory>();
        }

        public RaisePolicyOwnershipValidationService GetService()
        {
            return new RaisePolicyOwnershipValidationService(
                 _policyOwnerService.Object,
                _raisePolicyValidationService.Object,
                _raisePolicyFactory.Object,
                _policyOverviewProvider.Object,
                _partyService.Object,
                _externalRefDetailsFactory.Object);
        }

        [TestCase(ExternalCustomerRefRequired.NotRequired, "123", true)]
        [TestCase(ExternalCustomerRefRequired.Optional, "123", true)]
        [TestCase(ExternalCustomerRefRequired.Mandatory, "123", true)]
        [TestCase(ExternalCustomerRefRequired.NotRequired, null, true)]
        [TestCase(ExternalCustomerRefRequired.Optional, null, true)]
        [TestCase(ExternalCustomerRefRequired.Mandatory, null, false)]
        [TestCase(ExternalCustomerRefRequired.Mandatory, "", false)]
        public void IsCompletedTest_WhenExternalRefOptionalOrNotRequiredOrMandatoryAndExternalCustomerValueProvidedAndNotProvided_ValidatesSuccessOrFailure(ExternalCustomerRefRequired externalCustomerRefValue, string customerReference, bool epectedValue)
        {
            //Arrange
            var quoteRef = "TEST123";
            var externalCustomerRefRequired = externalCustomerRefValue;
            var externalReflabel = "Test Number";
            var policyOwnerType = PolicyOwnerType.Ordinary;
            _policyOwnerService.Setup(call => call.GetOwnerShipType(quoteRef)).Returns(policyOwnerType);

            var raisePolicyObj = new ServiceLayer.Policy.RaisePolicy.Models.RaisePolicy()
            {
                Id = 1,
                QuoteReference = quoteRef,
                BrandKey = "YB",
                Risks = new List<RaisePolicyRisk>() { new RaisePolicyRisk() { Id = 1, PartyId = 2 } },
                Owner = new RaisePolicyOwner()
                {
                    Address = "123 Address",
                    Country = Country.Australia,
                    EmailAddress = "Test@tal.com.au",
                    FirstName = "Stu",
                    FundName = "ABN",
                    HomeNumber = "0233334444",
                    MobileNumber = "0450991233",
                    ExternalCustomerReference = customerReference,
                    OwnerType = policyOwnerType,
                    PartyId = 2,
                    Postcode = "2000",
                    State = State.NSW,
                    Suburb = "St Leonards",
                    Surname = "Victor",
                    Title = Title.Mr
                }
            };
            _raisePolicyFactory.Setup(call => call.GetFromQuoteReference(quoteRef))
                .Returns(raisePolicyObj);

            var ownerDetails = new PolicyOwnerDetailsParam();
            _policyOverviewProvider.Setup(call => call.GetOwnerDetailsFor(quoteRef)).Returns(ownerDetails);


            var partyDto = new PartyDto();
            _partyService.Setup(call => call.GetParty(raisePolicyObj.Risks[0].PartyId)).Returns(partyDto);

            _raisePolicyValidationService.Setup(call => call.ValidateOwnerForInforce(raisePolicyObj.Owner))
                     .Returns(new List<RuleResult>());

            if (string.IsNullOrEmpty(raisePolicyObj.Owner.ExternalCustomerReference))
            {
                _raisePolicyValidationService.Setup(
                    call =>
                        call.ValidateOwnerExternalCustomerRefForInforce(raisePolicyObj.Owner.ExternalCustomerReference))
                    .Returns(new List<RuleResult>() {new RuleResult() {Key = "Owner.ExternalCustomerReference"}});
            }
            else
            {
                _raisePolicyValidationService.Setup(
                    call =>
                        call.ValidateOwnerExternalCustomerRefForInforce(raisePolicyObj.Owner.ExternalCustomerReference))
                    .Returns(new List<RuleResult>() {new RuleResult(true)});
            }

            _raisePolicyValidationService.Setup(call => call.ValidatePartyForInforce(partyDto))
                   .Returns(new List<RuleResult>());
            _externalRefDetailsFactory.Setup(call => call.ExternalCustomerRefConfigDetails(raisePolicyObj.BrandKey))
                .Returns(new ExternalCustomerReferenceDetails(externalCustomerRefRequired, externalReflabel)
                {
                    ExternalCustomerRefRequired = externalCustomerRefRequired,
                    ExternalCustomerRefLabel = externalReflabel
                });

            //Act

            var raisePolicyOwnershipValidationService = GetService();
            var result = raisePolicyOwnershipValidationService.IsCompleted(quoteRef);

            //Assert

            Assert.That(result.Equals(epectedValue));
        }
    }
}
