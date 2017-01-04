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
    public class GetLoadingCalculatorInputServiceTests
    {
        private Mock<IPercentageLoadingFactorDtoRepository> _mockPercentageLoadingFactorRepo;
        private Mock<IPerMilleLoadingFactorDtoRepository> _mockPerMilleLoadingFactorRepo;
        private Mock<IGetFactorBCalculatorInputService> _mockGetFactorBCalculatorInputService;

        const int BrandId = 1;

        [TestFixtureSetUp]
        public void Setup()
        {
            var mockRepo = new MockRepository(MockBehavior.Strict);
            _mockPercentageLoadingFactorRepo = mockRepo.Create<IPercentageLoadingFactorDtoRepository>();
            _mockPerMilleLoadingFactorRepo = mockRepo.Create<IPerMilleLoadingFactorDtoRepository>();
            _mockGetFactorBCalculatorInputService = mockRepo.Create<IGetFactorBCalculatorInputService>();
        }

        [Test]
        public void GetLoadingCalculatorInput_PercentageAndPerMilleLoadingFactorsExist()
        {
            var mockPremiumCalculatorFactors = new PremiumCalculatorFactors(new PremiumCalculationRequest(PremiumFrequency.Yearly, null, BrandId),
                new RiskCalculationRequest(1, 20, Gender.Female, true, "AAA", null),
                new PlanCalculationRequest("ABC", true, true, 10000, PremiumType.Level, null, null, null, OccupationDefinition.Unknown, null,null, null, null, null),
                new CoverCalculationRequest("ABC", true, true, true, true, new Loadings(0,0), BrandId));

            _mockGetFactorBCalculatorInputService.Setup(
                call => call.GetFactorBCalculatorInput(mockPremiumCalculatorFactors)).Returns(
                    new FactorBCalculatorInput(1, 1));

            _mockPercentageLoadingFactorRepo.Setup(
                call => call.GetPercentageLoadingFactorByCoverCode(mockPremiumCalculatorFactors.CoverFactors.CoverCode, BrandId))
                .Returns(new PercentageLoadingFactorDto {Factor = 999});

            _mockPerMilleLoadingFactorRepo.Setup(
                call => call.GetPerMilleLoadingFactorByCoverCode(mockPremiumCalculatorFactors.CoverFactors.CoverCode, BrandId))
                .Returns(new PerMilleLoadingFactorDto { Factor = 987 });

            var svc = GetService();

            var result = svc.GetLoadingCalculatorInput(mockPremiumCalculatorFactors, 100);
            
            Assert.That(result.FactorB, Is.EqualTo(1));
            Assert.That(result.PerMilleLoadingFactor, Is.EqualTo(987));
            Assert.That(result.PercentageLoadingFactor, Is.EqualTo(999));
        }

        [Test]
        public void GetLoadingCalculatorInput_NoPercentageLoadingFactor_ZeroReturned()
        {
            var mockPremiumCalculatorFactors = new PremiumCalculatorFactors(new PremiumCalculationRequest(PremiumFrequency.Yearly, null, BrandId),
                new RiskCalculationRequest(1, 20, Gender.Female, true, "AAA",  null),
                new PlanCalculationRequest("ABC", true, true, 10000, PremiumType.Level, null, null, null, OccupationDefinition.Unknown, null, null, null, null, null),
                new CoverCalculationRequest("ABC", true, true, true, true, new Loadings(0, 0), BrandId));

            _mockGetFactorBCalculatorInputService.Setup(
                call => call.GetFactorBCalculatorInput(mockPremiumCalculatorFactors)).Returns(
                    new FactorBCalculatorInput(1, 1));

            _mockPercentageLoadingFactorRepo.Setup(
                call => call.GetPercentageLoadingFactorByCoverCode(mockPremiumCalculatorFactors.CoverFactors.CoverCode, BrandId))
                .Returns(()=> null);

            _mockPerMilleLoadingFactorRepo.Setup(
                call => call.GetPerMilleLoadingFactorByCoverCode(mockPremiumCalculatorFactors.CoverFactors.CoverCode, BrandId))
                .Returns(new PerMilleLoadingFactorDto { Factor = 987 });

            var svc = GetService();

            var result = svc.GetLoadingCalculatorInput(mockPremiumCalculatorFactors, 100);

            Assert.That(result.PercentageLoadingFactor, Is.EqualTo(0));
        }

        [Test]
        public void GetLoadingCalculatorInput_NoPerMilleLoadingFactor_ZeroReturned()
        {
            var mockPremiumCalculatorFactors = new PremiumCalculatorFactors(new PremiumCalculationRequest(PremiumFrequency.Yearly, null, BrandId),
                new RiskCalculationRequest(1, 20, Gender.Female, true, "AAA", null),
                new PlanCalculationRequest("ABC", true, true, 10000, PremiumType.Level, null, null, null, OccupationDefinition.Unknown, null, null, null, null, null),
                new CoverCalculationRequest("ABC", true, true, true, true, new Loadings(0, 0), BrandId));

            _mockGetFactorBCalculatorInputService.Setup(
                call => call.GetFactorBCalculatorInput(mockPremiumCalculatorFactors)).Returns(
                    new FactorBCalculatorInput(1, 1));

            _mockPercentageLoadingFactorRepo.Setup(
                call => call.GetPercentageLoadingFactorByCoverCode(mockPremiumCalculatorFactors.CoverFactors.CoverCode, BrandId))
                .Returns(new PercentageLoadingFactorDto { Factor = 999 });

            _mockPerMilleLoadingFactorRepo.Setup(
                call => call.GetPerMilleLoadingFactorByCoverCode(mockPremiumCalculatorFactors.CoverFactors.CoverCode, BrandId))
                .Returns(()=> null);

            var svc = GetService();

            var result = svc.GetLoadingCalculatorInput(mockPremiumCalculatorFactors, 100);

            Assert.That(result.PerMilleLoadingFactor, Is.EqualTo(0));
        }

        private GetLoadingCalculatorInputService GetService()
        {
            return new GetLoadingCalculatorInputService(_mockPercentageLoadingFactorRepo.Object, 
                _mockPerMilleLoadingFactorRepo.Object, _mockGetFactorBCalculatorInputService.Object);
        }
    }
}