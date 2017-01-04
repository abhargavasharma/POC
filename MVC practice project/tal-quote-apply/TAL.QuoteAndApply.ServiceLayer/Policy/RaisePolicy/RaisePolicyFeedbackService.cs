using System;
using System.Linq;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Infrastructure.Logging;
using TAL.QuoteAndApply.Notifications.Service;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Policy.Service.RaisePolicy;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy
{
    public interface IRaisePolicyFeedbackService
    {
        void ProcessRaisePolicyFeedback(string rawFeedbackXml);
    }

    public class RaisePolicyFeedbackService : IRaisePolicyFeedbackService
    {
        private readonly IPolicyService _policyService;
        private readonly IRaisePolicySubmissionAuditService _raisePolicySubmissionAuditService;
        private readonly IRaisePolicyFeedbackResponseConverter _raisePolicyFeedbackResponseConverter;
        private readonly ILoggingService _loggingService;
        private readonly IEmailErrorNotificationService _emailErrorNotificationService;
        private readonly IRaisePolicyConfigurationProvider _raisePolicyConfigurationProvider;

        public RaisePolicyFeedbackService(IRaisePolicySubmissionAuditService raisePolicySubmissionAuditService, IRaisePolicyFeedbackResponseConverter raisePolicyFeedbackResponseConverter, ILoggingService loggingService, IPolicyService policyService, IEmailErrorNotificationService emailErrorNotificationService, IRaisePolicyConfigurationProvider raisePolicyConfigurationProvider)
        {
            _raisePolicySubmissionAuditService = raisePolicySubmissionAuditService;
            _raisePolicyFeedbackResponseConverter = raisePolicyFeedbackResponseConverter;
            _loggingService = loggingService;
            _policyService = policyService;
            _emailErrorNotificationService = emailErrorNotificationService;
            _raisePolicyConfigurationProvider = raisePolicyConfigurationProvider;
        }

        public void ProcessRaisePolicyFeedback(string rawFeedbackXml)
        {
            try
            {
                var canonicalEvent = rawFeedbackXml.FromXml<CanonicalEvent>();
                var feedback = _raisePolicyFeedbackResponseConverter.From(canonicalEvent);

                if (feedback.QuoteReferenceNumber == null)
                {
                    throw new Exception($"No quote reference number provided in the Raise Policy Feedback. XML: {rawFeedbackXml}");
                }

                var policy = _policyService.GetByQuoteReferenceNumber(feedback.QuoteReferenceNumber);
                if (policy == null)
                {
                    throw new Exception($"The quote reference number provided in the Raise Policy Feedback does not match a quote in this system. XML: {rawFeedbackXml}");
                }

                _raisePolicySubmissionAuditService.WriteFeedbackAudit(policy.Id, rawFeedbackXml);

                if (!feedback.HasErrors)
                {
                    _policyService.UpdatePolicyToInforce(policy);
                }
                else
                {
                    _policyService.UpdatePolicyToFailedDuringPolicyAdminSystemLoad(policy);
                    SendErrorEmail(feedback);
                }
            }
            catch (Exception ex)
            {
                _loggingService.Error("RaisePolicyFeedback Failure", ex);
            }
        }

        private void SendErrorEmail(RaisePolicyFeedbackResult feedback)
        {
            if (feedback.ValidationErrors != null && feedback.ValidationErrors.Any())
            {
                _emailErrorNotificationService.SendRaisePolicyValidationErrorEmail(feedback.QuoteReferenceNumber, feedback.ValidationErrors, _raisePolicyConfigurationProvider.ErrorNotificationEmailAddress);
            }
            if (!string.IsNullOrEmpty(feedback.ErrorDetail))
            {
                _emailErrorNotificationService.SendRaisePolicyErrorEmail(feedback.QuoteReferenceNumber, feedback.ErrorDetail, _raisePolicyConfigurationProvider.ErrorNotificationEmailAddress);
            }
        }
    }
}
