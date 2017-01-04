using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.ServiceLayer.Policy.PremiumCalculation;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public interface IPolicyAutoUpdateService
    {
        void AutoUpdatePlansForEligibililityAndRecalculatePremium(string quoteReference);
    }

    public class PolicyAutoUpdateService : IPolicyAutoUpdateService
    {
        private readonly IPolicyPremiumCalculation _policyPremiumCalculation;
        private readonly IPlanAutoUpdateService _planAutoUpdateService;
        private readonly IPolicyWithRisksService _policyWithRisksService;

        public PolicyAutoUpdateService(IPolicyPremiumCalculation policyPremiumCalculation,
            IPlanAutoUpdateService planAutoUpdateService, IPolicyWithRisksService policyWithRisksService)
        {
            _policyPremiumCalculation = policyPremiumCalculation;
            _planAutoUpdateService = planAutoUpdateService;
            _policyWithRisksService = policyWithRisksService;
        }

        public void AutoUpdatePlansForEligibililityAndRecalculatePremium(string quoteReference)
        {
            var policyWithRisks = _policyWithRisksService.GetFrom(quoteReference);
            foreach (var risk in policyWithRisks.Risks)
            {
                _planAutoUpdateService.UpdatePlansToConformWithPlanEligiblityRules(risk.Risk);
            }

            _policyPremiumCalculation.CalculateAndSavePolicy(quoteReference);
        }
    }
}
