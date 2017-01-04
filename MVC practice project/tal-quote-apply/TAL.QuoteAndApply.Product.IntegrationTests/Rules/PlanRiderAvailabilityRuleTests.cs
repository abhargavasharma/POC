using System.Collections.Generic;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Product;
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
    public class PlanRiderAvailabilityRuleTests
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
        public void IsSatisfiedBy_FeatureDefinedAgeMustbeGreaterThanMinimumEntryAgeNextBirthday_ShouldBeSatisfiedWhenApplicableAge()
        {
            var riderDefinition = new PlanDefinition()
            {
                RuleDefinition = new FeatureRule(new List<ProductConfigRule>()
                                    {
                                        new ProductConfigRule(Should.BeAllSelected, From.CurrentPlan)
                                            .WithMaximumEntryAgeNextBirthday(60)
                                            .WithMinimumEntryAgeNextBirthday(19)
                                    }, null)
            };

            var rule = new AvailabilityRule(riderDefinition, _nameLookupService);

            var selectedProdPlanOpts = new SelectedProductPlanOptions("DTH", "tal", new[] { "DTH" },
                new[] { "" }, new[] { "TPDDTH" }, new[] { "" }, 19, null, null, null);

            var result = rule.IsSatisfiedBy(selectedProdPlanOpts);
            Assert.That(result.IsSatisfied, Is.True);

            selectedProdPlanOpts = new SelectedProductPlanOptions("DTH", "tal", new[] { "DTH" },
                new[] { "" }, new[] { "TPDDTH" }, new[] { "" }, 18, null, null, null);

            result = rule.IsSatisfiedBy(selectedProdPlanOpts);
            Assert.That(result.IsSatisfied, Is.False);
        }

        [Test]
        public void IsSatisfiedBy_RiderFeatureDefinedAgeMustbeLessThanMaximumEntryAgeNextBirthday_ShouldBeSatisfiedWhenApplicableAge()
        {
            var riderDefinition = new PlanDefinition()
            {
                RuleDefinition = new FeatureRule(new List<ProductConfigRule>()
                                    {
                                        new ProductConfigRule(Should.BeAllSelected, From.CurrentPlan)
                                            .WithMaximumEntryAgeNextBirthday(60)
                                            .WithMinimumEntryAgeNextBirthday(19)
                                    }, null)
            };

            var rule = new AvailabilityRule(riderDefinition, _nameLookupService);

            var selectedProdPlanOpts = new SelectedProductPlanOptions("DTH", "tal", new[] { "DTH" },
                new[] { "" }, new[] { "TPDDTH" }, new[] { "" }, 60, null, null, null);

            var result = rule.IsSatisfiedBy(selectedProdPlanOpts);
            Assert.That(result.IsSatisfied, Is.True);

            selectedProdPlanOpts = new SelectedProductPlanOptions("DTH", "tal", new[] { "DTH" },
                new[] { "" }, new[] { "TPDDTH" }, new[] { "" }, 61, null, null, null);

            result = rule.IsSatisfiedBy(selectedProdPlanOpts);
            Assert.That(result.IsSatisfied, Is.False);
        }
    }
}
