using System.Collections.Generic;
using NUnit.Framework;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.Product.Models;
using TAL.QuoteAndApply.Product.Models.Definition;
using TAL.QuoteAndApply.Product.Models.Types;
using TAL.QuoteAndApply.Product.Rules.Common;
using TAL.QuoteAndApply.Product.Service;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.Product.IntegrationTests.Rules
{
    [TestFixture]
    public class PlanCoverAvailabilityRuleTests
    {
        private INameLookupService _nameLookupService;
        private IProductDefinitionBuilder _midMarketProductDefinitionBuilder;

        [SetUp]
        public void Setup()
        {
            _midMarketProductDefinitionBuilder = new ProductDefinitionBuilder(new MockProductBrandSettingsProvider());
            _nameLookupService = new NameLookupService(new PlanDefinitionProvider(_midMarketProductDefinitionBuilder), _midMarketProductDefinitionBuilder);
        }

        [Test]
        public void IsSatisfiedBy_NoFeatureRuleDefined_ShouldBeSatisfiedAlways()
        {
            var coverDefinition = new CoverDefinition("Accident", "Accident", true);
            var rule = new AvailabilityRule(coverDefinition, _nameLookupService);

            var selectedPlans = new SelectedPlans(new [] {"DTH", "IP"});
            var selectedCovers = new SelectedCovers(new[] { "SomeCoverA", "SomeCoverB" });
            var selectedProdPlanOpts = new SelectedProductPlanOptions("DTH", "tal", selectedPlans, selectedCovers, 60, null, null, null);

            var result = rule.IsSatisfiedBy(selectedProdPlanOpts);
            Assert.That(result.IsSatisfied, Is.True);
        }

        [Test]
        public void IsSatisfiedBy_FeatureDefinedPlanMustBeSelected_ShouldBeSatisfiedWhenPlanIsSelected()
        {
            var coverDefinition = new CoverDefinition("Accident", "Accident", true, new FeatureRule(new List<ProductConfigRule>()
                                    {
                                        new ProductConfigRule(Should.BeAllSelected, From.Product, "TPS")
                                    }, null)
                                );
            var rule = new AvailabilityRule(coverDefinition, _nameLookupService);

            var selectedPlans = new SelectedPlans(new[] { "DTH", "IP" });
            var selectedCovers = new SelectedCovers(new[] { "SomeCoverA", "SomeCoverB" });
            var selectedProdPlanOpts = new SelectedProductPlanOptions("DTH", "tal", selectedPlans, selectedCovers, new string[0], new string[0], 60, null, null, null);

            var result = rule.IsSatisfiedBy(selectedProdPlanOpts);
            Assert.That(result.IsSatisfied, Is.False);

            selectedPlans = new SelectedPlans(new[] { "DTH", "IP", "TPS" });
            selectedProdPlanOpts = new SelectedProductPlanOptions("DTH", "tal", selectedPlans, selectedCovers, new string[0], new string[0], 60, null, null, null);

            result = rule.IsSatisfiedBy(selectedProdPlanOpts);
            Assert.That(result.IsSatisfied, Is.True);
        }

        [Test]
        public void IsSatisfiedBy_FeatureDefinedCoverMustBeSelected_ShouldBeSatisfiedWhenCoverIsSelected()
        {
            var coverDefinition = new CoverDefinition("Accident", "Accident", true, 
                new FeatureRule(new List<ProductConfigRule>(), new List<ProductConfigRule>() { new ProductConfigRule(Should.BeAllSelected, From.CurrentPlan, "SomeCoverC")}));
            var rule = new AvailabilityRule(coverDefinition, _nameLookupService);

            var selectedPlans = new SelectedPlans(new[] { "DTH", "IP" });
            var selectedCovers = new SelectedCovers(new[] { "SomeCoverA", "SomeCoverB" });
            var selectedProdPlanOpts = new SelectedProductPlanOptions("DTH", "tal", selectedPlans, selectedCovers, new string[0], new string[0], 60, null, null, null);

            var result = rule.IsSatisfiedBy(selectedProdPlanOpts);
            Assert.That(result.IsSatisfied, Is.False);

            selectedCovers = new SelectedCovers(new[] { "SomeCoverC", "SomeCoverB" });
            selectedProdPlanOpts = new SelectedProductPlanOptions("DTH", "tal", selectedPlans, selectedCovers, new string[0], new string[0], 60, null, null, null);

            result = rule.IsSatisfiedBy(selectedProdPlanOpts);
            Assert.That(result.IsSatisfied, Is.True);
        }

        [Test]
        public void IsSatisfiedBy_FeatureDefinedMultiPlanMustBeSelected_ShouldBeSatisfiedWhenPlansAreSelected()
        {
            var coverDefinition = new CoverDefinition("Accident", "Accident", true, new FeatureRule(new List<ProductConfigRule>()
                                    {
                                        new ProductConfigRule(Should.BeAllSelected, From.Product, "TPS", "IP")
                                    }, null)
                                );
            var rule = new AvailabilityRule(coverDefinition, _nameLookupService);

            var selectedPlans = new SelectedPlans(new[] { "DTH", "TPS" });
            var selectedCovers = new SelectedCovers(new[] { "SomeCoverA", "SomeCoverB" });
            var selectedProdPlanOpts = new SelectedProductPlanOptions("DTH", "tal", selectedPlans, selectedCovers, new string[0], new string[0], 60, null, null, null);

            var result = rule.IsSatisfiedBy(selectedProdPlanOpts);
            Assert.That(result.IsSatisfied, Is.False);

            selectedPlans = new SelectedPlans(new[] { "DTH", "IP", "TPS" });
            selectedProdPlanOpts = new SelectedProductPlanOptions("DTH", "tal", selectedPlans, selectedCovers, new string[0], new string[0], 60, null, null, null);

            result = rule.IsSatisfiedBy(selectedProdPlanOpts);
            Assert.That(result.IsSatisfied, Is.True);
        }
        
        [Test]
        public void IsSatisfiedBy_FeatureDefinedMultiCoverMustBeSelected_ShouldBeSatisfiedWhenCoversAreSelected()
        {
            var coverDefinition = new CoverDefinition("Accident", "Accident", true, 
                new FeatureRule(new List<ProductConfigRule>(), new List<ProductConfigRule>() { new ProductConfigRule(Should.BeAllSelected, From.CurrentPlan, "SomeCoverB", "SomeCoverC")}));
            var rule = new AvailabilityRule(coverDefinition, _nameLookupService);

            var selectedPlans = new SelectedPlans(new[] { "DTH", "IP" });
            var selectedCovers = new SelectedCovers(new[] { "SomeCoverA", "SomeCoverB" });
            var selectedProdPlanOpts = new SelectedProductPlanOptions("DTH", "tal", selectedPlans, selectedCovers, new string[0], new string[0], 60, null, null, null);

            var result = rule.IsSatisfiedBy(selectedProdPlanOpts);
            Assert.That(result.IsSatisfied, Is.False);

            selectedCovers = new SelectedCovers(new[] { "SomeCoverC", "SomeCoverB" });
            selectedProdPlanOpts = new SelectedProductPlanOptions("DTH", "tal", selectedPlans, selectedCovers, new string[0], new string[0], 60, null, null, null);

            result = rule.IsSatisfiedBy(selectedProdPlanOpts);
            Assert.That(result.IsSatisfied, Is.True);
        }

        [Test]
        public void IsSatisfiedBy_FeatureDefinedPlanAndCoverMustBeSelected_ShouldBeSatisfiedWhenPlanAndCoverIsSelected()
        {
            var coverDefinition = new CoverDefinition("Accident", "Accident", true, 
                new FeatureRule(new List<ProductConfigRule>() { new ProductConfigRule(Should.BeAllSelected, From.Product, "TPS")},
                    new List<ProductConfigRule>() { new ProductConfigRule(Should.BeAllSelected, From.CurrentPlan, "SomeCoverC")}));
            var rule = new AvailabilityRule(coverDefinition, _nameLookupService);

            var selectedPlans = new SelectedPlans(new[] { "LIFE", "TPS" });
            var selectedCovers = new SelectedCovers(new[] { "SomeCoverA", "SomeCoverB" });
            var selectedProdPlanOpts = new SelectedProductPlanOptions("DTH", "tal", selectedPlans, selectedCovers, new string[0], new string[0], 60, null, null, null);

            var result = rule.IsSatisfiedBy(selectedProdPlanOpts);
            Assert.That(result.IsSatisfied, Is.False);

            selectedCovers = new SelectedCovers(new[] { "SomeCoverC", "SomeCoverB" });
            selectedProdPlanOpts = new SelectedProductPlanOptions("DTH", "tal", selectedPlans, selectedCovers, new string[0], new string[0], 60, null, null, null);

            result = rule.IsSatisfiedBy(selectedProdPlanOpts);
            Assert.That(result.IsSatisfied, Is.True);
        }
    }
}
