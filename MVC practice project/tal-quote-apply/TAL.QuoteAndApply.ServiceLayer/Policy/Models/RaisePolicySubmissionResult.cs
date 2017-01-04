using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.ServiceLayer.Validation;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public enum PolicySection
    {
        PersonalDetails,
        RatingFactors,
        Underwriting,
        Payment,
        Quote,
        Beneficiaries,
        PaymentType
    }

    public class SectionSubmissionResult
    {
        public PolicySection Section { get; }

        public bool Completed
        {
            get { return (Errors == null || (Errors !=null && !Errors.Any())) && (Warnings == null || (Warnings != null && !Warnings.Any())) ; }
        }

        public IEnumerable<ValidationError> Errors { get; }
        public IEnumerable<ValidationError> Warnings { get; }

        public SectionSubmissionResult(PolicySection section, IEnumerable<ValidationError> errors, IEnumerable<ValidationError> warnings)
        {
            Section = section;
            Errors = errors;
            Warnings = warnings;
        }
    }

    public class RiskSubmissionResult
    {
        public int RiskId { get; }
        public IEnumerable<SectionSubmissionResult> Sections { get; }

        public bool Completed
        {
            get { return Sections != null && Sections.All(s => s.Completed); }
        }

        public RiskSubmissionResult(int riskId, IEnumerable<SectionSubmissionResult> sections)
        {
            RiskId = riskId;
            Sections = sections;
        }
    }

    public class PolicySubmissionValidationResult
    {
        public IEnumerable<RiskSubmissionResult> RiskSectionStatuses { get; }

        public bool Completed
        {
            get { return RiskSectionStatuses != null && RiskSectionStatuses.All(s => s.Completed); }
        }

        public PolicySubmissionValidationResult(IEnumerable<RiskSubmissionResult> riskSectionStatuses)
        {
            RiskSectionStatuses = riskSectionStatuses;
        }
    }

    public class RaisePolicySubmissionResult
    {
        public bool SubmittedSuccessfully { get; }
        public bool SubmissionAttempted { get; }
        public PolicySubmissionValidationResult PolicySubmissionValidationResult { get; }

        public RaisePolicySubmissionResult(bool submittedSuccessfully, bool submissionAttempted, PolicySubmissionValidationResult policySubmissionValidationResult)
        {
            SubmittedSuccessfully = submittedSuccessfully;
            PolicySubmissionValidationResult = policySubmissionValidationResult;
            SubmissionAttempted = submissionAttempted;
        }

        public static RaisePolicySubmissionResult Success(PolicySubmissionValidationResult policySubmissionValidationResult)
        {
            return new RaisePolicySubmissionResult(true, true, policySubmissionValidationResult);
        }

        public static RaisePolicySubmissionResult FailedToSubmit(PolicySubmissionValidationResult policySubmissionValidationResult)
        {
            return new RaisePolicySubmissionResult(false, true, policySubmissionValidationResult);
        }

        public static RaisePolicySubmissionResult ValidationErrors(PolicySubmissionValidationResult policySubmissionValidationResult)
        {
            return new RaisePolicySubmissionResult(false, false, policySubmissionValidationResult);
        }
    }
}