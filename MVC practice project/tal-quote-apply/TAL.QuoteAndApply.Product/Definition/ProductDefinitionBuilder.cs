using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.DataModel.Underwriting;
using TAL.QuoteAndApply.Product.Contracts;
using TAL.QuoteAndApply.Product.Definition.Brands;
using TAL.QuoteAndApply.Product.Models.Definition;

namespace TAL.QuoteAndApply.Product.Definition
{
    public interface IProductDefinitionBuilder
    {
        ProductDefinition BuildProductDefinition(string brandKey);
    }

    public static class PlanVariableDefinitions
    {
        public static PlanVariablesDefinition InflationProtection =
            new PlanVariablesDefinition(ProductPlanVariableConstants.LinkedToCpi, "Inflation Protection",
                new List<PlanVariableOptionDefinition>
                {
                    new PlanVariableOptionDefinition("Yes", true, null),
                    new PlanVariableOptionDefinition("No", false, null)
                },
                new List<AccessControlType> {AccessControlType.Customer, AccessControlType.Agent}, null);

        public static PlanVariablesDefinition PremiumHoliday =
            new PlanVariablesDefinition(ProductPlanVariableConstants.PremiumHoliday, "Premium Holiday",
                new List<PlanVariableOptionDefinition>
                {
                    new PlanVariableOptionDefinition("Yes", true, null),
                    new PlanVariableOptionDefinition("No", false, null)
                },
                new List<AccessControlType> {AccessControlType.Agent}, null);

        public static PlanVariablesDefinition PremiumType =
            new PlanVariablesDefinition(ProductPlanVariableConstants.PremiumType, "Premium Type",
                new List<PlanVariableOptionDefinition>
                {
                    new PlanVariableOptionDefinition("Stepped", DataModel.Product.PremiumType.Stepped, null),
                    new PlanVariableOptionDefinition("Level", DataModel.Product.PremiumType.Level, null) //TODO: add config rule here to restrict by age
                },
                new List<AccessControlType> {AccessControlType.Agent}, null); //Customer can edit this but not as part of variables

        public static PlanVariablesDefinition WaitingPeriod =
            new PlanVariablesDefinition(ProductPlanVariableConstants.WaitingPeriod, "Waiting Period",
                new List<PlanVariableOptionDefinition>
                {
                    new PlanVariableOptionDefinition("2 Weeks", 2, new FeatureRule(
                    new List<ProductConfigRule>
                    {
                        new ProductConfigRule().DoNotAllowOccupationClasses(
                            ProductOccupationClassConstants.SRA)
                    }, null)),
                    new PlanVariableOptionDefinition("4 Weeks", 4, null),
                    new PlanVariableOptionDefinition("13 Weeks", 13, null),
                    new PlanVariableOptionDefinition("104 weeks (2 years)", 104, null)
                },
                new List<AccessControlType> {AccessControlType.Customer, AccessControlType.Agent},
                null);

        public static PlanVariablesDefinition BenefitPeriod =
            new PlanVariablesDefinition(ProductPlanVariableConstants.BenefitPeriod, "Benefit Period",
                new List<PlanVariableOptionDefinition>
                {
                    new PlanVariableOptionDefinition("1 Year", 1, null),
                    new PlanVariableOptionDefinition("2 Years", 2, null),
                    new PlanVariableOptionDefinition("5 Years", 5, null)
                },
                new List<AccessControlType> {AccessControlType.Customer, AccessControlType.Agent}, null);

        public static PlanVariablesDefinition OccupationDefinition =
            new PlanVariablesDefinition(ProductPlanVariableConstants.OccupationDefinition, "TPD Occupation Type",
                new List<PlanVariableOptionDefinition>
                {
                    new PlanVariableOptionDefinition("Any Occupation",
                        DataModel.Product.OccupationDefinition.AnyOccupation, null),
                    new PlanVariableOptionDefinition("Own Occupation",
                        DataModel.Product.OccupationDefinition.OwnOccupation, null) //TODO: add config rule here to restrict based on Occupation Class
                },
                new List<AccessControlType> {AccessControlType.Agent, AccessControlType.Customer}, null);
            //TODO: add access for customer when we implement TPD definition on Customer Site
    }

    public class ProductDefinitionBuilder : IProductDefinitionBuilder
    {
        private readonly IProductBrandSettingsProvider _productBrandSettingsProvider;

        private ProductDefinition BuildBaseDefinition()
        {
            var productDefinition = new ProductDefinition
            {
                MinimumAnnualIncomeDollars = 0,
                MinimumEntryAgeNextBirthday = 19,
                MaximumEntryAgeNextBirthday = 75,
                AustralianResidencyRequired = true,
                PremiumTypes = new List<PremiumTypeDefinition>
                {
                    new PremiumTypeDefinition(PremiumType.Unknown, "Unknown",null, false),
                    new PremiumTypeDefinition(PremiumType.Stepped, "Stepped",null),
                    new PremiumTypeDefinition(PremiumType.Level, "Level", 60)
                },
                Plans = new List<PlanDefinition>
                {
                    new PlanDefinition
                    {
                        Code = ProductPlanConstants.LifePlanCode,
                        ShortName = "Life",
                        Name = "Life Insurance",
                        MinimumEntryAgeNextBirthday = 19,
                        MaximumEntryAgeNextBirthday = 75,
                        BenefitExpiryAge = 100,
                        MinimumCoverAmount = 100000,
                        IncludedInMultiPlanDiscount = true,
                      
                        AgeRangeCoverAmountDefinitions = new List<AgeRangeCoverAmountDefinition>()
                        {
                            AgeRangeCoverAmountDefinition.Builder()
                                .WithAgeRangeDefinition(0, 45)
                                .WithAnnualIncomeFactor(20)
                                .WithNoIncomeMaxCoverAmount(500000)
                                .WithMaxCover(1500000)
                                .Build(),

                            AgeRangeCoverAmountDefinition.Builder()
                                .WithAgeRangeDefinition(46, 50)
                                .WithAnnualIncomeFactor(18)
                                .WithNoIncomeMaxCoverAmount(500000)
                                .WithMaxCover(1500000)
                                .Build(),

                            AgeRangeCoverAmountDefinition.Builder()
                                .WithAgeRangeDefinition(51, 55)
                                .WithAnnualIncomeFactor(12)
                                .WithNoIncomeMaxCoverAmount(500000)
                                .WithMaxCover(1000000)
                                .Build(),

                            AgeRangeCoverAmountDefinition.Builder()
                                .WithAgeRangeDefinition(56, 59)
                                .WithAnnualIncomeFactor(5)
                                .WithNoIncomeMaxCoverAmount(500000)
                                .WithMaxCover(750000)
                                .Build(),

                            AgeRangeCoverAmountDefinition.Builder()
                                .WithAgeRangeDefinition(60, 64)
                                .WithMaxCover(500000)
                                .Build(),

                            AgeRangeCoverAmountDefinition.Builder()
                                .WithAgeRangeDefinition(65, 74)
                                .WithMaxCover(250000)
                                .Build()
                        },
                        Variables = new List<PlanVariablesDefinition>
                        {
                            PlanVariableDefinitions.InflationProtection,
                            PlanVariableDefinitions.PremiumHoliday,
                            PlanVariableDefinitions.PremiumType
                        },
                        Options = new List<OptionDefinition>
                        {
                            new OptionDefinition(ProductOptionConstants.LifePremiumRelief, "Premium Relief",
                                new FeatureRule(new List<ProductConfigRule>()
                                {
                                    //US22049 - Selection of Premium Relief is not dependent on IP Plan, hence removing the dependency rule
                                    new ProductConfigRule(Should.BeAllSelected, From.Product, ProductPlanConstants.LifePlanCode )
                                        .WithMaximumEntryAgeNextBirthday(62)
                                },  null)
                            )
                        },
                        Covers = new List<CoverDefinition>
                        {
                            new CoverDefinition(ProductCoverConstants.LifeAccidentCover, "Accident Cover", true),
                            new CoverDefinition(ProductCoverConstants.LifeIllnessCover, "Illness Cover", true),
                            new CoverDefinition(ProductCoverConstants.LifeAdventureSportsCover, "Adventure Sports Cover", false,
                                new FeatureRule(null, new List<ProductConfigRule>() { new ProductConfigRule(Should.BeSomeSelected, From.CurrentPlan, ProductCoverConstants.LifeAccidentCover) }))
                                .WithSupportedLoadingTypes(new [] {LoadingType.PerMille})
                        },
                        Riders = new List<PlanDefinition>
                        {
                            new PlanDefinition
                            {
                                Code = ProductRiderConstants.PermanentDisabilityRiderCode,
                                Name = "Total Permanent Disability Insurance (TPD)",
                                ShortName = "TPD",
                                IncludedInMultiPlanDiscount = true,
                                UseOccupationDefinition = true,
                                RelatedPlanCode = ProductPlanConstants.PermanentDisabilityPlanCode,
                                MinimumCoverAmount = 10000,
                                MinimumEntryAgeNextBirthday = 19,
                                MaximumEntryAgeNextBirthday = 60,
                         
                                Options = new List<OptionDefinition>
                                {
                                    new OptionDefinition(ProductOptionConstants.TpdRiderDeathBuyBack, "Life Buy Back", true)
                                },
                                Covers = new List<CoverDefinition>
                                {
                                    new CoverDefinition(ProductRiderCoverConstants.TpdRiderAccidentCover, "Accident Cover", true, new FeatureRule(null, new List<ProductConfigRule>() { new ProductConfigRule(Should.BeSomeSelected, From.Rider, ProductCoverConstants.LifeAccidentCover) }))
                                        .WithUnderwritingCode("TPSAC")
                                        .AsRiderCodeFor("TPSAC"),
                                    new CoverDefinition(ProductRiderCoverConstants.TpdRiderIllnessCover, "Illness Cover", true, new FeatureRule(null, new List<ProductConfigRule>() { new ProductConfigRule(Should.BeSomeSelected, From.Rider, ProductCoverConstants.LifeIllnessCover) }))
                                        .WithUnderwritingCode("TPSIC")
                                        .AsRiderCodeFor("TPSIC"),
                                    new CoverDefinition(ProductRiderCoverConstants.TpdRiderAdventureSportsCover, "Sports Cover", false, new FeatureRule(null, new List<ProductConfigRule>() { new ProductConfigRule(Should.BeAllSelected, From.Rider, new[] { "TPDDTHAC", ProductCoverConstants.LifeAccidentCover, ProductCoverConstants.LifeAdventureSportsCover } )}))
                                        .WithUnderwritingCode("TPSASC")
                                        .AsRiderCodeFor("TPSASC")
                                        .WithSupportedLoadingTypes(new [] {LoadingType.PerMille})
                                },
                                RuleDefinition = new FeatureRule(new List<ProductConfigRule>()
                                {
                                    new ProductConfigRule(Should.BeAllSelected, From.Product, new [] { ProductPlanConstants.LifePlanCode })
                                        .WithMaximumEntryAgeNextBirthday(60)
                                        .WithMinimumEntryAgeNextBirthday(19),
                                    new ProductConfigRule(Should.NotAnyBeSelected, From.Product, new [] { ProductPlanConstants.PermanentDisabilityPlanCode })
                                        .WithMaximumEntryAgeNextBirthday(60)
                                        .WithMinimumEntryAgeNextBirthday(19),
                                },  null),
                                AgeRangeCoverAmountDefinitions = new List<AgeRangeCoverAmountDefinition>()
                                {
                                    AgeRangeCoverAmountDefinition.Builder()
                                        .WithAgeRangeDefinition(0, 45)
                                        .WithAnnualIncomeFactor(20)
                                        .WithNoIncomeMaxCoverAmount(500000)
                                        .WithMaxCover(1500000)
                                        .Build(),
                                    AgeRangeCoverAmountDefinition.Builder()
                                        .WithAgeRangeDefinition(46, 50)
                                        .WithAnnualIncomeFactor(18)
                                        .WithNoIncomeMaxCoverAmount(500000)
                                        .WithMaxCover(1500000)
                                        .Build(),
                                    AgeRangeCoverAmountDefinition.Builder()
                                        .WithAgeRangeDefinition(51, 55)
                                        .WithAnnualIncomeFactor(12)
                                        .WithNoIncomeMaxCoverAmount(500000)
                                        .WithMaxCover(1000000)
                                        .Build(),
                                    AgeRangeCoverAmountDefinition.Builder()
                                        .WithAgeRangeDefinition(56, 59)
                                        .WithAnnualIncomeFactor(5)
                                        .WithNoIncomeMaxCoverAmount(500000)
                                        .WithMaxCover(750000)
                                        .Build()
                                },
                                Variables = new List<PlanVariablesDefinition>()
                                {
                                    PlanVariableDefinitions.OccupationDefinition
                                }
                            },
                            new PlanDefinition
                            {
                                Code = ProductRiderConstants.CriticalIllnessRiderCode,
                                Name = "Recovery Insurance",
                                ShortName = "RI",
                                RelatedPlanCode = ProductPlanConstants.CriticalIllnessPlanCode,
                                MinimumCoverAmount = 10000,
                                MinimumEntryAgeNextBirthday = 19,
                                MaximumEntryAgeNextBirthday = 60,
                           
                                Options = new List<OptionDefinition>
                                {
                                    new OptionDefinition(ProductOptionConstants.CiRiderDeathBuyBack, "Life Buy Back", true)
                                },
                                Covers = new List<CoverDefinition>
                                {
                                    new CoverDefinition(ProductRiderCoverConstants.CiRiderSeriousInjuryCover, "Serious Injury Cover", true, new FeatureRule(null, new List<ProductConfigRule>() { new ProductConfigRule(Should.BeAllSelected, From.Rider, ProductCoverConstants.LifeAccidentCover) }))
                                        .WithUnderwritingCode("TRSSIN")
                                        .AsRiderCodeFor("TRSSIN"),
                                    new CoverDefinition(ProductRiderCoverConstants.CiRiderCancerCover, "Cancer Cover", true, new FeatureRule(null, new List<ProductConfigRule>() { new ProductConfigRule(Should.BeSomeSelected, From.Rider, ProductCoverConstants.LifeIllnessCover) }))
                                        .WithUnderwritingCode("TRSCC")
                                        .AsRiderCodeFor("TRSCC"),
                                    new CoverDefinition(ProductRiderCoverConstants.CiRiderSeriousIllnessCover, "Serious Illness Cover", true,  new FeatureRule(null, new List<ProductConfigRule>() { new ProductConfigRule(Should.BeSomeSelected, From.Rider, ProductCoverConstants.LifeIllnessCover) }))
                                        .WithUnderwritingCode("TRSSIC")
                                        .AsRiderCodeFor("TRSSIC")
                                },
                                RuleDefinition = new FeatureRule(new List<ProductConfigRule>()
                                {
                                    new ProductConfigRule(Should.BeAllSelected, From.Product, new [] { ProductPlanConstants.LifePlanCode })
                                        .WithMaximumEntryAgeNextBirthday(60)
                                        .WithMinimumEntryAgeNextBirthday(19),
                                    new ProductConfigRule(Should.NotAnyBeSelected, From.Product, new [] { ProductPlanConstants.CriticalIllnessPlanCode })
                                        .WithMaximumEntryAgeNextBirthday(60)
                                        .WithMinimumEntryAgeNextBirthday(19),
                                },  null),
                                AgeRangeCoverAmountDefinitions = new List<AgeRangeCoverAmountDefinition>()
                                {
                                    AgeRangeCoverAmountDefinition.Builder()
                                        .WithAgeRangeDefinition(0, 60)
                                        .WithMaxCover(500000)
                                        .Build()
                                }
                            }
                        }
                    },
                    new PlanDefinition
                    {
                        Code = ProductPlanConstants.PermanentDisabilityPlanCode,
                        Name = "Total Permanent Disability Insurance (TPD)",
                        ShortName = "TPD",
                        MinimumEntryAgeNextBirthday = 19,
                        MaximumEntryAgeNextBirthday = 60,
                        BenefitExpiryAge = 100,
                        UseOccupationDefinition = true,
                        IncludedInMultiPlanDiscount = true,
                        MinimumCoverAmount = 100000,
                   
                        Covers = new List<CoverDefinition>
                        {
                            new CoverDefinition(ProductCoverConstants.TpdAccidentCover, "Accident Cover", true),
                            new CoverDefinition(ProductCoverConstants.TpdIllnessCover, "Illness Cover", true),
                            new CoverDefinition(ProductCoverConstants.TpdAdventureSportsCover, "Sports Cover", false,
                                new FeatureRule(null, new List<ProductConfigRule>() { new ProductConfigRule(Should.BeSomeSelected, From.CurrentPlan, ProductCoverConstants.TpdAccidentCover) }))
                                .WithSupportedLoadingTypes(new [] {LoadingType.PerMille})
                        },
                        Variables = new List<PlanVariablesDefinition>
                        {
                            PlanVariableDefinitions.InflationProtection,
                            PlanVariableDefinitions.PremiumHoliday,
                            PlanVariableDefinitions.PremiumType,
                            PlanVariableDefinitions.OccupationDefinition
                        },
                        Options = new List<OptionDefinition>
                        {
                            new OptionDefinition(ProductOptionConstants.TpdPremiumRelief, "Premium Relief",
                                new FeatureRule(new List<ProductConfigRule>()
                                {
                                    //US22049 - Selection of Premium Relief is not dependent on IP Plan, hence removing the dependency rule
                                    new ProductConfigRule(Should.BeAllSelected, From.Product, ProductPlanConstants.PermanentDisabilityPlanCode )
                                        .WithMaximumEntryAgeNextBirthday(62)
                                },  null)
                            )
                        },
                        AgeRangeCoverAmountDefinitions = new List<AgeRangeCoverAmountDefinition>()
                        {
                            AgeRangeCoverAmountDefinition.Builder()
                                        .WithAgeRangeDefinition(0, 45)
                                        .WithAnnualIncomeFactor(20)
                                        .WithNoIncomeMaxCoverAmount(500000)
                                        .WithMaxCover(1500000)
                                        .Build(),
                                    AgeRangeCoverAmountDefinition.Builder()
                                        .WithAgeRangeDefinition(46, 50)
                                        .WithAnnualIncomeFactor(18)
                                        .WithNoIncomeMaxCoverAmount(500000)
                                        .WithMaxCover(1500000)
                                        .Build(),
                                    AgeRangeCoverAmountDefinition.Builder()
                                        .WithAgeRangeDefinition(51, 55)
                                        .WithAnnualIncomeFactor(12)
                                        .WithNoIncomeMaxCoverAmount(500000)
                                        .WithMaxCover(1000000)
                                        .Build(),
                                    AgeRangeCoverAmountDefinition.Builder()
                                        .WithAgeRangeDefinition(56, 59)
                                        .WithAnnualIncomeFactor(5)
                                        .WithNoIncomeMaxCoverAmount(500000)
                                        .WithMaxCover(750000)
                                        .Build()
                        }
                    },
                    new PlanDefinition
                    {
                        Code = ProductPlanConstants.CriticalIllnessPlanCode,
                        Name = "Recovery Insurance",
                        ShortName = "RI",
                        MinimumEntryAgeNextBirthday = 19,
                        MaximumEntryAgeNextBirthday = 60,
                        BenefitExpiryAge = 100,
                        MinimumCoverAmount = 100000,
                     
                        Covers = new List<CoverDefinition>
                        {
                            new CoverDefinition(ProductCoverConstants.CiSeriousInjuryCover, "Critical Injury Cover", true),
                            new CoverDefinition(ProductCoverConstants.CiSeriousIllnessCover, "Critical Illness Cover", true),
                            new CoverDefinition(ProductCoverConstants.CiCancerCover, "Cancer Cover", true)
                        },
                        Variables = new List<PlanVariablesDefinition>
                        {
                            PlanVariableDefinitions.InflationProtection,
                            PlanVariableDefinitions.PremiumHoliday,
                            PlanVariableDefinitions.PremiumType
                        },
                        Options = new List<OptionDefinition>
                        {
                            new OptionDefinition(ProductOptionConstants.TpdPremiumRelief, "Premium Relief",
                                new FeatureRule(new List<ProductConfigRule>()
                                {
                                    //US22049 - Selection of Premium Relief is not dependent on IP Plan, hence removing the dependency rule
                                    new ProductConfigRule(Should.BeAllSelected, From.Product, ProductPlanConstants.CriticalIllnessPlanCode)
                                        .WithMaximumEntryAgeNextBirthday(62)
                                },  null)
                            )
                        },
                        AgeRangeCoverAmountDefinitions = new List<AgeRangeCoverAmountDefinition>()
                        {
                            AgeRangeCoverAmountDefinition.Builder()
                                        .WithAgeRangeDefinition(0, 60)
                                        .WithMaxCover(500000)
                                        .Build()
                        }
                    },
                    new PlanDefinition
                    {
                        Code = ProductPlanConstants.IncomeProtectionPlanCode,
                        Name = "Income Protection",
                        ShortName = "Income",
                        MinimumEntryAgeNextBirthday = 19,
                        MaximumEntryAgeNextBirthday = 60, //Default max age if occupation class not available
                        MaximumEntryAgeNextBirthdayForOccupationClass = new List<MaximumEntryAgeNextBirthdayForOccupationClassDefinition>
                        {
                            new MaximumEntryAgeNextBirthdayForOccupationClassDefinition(60, ProductOccupationClassConstants.AAA),
                            new MaximumEntryAgeNextBirthdayForOccupationClassDefinition(60, ProductOccupationClassConstants.AAPlus),
                            new MaximumEntryAgeNextBirthdayForOccupationClassDefinition(60, ProductOccupationClassConstants.AA),
                            new MaximumEntryAgeNextBirthdayForOccupationClassDefinition(60, ProductOccupationClassConstants.A),
                            new MaximumEntryAgeNextBirthdayForOccupationClassDefinition(60, ProductOccupationClassConstants.BBB),
                            new MaximumEntryAgeNextBirthdayForOccupationClassDefinition(55, ProductOccupationClassConstants.BBPlus),
                            new MaximumEntryAgeNextBirthdayForOccupationClassDefinition(55, ProductOccupationClassConstants.BB),
                            new MaximumEntryAgeNextBirthdayForOccupationClassDefinition(55, ProductOccupationClassConstants.B),
                            new MaximumEntryAgeNextBirthdayForOccupationClassDefinition(55, ProductOccupationClassConstants.SRA),
                        },
                        BenefitExpiryAge = 100,
                        MinimumCoverAmount = 1000,
                        CoverAmountPercentageDefinition = new CoverAmountPercentageDefinition(75, 10000),
                        Covers = new List<CoverDefinition>
                        {
                            new CoverDefinition(ProductCoverConstants.IpAccidentCover, "Accident Cover", true)
                                .WithSupportedLoadingTypes(new [] {LoadingType.Variable}),
                            new CoverDefinition(ProductCoverConstants.IpIllnessCover, "Illness Cover", true)
                                .WithSupportedLoadingTypes(new [] {LoadingType.Variable}),
                            new CoverDefinition(ProductCoverConstants.IpAdventureSportsCover, "Sports Cover", false,
                                new FeatureRule(null, new List<ProductConfigRule>() { new ProductConfigRule(Should.BeSomeSelected, From.CurrentPlan, ProductCoverConstants.IpAccidentCover) }))
                                .WithSupportedLoadingTypes(new [] {LoadingType.Variable})
                        },
                        Options = new List<OptionDefinition>
                        {
                            new OptionDefinition(ProductOptionConstants.IpIncreasingClaims, "Increasing Claims",  new FeatureRule( new List<ProductConfigRule>()
                            {
                                 new ProductConfigRule(Should.BeAllSelected, From.Product, null)
                                    .WithInflationProtectionRequired()
                            }, null), false),
                            new OptionDefinition(ProductOptionConstants.IpDayOneAccident, "Day One Accident",
                            new FeatureRule( new List<ProductConfigRule>()
                            {
                                 new ProductConfigRule(Should.BeAllSelected, From.Product, null)
                                    .WithSupportedOccupationClasses(new [] { ProductOccupationClassConstants.AAPlus, ProductOccupationClassConstants.AAA, ProductOccupationClassConstants.AA,
                                        ProductOccupationClassConstants.A, ProductOccupationClassConstants.BBPlus, ProductOccupationClassConstants.BBB,
                                        ProductOccupationClassConstants.BB, ProductOccupationClassConstants.B }),
                                 new ProductConfigRule(Should.BeAllSelected, From.Product, null)
                                    .WithSupportedWaitingPeriods(new [] { 2,4 })
                            },
                            new List<ProductConfigRule>()
                            {
                                new ProductConfigRule(Should.BeSomeSelected, From.CurrentPlan, ProductCoverConstants.IpAccidentCover)
                            }), false)
                        },
                        Variables = new List<PlanVariablesDefinition>
                        {
                            PlanVariableDefinitions.InflationProtection,
                            PlanVariableDefinitions.PremiumHoliday,
                            PlanVariableDefinitions.PremiumType,
                            PlanVariableDefinitions.WaitingPeriod,
                            PlanVariableDefinitions.BenefitPeriod
                        }
                    }
                }
            };

            return productDefinition;
        }

        private Dictionary<string, IBrandSpecificBuilder> _brandAppliers;

        public void BuildAppliers()
        {
            _brandAppliers = new Dictionary<string, IBrandSpecificBuilder>
            {
                {"tal", new MidMarketProductDefinitionBuilder(_productBrandSettingsProvider, "tal")},
                {"yb", new YellowBrandProductDefinitionBuilder(_productBrandSettingsProvider, "yb")},
            };
        }

        public ProductDefinitionBuilder(IProductBrandSettingsProvider productBrandSettingsProvider)
        {
            _productBrandSettingsProvider = productBrandSettingsProvider;
            BuildAppliers();
        }

        public ProductDefinition BuildProductDefinition(string brandKey)
        {
            var productDefinition = BuildBaseDefinition();
            return _brandAppliers[brandKey.ToLower()].ApplyBrandSpecifics(productDefinition);
        }
    }
}