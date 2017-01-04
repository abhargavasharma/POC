using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Rules.Cover;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Product.Service;
using TAL.QuoteAndApply.Rules;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.ServiceLayer.Validation;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.Policy.Rules.Plan;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.Product.Models.Definition;
using TAL.QuoteAndApply.Product.Rules;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Product.Rules;

namespace TAL.QuoteAndApply.ServiceLayer.UnitTests.Policy.Plan
{
    [TestFixture]
    public class PlanRulesServiceTests
    {
        private Mock<IPlanService> _mockPlanService;
        private Mock<ICoverService> _mockCoverService;
        private Mock<ICoverRulesFactory> _mockCoverRulesFactory;
        private Mock<IPlanErrorMessageService> _mockPlanErrorMessageService;
        private Mock<INameLookupService> _mockNameLookupService;
        private Mock<IRule<ICover>> _mockCoverRule;

        private Mock<IProductDefinitionProvider> _mockProductDefinitionProvider;
        private Mock<IRiskService> _mockRiskService;
        private Mock<IRuleFactory> _mockRuleFactory;
        private Mock<IRule<DateTime>> _mockDobRule;
        private Mock<IProductErrorMessageService> _mockProductErrorMessageService;
        private Mock<IPlanDefinitionProvider> _mockPlanDefinitionProvider;
        private Mock<IPlanRulesFactory> _mockPlanRuleFactory;
        private Mock<IRule<decimal>> _mockCoverAmountRule;
        private Mock<IRule<IEnumerable<string>>> _mockPlanSelectedCoversRules;

        const string PlanCode = "PLAN";
        const string CoverCode = "COVER";
        const string CoverName = "COVER NAME";
        const string BrandKey = "TAL";

        [TestFixtureSetUp]
        public void Setup()
        {
            var mockRepo = new MockRepository(MockBehavior.Strict);

            _mockPlanService = mockRepo.Create<IPlanService>();
            _mockCoverService = mockRepo.Create<ICoverService>();
            _mockCoverRulesFactory = mockRepo.Create<ICoverRulesFactory>();
            _mockPlanErrorMessageService = mockRepo.Create<IPlanErrorMessageService>();
            _mockProductErrorMessageService = mockRepo.Create<IProductErrorMessageService>();
            _mockNameLookupService = mockRepo.Create<INameLookupService>();

            _mockRiskService = mockRepo.Create<IRiskService>();
            _mockProductDefinitionProvider = mockRepo.Create<IProductDefinitionProvider>();
            _mockRuleFactory = mockRepo.Create<IRuleFactory>();
            
            _mockDobRule = mockRepo.Create<IRule<DateTime>>();
            _mockCoverRule = mockRepo.Create<IRule<ICover>>();
            _mockCoverAmountRule = mockRepo.Create<IRule<decimal>>();
            _mockPlanSelectedCoversRules = mockRepo.Create<IRule<IEnumerable<string>>>();

            _mockPlanDefinitionProvider = mockRepo.Create<IPlanDefinitionProvider>();
            _mockPlanRuleFactory = mockRepo.Create<IPlanRulesFactory>();
            _mockPlanDefinitionProvider.Setup(call => call.GetPlanByCode(PlanCode, BrandKey)).Returns(new PlanDefinition
            {
                Variables = new List<PlanVariablesDefinition>()
            });
        }

        [Test]
        public void ValidatePlanOptions_PlanPassedRule_CoverPassedRule_NoValidationErrors()
        {
            var mockPlanOptionParam = PlanStateParam.BuildPlanStateParam(PlanCode, BrandKey, true, 1, 1, false, 100000, false, PremiumType.Level,
                0, 30, 100000, null, null, OccupationDefinition.Unknown, new List<PlanStateParam>(), new List<OptionsParam>(), new[] { new PlanIdentityInfo() {PlanCode = PlanCode} }, new[] {CoverCode});
            
            var mockPlan = new PlanDto() {Id = 1234, Code = PlanCode, RiskId = 99};
            var mockRisk = new RiskDto {DateOfBirth = DateTime.Now.AddYears(-30)};
            var mockCover = new CoverDto {Id = 987, Code = CoverCode };
            var mockPremiumTypeDefinition = new PremiumTypeDefinition(PremiumType.Level, "Level", 60);

            _mockRiskService.Setup(call => call.GetRisk(mockPlan.RiskId)).Returns(mockRisk);
            _mockProductDefinitionProvider.Setup(call => call.GetPremiumTypeDefinition(mockPlanOptionParam.PremiumType, BrandKey))
                .Returns(mockPremiumTypeDefinition);

            _mockDobRule.Setup(call => call.IsSatisfiedBy(mockRisk.DateOfBirth)).Returns(new RuleResult(true));
            _mockRuleFactory.Setup(call => call.GetEligibleForPremiumTypeRules(mockPremiumTypeDefinition)).Returns(new [] {_mockDobRule.Object});

            _mockCoverAmountRule.Setup(call=> call.IsSatisfiedBy(mockPlanOptionParam.CoverAmount)).Returns(new RuleResult(true));
            _mockRuleFactory.Setup(call => call.GetMustHaveCoverAmountRule()).Returns(_mockCoverAmountRule.Object);

            _mockPlanSelectedCoversRules.Setup(call => call.IsSatisfiedBy(mockPlanOptionParam.SelectedCoverCodes))
                .Returns(new RuleResult(true));
            _mockRuleFactory.Setup(call => call.GetSelectedPlanMustHaveAtLeastCoverSelected()).Returns(_mockPlanSelectedCoversRules.Object);

            //passing rule
            _mockCoverRule.Setup(call => call.IsSatisfiedBy(mockCover)).Returns(new RuleResult(true));

            _mockPlanService.Setup(
                call => call.GetByRiskIdAndPlanCode(mockPlanOptionParam.RiskId, mockPlanOptionParam.PlanCode))
                .Returns(mockPlan);

            _mockCoverRulesFactory.Setup(call => call.GetSelectedCoverNotUnderwritingDeclinedRule())
                .Returns(_mockCoverRule.Object);

            _mockCoverService.Setup(call => call.GetCoversForPlan(mockPlan.Id)).Returns(new [] { mockCover });

            var result = GetService().ValidatePlanStateParam(mockPlanOptionParam);

            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public void ValidatePlanOptions_PlanFailedRule_CoverPassedRule_PlanValidationErrors()
        {

            var mockPlanOptionParam = PlanStateParam.BuildPlanStateParam(PlanCode, BrandKey, true, 1, 1, false, 100000, false, PremiumType.Level,
                0, 30, 100000, null, null, OccupationDefinition.Unknown, new List<PlanStateParam>(), new List<OptionsParam>(), new[] { new PlanIdentityInfo() { PlanCode = PlanCode } }, new[] {CoverCode});

            var mockPlan = new PlanDto() { Id = 1234, Code = PlanCode, RiskId = 99 };
            var mockRisk = new RiskDto { DateOfBirth = DateTime.Now.AddYears(-30) };
            var mockCover = new CoverDto { Id = 987, Code = CoverCode };
            var mockPremiumTypeDefinition = new PremiumTypeDefinition(PremiumType.Level, "Level", 60);

            _mockRiskService.Setup(call => call.GetRisk(mockPlan.RiskId)).Returns(mockRisk);
            _mockProductDefinitionProvider.Setup(call => call.GetPremiumTypeDefinition(mockPlanOptionParam.PremiumType, BrandKey))
                .Returns(mockPremiumTypeDefinition);

            _mockDobRule.Setup(call => call.IsSatisfiedBy(mockRisk.DateOfBirth)).Returns(new RuleResult(false));

            _mockRuleFactory.Setup(call => call.GetEligibleForPremiumTypeRules(mockPremiumTypeDefinition)).Returns(new[] { _mockDobRule.Object });
            
            var premiumTypeErrorMessage = "BLAH BLAH";
            _mockPlanErrorMessageService.Setup(
                call =>
                    call.GetPremiumTypeNotAvailableMessage(mockPremiumTypeDefinition.PremiumType,
                        mockPremiumTypeDefinition.MaximumEntryAgeNextBirthday.Value)).Returns(premiumTypeErrorMessage);

            _mockCoverAmountRule.Setup(call => call.IsSatisfiedBy(mockPlanOptionParam.CoverAmount)).Returns(new RuleResult(true));
            _mockRuleFactory.Setup(call => call.GetMustHaveCoverAmountRule()).Returns(_mockCoverAmountRule.Object);

            _mockPlanSelectedCoversRules.Setup(call => call.IsSatisfiedBy(mockPlanOptionParam.SelectedCoverCodes))
                .Returns(new RuleResult(true));
            _mockRuleFactory.Setup(call => call.GetSelectedPlanMustHaveAtLeastCoverSelected()).Returns(_mockPlanSelectedCoversRules.Object);

            //passing rule
            _mockCoverRule.Setup(call => call.IsSatisfiedBy(mockCover)).Returns(new RuleResult(true));

            _mockPlanService.Setup(
                call => call.GetByRiskIdAndPlanCode(mockPlanOptionParam.RiskId, mockPlanOptionParam.PlanCode))
                .Returns(mockPlan);

            _mockCoverRulesFactory.Setup(call => call.GetSelectedCoverNotUnderwritingDeclinedRule())
                .Returns(_mockCoverRule.Object);

            _mockCoverService.Setup(call => call.GetCoversForPlan(mockPlan.Id)).Returns(new[] { mockCover });

            var result = GetService().ValidatePlanStateParam(mockPlanOptionParam);

            var validationError = result.ToArray()[0];
            Assert.That(validationError.Key, Is.EqualTo(ValidationKey.EligiblePremiumType));
            Assert.That(validationError.Type, Is.EqualTo(ValidationType.Error));
            Assert.That(validationError.Message, Is.EqualTo(premiumTypeErrorMessage));
        }

        [Test]
        public void ValidatePlanOptions_PlanPassedRule_CoverFailsRule_ValidationErrorReturned()
        {
           
            const string errorMessageCode = "ERROR CODE";
            const string errorMessage = "ERROR MSG";

            var mockPlanOptionParam = PlanStateParam.BuildPlanStateParam(PlanCode, BrandKey, true, 1, 1, false, 100000, false, PremiumType.Level,
                0, 30, 100000, null, null, OccupationDefinition.Unknown, new List<PlanStateParam>(), new List<OptionsParam>(), new[] { new PlanIdentityInfo() { PlanCode = PlanCode } }, new[] {CoverCode});
            

            var mockPlan = new PlanDto() { Id = 1234, Code = PlanCode, RiskId = 99 };
            var mockRisk = new RiskDto { DateOfBirth = DateTime.Now.AddYears(-30) };
            var mockCover = new CoverDto { Id = 987, Code = CoverCode };
            var mockPremiumTypeDefinition = new PremiumTypeDefinition(PremiumType.Level, "Level", 60);

            _mockRiskService.Setup(call => call.GetRisk(mockPlan.RiskId)).Returns(mockRisk);
            _mockProductDefinitionProvider.Setup(call => call.GetPremiumTypeDefinition(mockPlanOptionParam.PremiumType, BrandKey))
                .Returns(mockPremiumTypeDefinition);

            _mockDobRule.Setup(call => call.IsSatisfiedBy(mockRisk.DateOfBirth)).Returns(new RuleResult(true));

            _mockRuleFactory.Setup(call => call.GetEligibleForPremiumTypeRules(mockPremiumTypeDefinition)).Returns(new[] { _mockDobRule.Object });

            _mockCoverAmountRule.Setup(call => call.IsSatisfiedBy(mockPlanOptionParam.CoverAmount)).Returns(new RuleResult(true));
            _mockRuleFactory.Setup(call => call.GetMustHaveCoverAmountRule()).Returns(_mockCoverAmountRule.Object);

            _mockPlanSelectedCoversRules.Setup(call => call.IsSatisfiedBy(mockPlanOptionParam.SelectedCoverCodes))
                .Returns(new RuleResult(true));
            _mockRuleFactory.Setup(call => call.GetSelectedPlanMustHaveAtLeastCoverSelected()).Returns(_mockPlanSelectedCoversRules.Object);

            _mockCoverRule.Setup(call => call.IsSatisfiedBy(mockCover)).Returns(new RuleResult(false));

            _mockPlanService.Setup(
                call => call.GetByRiskIdAndPlanCode(mockPlanOptionParam.RiskId, mockPlanOptionParam.PlanCode))
                .Returns(mockPlan);

            _mockCoverRulesFactory.Setup(call => call.GetSelectedCoverNotUnderwritingDeclinedRule())
                .Returns(_mockCoverRule.Object);

            _mockCoverService.Setup(call => call.GetCoversForPlan(mockPlan.Id)).Returns(new[] { mockCover });

            _mockNameLookupService.Setup(call => call.GetCoverName(mockPlan.Code, mockCover.Code, BrandKey)).Returns(CoverName);

            _mockPlanErrorMessageService.Setup(
                call => call.GetCoverErrorMessageCode(mockPlan.Code, mockCover.Code, CoverName))
                .Returns(errorMessageCode);

            _mockPlanErrorMessageService.Setup(
                call => call.GetSelectedCoverUndwritingDeclinedMessage())
                .Returns(errorMessage);

            var result = GetService().ValidatePlanStateParam(mockPlanOptionParam);

            var validationError = result.ToArray()[0];
            Assert.That(validationError.Key, Is.EqualTo(ValidationKey.SelectedCoverUnderwritingDeclined));
            Assert.That(validationError.Type, Is.EqualTo(ValidationType.Error));
            Assert.That(validationError.Code, Is.EqualTo(errorMessageCode));
            Assert.That(validationError.Message, Is.EqualTo(errorMessage));

        }

        [Test]
        public void ValidatePlanOptions_PlanFailedRule_CoverFailsRule_ValidationErrorReturned()
        {
            

            const string errorMessageCode = "ERROR CODE";
            const string errorMessage = "ERROR MSG";

            var mockPlanOptionParam = PlanStateParam.BuildPlanStateParam(PlanCode, BrandKey, true, 1, 1, false, 100000, false, PremiumType.Level,
                0, 30, 100000, null, null, OccupationDefinition.Unknown, new List<PlanStateParam>(), new List<OptionsParam>(), new[] { new PlanIdentityInfo() { PlanCode = PlanCode } }, new[] {CoverCode});
            
            var mockPlan = new PlanDto() { Id = 1234, Code = PlanCode, RiskId = 99};
            var mockRisk = new RiskDto { DateOfBirth = DateTime.Now.AddYears(-30) };
            var mockCover = new CoverDto { Id = 987, Code = CoverCode };
            var mockPremiumTypeDefinition = new PremiumTypeDefinition(PremiumType.Level, "Level", 60);

            _mockRiskService.Setup(call => call.GetRisk(mockPlan.RiskId)).Returns(mockRisk);
            _mockProductDefinitionProvider.Setup(call => call.GetPremiumTypeDefinition(mockPlanOptionParam.PremiumType, BrandKey))
                .Returns(mockPremiumTypeDefinition);

            _mockDobRule.Setup(call => call.IsSatisfiedBy(mockRisk.DateOfBirth)).Returns(new RuleResult(false));

            _mockRuleFactory.Setup(call => call.GetEligibleForPremiumTypeRules(mockPremiumTypeDefinition)).Returns(new[] { _mockDobRule.Object });

            var premiumTypeErrorMessage = "BLAH BLAH";
            _mockPlanErrorMessageService.Setup(
                call =>
                    call.GetPremiumTypeNotAvailableMessage(mockPremiumTypeDefinition.PremiumType,
                        mockPremiumTypeDefinition.MaximumEntryAgeNextBirthday.Value)).Returns(premiumTypeErrorMessage);

            _mockCoverAmountRule.Setup(call => call.IsSatisfiedBy(mockPlanOptionParam.CoverAmount)).Returns(new RuleResult(true));
            _mockRuleFactory.Setup(call => call.GetMustHaveCoverAmountRule()).Returns(_mockCoverAmountRule.Object);

            _mockCoverAmountRule.Setup(call => call.IsSatisfiedBy(mockPlanOptionParam.CoverAmount)).Returns(new RuleResult(true));
            _mockRuleFactory.Setup(call => call.GetMustHaveCoverAmountRule()).Returns(_mockCoverAmountRule.Object);

            _mockPlanSelectedCoversRules.Setup(call => call.IsSatisfiedBy(mockPlanOptionParam.SelectedCoverCodes))
                .Returns(new RuleResult(true));
            _mockRuleFactory.Setup(call => call.GetSelectedPlanMustHaveAtLeastCoverSelected()).Returns(_mockPlanSelectedCoversRules.Object);

            _mockCoverRule.Setup(call => call.IsSatisfiedBy(mockCover)).Returns(new RuleResult(false));

            _mockPlanService.Setup(
                call => call.GetByRiskIdAndPlanCode(mockPlanOptionParam.RiskId, mockPlanOptionParam.PlanCode))
                .Returns(mockPlan);

            _mockCoverRulesFactory.Setup(call => call.GetSelectedCoverNotUnderwritingDeclinedRule())
                .Returns(_mockCoverRule.Object);

            _mockCoverService.Setup(call => call.GetCoversForPlan(mockPlan.Id)).Returns(new[] { mockCover });

            _mockNameLookupService.Setup(call => call.GetCoverName(mockPlan.Code, mockCover.Code, BrandKey)).Returns(CoverName);

            _mockPlanErrorMessageService.Setup(
                call => call.GetCoverErrorMessageCode(mockPlan.Code, mockCover.Code, CoverName))
                .Returns(errorMessageCode);

            _mockPlanErrorMessageService.Setup(
                call => call.GetSelectedCoverUndwritingDeclinedMessage())
                .Returns(errorMessage);

            var result = GetService().ValidatePlanStateParam(mockPlanOptionParam);

            var validationError = result.ToArray()[0];
            Assert.That(validationError.Key, Is.EqualTo(ValidationKey.EligiblePremiumType));
            Assert.That(validationError.Type, Is.EqualTo(ValidationType.Error));
            Assert.That(validationError.Message, Is.EqualTo(premiumTypeErrorMessage));

            validationError = result.ToArray()[1];
            Assert.That(validationError.Key, Is.EqualTo(ValidationKey.SelectedCoverUnderwritingDeclined));
            Assert.That(validationError.Type, Is.EqualTo(ValidationType.Error));
            Assert.That(validationError.Code, Is.EqualTo(errorMessageCode));
            Assert.That(validationError.Message, Is.EqualTo(errorMessage));

        }

        private PlanRulesService GetService()
        {
            return new PlanRulesService(_mockPlanService.Object, _mockCoverService.Object, 
                _mockCoverRulesFactory.Object, _mockPlanErrorMessageService.Object,
                _mockNameLookupService.Object, _mockRuleFactory.Object, _mockProductDefinitionProvider.Object, 
                _mockRiskService.Object, _mockProductErrorMessageService.Object, _mockPlanDefinitionProvider.Object, _mockPlanRuleFactory.Object);
        }
    }
}
