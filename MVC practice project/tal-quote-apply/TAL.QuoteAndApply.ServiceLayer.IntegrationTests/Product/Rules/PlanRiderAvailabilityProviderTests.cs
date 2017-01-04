using System.Linq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Product;
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
    public class PlanRiderAvailabilityProviderTests : IServiceIntegrationTest<PlanRiderAvailabilityProvider>
    {
        private INameLookupService _nameLookupService;
        private IProductDefinitionBuilder _midMarketProductDefinitionBuilder;

        [SetUp]
        public void Setup()
        {
            _midMarketProductDefinitionBuilder = new ProductDefinitionBuilder(new MockProductBrandSettingsProvider());
            _nameLookupService = new NameLookupService(new PlanDefinitionProvider(_midMarketProductDefinitionBuilder), _midMarketProductDefinitionBuilder);
        }

        public PlanRiderAvailabilityProvider GetServiceInstance()
        {
            return new PlanRiderAvailabilityProvider(_midMarketProductDefinitionBuilder, 
               new SelectedProductPlanOptionsConverter(), _nameLookupService);
        }

        [Test]
        public void GetAvailableRiders_LifePlan_NoOtherPlanSelectedAndNoCoversSelected_RidersAvailable()
        {
            var service = GetServiceInstance();
            var inputModel = new AvailabilityPlanStateParam()
            {
                PlanCode = "DTH",
                SelectedCoverCodes = new string[0],
                SelectedPlanCodes = new string[0],
                SelectedRiderCodes = new string[0],
                SelectedRiderCoverCodes = new string[0],
                Age = 19,
                BrandKey = "TAL"
            };
            var availableItems = service.GetAvailableRiders(inputModel).ToList();
            Assert.That(availableItems, Is.Not.Empty);
            Assert.That(availableItems.Any(x=> x.Code == "TPDDTH"), Is.True);
            Assert.That(availableItems.Any(x => x.Code == "TRADTH"), Is.True);
        }

        [Test]
        public void GetAvailableRiders_LifePlan_TpdPlanSelectedAndNoCoversSelected_NoTpdRiderAvailable()
        {
            var service = GetServiceInstance();
            var inputModel = new AvailabilityPlanStateParam()
            {
                PlanCode = "DTH",
                SelectedCoverCodes = new string[0],
                SelectedPlanCodes = new [] {"TPS", "DTH"},
                SelectedRiderCodes = new string[0],
                SelectedRiderCoverCodes = new string[0],
                Age = 19,
                BrandKey = "TAL"
            };
            var availableItems = service.GetAvailableRiders(inputModel).ToList();
            Assert.That(availableItems, Is.Not.Empty);
            Assert.That(availableItems.Any(x => x.Code == "TPDDTH" && x.IsAvailable), Is.False);
            Assert.That(availableItems.Any(x => x.Code == "TRADTH" && x.IsAvailable), Is.True);
        }


        [Test]
        public void GetAvailableRiders_LifePlan_CiPlanSelectedAndNoCoversSelected_NoCiRiderAvailable()
        {
            var service = GetServiceInstance();
            var inputModel = new AvailabilityPlanStateParam()
            {
                PlanCode = "DTH",
                SelectedCoverCodes = new string[0],
                SelectedPlanCodes = new[] { "TRS", "DTH" },
                SelectedRiderCodes = new string[0],
                SelectedRiderCoverCodes = new string[0],
                Age = 19,
                BrandKey = "TAL"
            };
            var availableItems = service.GetAvailableRiders(inputModel).ToList();
            Assert.That(availableItems, Is.Not.Empty);
            Assert.That(availableItems.Any(x => x.Code == "TPDDTH" && x.IsAvailable), Is.True);
            Assert.That(availableItems.Any(x => x.Code == "TRADTH" && x.IsAvailable), Is.False);
        }

        [Test]
        public void GetAvailableRiders_LifePlan_CiAndTpdPlanSelectedAndNoCoversSelected_NoRidersAvailable()
        {
            var service = GetServiceInstance();
            var inputModel = new AvailabilityPlanStateParam()
            {
                PlanCode = "DTH",
                SelectedCoverCodes = new string[0],
                SelectedPlanCodes = new[] { "TRS", "TPS" },
                SelectedRiderCodes = new string[0],
                SelectedRiderCoverCodes = new string[0],
                Age = 19,
                BrandKey = "TAL"
            };
            var availableItems = service.GetAvailableRiders(inputModel);
            Assert.That(availableItems.Any(x=> x.IsAvailable), Is.False);
        }
        
        [Test]
        public void GetAvailableRiderCovers_LifePlan_LifePlanSelectedAndNoCoversSelected_TpdAndTraumaRidersAvaialableWithNoTPSorTraumaCoversAvailable()
        {
            var service = GetServiceInstance();
            var inputModel = new AvailabilityPlanStateParam()
            {
                PlanCode = "DTH",
                SelectedCoverCodes = new string[0],
                SelectedPlanCodes = new[] { "DTH" },
                SelectedRiderCodes = new string[0],
                SelectedRiderCoverCodes = new string[0],
                Age = 19,
                BrandKey = "TAL"
            };
            var availableRiders = service.GetAvailableRiders(inputModel);
            Assert.That(availableRiders.Any(x => x.Code == "TPDDTH" && x.IsAvailable), Is.True);
            Assert.That(availableRiders.Any(x => x.Code == "TRADTH" && x.IsAvailable), Is.True);

            var availableItems = service.GetAvailableRiderCovers("TPDDTH", inputModel).ToList();
            Assert.That(availableItems, Is.Not.Empty);
            Assert.That(availableItems.Any(x => x.Code == "TPDDTHAC" && x.IsAvailable), Is.False);
            Assert.That(availableItems.Any(x => x.Code == "TPDDTHIC" && x.IsAvailable), Is.False);
            Assert.That(availableItems.Any(x => x.Code == "TPDDTHASC" && x.IsAvailable), Is.False);

            availableItems = service.GetAvailableRiderCovers("TRADTH", inputModel).ToList();
            Assert.That(availableItems, Is.Not.Empty);
            Assert.That(availableItems.Any(x => x.Code == "TRADTHSIN" && x.IsAvailable), Is.False);
            Assert.That(availableItems.Any(x => x.Code == "TRADTHCC" && x.IsAvailable), Is.False);
            Assert.That(availableItems.Any(x => x.Code == "TRADTHSIC" && x.IsAvailable), Is.False);
        }
        
        [Test]
        public void GetAvailableRiderCovers_LifePlan_LifePlanSelectedAndTpdRiderSelectedRiderCoversSelected_TpdRiderAvailableWithTPSCoverAvailable()
        {

        var service = GetServiceInstance();
            var inputModel = new AvailabilityPlanStateParam()
            {
                PlanCode = "DTH",
                SelectedCoverCodes = new [] { "DTHAC", "DTHIC", "DTHASC" },
                SelectedPlanCodes = new string[0],
                SelectedRiderCodes = new [] { "TPDDTH" },
                SelectedRiderCoverCodes = new[] { "TPDDTHAC" },
                Age = 19,
                BrandKey = "TAL"
            };
            var availableRiders = service.GetAvailableRiders(inputModel).ToList();
            Assert.That(availableRiders.Any(x=> x.Code == "TPDDTH" && x.IsAvailable), Is.False);

            inputModel.SelectedPlanCodes = new[] {"DTH"};
            availableRiders = service.GetAvailableRiders(inputModel).ToList();
            Assert.That(availableRiders.Any(x => x.Code == "TPDDTH" && x.IsAvailable), Is.True);

            var availableItems = service.GetAvailableRiderCovers("TPDDTH", inputModel).ToList();
            Assert.That(availableItems, Is.Not.Empty);
            Assert.That(availableItems.Any(x => x.Code == "TPDDTHAC" && x.IsAvailable), Is.True);
            Assert.That(availableItems.Any(x => x.Code == "TPDDTHIC" && x.IsAvailable), Is.True);
            Assert.That(availableItems.Any(x => x.Code == "TPDDTHASC" && x.IsAvailable), Is.True);
        }

        [Test]
        public void GetAvailableRiderCovers_LifePlan_LifePlanSelectedAndTraumaRiderSelectedRiderCoversSelected_TraumaRiderAvailableWithTraumaCoverAvailable()
        {

            var service = GetServiceInstance();
            var inputModel = new AvailabilityPlanStateParam()
            {
                PlanCode = "DTH",
                SelectedCoverCodes = new[] { "DTHAC", "DTHIC", "DTHASC" },
                SelectedPlanCodes = new string[0],
                SelectedRiderCodes = new[] { "TRADTH" },
                SelectedRiderCoverCodes = new[] { "TRADTHSIN" },
                Age = 19,
                BrandKey = "TAL"
            };
            var availableRiders = service.GetAvailableRiders(inputModel).ToList();
            Assert.That(availableRiders.Any(x => x.Code == "TRADTH" && x.IsAvailable), Is.False);

            inputModel.SelectedPlanCodes = new[] { "DTH" };
            availableRiders = service.GetAvailableRiders(inputModel).ToList();
            Assert.That(availableRiders.Any(x => x.Code == "TRADTH" && x.IsAvailable), Is.True);

            var availableItems = service.GetAvailableRiderCovers("TRADTH", inputModel).ToList();
            Assert.That(availableItems, Is.Not.Empty);
            Assert.That(availableItems.Any(x => x.Code == "TRADTHSIN" && x.IsAvailable), Is.True);
            Assert.That(availableItems.Any(x => x.Code == "TRADTHCC" && x.IsAvailable), Is.True);
            Assert.That(availableItems.Any(x => x.Code == "TRADTHSIC" && x.IsAvailable), Is.True);
        }

    }

    
}

