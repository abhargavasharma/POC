using System;
using NUnit.Framework;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.Product.Models;
using TAL.QuoteAndApply.Product.Service;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.Product.IntegrationTests.Services
{
    [TestFixture]
    public class CiPlanCoverAmountServiceTests
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
            var minCover = _coverAmountService.GetMinCover("TRS", "tal");

            //Assert
            Assert.AreEqual(100000, minCover);
        }

        [TestCase(44, 10000)]
        [TestCase(45, 10000)]
        [TestCase(46, 10000)]
        [TestCase(50, 10000)]
        [TestCase(51, 10000)]
        [TestCase(55, 10000)]
        [TestCase(56, 10000)]
        [TestCase(60, 10000)]

        [TestCase(44, 500000)]
        [TestCase(45, 500000)]
        [TestCase(46, 500000)]
        [TestCase(50, 500000)]
        [TestCase(51, 500000)]
        [TestCase(55, 500000)]
        [TestCase(56, 500000)]
        [TestCase(60, 500000)]
        public void GetMaxCover_AnyAge_AnyIncome_MaxCoverReturned(int age, int income)
        {
            var maxCover = _coverAmountService.GetMaxCover(new MaxCoverAmountParam("TRS", "tal", age, income, 100000, null));
            Assert.That(maxCover, Is.EqualTo(500000));
        }
       
        [Test]
        public void GetMaxCover_AgeAbove60_ReturnsZero()
        {
            var maxCover = _coverAmountService.GetMaxCover(new MaxCoverAmountParam("TRS", "tal", 61, 200000, 100000, null));
            Assert.That(maxCover, Is.EqualTo(0));
        }
    }
}
