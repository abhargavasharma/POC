using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.Product.Models;
using TAL.QuoteAndApply.Product.Models.Definition;
using TAL.QuoteAndApply.Product.Service;
using TAL.QuoteAndApply.Rules;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Product.Models.Converters;
using TAL.QuoteAndApply.ServiceLayer.Product.Rules;

namespace TAL.QuoteAndApply.ServiceLayer.UnitTests.Policy.Plan
{
    [TestFixture]
    public class PlanEligibilityServiceTests
    {
        private Mock<IPlanEligibilityRulesFactory> _mockPlanEligibilityRulesFactory;
        private Mock<IPlanDefinitionProvider> _mockPlanDefinitionProvider;
        private Mock<IProductErrorMessageService> _mockProductErrorMessagesService;
        private Mock<IPlanMaxEntryAgeNextBirthdayProvider> _mockPlanMaxEntryAgeNextBirthdayProvider;
        private MockRepository _mockRepo;
        private PlanDefinition _mockPlanDefinition;
        private PlanDto _mockPlan;
        private RiskDto _mockRisk;
        private Mock<IMaxCoverAmountParamConverter> _mockMaxCoverAmountParamConverter;
        private Mock<ICoverAmountService> _mockCoverAmountService;
        private Mock<IProductBrandProvider> _mockProductBrandProvider;

        [TestFixtureSetUp]
        public void Setup()
        {
            _mockRepo = new MockRepository(MockBehavior.Strict);
            _mockPlanEligibilityRulesFactory = _mockRepo.Create<IPlanEligibilityRulesFactory>();
            _mockPlanDefinitionProvider = _mockRepo.Create<IPlanDefinitionProvider>();
            _mockProductErrorMessagesService = _mockRepo.Create<IProductErrorMessageService>();
            _mockPlanMaxEntryAgeNextBirthdayProvider = _mockRepo.Create<IPlanMaxEntryAgeNextBirthdayProvider>();
            _mockProductBrandProvider = _mockRepo.Create<IProductBrandProvider>();

            _mockMaxCoverAmountParamConverter = _mockRepo.Create<IMaxCoverAmountParamConverter>();
            _mockCoverAmountService = _mockRepo.Create<ICoverAmountService>();
            

            _mockRisk = new RiskDto
            {
                DateOfBirth = DateTime.Today.AddYears(-30),
                OccupationClass = "AAA",
                AnnualIncome = 100000
            };
            _mockPlan = new PlanDto
            {
                Code = "TEST",
                CoverAmount = 100000
            };

            _mockPlanDefinition = new PlanDefinition
            {
                MinimumEntryAgeNextBirthday = 18,
                MaximumEntryAgeNextBirthday = 60
            };
        }

        [Test]
        public void IsRiskEligibleForPlan_RiskPassedAllRules_ReturnsTrue()
        {
            var passingDateRule = _mockRepo.Create<IRule<DateTime>>();
            passingDateRule.Setup(call => call.IsSatisfiedBy(_mockRisk.DateOfBirth)).Returns(new RuleResult(true));

            var passingIntRule = _mockRepo.Create<IRule<int>>();
            passingIntRule.Setup(call => call.IsSatisfiedBy(It.IsAny<int>())).Returns(new RuleResult(true));

            var maxAge = 100;

            _mockPlanDefinitionProvider.Setup(call => call.GetPlanByCode(_mockPlan.Code, "tal")).Returns(_mockPlanDefinition);
            _mockPlanEligibilityRulesFactory.Setup(
                call => call.GetMinmumAgeRule(_mockPlanDefinition.MinimumEntryAgeNextBirthday)).Returns(passingDateRule.Object);

            _mockPlanMaxEntryAgeNextBirthdayProvider.Setup(
                call => call.GetMaxAgeFrom(_mockPlanDefinition, _mockRisk.OccupationClass)).Returns(maxAge);
            _mockPlanEligibilityRulesFactory.Setup(
                call => call.GetMaximumAgeRule(maxAge)).Returns(passingDateRule.Object);
            _mockProductErrorMessagesService.Setup(call => call.GetPlanHasNoValidOptionsErrorMessage()).Returns("No valid cover options");
            
            var mockMaxCoverAmountParam = new MaxCoverAmountParam(_mockPlan.Code, "tal", _mockRisk.DateOfBirth.Age(), _mockRisk.AnnualIncome, _mockPlan.CoverAmount, null);
            _mockMaxCoverAmountParamConverter.Setup(call => call.CreateFrom(_mockRisk, _mockPlan, null, "tal"))
                .Returns(mockMaxCoverAmountParam);

            var maxCoverAmount = 200000;

            _mockCoverAmountService.Setup(call => call.GetMaxCover(mockMaxCoverAmountParam)).Returns(maxCoverAmount);
            _mockCoverAmountService.Setup(call => call.GetMinCover(_mockPlan.Code, "tal")).Returns(100000);
            _mockPlanEligibilityRulesFactory.Setup(
               call => call.GetMaxCoverAmountMustBeOverMinCoverAmountRule(_mockPlanDefinition)).Returns(passingIntRule.Object);

            var svc = GetService();
            var result = svc.IsRiskEligibleForPlan(_mockRisk, _mockPlan, ValidCovers());

            Assert.That(result.IsAvailable, Is.True);
        }

        [Test]
        public void IsRiskEligibleForPlan_FailingMinAgeRule_ReturnsFalse()
        {
            var failingRule = _mockRepo.Create<IRule<DateTime>>();
            failingRule.Setup(call => call.IsSatisfiedBy(_mockRisk.DateOfBirth)).Returns(new RuleResult(false));

            _mockPlanDefinitionProvider.Setup(call => call.GetPlanByCode(_mockPlan.Code, "tal")).Returns(_mockPlanDefinition);
            _mockPlanEligibilityRulesFactory.Setup(
                call => call.GetMinmumAgeRule(_mockPlanDefinition.MinimumEntryAgeNextBirthday)).Returns(failingRule.Object);
            _mockProductErrorMessagesService.Setup(call => call.GetMinimumAgeErrorMessage(It.IsAny<int>())).Returns("Too young yo");
            _mockProductBrandProvider.Setup(call => call.GetBrandKeyForRisk(_mockRisk)).Returns("tal");

            var svc = GetService();
            var result = svc.IsRiskEligibleForPlan(_mockRisk, _mockPlan, ValidCovers());

            Assert.That(result.IsAvailable, Is.False);
        }

        [Test]
        public void IsRiskEligibleForPlan_FailingMaxAgeRule_ReturnsFalse()
        {
            var passingRule = _mockRepo.Create<IRule<DateTime>>();
            passingRule.Setup(call => call.IsSatisfiedBy(_mockRisk.DateOfBirth)).Returns(new RuleResult(true));
            
            var failingRule = _mockRepo.Create<IRule<DateTime>>();
            failingRule.Setup(call => call.IsSatisfiedBy(_mockRisk.DateOfBirth)).Returns(new RuleResult(false));

            var maxAge = 100;

            _mockPlanDefinitionProvider.Setup(call => call.GetPlanByCode(_mockPlan.Code, "tal")).Returns(_mockPlanDefinition);
            _mockPlanEligibilityRulesFactory.Setup(
                call => call.GetMinmumAgeRule(_mockPlanDefinition.MinimumEntryAgeNextBirthday)).Returns(passingRule.Object);

            _mockPlanMaxEntryAgeNextBirthdayProvider.Setup(
                call => call.GetMaxAgeFrom(_mockPlanDefinition, _mockRisk.OccupationClass)).Returns(maxAge);
            _mockPlanEligibilityRulesFactory.Setup(
                call => call.GetMaximumAgeRule(maxAge)).Returns(failingRule.Object);
            _mockProductErrorMessagesService.Setup(call => call.GetMaximumAgeErrorMessage(It.IsAny<int>())).Returns("Too old yo");
            _mockProductBrandProvider.Setup(call => call.GetBrandKeyForRisk(_mockRisk)).Returns("tal");

            var svc = GetService();
            var result = svc.IsRiskEligibleForPlan(_mockRisk, _mockPlan, ValidCovers());

            Assert.That(result.IsAvailable, Is.False);
        }


        [Test]
        public void IsRiskEligibleForPlan_AllCoversAreIneligible_ReturnsFalse()
        {
            var passingRule = _mockRepo.Create<IRule<DateTime>>();
            passingRule.Setup(call => call.IsSatisfiedBy(_mockRisk.DateOfBirth)).Returns(new RuleResult(true));


            var failingRule = _mockRepo.Create<IRule<DateTime>>();
            failingRule.Setup(call => call.IsSatisfiedBy(_mockRisk.DateOfBirth)).Returns(new RuleResult(false));

            _mockPlanDefinitionProvider.Setup(call => call.GetPlanByCode(_mockPlan.Code, "tal")).Returns(_mockPlanDefinition);
            _mockProductErrorMessagesService.Setup(call => call.GetPlanHasNoValidOptionsErrorMessage()).Returns("No valid cover options");
            _mockProductBrandProvider.Setup(call => call.GetBrandKeyForRisk(_mockRisk)).Returns("tal");

            var coverEligibility = new List<CoverEligibilityResult>
            {
                CoverEligibilityResult.Ineligible("COVER1", "I'm invalid yo"),
                CoverEligibilityResult.Ineligible("COVER2", "I'm invalid as well yo")
            };

            var svc = GetService();
            var result = svc.IsRiskEligibleForPlan(_mockRisk, _mockPlan, coverEligibility);

            Assert.That(result.IsAvailable, Is.False);
            Assert.That(result.ReasonIfUnavailable.Any());
            Assert.That(result.ReasonIfUnavailable.First(), Is.EqualTo("No valid cover options"));
        }

        [Test]
        public void IsRiskEligibleForPlan_SomeCoversAreIneligible_ReturnsAvailableResult()
        {
            var passingRule = _mockRepo.Create<IRule<DateTime>>();
            passingRule.Setup(call => call.IsSatisfiedBy(_mockRisk.DateOfBirth)).Returns(new RuleResult(true));
            
            var failingRule = _mockRepo.Create<IRule<DateTime>>();
            failingRule.Setup(call => call.IsSatisfiedBy(_mockRisk.DateOfBirth)).Returns(new RuleResult(false));

            var passingIntRule = _mockRepo.Create<IRule<int>>();
            passingIntRule.Setup(call => call.IsSatisfiedBy(It.IsAny<int>())).Returns(new RuleResult(true));

            _mockPlanDefinitionProvider.Setup(call => call.GetPlanByCode(_mockPlan.Code, "tal")).Returns(_mockPlanDefinition);
            _mockPlanEligibilityRulesFactory.Setup(
                call => call.GetMinmumAgeRule(_mockPlanDefinition.MinimumEntryAgeNextBirthday))
                .Returns(passingRule.Object);
            _mockPlanEligibilityRulesFactory.Setup(
                call => call.GetMaximumAgeRule(_mockPlanDefinition.MaximumEntryAgeNextBirthday))
                .Returns(passingRule.Object);
            _mockProductBrandProvider.Setup(call => call.GetBrandKeyForRisk(_mockRisk)).Returns("tal");

            _mockProductErrorMessagesService.Setup(call => call.GetPlanHasNoValidOptionsErrorMessage())
                .Returns("No valid cover options");

            var coverEligibility = new List<CoverEligibilityResult>
            {
                CoverEligibilityResult.Eligible("COVER1"),
                CoverEligibilityResult.Ineligible("COVER2", "I'm invalid yo")
            };

            var mockMaxCoverAmountParam = new MaxCoverAmountParam(_mockPlan.Code, "tal", _mockRisk.DateOfBirth.Age(), _mockRisk.AnnualIncome, _mockPlan.CoverAmount, null);
            _mockMaxCoverAmountParamConverter.Setup(call => call.CreateFrom(_mockRisk, _mockPlan, null, "tal"))
                .Returns(mockMaxCoverAmountParam);

            var maxCoverAmount = 200000;

            _mockCoverAmountService.Setup(call => call.GetMaxCover(mockMaxCoverAmountParam)).Returns(maxCoverAmount);
            _mockCoverAmountService.Setup(call => call.GetMinCover(_mockPlan.Code, "tal")).Returns(100000);
            _mockPlanEligibilityRulesFactory.Setup(
               call => call.GetMaxCoverAmountMustBeOverMinCoverAmountRule(_mockPlanDefinition)).Returns(passingIntRule.Object);

            var svc = GetService();
            var result = svc.IsRiskEligibleForPlan(_mockRisk, _mockPlan, coverEligibility);

            Assert.That(result.IsAvailable);
        }

        [Test]
        public void IsRiskEligibleForPlan_PlanMinCoverMoreThanMax_ReturnsFalse()
        {
            var passingDateRule = _mockRepo.Create<IRule<DateTime>>();
            passingDateRule.Setup(call => call.IsSatisfiedBy(_mockRisk.DateOfBirth)).Returns(new RuleResult(true));

            var failingIntRule = _mockRepo.Create<IRule<int>>();
            failingIntRule.Setup(call => call.IsSatisfiedBy(It.IsAny<int>())).Returns(new RuleResult(false));

            var maxAge = 100;

            _mockPlanDefinitionProvider.Setup(call => call.GetPlanByCode(_mockPlan.Code, "tal")).Returns(_mockPlanDefinition);
            _mockPlanEligibilityRulesFactory.Setup(
                call => call.GetMinmumAgeRule(_mockPlanDefinition.MinimumEntryAgeNextBirthday)).Returns(passingDateRule.Object);

            _mockPlanMaxEntryAgeNextBirthdayProvider.Setup(
                call => call.GetMaxAgeFrom(_mockPlanDefinition, _mockRisk.OccupationClass)).Returns(maxAge);
            _mockPlanEligibilityRulesFactory.Setup(
                call => call.GetMaximumAgeRule(maxAge)).Returns(passingDateRule.Object);
            _mockProductErrorMessagesService.Setup(call => call.GetPlanHasNoValidOptionsErrorMessage()).Returns("No valid cover options");

            var mockMaxCoverAmountParam = new MaxCoverAmountParam(_mockPlan.Code, "tal", _mockRisk.DateOfBirth.Age(), _mockRisk.AnnualIncome, _mockPlan.CoverAmount, null);
            _mockMaxCoverAmountParamConverter.Setup(call => call.CreateFrom(_mockRisk, _mockPlan, null, "tal"))
                .Returns(mockMaxCoverAmountParam);

            var maxCoverAmount = 200000;
            var minCoverAmount = 100000;

            _mockCoverAmountService.Setup(call => call.GetMaxCover(mockMaxCoverAmountParam)).Returns(maxCoverAmount);
            _mockCoverAmountService.Setup(call => call.GetMinCover(_mockPlan.Code, "tal")).Returns(minCoverAmount);
            _mockPlanEligibilityRulesFactory.Setup(
               call => call.GetMaxCoverAmountMustBeOverMinCoverAmountRule(_mockPlanDefinition)).Returns(failingIntRule.Object);

            _mockProductErrorMessagesService.Setup(call => call.GetMinGreaterThanMaxCoverAmountErrorMessage(minCoverAmount, maxCoverAmount)).Returns("Min less than Max");

            var svc = GetService();
            var result = svc.IsRiskEligibleForPlan(_mockRisk, _mockPlan, ValidCovers());

            Assert.That(result.IsAvailable, Is.False);
            Assert.That(result.ReasonIfUnavailable.Any());
            Assert.That(result.ReasonIfUnavailable.First(), Is.EqualTo("Min less than Max"));
        }

        private IEnumerable<CoverEligibilityResult> ValidCovers()
        {
            yield return CoverEligibilityResult.Eligible("COVER1");
        }


        private PlanEligibilityService GetService()
        {
            return new PlanEligibilityService(_mockPlanEligibilityRulesFactory.Object, _mockPlanDefinitionProvider.Object, 
                _mockProductErrorMessagesService.Object, _mockPlanMaxEntryAgeNextBirthdayProvider.Object, 
                _mockMaxCoverAmountParamConverter.Object, _mockCoverAmountService.Object, _mockProductBrandProvider.Object);
        }
    }
}
