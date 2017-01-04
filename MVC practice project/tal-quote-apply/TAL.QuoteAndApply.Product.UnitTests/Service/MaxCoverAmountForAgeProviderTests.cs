using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.Product.Models.Definition;
using TAL.QuoteAndApply.Product.Service;

namespace TAL.QuoteAndApply.Product.UnitTests.Service
{
    [TestFixture]
    public class MaxCoverAmountForAgeProviderTests
    {
        private Mock<IPlanDefinitionProvider> _mockPlanDefinitionProvider;

        [TestFixtureSetUp]
        public void Setup()
        {
            _mockPlanDefinitionProvider = new Mock<IPlanDefinitionProvider>(MockBehavior.Strict);
        }

        [Test, ExpectedException(typeof(ApplicationException), MatchType= MessageMatch.StartsWith, ExpectedMessage = "No PlanDefinition for plan")]
        public void GetFor_NoPlanDefinition_ExceptionThrown()
        {
            var planCode = "TEST";

            _mockPlanDefinitionProvider.Setup(call => call.GetPlanByCode(planCode, "tal")).Returns(() => null);

            var svc = GetService();

            svc.GetFor(planCode, "tal", 20, 100000);
        }

        [Test, ExpectedException(typeof(ApplicationException), MatchType = MessageMatch.StartsWith, ExpectedMessage = "No AgeRangeCoverAmountDefinition for plan")]
        public void GetFor_NoAgeRangeCoverAmountDefinitionsForPlanDefinition_ExceptionThrown()
        {
            var planCode = "TEST";

            var planDefinitions = new PlanDefinition
            {
                Code = planCode,
                AgeRangeCoverAmountDefinitions = null
            };

            _mockPlanDefinitionProvider.Setup(call => call.GetPlanByCode(planCode, "tal")).Returns(planDefinitions);

            var svc = GetService();

            svc.GetFor(planCode, "tal", 20, 100000);
        }

        [Test]
        public void GetFor_NoMatchingAgeRangeCoverAmountDefinitionsForPlanDefinition_MaxCoverOfZeroReturned()
        {
            var planCode = "TEST";
            int age = 25;

            var planDefinitions = new PlanDefinition
            {
                Code = planCode,
                AgeRangeCoverAmountDefinitions = new List<AgeRangeCoverAmountDefinition>
                {
                    new AgeRangeCoverAmountDefinition(new AgeRangeDefinition(0, 20), 10, 1000000, null)
                }
            };

            _mockPlanDefinitionProvider.Setup(call => call.GetPlanByCode(planCode, "tal")).Returns(planDefinitions);

            var svc = GetService();

            var result = svc.GetFor(planCode, "tal", age, 100000);

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void GetFor_NoAnnualIncomeFactorForMatchingAgeRangeCoverAmountDefinitions_MaxCoverAmountReturned()
        {
            var planCode = "TEST";
            int age = 18;

            var maxCoverAmount = 100000;

            var planDefinitions = new PlanDefinition
            {
                Code = planCode,
                AgeRangeCoverAmountDefinitions = new List<AgeRangeCoverAmountDefinition>
                {
                    new AgeRangeCoverAmountDefinition(new AgeRangeDefinition(0, 20), null, maxCoverAmount, null)
                }
            };

            _mockPlanDefinitionProvider.Setup(call => call.GetPlanByCode(planCode, "tal")).Returns(planDefinitions);

            var svc = GetService();

            var result = svc.GetFor(planCode, "tal", age, 100000);

            Assert.That(result, Is.EqualTo(maxCoverAmount));
        }

        [Test]
        public void GetFor_AnnualIncomeMultipledByAnnualIncomeFactorIsHigherThanMaxCoverAmount_MaxCoverAmountReturned()
        {
            var planCode = "TEST";
            int age = 18;

            var maxCoverAmount = 100000;
            var annualIncome = 100000;
            var annualIncomeFactor = 10;

            var planDefinitions = new PlanDefinition
            {
                Code = planCode,
                AgeRangeCoverAmountDefinitions = new List<AgeRangeCoverAmountDefinition>
                {
                    new AgeRangeCoverAmountDefinition(new AgeRangeDefinition(0, 20), annualIncomeFactor, maxCoverAmount, null)
                }
            };

            _mockPlanDefinitionProvider.Setup(call => call.GetPlanByCode(planCode, "tal")).Returns(planDefinitions);

            var svc = GetService();

            var result = svc.GetFor(planCode, "tal", age, annualIncome);

            Assert.That(result, Is.EqualTo(maxCoverAmount));
        }

        [Test]
        public void GetFor_AnnualIncomeMultipledByAnnualIncomeFactorIsLoverThanUnrestrictedAmount_UnrestrictedAmountReturned()
        {
            var planCode = "TEST";
            int age = 18;

            var maxCoverAmount = 500000;
            var unrestrictedAmount = 400000;
            var annualIncome = 50000;
            var annualIncomeFactor = 5;

            var planDefinitions = new PlanDefinition
            {
                Code = planCode,
                AgeRangeCoverAmountDefinitions = new List<AgeRangeCoverAmountDefinition>
                {
                    new AgeRangeCoverAmountDefinition(new AgeRangeDefinition(0, 20), annualIncomeFactor, maxCoverAmount, unrestrictedAmount)
                }
            };

            _mockPlanDefinitionProvider.Setup(call => call.GetPlanByCode(planCode, "tal")).Returns(planDefinitions);

            var svc = GetService();

            var result = svc.GetFor(planCode, "tal", age, annualIncome);

            Assert.That(result, Is.EqualTo(unrestrictedAmount));
        }

        [Test]
        public void GetFor_AnnualIncomeMultipledByAnnualIncomeFactorIsLowerThanMaxCoverAmount_AnnualIncomeMultipledByAnnualIncomeFactorIsReturned()
        {
            var planCode = "TEST";
            int age = 18;

            var maxCoverAmount = 2500001;
            var unrestrictedAmount = 110000;
            var annualIncome = 100000;
            var annualIncomeFactor = 25;

            var planDefinitions = new PlanDefinition
            {
                Code = planCode,
                AgeRangeCoverAmountDefinitions = new List<AgeRangeCoverAmountDefinition>
                {
                    new AgeRangeCoverAmountDefinition(new AgeRangeDefinition(0, 20), annualIncomeFactor, maxCoverAmount, unrestrictedAmount)
                }
            };

            _mockPlanDefinitionProvider.Setup(call => call.GetPlanByCode(planCode, "tal")).Returns(planDefinitions);

            var svc = GetService();

            var result = svc.GetFor(planCode, "tal", age, annualIncome);

            Assert.That(result, Is.EqualTo(2500000));
        }

        private MaxCoverAmountForAgeProvider GetService()
        {
            return new MaxCoverAmountForAgeProvider(_mockPlanDefinitionProvider.Object);
        }
    }
}
