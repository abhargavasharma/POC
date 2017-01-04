using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Policy.Service.RaisePolicy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.Underwriting.Service;
using PolicyStatus = TAL.QuoteAndApply.DataModel.Policy.PolicyStatus;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy
{
    public interface IRaisePolicyService
    {
        RaisePolicySubmissionResult PostPolicy(string policyReferenceNumber);
    }

    public class RaisePolicyService : IRaisePolicyService
    {
        private readonly IHttpRaisePolicyService _httpRaisePolicyService;
        private readonly IRaisePolicyConverter _raisePolicyConverter;
        private readonly IPolicyService _policyService;
        private readonly IRaisePolicyFactory _raisePolicyFactory;
        private readonly IPolicySubmissionValidationService _policySubmissionValidationService;
        private readonly IRaisePolicySubmissionAuditService _raisePolicySubmissionAuditService;
        private readonly ICompleteUnderwritingInterview _completeUnderwritingInterview;
        private readonly IPolicyInteractionService _policyInteractionService;

        public RaisePolicyService(IHttpRaisePolicyService httpRaisePolicyService,
            IRaisePolicyConverter raisePolicyConverter, 
            IPolicyService policyService,
            IRaisePolicyFactory raisePolicyFactory, 
            IPolicySubmissionValidationService policySubmissionValidationService, 
            IRaisePolicySubmissionAuditService raisePolicySubmissionAuditService, 
            ICompleteUnderwritingInterview completeUnderwritingInterview, 
            IPolicyInteractionService policyInteractionService)
        {
            _httpRaisePolicyService = httpRaisePolicyService;
            _raisePolicyConverter = raisePolicyConverter;
            _policyService = policyService;
            _raisePolicyFactory = raisePolicyFactory;
            _policySubmissionValidationService = policySubmissionValidationService;
            _raisePolicySubmissionAuditService = raisePolicySubmissionAuditService;
            _completeUnderwritingInterview = completeUnderwritingInterview;
            _policyInteractionService = policyInteractionService;
        }

        public RaisePolicySubmissionResult PostPolicy(string policyReferenceNumber)
        {
            var uberPolicyObject = _raisePolicyFactory.GetFromQuoteReference(policyReferenceNumber);
            var policySubmissionValidationResult = _policySubmissionValidationService.ValidatePolicy(uberPolicyObject);
            
            if (policySubmissionValidationResult.Completed)
            {
                _policyService.UpdatePolicyToReadyToInforce(uberPolicyObject);
                var policyDto = _policyService.Get(uberPolicyObject.Id);
                if (policyDto.Status < PolicyStatus.RaisedToPolicyAdminSystem && policyDto.SubmittedToRaiseTS == null)
                {
                    var postResult = PostPolicy(uberPolicyObject);
                    if (!postResult)
                    {
                        return RaisePolicySubmissionResult.FailedToSubmit(policySubmissionValidationResult);
                    }
                }

                return RaisePolicySubmissionResult.Success(policySubmissionValidationResult);
            }

            return RaisePolicySubmissionResult.ValidationErrors(policySubmissionValidationResult);
        }

        public bool PostPolicy(Models.RaisePolicy raisePolicy)
        {
            var submitObj = _raisePolicyConverter.From(raisePolicy);
            var postResult = _httpRaisePolicyService.Submit(raisePolicy.QuoteReference, submitObj);

            if (postResult)
            {
                _policyService.UpdateRaisedPolicyFields(raisePolicy);
                _policyInteractionService.PolicySubmitted(raisePolicy.QuoteReference);
                _raisePolicySubmissionAuditService.WriteSubmissionAudit(raisePolicy.Id, submitObj);

                var updatedRisks = _policyService.GetRisksForPolicy(raisePolicy);

                foreach (var risk in updatedRisks)
                {
                    _completeUnderwritingInterview.Complete(risk.InterviewId, risk.InterviewConcurrencyToken);
                }
            }
            else
            {
                _policyService.UpdatePolicyToFailedToSendToPolicyAdminSystem(raisePolicy);
            }

            return postResult; 
        }
    }
}
