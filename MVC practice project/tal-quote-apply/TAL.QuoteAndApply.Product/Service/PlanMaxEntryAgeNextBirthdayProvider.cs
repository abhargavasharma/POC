using System.Linq;
using TAL.QuoteAndApply.Product.Models.Definition;

namespace TAL.QuoteAndApply.Product.Service
{
    public interface IPlanMaxEntryAgeNextBirthdayProvider
    {
        int GetMaxAgeFrom(PlanDefinition planDefinition, string occupationClass);
    }

    public class PlanMaxEntryAgeNextBirthdayProvider : IPlanMaxEntryAgeNextBirthdayProvider
    {
        public int GetMaxAgeFrom(PlanDefinition planDefinition, string occupationClass)
        {
            if (planDefinition.MaximumEntryAgeNextBirthdayForOccupationClass != null)
            {
                var maxAgeForOccClass =
                    planDefinition.MaximumEntryAgeNextBirthdayForOccupationClass.FirstOrDefault(
                        a => a.OccupationClass == occupationClass);

                if (maxAgeForOccClass != null)
                {
                    return maxAgeForOccClass.MaximumAge;
                }
            }

            return planDefinition.MaximumEntryAgeNextBirthday;
        }
    }
}
