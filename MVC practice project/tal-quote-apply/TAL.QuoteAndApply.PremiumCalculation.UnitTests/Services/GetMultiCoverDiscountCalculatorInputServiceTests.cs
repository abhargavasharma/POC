using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.PremiumCalculation.Data;
using TAL.QuoteAndApply.PremiumCalculation.Models;
using TAL.QuoteAndApply.PremiumCalculation.Services;

namespace TAL.QuoteAndApply.PremiumCalculation.UnitTests.Services
{
    [TestFixture]
    public class GetMultiCoverDiscountCalculatorInputServiceTests
    {
        private Mock<IMultiCoverDiscountFactorDtoRepository> _multiCoverDiscountFactorDtoRepository;

        const int BrandId = 1;

        [TestFixtureSetUp]
        public void Setup()
        {
            var mockRepo = new MockRepository(MockBehavior.Strict);
            _multiCoverDiscountFactorDtoRepository = mockRepo.Create<IMultiCoverDiscountFactorDtoRepository>();
        }

        [Test]
        public void GetPlanDiscountCalculatorInput_NullMultiCoverDiscountFactor_ReturnsZeroMultiCoverDiscountFactor()
        {
            const string coverCode = "123";

            var premCalcRequest = new PremiumCalculationRequest(PremiumFrequency.Monthly,
                new List<RiskCalculationRequest>
                {
                    new RiskCalculationRequest(1, 18, Gender.Male, true, null, new List<PlanCalculationRequest>
                    {
                        new PlanCalculationRequest("ABC", true, true, 100000, PremiumType.Level, null, null, null, OccupationDefinition.Unknown, null, true, false, false,
                            new List<CoverCalculationRequest> {new CoverCalculationRequest(coverCode, true, true, true, true, new Loadings(0, 0), BrandId) }),
                    })
                }, BrandId);

            var planFactor = premCalcRequest.Risks.First().Plans.First();
            var selectedCoversCodes =
                planFactor.Covers.Where(p => p.Active).OrderBy(p => p.CoverCode).Select(p => p.CoverCode);

            var coverPremiumResult = new CoverPremiumCalculationResult(coverCode, 5000m, 0);

            _multiCoverDiscountFactorDtoRepository.Setup(
                call => call.GetMultiCoverDiscountFactorForPlan(planFactor.PlanCode, BrandId, string.Join("|", selectedCoversCodes))).Returns(() => null);

            var svc = GetService();
            var result = svc.GetPlanDiscountCalculatorInput(planFactor, planFactor.Covers, new List<CoverPremiumCalculationResult> { coverPremiumResult }, BrandId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.MultiCoverDiscountFactor, Is.EqualTo(0));
        }


        [Test]
        public void GetPlanDiscountCalculatorInput_OneCover_InputReturned()
        {
            const string coverCode = "123";
            const decimal coverPremium = 500.55m;
            const bool rateableCover = true;
            const decimal multiCoverDiscountFactor = .5m;

            var premCalcRequest = new PremiumCalculationRequest(PremiumFrequency.Monthly,
                new List<RiskCalculationRequest>
                {
                    new RiskCalculationRequest(1, 18, Gender.Male, true, null, new List<PlanCalculationRequest>
                    {
                        new PlanCalculationRequest("ABC", true, true, 100000, PremiumType.Level, null, null, null, OccupationDefinition.Unknown, null, true, null, null,
                            new List<CoverCalculationRequest> {new CoverCalculationRequest(coverCode, true, rateableCover, true, true, new Loadings(0, 0), BrandId) }),
                    })
                }, BrandId);

            var planFactor = premCalcRequest.Risks.First().Plans.First();
            var selectedCoversCodes =
                planFactor.Covers.Where(p => p.Active).OrderBy(p => p.CoverCode).Select(p => p.CoverCode);

            var coverPremiumResult = new CoverPremiumCalculationResult(coverCode, coverPremium, 0);

            _multiCoverDiscountFactorDtoRepository.Setup(
                call => call.GetMultiCoverDiscountFactorForPlan(planFactor.PlanCode, BrandId, string.Join("|", selectedCoversCodes))).Returns(new MultiCoverDiscountFactorDto {Factor = multiCoverDiscountFactor });

            var svc = GetService();
            var result = svc.GetPlanDiscountCalculatorInput(planFactor, planFactor.Covers, new List<CoverPremiumCalculationResult> { coverPremiumResult }, BrandId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.MultiCoverDiscountFactor, Is.EqualTo(multiCoverDiscountFactor));
            Assert.That(result.Covers.Count, Is.EqualTo(1));

            var coverResult = result.Covers.First();
            Assert.That(coverResult.Premium, Is.EqualTo(coverPremium));
            Assert.That(coverResult.IncludeInMultiCoverDiscount, Is.EqualTo(rateableCover));
        }


        [Test]
        public void GetPlanDiscountCalculatorInput_TwoCovers_InputReturned()
        {
            const string coverCode1 = "123";
            const decimal coverPremium1 = 500.55m;
            const bool coverIncludedInDiscount1 = true;

            const string coverCode2 = "987";
            const decimal coverPremium2 = 500.55m;
            const bool coverIncludedInDiscount2 = true;


            const decimal multiCoverDiscountFactor = .5m;

            var premCalcRequest = new PremiumCalculationRequest(PremiumFrequency.Monthly,
                new List<RiskCalculationRequest>
                {
                    new RiskCalculationRequest(1, 18, Gender.Male, true, null, new List<PlanCalculationRequest>
                    {
                        new PlanCalculationRequest("ABC", true, true, 100000, PremiumType.Level, null, null, null, OccupationDefinition.Unknown, null, true, false, false,
                            new List<CoverCalculationRequest>
                            {
                                new CoverCalculationRequest(coverCode1, true, coverIncludedInDiscount1, true, true, new Loadings(0, 0), BrandId),
                                new CoverCalculationRequest(coverCode2, true, coverIncludedInDiscount2, true, true, new Loadings(0, 0), BrandId)
                            }),
                    })
                }, BrandId);

            var planFactor = premCalcRequest.Risks.First().Plans.First();
            var selectedCoversCodes =
                planFactor.Covers.Where(p => p.Active).OrderBy(p => p.CoverCode).Select(p => p.CoverCode);

            var coverPremiumResult1 = new CoverPremiumCalculationResult(coverCode1, coverPremium1, 0);
            var coverPremiumResult2 = new CoverPremiumCalculationResult(coverCode2, coverPremium2, 0);

            _multiCoverDiscountFactorDtoRepository.Setup(
                call => call.GetMultiCoverDiscountFactorForPlan(planFactor.PlanCode, BrandId, string.Join("|", selectedCoversCodes))).Returns(new MultiCoverDiscountFactorDto { Factor = multiCoverDiscountFactor });

            var svc = GetService();
            var result = svc.GetPlanDiscountCalculatorInput(planFactor, planFactor.Covers, new List<CoverPremiumCalculationResult> { coverPremiumResult1, coverPremiumResult2 }, BrandId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.MultiCoverDiscountFactor, Is.EqualTo(multiCoverDiscountFactor));
            Assert.That(result.Covers.Count, Is.EqualTo(2));

            var coverResult = result.Covers[0];
            Assert.That(coverResult.Premium, Is.EqualTo(coverPremium1));
            Assert.That(coverResult.IncludeInMultiCoverDiscount, Is.EqualTo(coverIncludedInDiscount1));

            coverResult = result.Covers[1];
            Assert.That(coverResult.Premium, Is.EqualTo(coverPremium2));
            Assert.That(coverResult.IncludeInMultiCoverDiscount, Is.EqualTo(coverIncludedInDiscount2));
        }

        private GetMultiCoverDiscountCalculatorInputService GetService()
        {
            return new GetMultiCoverDiscountCalculatorInputService(_multiCoverDiscountFactorDtoRepository.Object);
        }
    }
}
