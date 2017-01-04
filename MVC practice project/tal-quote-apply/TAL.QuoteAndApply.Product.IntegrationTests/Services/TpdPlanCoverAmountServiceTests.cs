using NUnit.Framework;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.Product.Models;
using TAL.QuoteAndApply.Product.Service;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.Product.IntegrationTests.Services
{
    [TestFixture]
    public class TpdPlanCoverAmountServiceTests
    {
        private IProductDefinitionBuilder _productDefinitionBuilderMock;
        private IPlanDefinitionProvider _planDefinitionProvider;
        private CoverAmountService _coverAmountService;

        [SetUp]
        public void Setup()
        {
            _productDefinitionBuilderMock = new ProductDefinitionBuilder(new MockProductBrandSettingsProvider());
            _planDefinitionProvider = new PlanDefinitionProvider(_productDefinitionBuilderMock);

            var maxCoverAmountForAgeProvider = new MaxCoverAmountForAgeProvider(_planDefinitionProvider);
            var maxCoverAmountPercentageOfIncomeProvider = new MaxCoverAmountPercentageOfIncomeProvider(_planDefinitionProvider);
            _coverAmountService = new CoverAmountService(_planDefinitionProvider, maxCoverAmountForAgeProvider, maxCoverAmountPercentageOfIncomeProvider);
        }

        [Test]
        public void GetMinCover_TpdPlanMinCoverAmount_Is100000()
        {
            //Arrange //Act
            var minCover = _coverAmountService.GetMinCover("TPS", "tal");

            //Assert
            Assert.NotNull(minCover);
            Assert.AreEqual(100000, minCover);
        }

        [TestCase(44)]
        [TestCase(45)]
        public void GetMaxCover_ToAge45Range_IncomeMultipledByIncomeFactorAboveMaxCover_MaxCoverReturned(int age)
        {
            var maxCover = _coverAmountService.GetMaxCover(new MaxCoverAmountParam("TPS", "tal", age, 100000, 100000, null));
            Assert.That(maxCover, Is.EqualTo(1500000));
        }

        [TestCase(44)]
        [TestCase(45)]
        public void GetMaxCover_ToAge45Range_IncomeMultipledByIncomeFactorLessThanMaxCover_IncomeMultipledByIncomeFactorReturned(int age)
        {
            var maxCover = _coverAmountService.GetMaxCover(new MaxCoverAmountParam("TPS", "tal", age, 50000, 100000, null));
            Assert.That(maxCover, Is.EqualTo(1000000));
        }

        [TestCase(46)]
        [TestCase(50)]
        public void GetMaxCover_46to50AgeRange_IncomeMultipledByIncomeFactorAboveMaxCover_MaxCoverReturned(int age)
        {
            var maxCover = _coverAmountService.GetMaxCover(new MaxCoverAmountParam("TPS", "tal", age, 100000, 100000, null));
            Assert.That(maxCover, Is.EqualTo(1500000));
        }

        [TestCase(46)]
        [TestCase(50)]
        public void GetMaxCover_46to50AgeRange_IncomeMultipledByIncomeFactorLessThanMaxCover_IncomeMultipledByIncomeFactorReturned(int age)
        {
            var maxCover = _coverAmountService.GetMaxCover(new MaxCoverAmountParam("TPS", "tal", age, 50000, 100000, null));
            Assert.That(maxCover, Is.EqualTo(900000));
        }

        [TestCase(51)]
        [TestCase(55)]
        public void GetMaxCover_51to55AgeRange_IncomeMultipledByIncomeFactorAboveMaxCover_MaxCoverReturned(int age)
        {
            var maxCover = _coverAmountService.GetMaxCover(new MaxCoverAmountParam("TPS", "tal", age, 100000, 100000, null));
            Assert.That(maxCover, Is.EqualTo(1000000));
        }

        [TestCase(51)]
        [TestCase(55)]
        public void GetMaxCover_51to55AgeRange_IncomeMultipledByIncomeFactorLessThanMaxCover_IncomeMultipledByIncomeFactorReturned(int age)
        {
            var maxCover = _coverAmountService.GetMaxCover(new MaxCoverAmountParam("TPS", "tal", age, 50000, 100000, null));
            Assert.That(maxCover, Is.EqualTo(600000));
        }

        [TestCase(56)]
        [TestCase(59)]
        public void GetMaxCover_56to59AgeRange_IncomeMultipledByIncomeFactorAboveMaxCover_MaxCoverReturned(int age)
        {
            var maxCover = _coverAmountService.GetMaxCover(new MaxCoverAmountParam("TPS", "tal", age, 200000, 100000, null));
            Assert.That(maxCover, Is.EqualTo(750000));
        }

        [TestCase(56)]
        [TestCase(59)]
        public void GetMaxCover_56to59AgeRange_IncomeMultipledByIncomeFactorLessThanMaxCover_IncomeMultipledByIncomeFactorReturned(int age)
        {
            var maxCover = _coverAmountService.GetMaxCover(new MaxCoverAmountParam("TPS", "tal", age, 120000, 100000, null));
            Assert.That(maxCover, Is.EqualTo(600000));
        }

        [TestCase(60)]
        [TestCase(61)]
        [TestCase(70)]
        public void GetMaxCover_AgeAbove59_ExceptionThrown(int age)
        {
            var maxCover = _coverAmountService.GetMaxCover(new MaxCoverAmountParam("TPS", "tal", age, 200000, 100000, null));
            Assert.That(maxCover, Is.EqualTo(0));
        }
    }
}
