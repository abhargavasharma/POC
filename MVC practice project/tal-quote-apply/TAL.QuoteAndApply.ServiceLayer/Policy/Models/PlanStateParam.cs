using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.ServiceLayer.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public class PlanIdentityInfo
    {
        public int PlanId { get; set; }
        public string PlanCode { get; set; }
        public bool Selected { get; set; }
        public PlanStatus Status { get; set; }

        public PlanIdentityInfo()
        {
            
        }

        public PlanIdentityInfo(int planId, string planCode, bool selected)
        {
            PlanId = planId;
            PlanCode = planCode;
            Selected = selected;
        }
    }

    public class PlanStateParam
    {
        public string PlanCode { get; set; }
        public string BrandKey { get; set; }
        public int CoverAmount { get; set; }
		public bool? LinkedToCpi { get; set; }
        public bool? PremiumHoliday { get; set; }
        public bool Selected { get; set; }
        public int MaxCoverAmount { get; set; }
        public int MinCoverAmount { get; set; }
        public long Income { get; set; }
        public int PolicyId { get; }
        public int RiskId { get; }
        public PremiumType PremiumType { get; }
        public int PlanId { get; }
        public int Age { get; }
        public int? ParentPlanCoverCap { get; }
        
        public int? WaitingPeriod { get; }
        public int? BenefitPeriod { get; }
        public OccupationDefinition OccupationDefinition { get; }

        public IReadOnlyList<PlanIdentityInfo> AllPlans { get; }
        public IReadOnlyList<string> SelectedCoverCodes { get; private set; }
 
        public IReadOnlyList<PlanStateParam> Riders { get; }
        public IReadOnlyList<OptionsParam> PlanOptions { get; }
        public DataModel.Policy.MarketingStatus MarketingStatus { get; private set; }

        public static PlanStateParam BuildBasicPlanStateParam(string planCode, string brandKey, bool selected, int policyId, int riskId, bool? linkedToCpi,
            int coverAmount, bool? premiumHoliday, PremiumType premiumType, int planId, int? waitingPeriod, int? benefitPeriod, OccupationDefinition occupationDefinition)
        {
            return new PlanStateParam(planCode, brandKey, selected, policyId, riskId, linkedToCpi, coverAmount, premiumHoliday, premiumType, planId, waitingPeriod, benefitPeriod, occupationDefinition);
        }

        private PlanStateParam(string planCode, string brandKey, bool selected, int policyId, int riskId, bool? linkedToCpi,
            int coverAmount, bool? premiumHoliday, PremiumType premiumType, int planId, int? waitingPeriod, int? benefitPeriod, OccupationDefinition occupationDefinition)
        {
            PlanCode = planCode;
            BrandKey = brandKey;
            Selected = selected;
            PolicyId = policyId;
            RiskId = riskId;
            LinkedToCpi = linkedToCpi;
            CoverAmount = coverAmount;
            PremiumHoliday = premiumHoliday;
            PremiumType = premiumType;
            PlanId = planId;
            WaitingPeriod = waitingPeriod;
            BenefitPeriod = benefitPeriod;
            OccupationDefinition = occupationDefinition;
        }

        public static PlanStateParam BuildRiderPlanStateParam(string planCode, string brandKey, bool selected, int policyId, int riskId, bool? linkedToCpi,
           int coverAmount, bool? premiumHoliday, PremiumType premiumType, int planId, int age, long income,
           IEnumerable<OptionsParam> planOptions, 
           IEnumerable<string> selectedCoverCodes, int? parentPlanCoverCap, OccupationDefinition occupationDefinition)
        {
            return new PlanStateParam(planCode, brandKey, selected, policyId, riskId,  linkedToCpi, coverAmount,premiumHoliday, premiumType, planId, age, income, planOptions, selectedCoverCodes, parentPlanCoverCap, occupationDefinition);
        }

        private PlanStateParam(string planCode, string brandKey, bool selected, int policyId, int riskId, bool? linkedToCpi,
           int coverAmount, bool? premiumHoliday, PremiumType premiumType, int planId, int age, long income,
           IEnumerable<OptionsParam> planOptions,
           IEnumerable<string> selectedCoverCodes, int? parentPlanCoverCap, OccupationDefinition occupationDefinition)
        {
            PlanId = planId;
            BrandKey = brandKey;
            SelectedCoverCodes = selectedCoverCodes.ToList();
            CoverAmount = coverAmount;
            Age = age;
            Income = income;
            PolicyId = policyId;
            RiskId = riskId;
            PlanCode = planCode;
            PlanOptions = planOptions.ToList();
            LinkedToCpi = linkedToCpi;
            Selected = selected;
            PremiumHoliday = premiumHoliday;
            PremiumType = premiumType;
            ParentPlanCoverCap = parentPlanCoverCap;
            OccupationDefinition = occupationDefinition;
        }

        public static PlanStateParam BuildPlanStateParam(string planCode, string brandKey, bool selected, int policyId, int riskId, bool? linkedToCpi,
            int coverAmount, bool? premiumHoliday, PremiumType premiumType, int planId, int age, long income,
            int? waitingPeriod, int? benefitPeriod, OccupationDefinition occupationDefinition,
            IEnumerable<PlanStateParam> riders,
            IEnumerable<OptionsParam> planOptions,
            IEnumerable<PlanIdentityInfo> allPlans,
            IEnumerable<string> selectedCoverCodes)
        {
            return new PlanStateParam(planCode, brandKey, selected, policyId, riskId, linkedToCpi,
                coverAmount, premiumHoliday, premiumType, planId, age, income,
                waitingPeriod, benefitPeriod, occupationDefinition, 
                riders, planOptions, allPlans, selectedCoverCodes);
        }

        private PlanStateParam(string planCode, string brandKey, bool selected, int policyId, int riskId, bool? linkedToCpi,
            int coverAmount, bool? premiumHoliday, PremiumType premiumType, int planId, int age, long income,
            int? waitingPeriod, int? benefitPeriod, OccupationDefinition occupationDefinition,
            IEnumerable<PlanStateParam> riders,
            IEnumerable<OptionsParam> planOptions,
            IEnumerable<PlanIdentityInfo> allPlans,
            IEnumerable<string> selectedCoverCodes)
        {
            PlanId = planId;
            BrandKey = brandKey;
            AllPlans = allPlans.ToList();
            SelectedCoverCodes = selectedCoverCodes.ToList();
            CoverAmount = coverAmount;
            Age = age;
            Income = income;
            PolicyId = policyId;
            RiskId = riskId;
            PlanCode = planCode;
            Riders = riders.ToList();
            PlanOptions = planOptions.ToList();
            LinkedToCpi = linkedToCpi;
            Selected = selected;
            PremiumHoliday = premiumHoliday;
            PremiumType = premiumType;
            WaitingPeriod = waitingPeriod;
            BenefitPeriod = benefitPeriod;
            OccupationDefinition = occupationDefinition;
        }

        public AvailabilityPlanStateParam ToAvailabilityPlanStateParam(string occupationClass)
        {
            return new AvailabilityPlanStateParam
            {
                PlanCode = PlanCode,
                BrandKey = BrandKey,
                RiskId = RiskId,
                Age = Age,
                WaitingPeriod = WaitingPeriod,
                OccupationClass = occupationClass,
                LinkedToCpi = LinkedToCpi,
                SelectedPlanCodes = AllPlans?.Where(o => o.Selected).Select(p => p.PlanCode) ?? new List<string>(),
                SelectedCoverCodes = SelectedCoverCodes ?? new List<string>(),
                SelectedPlanOptionCodes = PlanOptions?.Where(o => o.Selected.HasValue && o.Selected.Value).Select(o => o.Code) ?? new List<string>(),
                SelectedRiderCodes = Riders?.Where(r => r.Selected).Select(r => r.PlanCode) ?? new List<string>(),
                SelectedRiderCoverCodes = Riders?.SelectMany(r => r.SelectedCoverCodes) ?? new List<string>(),
                SelectedRiderOptionCodes = Riders?.SelectMany(r => r.PlanOptions).Where(o => o.Selected.HasValue && o.Selected.Value).Select(o => o.Code) ?? new List<string>(),
            };
        }

        public void UpdateSelectedCovers(IEnumerable<string> coverCodes)
        {
            SelectedCoverCodes = coverCodes.ToList();
        }

        public PlanStateParam Clone()
        {
            return new PlanStateParam(PlanCode, BrandKey, Selected, PolicyId, RiskId, LinkedToCpi, CoverAmount, PremiumHoliday,
                PremiumType, PlanId, Age, Income, WaitingPeriod, BenefitPeriod, OccupationDefinition
                ,
                Riders?.Select(
                    r =>
                        new PlanStateParam(r.PlanCode, r.BrandKey, r.Selected, r.PolicyId, r.RiskId, r.LinkedToCpi, r.CoverAmount,
                            r.PremiumHoliday, r.PremiumType, r.PlanId, r.Age, r.Income,
                            r.PlanOptions.Select(o => new OptionsParam(o.Code, o.Selected)), r.SelectedCoverCodes.ToList(), r.ParentPlanCoverCap, r.OccupationDefinition)),
                    PlanOptions.Select(o => new OptionsParam(o.Code, o.Selected)),
                    AllPlans.Select(p => new PlanIdentityInfo(p.PlanId, p.PlanCode, p.Selected)),
                    SelectedCoverCodes.ToList()
                );
        }
    }
}
