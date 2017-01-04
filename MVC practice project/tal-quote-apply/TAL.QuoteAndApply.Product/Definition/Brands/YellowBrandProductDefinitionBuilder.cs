using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Product.Contracts;
using TAL.QuoteAndApply.Product.Models;
using TAL.QuoteAndApply.Product.Models.Definition;

namespace TAL.QuoteAndApply.Product.Definition.Brands
{
    internal class YellowBrandProductDefinitionBuilder : BaseBrandSpecificBuilder
    {
        protected override ProductDefinition ApplyCustomSettings(ProductDefinition baseProductDefinition)
        {
            baseProductDefinition.Code = "NTLP";
            baseProductDefinition.Name = "NON TAL Life Protection";
            baseProductDefinition.MaximumNumberOfBeneficiaries = 1;

            baseProductDefinition.AvailablePremiumFrequencies = new List<PremiumFrequency> { PremiumFrequency.HalfYearly, PremiumFrequency.Yearly };
            baseProductDefinition.DefaultPremiumFrequency = PremiumFrequency.HalfYearly;
            baseProductDefinition.ExternalCustomerRefConfigSettings = new ExternalCustomerRefConfigSettings
            {
                ExternalCustomerRefRequired = ExternalCustomerRefRequired.Mandatory,
                ExternalCustomerRefLabel = "Test Number"
            };

            baseProductDefinition.IsQuoteSaveLoadEnabled = false;
            baseProductDefinition.SaveGatePosition = SaveGatePosition.BeforeUnderWriting;

            return baseProductDefinition;
        }

        public YellowBrandProductDefinitionBuilder(IProductBrandSettingsProvider productBrandSettingsProvider,
            string brandKey) : base(productBrandSettingsProvider, brandKey)
        {
        }
    }
}
