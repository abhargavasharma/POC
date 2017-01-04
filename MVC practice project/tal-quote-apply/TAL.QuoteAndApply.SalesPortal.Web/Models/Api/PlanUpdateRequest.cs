using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class PlanUpdateRequest
    {
        public string RiskId { get; set; }

        public string QuoteReferenceNumber { get; set; }

        public IEnumerable<string> SelectedPlanCodes { get; set; }

        public PlanConfigurationRequest CurrentActivePlan { get; set; }

        public bool IsValidForInforce { get; set; }
    }

    public class PlanConfigurationRequest
    {
        public string PlanCode { get; set; }

        public int PlanId { get; set; }

        public int CoverAmount { get; set; }

        public bool? LinkedToCpi { get; set; }

        public bool? PremiumHoliday { get; set; }

        public IEnumerable<string> SelectedCoverCodes { get; set; }

        public IEnumerable<OptionConfigurationRequest> SelectedOptionCodes { get; set; }

        public IEnumerable<PlanConfigurationRequest> SelectedRiders { get; set; }
		public string PremiumType { get; set; }

        public bool Selected { get; set; }

        public int? WaitingPeriod { get; set; }
        public int? BenefitPeriod { get; set; }
        public string OccupationDefinition { get; set; }
    }

    public class OptionConfigurationRequest
    {
        public OptionConfigurationRequest(string code, string name, bool? selected)
        {
            Code = code;
            Name = name;
            Selected = selected;
        }

        public string Name { get; set; }
        public string Code { get; set; }
        public bool? Selected { get; set; }
    }


}