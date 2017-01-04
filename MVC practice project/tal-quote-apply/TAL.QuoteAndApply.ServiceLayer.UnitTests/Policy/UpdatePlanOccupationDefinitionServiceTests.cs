using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk;
using TAL.QuoteAndApply.ServiceLayer.Product;

namespace TAL.QuoteAndApply.ServiceLayer.UnitTests.Policy
{
    [TestFixture]
    public class UpdatePlanOccupationDefinitionServiceTests
    {
        private Mock<IPlanService> _mockPlanService;
        private Mock<IPlanDefinitionProvider> _mockPlanDefinitionProvider;
        private Mock<IProductBrandProvider> _mockProductBrandProvider;

        [SetUp]
        public void Setup()
        {
            _mockPlanService = new Mock<IPlanService>();
            _mockProductBrandProvider = new Mock<IProductBrandProvider>();
            _mockPlanDefinitionProvider = new Mock<IPlanDefinitionProvider>();
        }

        [Test]
        public void GetOccupationDefinition_UnknownNoAnyNoOwn_Unknown()
        {
            var service = new UpdatePlanOccupationDefinitionService(_mockPlanService.Object, _mockPlanDefinitionProvider.Object, _mockProductBrandProvider.Object);
            var result = service.GetOccupationDefinition(OccupationDefinition.Unknown, false, false);

            Assert.That(result, Is.EqualTo(OccupationDefinition.Unknown));
        }

        [Test]
        public void GetOccupationDefinition_UnknownYesAnyNoOwn_AnyOccupation()
        {
            var service = new UpdatePlanOccupationDefinitionService(_mockPlanService.Object, _mockPlanDefinitionProvider.Object, _mockProductBrandProvider.Object);
            var result = service.GetOccupationDefinition(OccupationDefinition.Unknown, true, false);

            Assert.That(result, Is.EqualTo(OccupationDefinition.AnyOccupation));
        }

        [Test]
        public void GetOccupationDefinition_UnknownNoAnyYesOwn_OwnOccupation()
        {
            var service = new UpdatePlanOccupationDefinitionService(_mockPlanService.Object, _mockPlanDefinitionProvider.Object, _mockProductBrandProvider.Object);
            var result = service.GetOccupationDefinition(OccupationDefinition.Unknown, false, true);

            Assert.That(result, Is.EqualTo(OccupationDefinition.OwnOccupation));
        }

        [Test]
        public void GetOccupationDefinition_UnknownYesAnyYesOwn_AnyOccupation()
        {
            var service = new UpdatePlanOccupationDefinitionService(_mockPlanService.Object, _mockPlanDefinitionProvider.Object, _mockProductBrandProvider.Object);
            var result = service.GetOccupationDefinition(OccupationDefinition.Unknown, true, true);

            Assert.That(result, Is.EqualTo(OccupationDefinition.AnyOccupation));
        }

        [Test]
        public void GetOccupationDefinition_AnyNoAnyNoOwn_Unknown()
        {
            var service = new UpdatePlanOccupationDefinitionService(_mockPlanService.Object, _mockPlanDefinitionProvider.Object, _mockProductBrandProvider.Object);
            var result = service.GetOccupationDefinition(OccupationDefinition.AnyOccupation, false, false);

            Assert.That(result, Is.EqualTo(OccupationDefinition.Unknown));
        }

        [Test]
        public void GetOccupationDefinition_AnyYesAnyNoOwn_AnyOccupation()
        {
            var service = new UpdatePlanOccupationDefinitionService(_mockPlanService.Object, _mockPlanDefinitionProvider.Object, _mockProductBrandProvider.Object);
            var result = service.GetOccupationDefinition(OccupationDefinition.AnyOccupation, true, false);

            Assert.That(result, Is.EqualTo(OccupationDefinition.AnyOccupation));
        }

        [Test]
        public void GetOccupationDefinition_AnyYesAnyYesOwn_AnyOccupation()
        {
            var service = new UpdatePlanOccupationDefinitionService(_mockPlanService.Object, _mockPlanDefinitionProvider.Object, _mockProductBrandProvider.Object);
            var result = service.GetOccupationDefinition(OccupationDefinition.AnyOccupation, true, true);

            Assert.That(result, Is.EqualTo(OccupationDefinition.AnyOccupation));
        }

        [Test]
        public void GetOccupationDefinition_OwnNoAnyNoOwn_Unknown()
        {
            var service = new UpdatePlanOccupationDefinitionService(_mockPlanService.Object, _mockPlanDefinitionProvider.Object, _mockProductBrandProvider.Object);
            var result = service.GetOccupationDefinition(OccupationDefinition.OwnOccupation, false, false);

            Assert.That(result, Is.EqualTo(OccupationDefinition.Unknown));
        }

        [Test]
        public void GetOccupationDefinition_OwnYesAnyNoOwn_AnyOccupation()
        {
            var service = new UpdatePlanOccupationDefinitionService(_mockPlanService.Object, _mockPlanDefinitionProvider.Object, _mockProductBrandProvider.Object);
            var result = service.GetOccupationDefinition(OccupationDefinition.OwnOccupation, true, false);

            Assert.That(result, Is.EqualTo(OccupationDefinition.AnyOccupation));
        }

        [Test]
        public void GetOccupationDefinition_OwnNoAnyYesOwn_OwnOccupation()
        {
            var service = new UpdatePlanOccupationDefinitionService(_mockPlanService.Object, _mockPlanDefinitionProvider.Object, _mockProductBrandProvider.Object);
            var result = service.GetOccupationDefinition(OccupationDefinition.OwnOccupation, false, true);

            Assert.That(result, Is.EqualTo(OccupationDefinition.OwnOccupation));
        }

        [Test]
        public void GetOccupationDefinition_OwnYesAnyYesOwn_AnyOccupation()
        {
            var service = new UpdatePlanOccupationDefinitionService(_mockPlanService.Object, _mockPlanDefinitionProvider.Object, _mockProductBrandProvider.Object);
            var result = service.GetOccupationDefinition(OccupationDefinition.OwnOccupation, true, true);

            Assert.That(result, Is.EqualTo(OccupationDefinition.OwnOccupation));
        }
    }
}
