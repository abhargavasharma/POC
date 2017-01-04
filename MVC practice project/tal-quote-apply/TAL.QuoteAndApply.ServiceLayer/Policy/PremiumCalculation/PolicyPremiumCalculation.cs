using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.PremiumCalculation;
using TAL.QuoteAndApply.PremiumCalculation.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.PremiumCalculation
{
    public interface IPolicyPremiumCalculation
    {
        PremiumCalculationResult Calculate(string quoteReferenceNo, IPremiumCalculationRequestLoadingAdjuster loadingAdjuster = null);
        PolicyPremiumSummary CalculateAndSavePolicy(string quoteReferenceNo);
        PolicyPremiumSummary CalculateForPremiumType(string quoteReferenceNo, PremiumType premiumType);
        PolicyPremiumSummary CalculateForPremiumFrequency(string quoteReferenceNo, PremiumFrequency premiumFrequency);
    }

    public class PolicyPremiumCalculation : IPolicyPremiumCalculation
    {
        private readonly IGetPremiumCalculationRequestService _getPremiumCalculationRequestService;        
        private readonly IPolicyWithRisksService _policyWithRisksService;
        private readonly IPremiumCalculationService _premiumCalculationService;
        private readonly IPolicyPremiumSummaryConverter _policyPremiumSummaryConverter;
        private readonly IPolicyWithRisksPremiumUpdater _policyWithRisksPremiumUpdater;

        public PolicyPremiumCalculation(
            IGetPremiumCalculationRequestService getPremiumCalculationRequestService,
            IPolicyWithRisksService policyWithRisksService,
            IPremiumCalculationService premiumCalculationService,
            IPolicyPremiumSummaryConverter policyPremiumSummaryConverter,
            IPolicyWithRisksPremiumUpdater policyWithRisksPremiumUpdater)
        {
            _getPremiumCalculationRequestService = getPremiumCalculationRequestService;
            _policyWithRisksService = policyWithRisksService;
            _premiumCalculationService = premiumCalculationService;
            _policyPremiumSummaryConverter = policyPremiumSummaryConverter;
            _policyWithRisksPremiumUpdater = policyWithRisksPremiumUpdater;
        }

        public PremiumCalculationResult Calculate(string quoteReferenceNo, IPremiumCalculationRequestLoadingAdjuster loadingAdjuster = null)
        {
            var policyWithRisks = _policyWithRisksService.GetFrom(quoteReferenceNo);
            var calculationRequest = _getPremiumCalculationRequestService.From(policyWithRisks, loadingAdjuster: loadingAdjuster);
            var calculationResult = _premiumCalculationService.Calculate(calculationRequest);
            return calculationResult;
        }

        public PolicyPremiumSummary CalculateAndSavePolicy(string quoteReferenceNo)
        {
            var calculationResult = Calculate(quoteReferenceNo);

            //reget everything to avoid concurrency issues
            var policyWithRisks = _policyWithRisksService.GetFrom(quoteReferenceNo);
            _policyWithRisksPremiumUpdater.UpdatePremiumsOnPolicyWithRisks(policyWithRisks, calculationResult);
            
            _policyWithRisksService.SaveAll(policyWithRisks);

            return _policyPremiumSummaryConverter.CreateFrom(policyWithRisks);
        }

        public PolicyPremiumSummary CalculateForPremiumType(string quoteReferenceNo, PremiumType premiumType)
        {
            var policyWithRisks = _policyWithRisksService.GetFrom(quoteReferenceNo);

            var calculationRequest = _getPremiumCalculationRequestService.From(policyWithRisks, overridePremiumType: premiumType);

            var calculationResult = _premiumCalculationService.Calculate(calculationRequest);
            _policyWithRisksPremiumUpdater.UpdatePremiumsOnPolicyWithRisks(policyWithRisks, calculationResult);

            return _policyPremiumSummaryConverter.CreateFrom(policyWithRisks);
        }

        public PolicyPremiumSummary CalculateForPremiumFrequency(string quoteReferenceNo, PremiumFrequency premiumFrequency)
        {
            var policyWithRisks = _policyWithRisksService.GetFrom(quoteReferenceNo);

            var calculationRequest = _getPremiumCalculationRequestService.From(policyWithRisks, overridePremiumFrequency: premiumFrequency);

            var calculationResult = _premiumCalculationService.Calculate(calculationRequest);
            _policyWithRisksPremiumUpdater.UpdatePremiumsOnPolicyWithRisks(policyWithRisks, calculationResult);

            return _policyPremiumSummaryConverter.CreateFrom(policyWithRisks);
        }
    }
}
