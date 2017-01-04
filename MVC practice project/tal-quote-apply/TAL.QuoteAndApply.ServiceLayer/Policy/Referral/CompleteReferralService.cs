using System;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure.Time;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters;
using TAL.QuoteAndApply.ServiceLayer.Policy.PremiumCalculation;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Events;
using TAL.QuoteAndApply.Underwriting.Service;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Referral
{
    public interface ICompleteReferralService
    {
        void CompleteReferral(string quoteReferenceNo);
        DateTime? GetLastCompletedReferralDateTimeForPolicy(string quoteReference);
    }

    public class CompleteReferralService : ICompleteReferralService
    {
        private readonly IReferralService _referralService;
        private readonly IPolicyWithRisksService _policyWithRisksService;
        private readonly IPolicyService _policyService;
        private readonly IPolicyPremiumCalculation _policyPremiumCalculation;
        private readonly IGetUnderwritingInterview _getUnderwritingInterview;
        private readonly IUnderwritingBenefitsResponseChangeSubject _underwritingBenefitsResponseChangeSubject;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IPolicyInteractionService _policyInteractionService;

        public CompleteReferralService(IReferralService referralService, 
            IPolicyWithRisksService policyWithRisksService,
            IPolicyService policyService, 
            IPolicyPremiumCalculation policyPremiumCalculation,
            IGetUnderwritingInterview getUnderwritingInterview,
            IUnderwritingBenefitsResponseChangeSubject underwritingBenefitsResponseChangeSubject,
            ICurrentUserProvider currentUserProvider, 
            IDateTimeProvider dateTimeProvider, 
            IPolicyInteractionService policyInteractionService)
        {
            _referralService = referralService;
            _policyWithRisksService = policyWithRisksService;
            _policyService = policyService;
            _policyPremiumCalculation = policyPremiumCalculation;
            _getUnderwritingInterview = getUnderwritingInterview;
            _currentUserProvider = currentUserProvider;
            _dateTimeProvider = dateTimeProvider;
            _policyInteractionService = policyInteractionService;
            _underwritingBenefitsResponseChangeSubject = underwritingBenefitsResponseChangeSubject;
        }

        public void CompleteReferral(string quoteReferenceNo)
        {
            var policyWrapper = _policyWithRisksService.GetFrom(quoteReferenceNo);
            
             _referralService.CompleteReferral(policyWrapper.Policy.Id, _currentUserProvider.GetForApplication().UserName,
                _dateTimeProvider.GetCurrentDateAndTime());

            _policyService.UpdatePolicyToIncomplete(policyWrapper.Policy);
            _policyInteractionService.PolicyReferralCompletedByUnderwriter(quoteReferenceNo);

            //sync all the underwriting
            foreach (var riskWrapper in policyWrapper.Risks)
            {
                _getUnderwritingInterview.GetInterview(riskWrapper.Risk.InterviewId,
                    riskWrapper.Risk.InterviewConcurrencyToken, _underwritingBenefitsResponseChangeSubject);
            }

            _policyPremiumCalculation.CalculateAndSavePolicy(quoteReferenceNo);
        }

        public DateTime? GetLastCompletedReferralDateTimeForPolicy(string quoteReference)
        {
            var policy = _policyService.GetByQuoteReferenceNumber(quoteReference);

            var lastCompletedReferral = _referralService.GetLastCompletedReferralForPolicy(policy.Id);

            return lastCompletedReferral?.CompletedTS;
        }
    }
}
