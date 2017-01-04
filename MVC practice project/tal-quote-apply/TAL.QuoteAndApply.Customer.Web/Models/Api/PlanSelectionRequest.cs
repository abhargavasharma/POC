using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Customer.Web.Models.Api.Underwriting;

namespace TAL.QuoteAndApply.Customer.Web.Models.Api
{
    public class AttachRiderRequestMulti
    {
        public List<AttachRiderRequest> Requests { get; set; }
    }

    public class AttachRiderRequest
    {
        public string PlanToBecomeRiderCode { get; set; }
        public string PlanToHostRiderCode { get; set; }
    }

    public class DetachRiderRequest
    {
        public string RiderCode { get; set; }
    }

    public class UpdatePlansRequest
    {
        public bool AllowPartialUpdate { get; set; }

        public List<UpdatePlanRequest> Requests { get; set; }
    }

    public class UpdatePlanRequest
    {
        public bool? IncludeReviewInfoInResponse { get; set; }
        public int PlanId { get; set; }
        public string PlanCode { get; set; }
        public bool IsSelected { get; set; }
        public string PremiumType { get; set; }
        public int SelectedCoverAmount { get; set; }
        public bool PremiumHoliday { get; set; } //TODO: This can probably be moved into Variables list, keeping it here for now cause it's less hassle
        public List<string> SelectedCovers { get; set; }
        public List<OptionConfigurationRequest> Options { get; set; }
        public List<string> SelectedPlans { get; set; }
        public List<PlanRiderRequest> Riders { get; set; }
        public List<UpdatePlanVariableRequest> Variables { get; set; }
    }

    public class OptionConfigurationRequest
    {
        public string Code { get; set; }
        public bool IsSelected { get; set; }
    }

    public class PlanRiderRequest
    {
        public int PlanId { get; set; }
        public string PlanCode { get; set; }
        public bool IsSelected { get; set; }
        public string OccupationDefinition { get; set; }
        public int SelectedCoverAmount { get; set; }
        public List<string> SelectedCovers { get; set; }
        public List<OptionConfigurationRequest> Options { get; set; }
    }

    public class UpdatePlanVariableRequest
    {
        public string Code { get; set; }
        public object SelectedValue { get; set; }
    }


    public class UpdatePlanResponse
    {
        public PlanSelectionResponse UpdatedPlan { get; set; }
        public List<string> AvailableCovers { get; set; }
        public List<string> AvailableOptions { get; set; }
        public List<string> AvailablePlans { get; set; }
        public decimal TotalPremium { get; set; }
    }

    public class GetPlanResponse
    {
        public List<PlanSelectionResponse> Plans { get; set; }
        public decimal TotalPremium { get; set; }
        public string PaymentFrequency { get; set; }
        public PremiumTypeOptions PremiumTypeOptions { get; set; }
        public bool IsOccupationTpdAny { get; set; }
        public bool IsOccupationTpdOwn { get; set; }
    }

    public class PlanSelectionResponse
    {
        public int PlanId { get; set; }
        public string PlanCode { get; set; }
        public string CoverTitle { get; set; }
        public decimal Premium { get; set; }
        public decimal TotalPremium { get; set; }
        public bool IsSelected { get; set; }
        public bool IsEligibleForPlan { get; set; }
        public bool PremiumHoliday { get; set; } //TODO: This can probably be moved into Variables list, keeping it here for now cause it's less hassle
        public string PremiumType { get; set; } 
        public string RiderFor { get; set; }

        public string OccupationDefinition { get; set; }
        public string AttachesTo { get; set; }

        public AvailabilityResponse Availability { get; set; }

        public List<PlanCoversReponse> Covers { get; set; }
        public List<PlanOptionReponse> Options { get; set; }
        public List<PlanSelectionResponse> Riders { get; set; }
        public decimal SelectedCoverAmount { get; set; }

        public List<PlanVariableResponse> Variables { get; set; }

        public List<string> ExclusionNames
        {
            get
            {
                return Covers?.Where(c => c.IsSelected).SelectMany(c => c.ExclusionNames).Distinct().ToList() ?? null;
            }
        }

        public bool IsBundled { get; set; }

        public int PlanWidth
        {
            get { return Riders?.Count(r => r.IsSelected) ?? 1; }
        }

        public bool HasRiders => Riders != null && Riders.Any();
    }

    public class PlanReviewResponse : PlanSelectionResponse
    {
        public IEnumerable<QuestionResponse> ChoicePointQuestions { get; set; }
    }

    public class PlanVariableResponse
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public object SelectedValue { get; set; }
        public IEnumerable<PlanVariableOptionResponse> Options { get; set; }
    }

    public class PlanVariableOptionResponse
    {
        public string Name { get; set; }
        public object Value { get; set; }  
        public bool IsAvailable { get; set; }      
    }

    public class PlanCoversReponse
    {
        public string Code { get; set; }
        public bool IsSelected { get; set; }
        public int CoverAmount { get; set; }
        public decimal Premium { get; set; }
        public string CoverFor { get; set; }
        public AvailabilityResponse Availability { get; set; }
        public IEnumerable<string> ExclusionNames { get; set; }
		public IEnumerable<QuestionResponse> ChoicePointQuestions { get; set; }
    }

    public class PlanOptionReponse
    {
        public string Code { get; set; }
        public bool IsSelected { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class AvailabilityResponse
    {
        public bool IsAvailable { get; set; }
        public IList<string> UnavailableReasons { get; set; }
    }
}