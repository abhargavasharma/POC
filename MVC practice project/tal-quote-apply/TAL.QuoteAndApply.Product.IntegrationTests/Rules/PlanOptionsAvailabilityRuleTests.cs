using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Policy;
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
    public class PlanOptionsAvailabilityRuleTests
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
        public void IsSatisfiedBy_WaitingPeriodNotInSupportedWaitingPeriods_IsBroken()
        {
            var optionDefinition = new OptionDefinition(ProductOptionConstants.IpDayOneAccident, "Day One Accident",
                            new FeatureRule(new List<ProductConfigRule>()
                            {
                                 new ProductConfigRule(Should.BeAllSelected, From.Product, null)
                                    .WithSupportedWaitingPeriods(new [] { 2,4 })
                            }, null), null);

            var rule = new AvailabilityRule(optionDefinition, _nameLookupService);

            var selectedPlans = new SelectedPlans(new[] { "IP" });
            var selectedCovers = new SelectedCovers(new[] { "SomeCoverA", "SomeCoverB" });
            var selectedProdPlanOpts = new SelectedProductPlanOptions("IP", "tal", selectedPlans, selectedCovers, new string[0], new string[0], 60, null, 1, null);

            var result = rule.IsSatisfiedBy(selectedProdPlanOpts);
            Assert.That(result.IsBroken, Is.True);
        }

        [Test]
        public void IsSatisfiedBy_WaitingPeriodInSupportedWaitingPeriods_IsSatisfied()
        {
            var optionDefinition = new OptionDefinition(ProductOptionConstants.IpDayOneAccident, "Day One Accident",
                            new FeatureRule(new List<ProductConfigRule>()
                            {
                                 new ProductConfigRule(Should.BeAllSelected, From.Product, null)
                                    .WithSupportedWaitingPeriods(new [] { 2,4 })
                            }, null), null);

            var rule = new AvailabilityRule(optionDefinition, _nameLookupService);

            var selectedPlans = new SelectedPlans(new[] { "IP" });
            var selectedCovers = new SelectedCovers(new[] { "SomeCoverA", "SomeCoverB" });
            var selectedProdPlanOpts = new SelectedProductPlanOptions("IP", "tal", selectedPlans, selectedCovers, new string[0], new string[0], 60, null, 2, null);

            var result = rule.IsSatisfiedBy(selectedProdPlanOpts);
            Assert.That(result.IsSatisfied, Is.True);
        }


        [Test]
        public void IsSatisfiedBy_OccupationClassNotInSupportedOccupationClasses_IsBroken()
        {
            var optionDefinition = new OptionDefinition(ProductOptionConstants.IpDayOneAccident, "Day One Accident",
                            new FeatureRule(new List<ProductConfigRule>()
                            {
                                 new ProductConfigRule(Should.BeAllSelected, From.Product, null)
                                    .WithSupportedOccupationClasses(new [] { ProductOccupationClassConstants.AAPlus, ProductOccupationClassConstants.AAA, ProductOccupationClassConstants.AA,
                                        ProductOccupationClassConstants.A, ProductOccupationClassConstants.BBPlus, ProductOccupationClassConstants.BBB,
                                        ProductOccupationClassConstants.BB, ProductOccupationClassConstants.B })
                            }, null), null);

            var rule = new AvailabilityRule(optionDefinition, _nameLookupService);

            var selectedPlans = new SelectedPlans(new[] { "IP" });
            var selectedCovers = new SelectedCovers(new[] { "SomeCoverA", "SomeCoverB" });
            var selectedProdPlanOpts = new SelectedProductPlanOptions("IP", "tal", selectedPlans, selectedCovers, new string[0], new string[0], 60, ProductOccupationClassConstants.SRA, null, null);

            var result = rule.IsSatisfiedBy(selectedProdPlanOpts);
            Assert.That(result.IsBroken, Is.True);
        }

        [Test]
        public void IsSatisfiedBy_OccupationClassInSupportedOccupationClasses_IsSatisfied()
        {
            var optionDefinition = new OptionDefinition(ProductOptionConstants.IpDayOneAccident, "Day One Accident",
                            new FeatureRule(new List<ProductConfigRule>()
                            {
                                 new ProductConfigRule(Should.BeAllSelected, From.Product, null)
                                   .WithSupportedOccupationClasses(new [] { ProductOccupationClassConstants.AAPlus, ProductOccupationClassConstants.AAA, ProductOccupationClassConstants.AA,
                                        ProductOccupationClassConstants.A, ProductOccupationClassConstants.BBPlus, ProductOccupationClassConstants.BBB,
                                        ProductOccupationClassConstants.BB, ProductOccupationClassConstants.B })
                            }, null), null);

            var rule = new AvailabilityRule(optionDefinition, _nameLookupService);

            var selectedPlans = new SelectedPlans(new[] { "IP" });
            var selectedCovers = new SelectedCovers(new[] { "SomeCoverA", "SomeCoverB" });
            var selectedProdPlanOpts = new SelectedProductPlanOptions("IP", "tal", selectedPlans, selectedCovers, new string[0], new string[0], 60, ProductOccupationClassConstants.AAA, null, null);

            var result = rule.IsSatisfiedBy(selectedProdPlanOpts);
            Assert.That(result.IsSatisfied, Is.True);
        }

        [Test]
        public void IsSatisfiedBy_IncreasingClaimsIsTurnedOnButLinkedToCpiIsNotSelected_IsBroken()
        {
            bool? linkedToCpi = null;

            var optionDefinition = new OptionDefinition(ProductOptionConstants.IpIncreasingClaims, "Increasing Claims",
                new FeatureRule(new List<ProductConfigRule>()
                {
                    new ProductConfigRule(Should.BeAllSelected, From.Product, null)
                        .WithInflationProtectionRequired()
                }, null), false);

            var rule = new AvailabilityRule(optionDefinition, _nameLookupService);

            var selectedPlans = new SelectedPlans(new[] { "IP" });
            var selectedCovers = new SelectedCovers(new[] { "IPSAC" });
            var selectedProdPlanOpts = new SelectedProductPlanOptions("IP", "tal", selectedPlans, selectedCovers, new string[0], new string[0], 60, null, 1, linkedToCpi);

            var result = rule.IsSatisfiedBy(selectedProdPlanOpts);
            Assert.That(result.IsBroken, Is.True);
        }

        [Test]
        public void IsSatisfiedBy_IncreasingClaimsIsTurnedOnButLinkedToCpiIsOff_IsBroken()
        {
            bool? linkedToCpi = false;

            var optionDefinition = new OptionDefinition(ProductOptionConstants.IpIncreasingClaims, "Increasing Claims",
                new FeatureRule(new List<ProductConfigRule>()
                {
                    new ProductConfigRule(Should.BeAllSelected, From.Product, null)
                        .WithInflationProtectionRequired()
                }, null), false);

            var rule = new AvailabilityRule(optionDefinition, _nameLookupService);

            var selectedPlans = new SelectedPlans(new[] { "IP" });
            var selectedCovers = new SelectedCovers(new[] { "IPSAC" });
            var selectedProdPlanOpts = new SelectedProductPlanOptions("IP", "tal", selectedPlans, selectedCovers, new string[0], new string[0], 60, null, 1, linkedToCpi);

            var result = rule.IsSatisfiedBy(selectedProdPlanOpts);
            Assert.That(result.IsBroken, Is.True);
        }

        [Test]
        public void IsSatisfiedBy_IncreasingClaimsIsTurnedOnButLinkedToCpiIsOn_IsSatisfied()
        {
            bool? linkedToCpi = true;

            var optionDefinition = new OptionDefinition(ProductOptionConstants.IpIncreasingClaims, "Increasing Claims",
                new FeatureRule(new List<ProductConfigRule>()
                {
                    new ProductConfigRule(Should.BeAllSelected, From.Product, null)
                        .WithInflationProtectionRequired()
                }, null), false);

            var rule = new AvailabilityRule(optionDefinition, _nameLookupService);

            var selectedPlans = new SelectedPlans(new[] { "IP" });
            var selectedCovers = new SelectedCovers(new[] { "IPSAC" });
            var selectedProdPlanOpts = new SelectedProductPlanOptions("IP", "tal", selectedPlans, selectedCovers, new string[0], new string[0], 60, null, 1, linkedToCpi);

            var result = rule.IsSatisfiedBy(selectedProdPlanOpts);
            Assert.That(result.IsSatisfied, Is.True);
        }

        [Test]
        public void IsSatisfiedBy_VariableNotAllowedForOccupationClass_IsNotSatisfied()
        {
            bool? linkedToCpi = true;

            var WaitingPeriod =
                new PlanVariablesDefinition(ProductPlanVariableConstants.WaitingPeriod, "Waiting Period",
                    new List<PlanVariableOptionDefinition>
                    {
                        new PlanVariableOptionDefinition("2 Weeks", 2, null),
                        new PlanVariableOptionDefinition("4 Weeks", 4, null),
                        new PlanVariableOptionDefinition("13 Weeks", 13, null),
                        new PlanVariableOptionDefinition("104 weeks (2 years)", 104, null)
                    },
                    new List<AccessControlType> {},
                    new FeatureRule(
                        new List<ProductConfigRule>
                        {
                            new ProductConfigRule().DoNotAllowOccupationClasses(
                                ProductOccupationClassConstants.SRA)
                        }, null));

            var rule = new AvailabilityRule(WaitingPeriod, _nameLookupService);

            var selectedPlans = new SelectedPlans(new[] {"IP"});
            var selectedCovers = new SelectedCovers(new[] {"IPSAC"});
            var selectedProdPlanOpts = new SelectedProductPlanOptions("IP", "tal", selectedPlans, selectedCovers, new string[0],
                new string[0], 60, ProductOccupationClassConstants.SRA, 2, linkedToCpi);

            var result = rule.IsSatisfiedBy(selectedProdPlanOpts);
            Assert.That(result.IsSatisfied, Is.False);
        }

        [Test]
        public void IsSatisfiedBy_VariableOptionNotAllowedForOccupationClass_IsNotSatisfied()
        {
            bool? linkedToCpi = true;


            var variableOption = new PlanVariableOptionDefinition("2 Weeks", 2, new FeatureRule(
                        new List<ProductConfigRule>
                        {
                            new ProductConfigRule().DoNotAllowOccupationClasses(
                                ProductOccupationClassConstants.SRA)
                        }, null));

            var rule = new AvailabilityRule(variableOption, _nameLookupService);

            var selectedPlans = new SelectedPlans(new[] { "IP" });
            var selectedCovers = new SelectedCovers(new[] { "IPSAC" });
            var selectedProdPlanOpts = new SelectedProductPlanOptions("IP", "tal", selectedPlans, selectedCovers, new string[0],
                new string[0], 60, ProductOccupationClassConstants.SRA, 2, linkedToCpi);

            var result = rule.IsSatisfiedBy(selectedProdPlanOpts);
            Assert.That(result.IsSatisfied, Is.False);
        }
    }
}
