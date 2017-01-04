using System.Collections.Generic;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class SelectedPlanOptions
    {
        public string PlanCode { get; set; }
        public IEnumerable<string> SelectedPlans { get; set; }
        public IEnumerable<string> SelectedCovers { get; set; }
        public IEnumerable<PlanStateParam> SelectedRiders { get; set; }
        public IEnumerable<string> SelectedRiderCovers { get; set; }
    }
}