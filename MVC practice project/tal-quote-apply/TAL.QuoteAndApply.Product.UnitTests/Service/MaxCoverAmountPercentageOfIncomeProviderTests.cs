using System;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.Product.Models.Definition;
using TAL.QuoteAndApply.Product.Service;

namespace TAL.QuoteAndApply.Product.UnitTests.Service
{
    [TestFixture]
    public class MaxCoverAmountPercentageOfIncomeProviderTests
    {
        private Mock<IPlanDefinitionProvider> _mockPlanDefinitionProvider;

        [TestFixtureSetUp]
        public void Setup()
        {
            _mockPlanDefinitionProvider = new Mock<IPlanDefinitionProvider>(MockBehavior.Strict);
        }

        [Test, ExpectedException(typeof(ApplicationException), MatchType = MessageMatch.StartsWith, ExpectedMessage = "No PlanDefinition for plan")]
        public void GetFor_NoPlanDefinition_ExceptionThrown()
        {
            var planCode = "TEST";

            _mockPlanDefinitionProvider.Setup(call => call.GetPlanByCode(planCode, "tal")).Returns(() => null);

            var svc = GetService();

            svc.GetFor(planCode, "tal", 100000);
        }

        [Test, ExpectedException(typeof(ApplicationException), MatchType = MessageMatch.StartsWith, ExpectedMessage = "No CoverAmountPercentageDefinition for plan")]
        public void GetFor_NoAgeRangeCoverAmountDefinitionsForPlanDefinition_ExceptionThrown()
        {
            var planCode = "TEST";

            var planDefinitions = new PlanDefinition
            {
                Code = planCode,
                CoverAmountPercentageDefinition = null
            };

            _mockPlanDefinitionProvider.Setup(call => call.GetPlanByCode(planCode, "tal")).Returns(planDefinitions);

            var svc = GetService();

            svc.GetFor(planCode, "tal", 100000);
        }

        [Test]
        public void GetFor_PercentageOfIncomeGreaterThanMaxCoverAmount_MaxCoverAmountReturned()
        {
            var planCode = "TEST";

            var planDefinitions = new PlanDefinition
            {
                Code = planCode,
                CoverAmountPercentageDefinition = new CoverAmountPercentageDefinition(100, 9000)
            };

            _mockPlanDefinitionProvider.Setup(call => call.GetPlanByCode(planCode, "tal")).Returns(planDefinitions);

            var svc = GetService();

            var result = svc.GetFor(planCode, "tal", 120000);

            Assert.That(result, Is.EqualTo(9000));
        }

        [Test]
        public void GetFor_PercentageOfIncomeLessThanMaxCoverAmount_PercentageOfIncomeReturned()
        {
            var planCode = "TEST";

            var planDefinitions = new PlanDefinition
            {
                Code = planCode,
                CoverAmountPercentageDefinition = new CoverAmountPercentageDefinition(100, 11000)
            };

            _mockPlanDefinitionProvider.Setup(call => call.GetPlanByCode(planCode, "tal")).Returns(planDefinitions);

            var svc = GetService();

            var result = svc.GetFor(planCode, "tal", 120000);

            Assert.That(result, Is.EqualTo(10000));
        }

        private MaxCoverAmountPercentageOfIncomeProvider GetService()
        {
            return new MaxCoverAmountPercentageOfIncomeProvider(_mockPlanDefinitionProvider.Object);
        }
    }
}
