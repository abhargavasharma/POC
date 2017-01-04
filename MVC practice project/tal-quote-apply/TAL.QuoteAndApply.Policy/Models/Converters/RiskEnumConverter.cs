using System;

namespace TAL.QuoteAndApply.Policy.Models.Converters
{
    public interface IResidencyStatusConverter
    {
        ResidencyStatus GetResidency(bool? isAustralianResident);
    }

    public class RiskEnumConverter : IResidencyStatusConverter
    {
        public ResidencyStatus GetResidency(bool? isAustralianResident)
        {
            if (isAustralianResident.HasValue)
            {
                return isAustralianResident.Value ? ResidencyStatus.Australian : ResidencyStatus.NonAustralian;
            }

            return ResidencyStatus.Unknown;
        }
    }
}
