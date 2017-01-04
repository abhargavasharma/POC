using System;
using System.Collections.Generic;
using TAL.QuoteAndApply.PremiumCalculation.Calculators;
using TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs;
using TAL.QuoteAndApply.PremiumCalculation.Data;
using TAL.QuoteAndApply.PremiumCalculation.Models;

namespace TAL.QuoteAndApply.PremiumCalculation.Services
{
    public interface IGetCoverCalculatorInputService
    {
        CoverCalculatorInput GetCoverCalculatorInput(PremiumCalculatorFactors premiumCalculatorFactors, IEnumerable<IPlanFactors> allPlans, int brandId);
    }

    public class GetCoverCalculatorInputService : IGetCoverCalculatorInputService
    {
        private readonly ICoverDivisionalFactorDtoRepository _coverDivisionalFactorDtoRepository;
        private readonly ICoverBaseRateLookupRequestProvider _coverBaseRateLookupRequestProvider;
        private readonly ICoverBaseRateDtoRepository _coverBaseRateDtoRepository;
        private readonly IGetFactorACalculatorInputService _getFactorACalculatorInputService;
        private readonly IGetFactorBCalculatorInputService _getFactorBCalculatorInputService;

        public GetCoverCalculatorInputService(ICoverDivisionalFactorDtoRepository coverDivisionalFactorDtoRepository,
            ICoverBaseRateLookupRequestProvider coverBaseRateLookupRequestProvider,
            ICoverBaseRateDtoRepository coverBaseRateDtoRepository,
            IGetFactorACalculatorInputService getFactorACalculatorInputService,
            IGetFactorBCalculatorInputService getFactorBCalculatorInputService)
        {
            _coverDivisionalFactorDtoRepository = coverDivisionalFactorDtoRepository;
            _coverBaseRateLookupRequestProvider = coverBaseRateLookupRequestProvider;
            _coverBaseRateDtoRepository = coverBaseRateDtoRepository;
            _getFactorACalculatorInputService = getFactorACalculatorInputService;
            _getFactorBCalculatorInputService = getFactorBCalculatorInputService;
        }

        public CoverCalculatorInput GetCoverCalculatorInput(PremiumCalculatorFactors premiumCalculatorFactors, IEnumerable<IPlanFactors> allPlans, int brandId)
        {
            //base rate lookup
            var baseRate = GetBaseRate(premiumCalculatorFactors); 

            //divsional factor lookup
            var divisionalFactor = GetDivsionalFactor(premiumCalculatorFactors.CoverFactors.CoverCode, premiumCalculatorFactors.PolicyFactors.BrandId);

            var factorA = GetFactorA(premiumCalculatorFactors, allPlans, brandId);
            var factorB = GetFactorB(premiumCalculatorFactors);

            return new CoverCalculatorInput(premiumCalculatorFactors.PlanFactors.CoverAmount, baseRate, factorA, factorB, divisionalFactor);
        }

        private decimal GetFactorB(PremiumCalculatorFactors premiumCalculatorFactors)
        {
            var input = _getFactorBCalculatorInputService.GetFactorBCalculatorInput(premiumCalculatorFactors);

            return FactorBCalculator.Calculate(input);
        }

        private decimal GetFactorA(PremiumCalculatorFactors premiumCalculatorFactors, IEnumerable<IPlanFactors> allPlans, int brandId)
        {
            var input = _getFactorACalculatorInputService.GetFactorACalculatorInput(premiumCalculatorFactors, allPlans, brandId);

            return FactorACalculator.Calculate(input);
        }

        private decimal GetBaseRate(PremiumCalculatorFactors premiumCalculatorFactors)
        {
            var baseRateLookupRequest = _coverBaseRateLookupRequestProvider.GetCoverBaseRateLookupRequestFor(premiumCalculatorFactors);

            var lookupResult = _coverBaseRateDtoRepository.GetBaseRateForCriteria(baseRateLookupRequest);

            if (lookupResult == null)
            {
                throw new ApplicationException($"No cover base rate found for {baseRateLookupRequest}");
            }

            return lookupResult.BaseRate;

        }

        private int GetDivsionalFactor(string coverCode, int brandId)
        {
            var lookupResult = _coverDivisionalFactorDtoRepository.GetCoverDivisionalFactorByCoverCode(coverCode, brandId);

            if (lookupResult == null)
            {
                throw new ApplicationException($"No divisional factor configured for cover code {coverCode}");
            }

            return lookupResult.DivisionalFactor;
        }
    }
}



