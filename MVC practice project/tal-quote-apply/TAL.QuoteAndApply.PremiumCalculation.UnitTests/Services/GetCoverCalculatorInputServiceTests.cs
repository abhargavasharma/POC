using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs;
using TAL.QuoteAndApply.PremiumCalculation.Data;
using TAL.QuoteAndApply.PremiumCalculation.Models;
using TAL.QuoteAndApply.PremiumCalculation.Services;

namespace TAL.QuoteAndApply.PremiumCalculation.UnitTests.Services
{
    [TestFixture]
    public class GetCoverCalculatorInputServiceTests
    {
        private Mock<ICoverDivisionalFactorDtoRepository> _coverDivisionalFactorDtoRepository;
        private Mock<ICoverBaseRateLookupRequestProvider> _coverBaseRateLookupRequestProvider;
        private Mock<ICoverBaseRateDtoRepository> _coverBaseRateDtoRepository;
        private Mock<IGetFactorACalculatorInputService> _getFactorACalculatorInputService;
        private Mock<IGetFactorBCalculatorInputService> _getFactorBCalculatorInputService;

        const int BrandId = 1;

        [TestFixtureSetUp]
        public void Setup()
        {
            var mockRepo = new MockRepository(MockBehavior.Strict);
            _coverDivisionalFactorDtoRepository = mockRepo.Create<ICoverDivisionalFactorDtoRepository>();
            _coverBaseRateLookupRequestProvider = mockRepo.Create<ICoverBaseRateLookupRequestProvider>();
            _coverBaseRateDtoRepository = mockRepo.Create<ICoverBaseRateDtoRepository>();
            _getFactorACalculatorInputService = mockRepo.Create<IGetFactorACalculatorInputService>();
            _getFactorBCalculatorInputService = mockRepo.Create<IGetFactorBCalculatorInputService>();
        }

        [Test, ExpectedException(typeof(ApplicationException))]
        public void GetCoverCalculatorInput_NullCoverBaseRate_ExceptionThrown()
        {
            var mockPremCalcRequest = GetMockPremiumCalculatorFactors();
            var allPlans = ((RiskCalculationRequest)mockPremCalcRequest.RiskFactors).Plans;

            var mockCriteria = new CoverBaseRateLookupRequest("", "", 1, Gender.Unknown, PremiumType.Unknown, null, null,
                null, null, null, BrandId);

            _coverBaseRateLookupRequestProvider.Setup(
                call => call.GetCoverBaseRateLookupRequestFor(mockPremCalcRequest)).Returns(mockCriteria);

            _coverBaseRateDtoRepository.Setup(call => call.GetBaseRateForCriteria(mockCriteria)).Returns(() => null);

            var svc = GetService();
            svc.GetCoverCalculatorInput(mockPremCalcRequest, allPlans, BrandId);
        }

        [Test, ExpectedException(typeof(ApplicationException))]
        public void GetCoverCalculatorInput_NullDivisionalFactor_ExceptionThrown()
        {
            var mockPremCalcRequest = GetMockPremiumCalculatorFactors();
            var allPlans = ((RiskCalculationRequest)mockPremCalcRequest.RiskFactors).Plans;

            var mockCriteria = new CoverBaseRateLookupRequest("", "", 1, Gender.Unknown, PremiumType.Unknown, null, null,
                null, null, null, BrandId);

            _coverBaseRateLookupRequestProvider.Setup(
                call => call.GetCoverBaseRateLookupRequestFor(mockPremCalcRequest)).Returns(mockCriteria);

            _coverBaseRateDtoRepository.Setup(call => call.GetBaseRateForCriteria(mockCriteria))
                .Returns(new CoverBaseRateDto {BaseRate = 1m});

            _coverDivisionalFactorDtoRepository.Setup(
                call => call.GetCoverDivisionalFactorByCoverCode(mockPremCalcRequest.CoverFactors.CoverCode, mockPremCalcRequest.PolicyFactors.BrandId))
                .Returns(() => null);               

            var svc = GetService();
            svc.GetCoverCalculatorInput(mockPremCalcRequest, allPlans, BrandId);
        }

        [Test]
        public void GetCoverCalculatorInput_ReturnsCalcuatorInput()
        {
            var mockPremCalcRequest = GetMockPremiumCalculatorFactors();
            var allPlans = ((RiskCalculationRequest)mockPremCalcRequest.RiskFactors).Plans;

            var mockCriteria = new CoverBaseRateLookupRequest("", "", 1, Gender.Unknown, PremiumType.Unknown, null, null,
                null, null, null, BrandId);

            _coverBaseRateLookupRequestProvider.Setup(
                call => call.GetCoverBaseRateLookupRequestFor(mockPremCalcRequest)).Returns(mockCriteria);

            _coverBaseRateDtoRepository.Setup(call => call.GetBaseRateForCriteria(mockCriteria))
                .Returns(new CoverBaseRateDto { BaseRate = 1m });

            _coverDivisionalFactorDtoRepository.Setup(
                call => call.GetCoverDivisionalFactorByCoverCode(mockPremCalcRequest.CoverFactors.CoverCode, mockPremCalcRequest.PolicyFactors.BrandId))
                .Returns(() => new CoverDivisionalFactorDto {DivisionalFactor = 1000});

            _getFactorACalculatorInputService.Setup(call => call.GetFactorACalculatorInput(mockPremCalcRequest, allPlans, BrandId))
                .Returns(new FactorACalculatorInput(mockPremCalcRequest.PlanFactors.IncludeInMultiPlanDiscount, 1, 1, 1, 1, 1, 1, 1, 1,1));

            _getFactorBCalculatorInputService.Setup(call => call.GetFactorBCalculatorInput(mockPremCalcRequest))
                .Returns(new FactorBCalculatorInput(1,1));

            var svc = GetService();
            var result = svc.GetCoverCalculatorInput(mockPremCalcRequest, allPlans, BrandId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.BaseRate, Is.EqualTo(1));
            Assert.That(result.DivionalFactor, Is.EqualTo(1000));
        }
        
        private PremiumCalculatorFactors GetMockPremiumCalculatorFactors()
        {
            var brandId = 1;
            var premCalcRequest = new PremiumCalculationRequest(PremiumFrequency.Monthly,
                new List<RiskCalculationRequest>
                {
                    new RiskCalculationRequest(1, 18, Gender.Male, true, null, new List<PlanCalculationRequest>
                    {
                        new PlanCalculationRequest("ABC", true, true, 100000, PremiumType.Level, null, null, null, OccupationDefinition.Unknown, 1, true, false, false,
                            new List<CoverCalculationRequest> {new CoverCalculationRequest("123", true, true, true, true, new Loadings(0, 0), brandId) }),
                    })
                }, brandId);

            var risk = premCalcRequest.Risks.First();
            var plan = risk.Plans.First();
            var cover = plan.Covers.First();
            return new PremiumCalculatorFactors(premCalcRequest, risk, plan, cover);
        }

        private GetCoverCalculatorInputService GetService()
        {
            return new GetCoverCalculatorInputService(_coverDivisionalFactorDtoRepository.Object, _coverBaseRateLookupRequestProvider.Object,
                _coverBaseRateDtoRepository.Object, _getFactorACalculatorInputService.Object, _getFactorBCalculatorInputService.Object);
        }
    }
}