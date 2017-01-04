using System;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs;
using TAL.QuoteAndApply.PremiumCalculation.Data;
using TAL.QuoteAndApply.PremiumCalculation.Models;

namespace TAL.QuoteAndApply.PremiumCalculation.Services
{
    public interface IGetFactorBCalculatorInputService
    {
        FactorBCalculatorInput GetFactorBCalculatorInput(PremiumCalculatorFactors premiumCalculatorFactors);
    }

    public class GetFactorBCalculatorInputService : IGetFactorBCalculatorInputService
    {
        private readonly IModalFrequencyFactorDtoRepository _modalFrequencyFactorDtoRepository;
        private readonly IPremiumReliefFactorDtoRepository _premiumReliefFactorDtoRepository;

        public GetFactorBCalculatorInputService(IModalFrequencyFactorDtoRepository modalFrequencyFactorDtoRepository, IPremiumReliefFactorDtoRepository premiumReliefFactorDtoRepository)
        {
            _modalFrequencyFactorDtoRepository = modalFrequencyFactorDtoRepository;
            _premiumReliefFactorDtoRepository = premiumReliefFactorDtoRepository;
        }

        public FactorBCalculatorInput GetFactorBCalculatorInput(PremiumCalculatorFactors premiumCalculatorFactors)
        {
            var premiumReliefOptionFactor = GetPremiumReliefOptionFactor(premiumCalculatorFactors.PlanFactors.PremiumReliefOptionSelected);
            var modalFrequencyFactor = GetModalFrequencyFactor(premiumCalculatorFactors.PolicyFactors.PremiumFrequency,
                premiumCalculatorFactors.PolicyFactors.BrandId);

            return new FactorBCalculatorInput(premiumReliefOptionFactor, modalFrequencyFactor);
        }

        private decimal GetPremiumReliefOptionFactor(bool? premiumReliefOptionSelected)
        {
            var selected = false;
            if (premiumReliefOptionSelected.HasValue)
            {
                selected = premiumReliefOptionSelected.Value;
            }

            var lookupResult = _premiumReliefFactorDtoRepository.GetPremiumReliefFactor(selected, 1);

            if (lookupResult == null)
            {
                throw new ApplicationException($"No premium relief option factor for {selected}");
            }

            return lookupResult.Factor;
        }

        private decimal GetModalFrequencyFactor(PremiumFrequency premiumFrequency, int brandId)
        {
            var lookupResult = _modalFrequencyFactorDtoRepository.GetModalFrequencyFactorForPremiumFrequency(premiumFrequency, brandId);

            if (lookupResult == null)
            {
                throw new ApplicationException($"No modal frequency factor for premium frequency {premiumFrequency}");
            }

            return lookupResult.Factor;
        }
    }
}