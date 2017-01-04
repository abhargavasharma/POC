using TAL.QuoteAndApply.PremiumCalculation.Calculators;
using TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs;
using TAL.QuoteAndApply.PremiumCalculation.Data;
using TAL.QuoteAndApply.PremiumCalculation.Models;

namespace TAL.QuoteAndApply.PremiumCalculation.Services
{

    public interface IGetLoadingCalculatorInputService
    {
        LoadingCalculatorInput GetLoadingCalculatorInput(PremiumCalculatorFactors premiumCalculatorFactors, decimal planBasePremium);
    }

    public class GetLoadingCalculatorInputService : IGetLoadingCalculatorInputService
    {
        private readonly IPercentageLoadingFactorDtoRepository _percentageLoadingFactorDtoRepository;
        private readonly IPerMilleLoadingFactorDtoRepository _perMilleLoadingFactorDtoRepository;
        private readonly IGetFactorBCalculatorInputService _getFactorBCalculatorInputService;

        public GetLoadingCalculatorInputService(
            IPercentageLoadingFactorDtoRepository percentageLoadingFactorDtoRepository,
            IPerMilleLoadingFactorDtoRepository perMilleLoadingFactorDtoRepository,
            IGetFactorBCalculatorInputService getFactorBCalculatorInputService)
        {
            _percentageLoadingFactorDtoRepository = percentageLoadingFactorDtoRepository;
            _perMilleLoadingFactorDtoRepository = perMilleLoadingFactorDtoRepository;
            _getFactorBCalculatorInputService = getFactorBCalculatorInputService;
        }

        public LoadingCalculatorInput GetLoadingCalculatorInput(PremiumCalculatorFactors premiumCalculatorFactors, decimal planBasePremium)
        {
            var factorBInput = _getFactorBCalculatorInputService.GetFactorBCalculatorInput(premiumCalculatorFactors);
            var factorB = FactorBCalculator.Calculate(factorBInput);

            var percentageLoadingFactor = GetPercentageLoadingFactor(premiumCalculatorFactors.CoverFactors.CoverCode, premiumCalculatorFactors.CoverFactors.BrandId);
            var perMilleLoadingFactor = GetPerMillLoadingFactor(premiumCalculatorFactors.CoverFactors.CoverCode, premiumCalculatorFactors.CoverFactors.BrandId);

            return new LoadingCalculatorInput(planBasePremium, 
                premiumCalculatorFactors.PlanFactors.CoverAmount, 
                factorB, 
                premiumCalculatorFactors.CoverFactors,
                percentageLoadingFactor,
                perMilleLoadingFactor);
        }

        private decimal GetPerMillLoadingFactor(string coverCode, int brandId)
        {
            var perMilleLoadingFactor =
                _perMilleLoadingFactorDtoRepository.GetPerMilleLoadingFactorByCoverCode(coverCode, brandId);

            if (perMilleLoadingFactor == null)
            {
                return 0;
            }

            return perMilleLoadingFactor.Factor;
        }

        private decimal GetPercentageLoadingFactor(string coverCode, int brandId)
        {
            var percentageLoadingFactor =
                _percentageLoadingFactorDtoRepository.GetPercentageLoadingFactorByCoverCode(coverCode, brandId);

            if (percentageLoadingFactor == null)
            {
                return 0;
            }

            return percentageLoadingFactor.Factor;
        }
    }
}
