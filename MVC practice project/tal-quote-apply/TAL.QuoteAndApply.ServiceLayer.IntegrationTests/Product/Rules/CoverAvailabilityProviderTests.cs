using System.Linq;
using NUnit.Framework;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.Product.Service;
using TAL.QuoteAndApply.ServiceLayer.Models;
using TAL.QuoteAndApply.ServiceLayer.Models.Mappers;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Product.Rules;
using TAL.QuoteAndApply.Tests.Shared;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.ServiceLayer.IntegrationTests.Product.Rules
{
    [TestFixture]
    public class CoverAvailabilityProviderTests : IServiceIntegrationTest<CoverAvailabilityProvider>
    {
        private INameLookupService _nameLookupService;
        private IProductDefinitionBuilder _midMarketProductDefinitionBuilder;

        [SetUp]
        public void Setup()
        {
            _midMarketProductDefinitionBuilder = new ProductDefinitionBuilder(new MockProductBrandSettingsProvider());
            _nameLookupService = new NameLookupService(new PlanDefinitionProvider(_midMarketProductDefinitionBuilder), _midMarketProductDefinitionBuilder);
        }

        public CoverAvailabilityProvider GetServiceInstance()
        {
            return new CoverAvailabilityProvider(_midMarketProductDefinitionBuilder, 
               new SelectedProductPlanOptionsConverter(), _nameLookupService);
        }

        [Test]
        public void GetAvailableCovers_LifePlan_NoOtherPlanSelectedAndNoCoversSelected_NoSportsAvailable()
        {
            var service = GetServiceInstance();
            var inputModel = new AvailabilityPlanStateParam()
            {
                PlanCode = "DTH",
                SelectedCoverCodes = new string[0],
                SelectedPlanCodes = new string[0],
                SelectedRiderCodes = new string[0],
                SelectedRiderCoverCodes = new string[0],
                BrandKey = "TAL"
            };
            var availableItems = service.GetAvailableCovers(inputModel).ToList();
            Assert.That(availableItems, Is.Not.Empty);
            Assert.That(availableItems.Any(x=> x.Code == "DTHASC" && x.IsAvailable), Is.False);
            Assert.That(availableItems.Any(x => x.Code == "DTHIC" && x.IsAvailable), Is.True);
            Assert.That(availableItems.Any(x => x.Code == "DTHAC" && x.IsAvailable), Is.True);
        }

        [Test]
        public void GetAvailableCovers_LifePlan_NoOtherPlanSelectedAndAtLeastOneCoversSelected_SportsAvailable()
        {
            var service = GetServiceInstance();
            var inputModel = new AvailabilityPlanStateParam()
            {
                PlanCode = "DTH",
                SelectedCoverCodes = new [] { "DTHAC" },
                SelectedPlanCodes = new string[0],
                SelectedRiderCodes = new string[0],
                SelectedRiderCoverCodes = new string[0],
                BrandKey = "TAL"
            };
            var availableItems = service.GetAvailableCovers(inputModel).ToList();
            Assert.That(availableItems, Is.Not.Empty);
            Assert.That(availableItems.Any(x => x.Code == "DTHASC" && x.IsAvailable), Is.True);
            Assert.That(availableItems.Any(x => x.Code == "DTHIC" && x.IsAvailable), Is.True);
            Assert.That(availableItems.Any(x => x.Code == "DTHAC" && x.IsAvailable), Is.True);
        }
    }

    
}

