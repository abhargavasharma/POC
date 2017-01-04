using System.Collections.Generic;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.ServiceLayer.Validation;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public class PolicyRiskPlanStatusesResult
    {
        public PlanStatus OverallPlanStatus { get; }
        public IEnumerable<PlanValidationStatus> PlanValidationStatuses { get; }

        public PolicyRiskPlanStatusesResult(PlanStatus overallPlanStatus, IEnumerable<PlanValidationStatus> planValidationStatuses)
        {
            OverallPlanStatus = overallPlanStatus;
            PlanValidationStatuses = planValidationStatuses;
        }
    }

    public class PlanValidationStatus
    {
        public string PlanCode { get; }
        public PlanStatus PlanStatus { get; }
        public IEnumerable<ValidationError> Errors { get; }
        public IEnumerable<ValidationError> Warnings { get; }

        public PlanValidationStatus(string planCode, PlanStatus planStatus, IEnumerable<ValidationError> errors, IEnumerable<ValidationError> warnings)
        {
            PlanCode = planCode;
            PlanStatus = planStatus;
            Errors = errors;
            Warnings = warnings;
        }
    }
}