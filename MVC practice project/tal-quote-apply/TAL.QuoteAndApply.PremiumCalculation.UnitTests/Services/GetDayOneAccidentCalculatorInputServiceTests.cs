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
    public class GetDayOneAccidentCalculatorInputServiceTests
    {
        private Mock<IDayOneAccidentBaseRateDtoRepository> _dayOneAccidentBaseRateDtoRepository;
        private Mock<IGetFactorBCalculatorInputService> _getFactorBCalculatorInputService;
        private Mock<IOccupationClassFactorDtoRepository> _occupationClassFactorDtoRepository;

        [TestFixtureSetUp]
        public void Setup()
        {
            var mockRepo = new MockRepository(MockBehavior.Strict);

            _dayOneAccidentBaseRateDtoRepository = mockRepo.Create<IDayOneAccidentBaseRateDtoRepository>();
            _getFactorBCalculatorInputService = mockRepo.Create<IGetFactorBCalculatorInputService>();
            _occupationClassFactorDtoRepository = mockRepo.Create<IOccupationClassFactorDtoRepository>();
        }

        [Test]
        public void GetDayOneAccidentCalculatorInput_BaseRateNull_ZeroBaseRateReturned()
        {
            var mockFactors = GetMockPremiumCalculatorFactors();

            _dayOneAccidentBaseRateDtoRepository.Setup(call => call.GetAccidentBaseRate(
                mockFactors.PlanFactors.PlanCode,
                mockFactors.CoverFactors.CoverCode,
                mockFactors.PolicyFactors.BrandId,
                mockFactors.RiskFactors.Age,
                mockFactors.RiskFactors.Gender,
                mockFactors.PlanFactors.PremiumType,
                mockFactors.PlanFactors.WaitingPeriod)).Returns(() => null);

            _getFactorBCalculatorInputService.Setup(call => call.GetFactorBCalculatorInput(mockFactors))
                .Returns(new FactorBCalculatorInput(1, 1));

            _occupationClassFactorDtoRepository.Setup(
                call =>
                    call.GetOccupationClassFactorByGenderOccupationClassAndPlan(mockFactors.RiskFactors.Gender,
                        mockFactors.RiskFactors.OccupationClass, mockFactors.PlanFactors.PlanCode,
                        mockFactors.PolicyFactors.BrandId))
                        .Returns(new OccupationClassFactorDto {Factor = 1});

            var svc = GetService();
            var result = svc.GetDayOneAccidentCalculatorInput(mockFactors);

            Assert.That(result.BaseRate, Is.EqualTo(0));
        }

        [Test]
        public void GetDayOneAccidentCalculatorInput_BaseRateReturned()
        {
            var mockFactors = GetMockPremiumCalculatorFactors();

            _dayOneAccidentBaseRateDtoRepository.Setup(call => call.GetAccidentBaseRate(mockFactors.PlanFactors.PlanCode,
                mockFactors.CoverFactors.CoverCode,
                mockFactors.PolicyFactors.BrandId,
                mockFactors.RiskFactors.Age,
                mockFactors.RiskFactors.Gender,
                mockFactors.PlanFactors.PremiumType,
                mockFactors.PlanFactors.WaitingPeriod)).Returns(new DayOneAccidentBaseRateDto {BaseRate = 99});

            _getFactorBCalculatorInputService.Setup(call => call.GetFactorBCalculatorInput(mockFactors))
                .Returns(new FactorBCalculatorInput(1, 1));

            _occupationClassFactorDtoRepository.Setup(
                call =>
                    call.GetOccupationClassFactorByGenderOccupationClassAndPlan(mockFactors.RiskFactors.Gender,
                        mockFactors.RiskFactors.OccupationClass, mockFactors.PlanFactors.PlanCode,
                        mockFactors.PolicyFactors.BrandId))
                        .Returns(new OccupationClassFactorDto { Factor = 1 });

            var svc = GetService();
            var result = svc.GetDayOneAccidentCalculatorInput(mockFactors);

            Assert.That(result.BaseRate, Is.EqualTo(99));
        }

        [Test]
        public void GetDayOneAccidentCalculatorInput_OccupationClassLookupReturnsNull_ZeroOccFactor()
        {
            var mockFactors = GetMockPremiumCalculatorFactors();

            _dayOneAccidentBaseRateDtoRepository.Setup(call => call.GetAccidentBaseRate(mockFactors.PlanFactors.PlanCode,
                mockFactors.CoverFactors.CoverCode,
                mockFactors.PolicyFactors.BrandId,
                mockFactors.RiskFactors.Age,
                mockFactors.RiskFactors.Gender,
                mockFactors.PlanFactors.PremiumType,
                mockFactors.PlanFactors.WaitingPeriod)).Returns(new DayOneAccidentBaseRateDto { BaseRate = 99 });

            _getFactorBCalculatorInputService.Setup(call => call.GetFactorBCalculatorInput(mockFactors))
                .Returns(new FactorBCalculatorInput(1, 1));

            _occupationClassFactorDtoRepository.Setup(
                call =>
                    call.GetOccupationClassFactorByGenderOccupationClassAndPlan(mockFactors.RiskFactors.Gender,
                        mockFactors.RiskFactors.OccupationClass, mockFactors.PlanFactors.PlanCode,
                        mockFactors.PolicyFactors.BrandId))
                        .Returns(()=> null);

            var svc = GetService();
            var result = svc.GetDayOneAccidentCalculatorInput(mockFactors);

            Assert.That(result.BaseRate, Is.EqualTo(99));
            Assert.That(result.OccupationFactor, Is.EqualTo(0));
        }

        [Test]
        public void GetDayOneAccidentCalculatorInput_OccupationFactorReturned()
        {
            var mockFactors = GetMockPremiumCalculatorFactors();

            _dayOneAccidentBaseRateDtoRepository.Setup(call => call.GetAccidentBaseRate(mockFactors.PlanFactors.PlanCode,
                mockFactors.CoverFactors.CoverCode,
                mockFactors.PolicyFactors.BrandId,
                mockFactors.RiskFactors.Age,
                mockFactors.RiskFactors.Gender,
                mockFactors.PlanFactors.PremiumType,
                mockFactors.PlanFactors.WaitingPeriod)).Returns(new DayOneAccidentBaseRateDto { BaseRate = 99 });

            _getFactorBCalculatorInputService.Setup(call => call.GetFactorBCalculatorInput(mockFactors))
                .Returns(new FactorBCalculatorInput(1, 1));

            _occupationClassFactorDtoRepository.Setup(
                call =>
                    call.GetOccupationClassFactorByGenderOccupationClassAndPlan(mockFactors.RiskFactors.Gender,
                        mockFactors.RiskFactors.OccupationClass, mockFactors.PlanFactors.PlanCode,
                        mockFactors.PolicyFactors.BrandId))
                        .Returns(new OccupationClassFactorDto { Factor = 99 });

            var svc = GetService();
            var result = svc.GetDayOneAccidentCalculatorInput(mockFactors);

            Assert.That(result.BaseRate, Is.EqualTo(99));
            Assert.That(result.OccupationFactor, Is.EqualTo(99));
        }

        private PremiumCalculatorFactors GetMockPremiumCalculatorFactors()
        {
            var brandId = 1;
            var premCalcRequest = new PremiumCalculationRequest(PremiumFrequency.Monthly,
                new List<RiskCalculationRequest>
                {
                    new RiskCalculationRequest(1, 18, Gender.Male, true, "AAA", new List<PlanCalculationRequest>
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

        private GetDayOneAccidentCalculatorInputService GetService()
        {
            return new GetDayOneAccidentCalculatorInputService(_dayOneAccidentBaseRateDtoRepository.Object, 
                _getFactorBCalculatorInputService.Object, 
                _occupationClassFactorDtoRepository.Object);
        }
    }
}
