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
    public class GetFactorACalculatorInputServiceTests
    {
        private Mock<IMultiPlanDiscountFactorService> _multiPlanDiscountService;
        private Mock<ILargeSumInsuredDiscountFactorDtoRepository> _largeSumInsuredDiscountFactorDtoRepository;
        private Mock<IOccupationClassFactorDtoRepository> _occupationClassFactorDtoRepository;
        private Mock<ISmokerFactorDtoRepository> _smokerFactorDtoRepository;
        private Mock<IIncreasingClaimsFactorDtoRepository> _increasingClaimFactorDtoRepository;
        private Mock<IIndemnityFactorDtoRepository> _indemnityFactorDtoRepository;
        private Mock<IWaitingPeriodFactorDtoRepository> _waitingPeriodFactorDtoRepository;
        private Mock<IOccupationDefinitionTypeFactorDtoRepository> _occupationDefinitionTypeFactorDtoRepository;

        const int BrandId = 1;

        [TestFixtureSetUp]
        public void Setup()
        {
            var mockRepo = new MockRepository(MockBehavior.Strict);

            _multiPlanDiscountService = mockRepo.Create<IMultiPlanDiscountFactorService>();
            _largeSumInsuredDiscountFactorDtoRepository = mockRepo.Create<ILargeSumInsuredDiscountFactorDtoRepository>();
            _occupationClassFactorDtoRepository = mockRepo.Create<IOccupationClassFactorDtoRepository>();
            _smokerFactorDtoRepository = mockRepo.Create<ISmokerFactorDtoRepository>();
            _increasingClaimFactorDtoRepository = mockRepo.Create<IIncreasingClaimsFactorDtoRepository>();
            _indemnityFactorDtoRepository = mockRepo.Create<IIndemnityFactorDtoRepository>();
            _waitingPeriodFactorDtoRepository = mockRepo.Create<IWaitingPeriodFactorDtoRepository>();
            _occupationDefinitionTypeFactorDtoRepository =
                mockRepo.Create<IOccupationDefinitionTypeFactorDtoRepository>();
        }

        [Test, ExpectedException(typeof(ApplicationException))]
        public void GetFactorACalculatorInput_NullLargeSumInsuredDiscount_ExceptionThrown()
        {
            var mockPremCalcRequest = GetMockPremiumCalculatorFactors();
            var allPlans = ((RiskCalculationRequest) mockPremCalcRequest.RiskFactors).Plans;

            _multiPlanDiscountService.Setup(
                call => call.GetFor(mockPremCalcRequest.PlanFactors, allPlans, 1)).Returns(1m);

            _largeSumInsuredDiscountFactorDtoRepository.Setup(
                call =>
                    call.GetLargeSumInsuredDiscountForSumInsured(mockPremCalcRequest.PlanFactors.CoverAmount,
                        mockPremCalcRequest.PlanFactors.PlanCode, mockPremCalcRequest.PolicyFactors.BrandId))
                .Returns(() => null);

            var svc = GetService();
            svc.GetFactorACalculatorInput(mockPremCalcRequest, allPlans, 1);
        }

        [Test]
        public void GetFactorACalculatorInput_RepositoryDefaultsWhenNullReturned_FactorAInputReturned()
        {
            var multiPlanDiscountFactor = .99m;
            var largeSumInsuredDiscountFactor = .98m;
            var expectedOccupationFactor = 1;
            var expectedSmokingFactor = 1;
            var expectedIncreasingClaimsOptionFactor = 1;
            var expectedIndemnityOptionFactor = 1;
            var expectedWaitingPeriodFactor = 1;
            var expectedOccupationDefinitionFactor = 1;
            var expectedOccupationLoadingFactor = 1;

            var mockPremCalcRequest = GetMockPremiumCalculatorFactors();
            var allPlans = ((RiskCalculationRequest)mockPremCalcRequest.RiskFactors).Plans;

            _multiPlanDiscountService.Setup(
                call => call.GetFor(mockPremCalcRequest.PlanFactors, allPlans, 1)).Returns(multiPlanDiscountFactor);

            _largeSumInsuredDiscountFactorDtoRepository.Setup(
                call =>
                    call.GetLargeSumInsuredDiscountForSumInsured(mockPremCalcRequest.PlanFactors.CoverAmount,
                        mockPremCalcRequest.PlanFactors.PlanCode, mockPremCalcRequest.PolicyFactors.BrandId))
                .Returns(new LargeSumInsuredDiscountFactorDto { Factor = largeSumInsuredDiscountFactor });

            _occupationClassFactorDtoRepository.Setup(
                call =>
                    call.GetOccupationClassFactorByGenderOccupationClassAndPlan(mockPremCalcRequest.RiskFactors.Gender,
                        mockPremCalcRequest.RiskFactors.OccupationClass, mockPremCalcRequest.PlanFactors.PlanCode,
                        mockPremCalcRequest.PolicyFactors.BrandId))
                .Returns(() => null);

            _smokerFactorDtoRepository.Setup(
                call =>
                    call.GetSmokerFactorBySmokerAndPlan(mockPremCalcRequest.RiskFactors.Smoker,
                        mockPremCalcRequest.PlanFactors.PlanCode, mockPremCalcRequest.PolicyFactors.BrandId))
                .Returns(() => null);

            _increasingClaimFactorDtoRepository.Setup(
                call =>
                    call.GetIncreasingClaimFactor(mockPremCalcRequest.PlanFactors.PlanCode,
                        mockPremCalcRequest.PolicyFactors.BrandId,
                        mockPremCalcRequest.PlanFactors.IncreasingClaimsSelected.Value,
                        mockPremCalcRequest.PlanFactors.BenefitPeriod))
                .Returns(() => null);

            _indemnityFactorDtoRepository.Setup(call => call.GetIndemnityFactorByPlanCode(mockPremCalcRequest.PlanFactors.PlanCode, mockPremCalcRequest.PolicyFactors.BrandId))
                .Returns(() => null);

            _waitingPeriodFactorDtoRepository.Setup(
                call =>
                    call.GetWaitingPeriodFactorByWaitingPeriod(mockPremCalcRequest.PlanFactors.WaitingPeriod,
                        mockPremCalcRequest.PlanFactors.PlanCode, mockPremCalcRequest.PolicyFactors.BrandId))
                .Returns(() => null);

            _occupationDefinitionTypeFactorDtoRepository.Setup(
                call =>
                    call.GetOccupationDefinitionTypeFactorForOccupationDefinition(
                        mockPremCalcRequest.PlanFactors.OccupationDefinition,
                        mockPremCalcRequest.PolicyFactors.BrandId)).Returns(() => null);

            var svc = GetService();

            var result = svc.GetFactorACalculatorInput(mockPremCalcRequest, allPlans, 1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.MultiPlanDiscountFactor, Is.EqualTo(multiPlanDiscountFactor));
            Assert.That(result.LargeSumInsuredDiscountFactor, Is.EqualTo(largeSumInsuredDiscountFactor));

            Assert.That(result.OccupationFactor, Is.EqualTo(expectedOccupationFactor));
            Assert.That(result.SmokingFactor, Is.EqualTo(expectedSmokingFactor));
            Assert.That(result.IncreasingClaimsOptionFactor, Is.EqualTo(expectedIncreasingClaimsOptionFactor));
            Assert.That(result.IndemnityOptionFactor, Is.EqualTo(expectedIndemnityOptionFactor));
            Assert.That(result.WaitingPeriodFactor, Is.EqualTo(expectedWaitingPeriodFactor));
            Assert.That(result.OccupationDefinitionFactor, Is.EqualTo(expectedOccupationDefinitionFactor));
            Assert.That(result.OccupationLoadingFactor, Is.EqualTo(expectedOccupationLoadingFactor));
        }

        [Test]
        public void GetFactorACalculatorInput_AllRepositoriesReturnValues_FactorAInputReturned()
        {
            var multiPlanDiscountFactor = .99m;
            var largeSumInsuredDiscountFactor = .98m;
            var occupationFactor = .97m;
            var smokingFactor = .96m;
            var increasingClaimsOptionFactor = .95m;
            var indemnityOptionFactor = .94m;
            var waitingPeriodFactor = .93m;

            var expectedOccupationDefinitionFactor = .92m;
            var expectedOccupationLoadingFactor = 1;

            var mockPremCalcRequest = GetMockPremiumCalculatorFactors();
            var allPlans = ((RiskCalculationRequest)mockPremCalcRequest.RiskFactors).Plans;

            _multiPlanDiscountService.Setup(
                call => call.GetFor(mockPremCalcRequest.PlanFactors, allPlans, 1)).Returns(multiPlanDiscountFactor);

            _largeSumInsuredDiscountFactorDtoRepository.Setup(
                call =>
                    call.GetLargeSumInsuredDiscountForSumInsured(mockPremCalcRequest.PlanFactors.CoverAmount,
                        mockPremCalcRequest.PlanFactors.PlanCode, mockPremCalcRequest.PolicyFactors.BrandId))
                .Returns(new LargeSumInsuredDiscountFactorDto {Factor = largeSumInsuredDiscountFactor });

            _occupationClassFactorDtoRepository.Setup(
                call =>
                    call.GetOccupationClassFactorByGenderOccupationClassAndPlan(mockPremCalcRequest.RiskFactors.Gender,
                        mockPremCalcRequest.RiskFactors.OccupationClass, mockPremCalcRequest.PlanFactors.PlanCode,
                        mockPremCalcRequest.PolicyFactors.BrandId))
                .Returns(new OccupationClassFactorDto {Factor = occupationFactor});

            _smokerFactorDtoRepository.Setup(
                call =>
                    call.GetSmokerFactorBySmokerAndPlan(mockPremCalcRequest.RiskFactors.Smoker,
                        mockPremCalcRequest.PlanFactors.PlanCode,
                        mockPremCalcRequest.PolicyFactors.BrandId))
                .Returns(new SmokerFactorDto {Factor = smokingFactor});

            _increasingClaimFactorDtoRepository.Setup(
                call =>
                    call.GetIncreasingClaimFactor(mockPremCalcRequest.PlanFactors.PlanCode,
                        mockPremCalcRequest.PolicyFactors.BrandId,
                        mockPremCalcRequest.PlanFactors.IncreasingClaimsSelected.Value,
                        mockPremCalcRequest.PlanFactors.BenefitPeriod))
                .Returns(new IncreasingClaimsFactorDto {Factor = increasingClaimsOptionFactor});

            _indemnityFactorDtoRepository.Setup(
                call => call.GetIndemnityFactorByPlanCode(mockPremCalcRequest.PlanFactors.PlanCode, mockPremCalcRequest.PolicyFactors.BrandId))
                .Returns(new IndemnityFactorDto {Factor = indemnityOptionFactor});

            _waitingPeriodFactorDtoRepository.Setup(
                call =>
                    call.GetWaitingPeriodFactorByWaitingPeriod(mockPremCalcRequest.PlanFactors.WaitingPeriod,
                        mockPremCalcRequest.PlanFactors.PlanCode, mockPremCalcRequest.PolicyFactors.BrandId))
                .Returns(new WaitingPeriodFactorDto {Factor = waitingPeriodFactor});

            _occupationDefinitionTypeFactorDtoRepository.Setup(
                call =>
                    call.GetOccupationDefinitionTypeFactorForOccupationDefinition(
                        mockPremCalcRequest.PlanFactors.OccupationDefinition, mockPremCalcRequest.PolicyFactors.BrandId))
                .Returns(new OccupationDefinitionTypeFactorDto {Factor = expectedOccupationDefinitionFactor});

            var svc = GetService();

            var result = svc.GetFactorACalculatorInput(mockPremCalcRequest, allPlans, 1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.MultiPlanDiscountFactor, Is.EqualTo(multiPlanDiscountFactor));
            Assert.That(result.LargeSumInsuredDiscountFactor, Is.EqualTo(largeSumInsuredDiscountFactor));

            Assert.That(result.OccupationFactor, Is.EqualTo(occupationFactor));
            Assert.That(result.SmokingFactor, Is.EqualTo(smokingFactor));
            Assert.That(result.IncreasingClaimsOptionFactor, Is.EqualTo(increasingClaimsOptionFactor));
            Assert.That(result.IndemnityOptionFactor, Is.EqualTo(indemnityOptionFactor));
            Assert.That(result.WaitingPeriodFactor, Is.EqualTo(waitingPeriodFactor));
            Assert.That(result.OccupationDefinitionFactor, Is.EqualTo(expectedOccupationDefinitionFactor));
            Assert.That(result.OccupationLoadingFactor, Is.EqualTo(expectedOccupationLoadingFactor));
        }

        private PremiumCalculatorFactors GetMockPremiumCalculatorFactors()
        {
            var brandId = 1;
            var premCalcRequest = new PremiumCalculationRequest(PremiumFrequency.Monthly,
                new List<RiskCalculationRequest>
                {
                    new RiskCalculationRequest(1, 18, Gender.Male, true, null, new List<PlanCalculationRequest>
                    {
                        new PlanCalculationRequest("ABC", true, true, 100000, PremiumType.Level, null, null, null, OccupationDefinition.Unknown, null, true,true, true,
                            new List<CoverCalculationRequest> {new CoverCalculationRequest("123", true, true, true, true, new Loadings(0, 0), brandId) }),
                    })
                }, brandId);

            var risk = premCalcRequest.Risks.First();
            var plan = risk.Plans.First();
            var cover = plan.Covers.First();
            return new PremiumCalculatorFactors(premCalcRequest, risk, plan, cover);
        }

        private GetFactorACalculatorInputService GetService()
        {
            return new GetFactorACalculatorInputService(_multiPlanDiscountService.Object, 
                _largeSumInsuredDiscountFactorDtoRepository.Object,
                _occupationClassFactorDtoRepository.Object,
                _smokerFactorDtoRepository.Object,
                _increasingClaimFactorDtoRepository.Object,
                _indemnityFactorDtoRepository.Object,
                _waitingPeriodFactorDtoRepository.Object,
                _occupationDefinitionTypeFactorDtoRepository.Object);
        }
    }
}
