using NUnit.Framework;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.Product.Models;
using TAL.QuoteAndApply.Product.Service;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.Product.IntegrationTests.Services
{
    [TestFixture]
    public class IpPlanCoverAmountServiceTests
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
        public void GetMinCover_LifePlanMinCoverAmount_Is1000()
        {
            //Arrange //Act
            var minCover = _coverAmountService.GetMinCover("IP", "tal");

            //Assert
            Assert.NotNull(minCover);
            Assert.AreEqual(1000, minCover);
        }

        [Test]
        public void GetMaxCover_PercentageOfMonthlyIncomeIsOver10000_MaxCover10000Returned()
        {
            var maxCover = _coverAmountService.GetMaxCover(new MaxCoverAmountParam("IP", "tal", 20, 200000, 1000, null));
            Assert.That(maxCover, Is.EqualTo(10000));
        }

        [Test]
        public void GetMaxCover_PercentageOfMonthlyIncomeIsUnder10000_PercentageValueReturned()
        {
            var maxCover = _coverAmountService.GetMaxCover(new MaxCoverAmountParam("IP", "tal", 20, 100000, 1000, null));
            Assert.That(maxCover, Is.EqualTo(6250));
        }

    }
}