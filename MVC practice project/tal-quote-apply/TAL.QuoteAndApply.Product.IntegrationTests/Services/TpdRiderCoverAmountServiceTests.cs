using NUnit.Framework;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.Product.Models;
using TAL.QuoteAndApply.Product.Service;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.Product.IntegrationTests.Services
{
    [TestFixture]
    public class TpdRiderCoverAmountServiceTests
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
        public void IsSatisfiedBy_LifePlanTpdRiderMinCoverAmount_Is10000()
        {
            //Arrange //Act
            var minCover = _coverAmountService.GetMinCover("TPDDTH", "tal");

            //Assert
            Assert.AreEqual(10000, minCover);
        }

        [TestCase(44)]
        [TestCase(45)]
        [TestCase(50)]
        [TestCase(51)]
        [TestCase(59)]
        public void GetMaxCover_AnyAge_IncomeMultipledByIncomeFactorLessThanUnrestrictedMaxCover_MaximumUnrestrictedCoverReturned(int age)
        {
            var maxCover = _coverAmountService.GetMaxCover(new MaxCoverAmountParam("TPDDTH", "tal", age, 10000, 100000, null));
            Assert.That(maxCover, Is.EqualTo(500000));
        }

        [TestCase(44)]
        [TestCase(45)]
        public void GetMaxCover_ToAge45Range_IncomeMultipledByIncomeFactorAboveMaxCover_MaxCoverReturned(int age)
        {
            var maxCover = _coverAmountService.GetMaxCover(new MaxCoverAmountParam("TPDDTH", "tal", age, 100000, 100000, null));
            Assert.That(maxCover, Is.EqualTo(1500000));
        }

        [TestCase(44)]
        [TestCase(45)]
        public void GetMaxCover_ToAge45Range_IncomeMultipledByIncomeFactorLessThanMaxCover_IncomeMultipledByIncomeFactorReturned(int age)
        {
            var maxCover = _coverAmountService.GetMaxCover(new MaxCoverAmountParam("TPDDTH", "tal", age, 50000, 100000, null));
            Assert.That(maxCover, Is.EqualTo(1000000));
        }

        [TestCase(44)]
        [TestCase(45)]
        public void GetMaxCover_ToAge45Range_ParentPlanCoverAmountLowerThanAgeRangeDefinition_ParentPlanCoverAmountReturned(int age)
        {
            var maxCover = _coverAmountService.GetMaxCover(new MaxCoverAmountParam("TPDDTH", "tal", age, 10000, 100000, 99999));
            Assert.That(maxCover, Is.EqualTo(99999));
        }

        [TestCase(46)]
        [TestCase(50)]
        public void GetMaxCover_46to50AgeRange_IncomeMultipledByIncomeFactorAboveMaxCover_MaxCoverReturned(int age)
        {
            var maxCover = _coverAmountService.GetMaxCover(new MaxCoverAmountParam("TPDDTH", "tal", age, 100000, 100000, null));
            Assert.That(maxCover, Is.EqualTo(1500000));
        }

        [TestCase(46)]
        [TestCase(50)]
        public void GetMaxCover_46to50AgeRange_IncomeMultipledByIncomeFactorLessThanMaxCover_IncomeMultipledByIncomeFactorReturned(int age)
        {
            var maxCover = _coverAmountService.GetMaxCover(new MaxCoverAmountParam("TPDDTH", "tal", age, 50000, 100000, null));
            Assert.That(maxCover, Is.EqualTo(900000));
        }
        
        [TestCase(46)]
        [TestCase(50)]
        public void GetMaxCover_46to50AgeRange_ParentPlanCoverAmountLowerThanAgeRangeDefinition_ParentPlanCoverAmountReturned(int age)
        {
            var maxCover = _coverAmountService.GetMaxCover(new MaxCoverAmountParam("TPDDTH", "tal", age, 100000, 100000, 99999));
            Assert.That(maxCover, Is.EqualTo(99999));
        }

        [TestCase(51)]
        [TestCase(55)]
        public void GetMaxCover_51to55AgeRange_IncomeMultipledByIncomeFactorAboveMaxCover_MaxCoverReturned(int age)
        {
            var maxCover = _coverAmountService.GetMaxCover(new MaxCoverAmountParam("TPDDTH", "tal", age, 100000, 100000, null));
            Assert.That(maxCover, Is.EqualTo(1000000));
        }

        [TestCase(51)]
        [TestCase(55)]
        public void GetMaxCover_51to55AgeRange_IncomeMultipledByIncomeFactorLessThanMaxCover_IncomeMultipledByIncomeFactorReturned(int age)
        {
            var maxCover = _coverAmountService.GetMaxCover(new MaxCoverAmountParam("TPDDTH", "tal", age, 50000, 100000, null));
            Assert.That(maxCover, Is.EqualTo(600000));
        }

        [TestCase(51)]
        [TestCase(55)]
        public void GetMaxCover_51to55AgeRange_ParentPlanCoverAmountLowerThanAgeRangeDefinition_ParentPlanCoverAmountReturned(int age)
        {
            var maxCover = _coverAmountService.GetMaxCover(new MaxCoverAmountParam("TPDDTH", "tal", age, 100000, 100000, 99999));
            Assert.That(maxCover, Is.EqualTo(99999));
        }

        [TestCase(56)]
        [TestCase(59)]
        public void GetMaxCover_56to59AgeRange_IncomeMultipledByIncomeFactorAboveMaxCover_MaxCoverReturned(int age)
        {
            var maxCover = _coverAmountService.GetMaxCover(new MaxCoverAmountParam("TPDDTH", "tal", age, 200000, 100000, null));
            Assert.That(maxCover, Is.EqualTo(750000));
        }

        [TestCase(56)]
        [TestCase(59)]
        public void GetMaxCover_56to59AgeRange_ParentPlanCoverAmountLowerThanAgeRangeDefinition_ParentPlanCoverAmountReturned(int age)
        {
            var maxCover = _coverAmountService.GetMaxCover(new MaxCoverAmountParam("TPDDTH", "tal", age, 100000, 100000, 99999));
            Assert.That(maxCover, Is.EqualTo(99999));
        }

        [Test]
        public void GetMaxCover_AgeAbove59_ExceptionThrown()
        {
            var maxCover = _coverAmountService.GetMaxCover(new MaxCoverAmountParam("TPDDTH", "tal", 60, 200000, 100000, null));
            Assert.That(maxCover, Is.EqualTo(0));
        }
    }
}
