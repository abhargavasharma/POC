using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.PremiumCalculation.Configuration;
using TAL.QuoteAndApply.PremiumCalculation.Data;
using TAL.QuoteAndApply.PremiumCalculation.Models;
using TAL.QuoteAndApply.PremiumCalculation.Services;
using TAL.QuoteAndApply.Tests.Shared;

namespace TAL.QuoteAndApply.PremiumCalculation.UnitTests.Services
{
    [TestFixture]
    public class MultiPlanDiscountFactorServiceTests
    {
        private Mock<IPlanMinimumCoverAmountForMultiPlanDiscountDtoRepository> _mockPlanMinimumCoverAmountForMultiPlanDiscountDtoRepository;
        private Mock<IMultiPlanDiscountFactorDtoRepository> _mockMultiPlanDiscountFactorDtoRepository;
        private Mock<IPremiumCalculationConfigurationProvider> _mockPremiumCalcConfigurationService;

        const int BrandId = 1;

        [TestFixtureSetUp]
        public void Setup()
        {
            var mockRepo = new MockRepository(MockBehavior.Strict);
            _mockPlanMinimumCoverAmountForMultiPlanDiscountDtoRepository = mockRepo.Create<IPlanMinimumCoverAmountForMultiPlanDiscountDtoRepository>();
            _mockMultiPlanDiscountFactorDtoRepository = mockRepo.Create<IMultiPlanDiscountFactorDtoRepository>();
            _mockPremiumCalcConfigurationService = mockRepo.Create<IPremiumCalculationConfigurationProvider>();
        }

        [Test]
        public void GetFor_PlanNotIncludedInMultiplanDiscount_Returns1()
        {
            var includedInMultiPlanDiscount = false;

            var planFactor = new PlanCalculationRequest("ABC", true, includedInMultiPlanDiscount, 100000,
                PremiumType.Level, null, null, null, OccupationDefinition.Unknown, null, true, true, true,
                new List<CoverCalculationRequest>
                {
                    new CoverCalculationRequest("123", true, true, true, true, new Loadings(0, 0), BrandId)
                });

            var allPlanFactors = new List<PlanCalculationRequest>
            {
                planFactor
            };

            var premCalcRequest = new PremiumCalculationRequest(PremiumFrequency.Monthly,
                new List<RiskCalculationRequest>
                {
                    new RiskCalculationRequest(1, 18, Gender.Male, true, null, allPlanFactors)
                }, BrandId);

            var risk = premCalcRequest.Risks.First();
            var plan = risk.Plans.First();
            var cover = plan.Covers.First();

            _mockPremiumCalcConfigurationService.Setup(call => call.MultiPlanDiscountPlanLimit).Returns(4);

            var mockCalcFactors = new PremiumCalculatorFactors(premCalcRequest, risk, plan, cover);

            var svc = GetService();
            var result = svc.GetFor(mockCalcFactors.PlanFactors, allPlanFactors, BrandId);

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void GetFor_PlanIncludedInMultiplanDiscount_TwoActivePlansBothAboveMinimumCoverAmount()
        {
            var includedInMultiPlanDiscount = true;

            var planFactor = new PlanCalculationRequest("ABC", true, includedInMultiPlanDiscount, 100000,
                PremiumType.Level, null, null, null, OccupationDefinition.Unknown, null, true, true, true,
                new List<CoverCalculationRequest>
                {
                    new CoverCalculationRequest("123", true, true, true, true, new Loadings(0, 0), BrandId)
                });

            var allPlanFactors = new List<PlanCalculationRequest>
            {
                planFactor,
                new PlanCalculationRequest("ZYX", true, includedInMultiPlanDiscount, 100000,
                PremiumType.Level, null, null, null, OccupationDefinition.Unknown, null, true, true, true,
                new List<CoverCalculationRequest>
                {
                    new CoverCalculationRequest("123", true, true, true, true, new Loadings(0, 0), BrandId)
                })
            };

            var premCalcRequest = new PremiumCalculationRequest(PremiumFrequency.Monthly,
                new List<RiskCalculationRequest>
                {
                    new RiskCalculationRequest(1, 18, Gender.Male, true, null, allPlanFactors)
                }, BrandId);

            var risk = premCalcRequest.Risks.First();
            var plan = risk.Plans.First();
            var cover = plan.Covers.First();

            var mockCalcFactors = new PremiumCalculatorFactors(premCalcRequest, risk, plan, cover);


            _mockPlanMinimumCoverAmountForMultiPlanDiscountDtoRepository.Setup(
                call => call.GetMinimumCoverAmountForMultiPlanDiscount(It.IsAny<string>(), BrandId))
                .ReturnsInOrder(new[]
                {
                    new PlanMinimumCoverAmountForMultiPlanDiscountDto {MinimumCoverAmount = 100000},
                    new PlanMinimumCoverAmountForMultiPlanDiscountDto {MinimumCoverAmount = 100000}
                });

            _mockMultiPlanDiscountFactorDtoRepository.Setup(call => call.GetMultiPlanDiscountFactorForPlanCount(2, BrandId))
                .Returns(new MultiPlanDiscountFactorDto {Factor = 99m});

            _mockPremiumCalcConfigurationService.Setup(call => call.MultiPlanDiscountPlanLimit).Returns(4);

            var svc = GetService();
            var result = svc.GetFor(mockCalcFactors.PlanFactors, allPlanFactors, BrandId);

            Assert.That(result, Is.EqualTo(99m));
        }

        [Test]
        public void GetFor_PlanIncludedInMultiplanDiscount_TwoActivePlansOneBothAboveMinimumCoverAmount()
        {
            var includedInMultiPlanDiscount = true;

            var planFactor = new PlanCalculationRequest("ABC", true, includedInMultiPlanDiscount, 100000,
                PremiumType.Level, null, null, null, OccupationDefinition.Unknown, null, true, true, true,
                new List<CoverCalculationRequest>
                {
                    new CoverCalculationRequest("123", true, true, true, true, new Loadings(0, 0), BrandId)
                });

            var allPlanFactors = new List<PlanCalculationRequest>
            {
                planFactor,
                new PlanCalculationRequest("ZYX", true, includedInMultiPlanDiscount, 100000,
                PremiumType.Level, null, null, null, OccupationDefinition.Unknown, null, true, true, true,
                new List<CoverCalculationRequest>
                {
                    new CoverCalculationRequest("123", true, true, true, true, new Loadings(0, 0), BrandId)
                })
            };

            var premCalcRequest = new PremiumCalculationRequest(PremiumFrequency.Monthly,
                new List<RiskCalculationRequest>
                {
                    new RiskCalculationRequest(1, 18, Gender.Male, true, null, allPlanFactors)
                }, BrandId);

            var risk = premCalcRequest.Risks.First();
            var plan = risk.Plans.First();
            var cover = plan.Covers.First();

            var mockCalcFactors = new PremiumCalculatorFactors(premCalcRequest, risk, plan, cover);


            _mockPlanMinimumCoverAmountForMultiPlanDiscountDtoRepository.Setup(
                call => call.GetMinimumCoverAmountForMultiPlanDiscount(It.IsAny<string>(), BrandId))
                .ReturnsInOrder(new[]
                {
                    new PlanMinimumCoverAmountForMultiPlanDiscountDto {MinimumCoverAmount = 100000},
                    new PlanMinimumCoverAmountForMultiPlanDiscountDto {MinimumCoverAmount = 99999}
                });

            _mockMultiPlanDiscountFactorDtoRepository.Setup(call => call.GetMultiPlanDiscountFactorForPlanCount(2, BrandId))
                .Returns(new MultiPlanDiscountFactorDto { Factor = 99m });

            _mockPremiumCalcConfigurationService.Setup(call => call.MultiPlanDiscountPlanLimit).Returns(4);

            var svc = GetService();
            var result = svc.GetFor(mockCalcFactors.PlanFactors, allPlanFactors, BrandId);

            Assert.That(result, Is.EqualTo(99m));
        }

        [Test]
        public void GetFor_PlanIncludedInMultiplanDiscount_TwoPlansOneActiveAboveMinimumCoverAmount()
        {
            var includedInMultiPlanDiscount = true;

            var planFactor = new PlanCalculationRequest("ABC", true, includedInMultiPlanDiscount, 100000,
                PremiumType.Level, null, null, null, OccupationDefinition.Unknown, null, true, true, true,
                new List<CoverCalculationRequest>
                {
                    new CoverCalculationRequest("123", true, true, true, true, new Loadings(0, 0), BrandId)
                });

            var allPlanFactors = new List<PlanCalculationRequest>
            {
                planFactor,
                new PlanCalculationRequest("ZYX", false, includedInMultiPlanDiscount, 100000,
                PremiumType.Level, null, null, null, OccupationDefinition.Unknown, null, true, true, true,
                new List<CoverCalculationRequest>
                {
                    new CoverCalculationRequest("123", true, true, true, true, new Loadings(0, 0), BrandId)
                })
            };

            var premCalcRequest = new PremiumCalculationRequest(PremiumFrequency.Monthly,
                new List<RiskCalculationRequest>
                {
                    new RiskCalculationRequest(1, 18, Gender.Male, true, null, allPlanFactors)
                }, BrandId);

            var risk = premCalcRequest.Risks.First();
            var plan = risk.Plans.First();
            var cover = plan.Covers.First();

            var mockCalcFactors = new PremiumCalculatorFactors(premCalcRequest, risk, plan, cover);


            _mockPlanMinimumCoverAmountForMultiPlanDiscountDtoRepository.Setup(
                call => call.GetMinimumCoverAmountForMultiPlanDiscount(It.IsAny<string>(), BrandId))
                .ReturnsInOrder(new[]
                {
                    new PlanMinimumCoverAmountForMultiPlanDiscountDto {MinimumCoverAmount = 100000}
                });

            _mockMultiPlanDiscountFactorDtoRepository.Setup(call => call.GetMultiPlanDiscountFactorForPlanCount(1, BrandId))
                .Returns(new MultiPlanDiscountFactorDto { Factor = 99m });

            _mockPremiumCalcConfigurationService.Setup(call => call.MultiPlanDiscountPlanLimit).Returns(4);

            var svc = GetService();
            var result = svc.GetFor(mockCalcFactors.PlanFactors, allPlanFactors, BrandId);

            Assert.That(result, Is.EqualTo(99m));
        }

        private MultiPlanDiscountFactorService GetService()
        {
            return new MultiPlanDiscountFactorService(_mockPlanMinimumCoverAmountForMultiPlanDiscountDtoRepository.Object, 
                _mockMultiPlanDiscountFactorDtoRepository.Object, _mockPremiumCalcConfigurationService.Object);
        }
    }
}
