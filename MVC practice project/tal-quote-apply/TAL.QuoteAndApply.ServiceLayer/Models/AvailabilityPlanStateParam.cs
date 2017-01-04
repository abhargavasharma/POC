using System.Collections.Generic;

namespace TAL.QuoteAndApply.ServiceLayer.Models
{
    public class AvailabilityPlanStateParam
    {
        public int RiskId { get; set; }
        public string PlanCode { get; set; }
        public string BrandKey { get; set; }

        public int? WaitingPeriod { get; set; }
        public string OccupationClass { get; set; }
        public int Age { get; set; }
        public bool? LinkedToCpi { get; set; }
        public IEnumerable<string> SelectedPlanCodes { get; set; }
        public IEnumerable<string> SelectedCoverCodes { get; set; }
        public IEnumerable<string> SelectedRiderCodes { get; set; }
        public IEnumerable<string> SelectedRiderCoverCodes { get; set; }

        public IEnumerable<string> SelectedPlanOptionCodes { get; set; }
        public IEnumerable<string> SelectedRiderOptionCodes { get; set; }
    }
}