using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.DataModel.Product;

namespace TAL.QuoteAndApply.ServiceLayer.Product
{
    public class PlanResponse
    {
        public int PlanId { get; set; }

        public string Name { get; set; }
        public string ShortName { get; set; }
        public string RelatedPlanCode { get; set; }
        public bool Selected { get; set; }
        
        public decimal Premium { get; set; }
        public decimal PremiumIncludingRiders { get; set; }
        public PremiumFrequency PremiumFrequency { get; set; }
        public PremiumType PremiumType { get; set; }
        
        public int CoverAmount { get; set; }
        
        public string PlanType { get; set; }

        public string Code { get; set; }

        public bool? LinkedToCpi { get; set; }

        public bool? PremiumHoliday { get; set; }

        public IEnumerable<CoverResponse> Covers { get; set; }
        
        public IEnumerable<OptionResponse> Options { get; set; }
        
        public IEnumerable<PlanResponse> Riders { get; set; }

        public bool IsFilledIn { get; set; }

        public int? WaitingPeriod { get; set; }

        public int? BenefitPeriod { get; set; }

        public OccupationDefinition OccupationDefinition { get; set; }

        public IEnumerable<VariableResponse> Variables { get; set; }
        public bool IsRider { get; set; }
    }
}