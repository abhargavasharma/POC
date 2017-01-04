using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.Notifications.Service;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Product.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.ServiceLayer.Product;

namespace TAL.QuoteAndApply.ServiceLayer.UnitTests.Policy
{
    [TestFixture]
    public class PolicyCorrespondenceEmailServiceTests
    {
        private MockRepository _mockRepository;

        private Mock<IPolicyInteractionService> _policyInteractionService;
        private Mock<IEmailQuoteService> _emailQuoteService;
        private Mock<IPolicyCorrespondenceRequestConverter> _policyCorrespondenceRequestConverter;
        private Mock<IPolicyOverviewProvider> _policyOverviewProvider;
        private Mock<IPlanStateParamProvider> _planStateParamProvider;
        private Mock<IPlanService> _planService;
        private Mock<IRiskService> _riskService;
        private Mock<IActivePlanValidationService> _activePlanValidationService;
        private Mock<INameLookupService> _nameLookupService;
        private Mock<IProductBrandProvider> _mockProductBrandProvider;

        [TestFixtureSetUp]
        public void Setup()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _policyInteractionService = _mockRepository.Create<IPolicyInteractionService>();
            _emailQuoteService = _mockRepository.Create<IEmailQuoteService>();
            _policyCorrespondenceRequestConverter = _mockRepository.Create<IPolicyCorrespondenceRequestConverter>();
            _policyOverviewProvider = _mockRepository.Create<IPolicyOverviewProvider>();
            _planStateParamProvider = _mockRepository.Create<IPlanStateParamProvider>();
            _planService = _mockRepository.Create<IPlanService>();
            _riskService = _mockRepository.Create<IRiskService>();
            _activePlanValidationService = _mockRepository.Create<IActivePlanValidationService>();
            _nameLookupService = _mockRepository.Create<INameLookupService>();
            _mockProductBrandProvider = _mockRepository.Create<IProductBrandProvider>();
        }

        [Test]
        public void CreateSaveQuoteRequest_OwnerEmailAddressIsSelected()
        {
            var risk = new RiskDto();
            var owner = new PolicyOwnerDetailsParam {EmailAddress = "main@address.com"};
            var allPlans = new List<IPlan>();

            _policyOverviewProvider.Setup(call => call.GetOwnerDetailsFor("1"))
                .Returns(owner);
            _riskService.Setup(call => call.GetRisk(2))
                .Returns(risk);
            _planService.Setup(call => call.GetPlansForRisk(2))
                .Returns(allPlans);
            _planService.Setup(call => call.GetParentPlansFromAllPlans(allPlans))
                .Returns(new List<IPlan>());
            _mockProductBrandProvider.Setup(call => call.GetBrandKeyForQuoteReferenceNumber("1"))
                .Returns("tal");


            var service = new PolicyCorrespondenceEmailService(
                _policyInteractionService.Object,
                _emailQuoteService.Object,
                _policyCorrespondenceRequestConverter.Object,
                _policyOverviewProvider.Object,
                _planStateParamProvider.Object,
                _planService.Object,
                _riskService.Object,
                _activePlanValidationService.Object,
                _nameLookupService.Object,
                _mockProductBrandProvider.Object
                );

            var request = service.CreateSaveQuoteRequest("1", 2);

            Assert.That(request.ClientEmailAddress, Is.EqualTo("main@address.com"));
        }
    }
}
