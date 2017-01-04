using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.Product.Models;
using TAL.QuoteAndApply.Product.Models.Definition;
using TAL.QuoteAndApply.Product.Service;

namespace TAL.QuoteAndApply.Product.UnitTests.Service
{
    [TestFixture]
    public class CoverAmountServiceTests
    {
        private Mock<IPlanDefinitionProvider> _mockPlanDefinitionProvider;
        private Mock<IMaxCoverAmountForAgeProvider> _mockMaxCoverAmountForAgeProvider;
        private Mock<IMaxCoverAmountPercentageOfIncomeProvider> _maxCoverAmountPercentageOfIncomeProvider;

        [TestFixtureSetUp]
        public void Setup()
        {
            _mockPlanDefinitionProvider = new Mock<IPlanDefinitionProvider>(MockBehavior.Strict);
            _mockMaxCoverAmountForAgeProvider = new Mock<IMaxCoverAmountForAgeProvider>(MockBehavior.Strict);
            _maxCoverAmountPercentageOfIncomeProvider = new Mock<IMaxCoverAmountPercentageOfIncomeProvider>(MockBehavior.Strict);
        }

        [Test, ExpectedException(typeof(ApplicationException), MatchType = MessageMatch.Contains, ExpectedMessage = "No max cover amount definition provided for plan")]
        public void GetMaxCover_NoDefinitions_ExceptionThrown()
        {
            var mockMaxCoverParam = new MaxCoverAmountParam("TEST", "tal", 20, 100000, 100000, null);

            _mockPlanDefinitionProvider.Setup(call => call.GetPlanByCode(mockMaxCoverParam.PlanCode, "tal"))
                .Returns(new PlanDefinition());

            var svc = GetService();
            svc.GetMaxCover(mockMaxCoverParam);
        }


        [Test]
        public void GetMaxCover_AgeRangeCoverAmountDefinitions_NoParentPlanCap_MaxForAgeReturned()
        {
            var maxCoverForAge = 500000;

            var mockMaxCoverParam = new MaxCoverAmountParam("TEST", "tal", 20, 100000, 100000, null);

            _mockMaxCoverAmountForAgeProvider.Setup(
                call => call.GetFor(mockMaxCoverParam.PlanCode, "tal", mockMaxCoverParam.Age, mockMaxCoverParam.AnnualIncome)).Returns(maxCoverForAge);

            _mockPlanDefinitionProvider.Setup(call => call.GetPlanByCode(mockMaxCoverParam.PlanCode, "tal"))
                .Returns(new PlanDefinition {AgeRangeCoverAmountDefinitions = new List<AgeRangeCoverAmountDefinition>()});

            var svc = GetService();
            var result = svc.GetMaxCover(mockMaxCoverParam);

            Assert.That(result, Is.EqualTo(maxCoverForAge));
        }

        [Test]
        public void GetMaxCover_AgeRangeCoverAmountDefinitions_ParentPlanCapHigherThanMaxForAge_MaxForAgeReturned()
        {
            var maxCoverForAge = 500000;
            var parentPlanCap = 500001;

            var mockMaxCoverParam = new MaxCoverAmountParam("TEST", "tal", 20, 100000, 100000, parentPlanCap);

            _mockMaxCoverAmountForAgeProvider.Setup(
                call => call.GetFor(mockMaxCoverParam.PlanCode, "tal", mockMaxCoverParam.Age, mockMaxCoverParam.AnnualIncome)).Returns(maxCoverForAge);

            _mockPlanDefinitionProvider.Setup(call => call.GetPlanByCode(mockMaxCoverParam.PlanCode, "tal"))
                .Returns(new PlanDefinition { AgeRangeCoverAmountDefinitions = new List<AgeRangeCoverAmountDefinition>() });

            var svc = GetService();
            var result = svc.GetMaxCover(mockMaxCoverParam);

            Assert.That(result, Is.EqualTo(maxCoverForAge));
        }

        [Test]
        public void GetMaxCover_AgeRangeCoverAmountDefinitions_ParentPlanCapLowerThanMaxForAge_ParentPlanCapReturned()
        {
            var maxCoverForAge = 500000;
            var parentPlanCap = 499999;

            var mockMaxCoverParam = new MaxCoverAmountParam("TEST", "tal", 20, 100000, 100000, parentPlanCap);

            _mockMaxCoverAmountForAgeProvider.Setup(
                call => call.GetFor(mockMaxCoverParam.PlanCode, "tal", mockMaxCoverParam.Age, mockMaxCoverParam.AnnualIncome)).Returns(maxCoverForAge);

            _mockPlanDefinitionProvider.Setup(call => call.GetPlanByCode(mockMaxCoverParam.PlanCode, "tal"))
                .Returns(new PlanDefinition { AgeRangeCoverAmountDefinitions = new List<AgeRangeCoverAmountDefinition>() });

            var svc = GetService();
            var result = svc.GetMaxCover(mockMaxCoverParam);

            Assert.That(result, Is.EqualTo(parentPlanCap));
        }

        [Test]
        public void GetMaxCover_AgeRangeCoverAmountDefinitions_ParentPlanCapIsZero_MaxForAgeReturned()
        {
            var maxCoverForAge = 500000;
            var parentPlanCap = 0;

            var mockMaxCoverParam = new MaxCoverAmountParam("TEST", "tal", 20, 100000, 100000, parentPlanCap);

            _mockMaxCoverAmountForAgeProvider.Setup(
                call => call.GetFor(mockMaxCoverParam.PlanCode, "tal", mockMaxCoverParam.Age, mockMaxCoverParam.AnnualIncome)).Returns(maxCoverForAge);

            _mockPlanDefinitionProvider.Setup(call => call.GetPlanByCode(mockMaxCoverParam.PlanCode, "tal"))
                .Returns(new PlanDefinition { AgeRangeCoverAmountDefinitions = new List<AgeRangeCoverAmountDefinition>() });

            var svc = GetService();
            var result = svc.GetMaxCover(mockMaxCoverParam);

            Assert.That(result, Is.EqualTo(maxCoverForAge));
        }

        [Test]
        public void GetMaxCover_CoverAmountPercentageDefinition_NoParentPlanCap_MaxForAgeReturned()
        {
            var maxCoverForAge = 5000;
            var parentPlanCap = 0;

            var mockMaxCoverParam = new MaxCoverAmountParam("TEST", "tal", 20, 100000, 2000, parentPlanCap);

            _maxCoverAmountPercentageOfIncomeProvider.Setup(
                call => call.GetFor(mockMaxCoverParam.PlanCode, "tal", mockMaxCoverParam.AnnualIncome)).Returns(maxCoverForAge);

            _mockPlanDefinitionProvider.Setup(call => call.GetPlanByCode(mockMaxCoverParam.PlanCode, "tal"))
                .Returns(new PlanDefinition { CoverAmountPercentageDefinition = new CoverAmountPercentageDefinition(100, maxCoverForAge) });

            var svc = GetService();
            var result = svc.GetMaxCover(mockMaxCoverParam);

            Assert.That(result, Is.EqualTo(maxCoverForAge));
        }


        private CoverAmountService GetService()
        {
            return new CoverAmountService(_mockPlanDefinitionProvider.Object, _mockMaxCoverAmountForAgeProvider.Object, _maxCoverAmountPercentageOfIncomeProvider.Object);
        }
    }
}
