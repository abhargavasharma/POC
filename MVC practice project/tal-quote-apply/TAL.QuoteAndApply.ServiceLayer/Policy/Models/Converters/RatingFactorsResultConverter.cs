using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters
{
    public interface IRatingFactorsResultConverter
    {
        RatingFactorsResult CreateFrom(IRisk risk);
    }

    public class RatingFactorsResultConverter : IRatingFactorsResultConverter
    {
        public RatingFactorsResult CreateFrom(IRisk risk)
        {
            return new RatingFactorsResult(MapGender(risk.Gender), 
                risk.DateOfBirth, 
                MapResidency(risk.Residency), 
                MapSmokerStatus(risk.SmokerStatus), 
                risk.OccupationCode,
                risk.OccupationTitle,
                risk.AnnualIncome,
                risk.IndustryCode,
                risk.IndustryTitle);
        }

        private string MapSmokerStatus(SmokerStatus smokerStatus)
        {
            if (smokerStatus == SmokerStatus.Unknown)
                return null;

            return smokerStatus.ToString();
        }

        private bool? MapResidency(ResidencyStatus residency)
        {
            if (residency == ResidencyStatus.Australian) return true;
            if (residency == ResidencyStatus.NonAustralian) return false;

            return null;
        }

        private char? MapGender(Gender gender)
        {
            if (gender == Gender.Female) return 'F';
            if (gender == Gender.Male) return 'M';

            return null;
        }
    }
}