using System.Collections.Generic;

namespace TAL.QuoteAndApply.Product.Models
{
    public class SelectedProductPlanOptions
    {
        public SelectedProductPlanOptions(string planCode, string brandKey, IEnumerable<string> selectedPlans,
            IEnumerable<string> selectedCovers, IEnumerable<string> selectedRiders,
            IEnumerable<string> selectedRiderCovers, int ageNextBirthday, string occupationClass, int? waitingPeriod, bool? linkedToCpi)
        {
            PlanCode = planCode;
            BrandKey = brandKey;
            SelectedPlans = selectedPlans;
            SelectedCovers = selectedCovers;
            SelectedRiders = selectedRiders;
            SelectedRiderCovers = selectedRiderCovers;
            AgeNextBirthday = ageNextBirthday;
            WaitingPeriod = waitingPeriod;
            OccupationClass = occupationClass;
            LinkedToCpi = linkedToCpi;
        }

        public SelectedProductPlanOptions(string planCode, string brandKey, IEnumerable<string> selectedPlans,
            IEnumerable<string> selectedCovers, int ageNextBirthday, string occupationClass, int? waitingPeriod, bool? linkedToCpi)
        {
            PlanCode = planCode;
            BrandKey = brandKey;
            SelectedPlans = selectedPlans;
            SelectedCovers = selectedCovers;
            AgeNextBirthday = ageNextBirthday;
            WaitingPeriod = waitingPeriod;
            OccupationClass = occupationClass;
            LinkedToCpi = linkedToCpi;
        }
        
        public int AgeNextBirthday { get; private set; }
        public int? WaitingPeriod { get; private set; }
        public string OccupationClass { get; private set; }
        public bool? LinkedToCpi { get; private set; }
        public string PlanCode { get; private set; }
        public string BrandKey { get; private set; }
        public IEnumerable<string> SelectedPlans { get; private set; }
        public IEnumerable<string> SelectedCovers { get; private set; }

        public IEnumerable<string> SelectedRiders { get; private set; }
        public IEnumerable<string> SelectedRiderCovers { get; private set; }
    }
}