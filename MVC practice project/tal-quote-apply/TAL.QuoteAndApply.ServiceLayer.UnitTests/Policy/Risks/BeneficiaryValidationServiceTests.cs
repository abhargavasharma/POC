using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.Infrastructure.Time;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Rules.Beneficiary;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Beneficiary;

namespace TAL.QuoteAndApply.ServiceLayer.UnitTests.Policy.Risks
{
    [TestFixture]
    public class BeneficiaryValidationServiceTests
    {
        private MockRepository _mockRepository;
        private Mock<IRiskService> _mockRiskService;

        [TestFixtureSetUp]
        public void Setup()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _mockRiskService = _mockRepository.Create<IRiskService>();
        }

        [TestCase(true, false, false)]
        [TestCase(false, true, true)]
        public void ValidateBeneficiariesForInforceForRisk_LprBeneficiaryShouldPass(bool isLprBeneficiary, bool anyValidationModels, bool expectedErrors)
        {
            var riskId = 123;
            var mockRisk = new RiskDto { Id = riskId, LprBeneficiary = isLprBeneficiary };
            _mockRiskService.Setup(call => call.GetRisk(riskId)).Returns(mockRisk);
            _mockRiskService.Setup(call => call.GetBeneficiariesForRisk(mockRisk)).Returns(new List<IBeneficiary>());
            
            var validationService = new BeneficiaryValidationService(_mockRiskService.Object, new BenefeciaryRuleFactory(new DateTimeProvider()));

            var result = validationService.ValidateBeneficiariesForInforceForRisk(riskId);

            Assert.That(result.Any(), Is.EqualTo(anyValidationModels));
            Assert.That(result.Any(e => e.ValidationErrors.Any()), Is.EqualTo(expectedErrors));
        }
    }
}
