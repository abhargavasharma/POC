using System;
using System.Linq;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Product.Contracts;
using TAL.QuoteAndApply.Product.Models;
using TAL.QuoteAndApply.Product.Models.Definition;

namespace TAL.QuoteAndApply.Product.Definition.Brands
{
    internal class MidMarketProductDefinitionBuilder : BaseBrandSpecificBuilder
    {
 
        protected override ProductDefinition ApplyCustomSettings(ProductDefinition baseProductDefinition)
        {
            baseProductDefinition.Code = "TLP";
            baseProductDefinition.Name = "TAL Life Protection";
            baseProductDefinition.MaximumNumberOfBeneficiaries = 5;

            baseProductDefinition.AvailablePremiumFrequencies = Enum.GetValues(typeof(PremiumFrequency))
                .Cast<PremiumFrequency>()
                .Where(p => p != PremiumFrequency.Unknown)
                .ToList();

            baseProductDefinition.DefaultPremiumFrequency = PremiumFrequency.Monthly;

			baseProductDefinition.IsQuoteSaveLoadEnabled = true;
            baseProductDefinition.SaveGatePosition = SaveGatePosition.BeforeSelectPlan;

            baseProductDefinition.ExternalCustomerRefConfigSettings = new ExternalCustomerRefConfigSettings
            {
                ExternalCustomerRefRequired = ExternalCustomerRefRequired.NotRequired,
                ExternalCustomerRefLabel = "TAL"
            };

            return baseProductDefinition;
        }

        public MidMarketProductDefinitionBuilder(IProductBrandSettingsProvider productBrandSettingsProvider, string brandKey) : base(productBrandSettingsProvider, brandKey)
        {
        }
    }
}
