using System.Collections.Generic;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Product.Models.Definition;

namespace TAL.QuoteAndApply.ServiceLayer.Product.Models.Converters
{
    public interface IRiskProductDefinitionConverter
    {
        RiskProductDefinition CreateFrom(ProductDefinition productDefinition);
    }
    public class RiskProductDefinitionConverter : IRiskProductDefinitionConverter
    {
        public RiskProductDefinition CreateFrom(ProductDefinition productDefinition)
        {
            var riskProductDefinition = new RiskProductDefinition()
            {
                MinimumAnnualIncomeDollars = productDefinition.MinimumAnnualIncomeDollars,
                AustralianResidencyRequired = productDefinition.AustralianResidencyRequired,
                MaximumEntryAgeNextBirthday = productDefinition.MaximumEntryAgeNextBirthday,
                MinimumEntryAgeNextBirthday = productDefinition.MinimumEntryAgeNextBirthday,
            };

           return riskProductDefinition;
        }
    }
}
