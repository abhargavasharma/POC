using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.PremiumCalculation;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public interface IUpdatePolicyService
    {
        PolicyPremiumSummary UpdatePremiumFrequency(string quoteReferenceNumber, PremiumFrequency premiumFrequency);
        void UpdateProgress(string quoteReferenceNumber, PolicyProgress progress);
        void UpdateOwnerType(string quoteReferenceNumber, PolicyOwnerType ownerType);
        void UpdateOwnerPartyDetails(string quoteReferenceNumber, PolicyOwnerDetailsParam ownerDetails);
    }

    public class UpdatePolicyService : IUpdatePolicyService
    {
        private readonly IPolicyService _policyService;
        private readonly IPolicyPremiumCalculation _policyPremiumCalculation;
        private readonly IPolicyInteractionService _policyInteractionService;
        private readonly IPolicyOwnershipService _policyOwnershipService;

        public UpdatePolicyService(IPolicyService policyService, IPolicyPremiumCalculation policyPremiumCalculation, IPolicyInteractionService policyInteractionService, IPolicyOwnershipService policyOwnershipService)
        {
            _policyService = policyService;
            _policyPremiumCalculation = policyPremiumCalculation;
            _policyInteractionService = policyInteractionService;
            _policyOwnershipService = policyOwnershipService;
        }

        public PolicyPremiumSummary UpdatePremiumFrequency(string quoteReferenceNumber, PremiumFrequency premiumFrequency)
        {
            _policyService.UpdatePolicyPremiumFrequency(quoteReferenceNumber, premiumFrequency);

            var premiumCalculationResult = _policyPremiumCalculation.CalculateAndSavePolicy(quoteReferenceNumber);

            return premiumCalculationResult;
        }

        public void UpdateProgress(string quoteReferenceNumber, PolicyProgress progress)
        {
            _policyService.UpdatePolicyProgress(quoteReferenceNumber, progress);
            var policy = _policyService.GetByQuoteReferenceNumber(quoteReferenceNumber);
            _policyInteractionService.PolicyProgressChanged(policy.Id);
        }

        public void UpdateOwnerType(string quoteReferenceNumber, PolicyOwnerType ownerType)
        {
            switch (ownerType)
            {
                case PolicyOwnerType.SuperannuationFund:
                    _policyOwnershipService.SetSuperOwnership(quoteReferenceNumber);
                    break;
                case PolicyOwnerType.SelfManagedSuperFund:
                    _policyOwnershipService.SetSmsfOwnership(quoteReferenceNumber);
                    break;
                case PolicyOwnerType.Ordinary:
                    _policyOwnershipService.SetOrdinaryOwnership(quoteReferenceNumber);
                    break;
            }
        }

        public void UpdateOwnerPartyDetails(string quoteReferenceNumber, PolicyOwnerDetailsParam ownerDetails)
        {
            _policyOwnershipService.UpdateOwnerPartyDetails(quoteReferenceNumber, ownerDetails);
        }
    }
}
