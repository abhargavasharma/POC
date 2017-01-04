using System;
using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs;
using TAL.QuoteAndApply.PremiumCalculation.Data;
using TAL.QuoteAndApply.PremiumCalculation.Models;

namespace TAL.QuoteAndApply.PremiumCalculation.Services
{
    public interface IGetFactorACalculatorInputService
    {
        FactorACalculatorInput GetFactorACalculatorInput(PremiumCalculatorFactors premiumCalculatorFactors, IEnumerable<IPlanFactors> allPlans, int brandId);
    }

    public class GetFactorACalculatorInputService : IGetFactorACalculatorInputService
    {
        private readonly IMultiPlanDiscountFactorService _multiPlanDiscountFactorService;
        private readonly ILargeSumInsuredDiscountFactorDtoRepository _largeSumInsuredDiscountFactorDtoRepository;
        private readonly IOccupationClassFactorDtoRepository _occupationClassFactorDtoRepository;
        private readonly ISmokerFactorDtoRepository _smokerFactorDtoRepository;
        private readonly IIncreasingClaimsFactorDtoRepository _increasingClaimsFactorDtoRepository;
        private readonly IIndemnityFactorDtoRepository _indemnityFactorDtoRepository;
        private readonly IWaitingPeriodFactorDtoRepository _waitingPeriodFactorDtoRepository;
        private readonly IOccupationDefinitionTypeFactorDtoRepository _occupationDefinitionTypeFactorDtoRepository;

        public GetFactorACalculatorInputService(IMultiPlanDiscountFactorService multiPlanDiscountFactorService, 
            ILargeSumInsuredDiscountFactorDtoRepository largeSumInsuredDiscountFactorDtoRepository, 
            IOccupationClassFactorDtoRepository occupationClassFactorDtoRepository, 
            ISmokerFactorDtoRepository smokerFactorDtoRepository, 
            IIncreasingClaimsFactorDtoRepository increasingClaimsFactorDtoRepository, 
            IIndemnityFactorDtoRepository indemnityFactorDtoRepository, 
            IWaitingPeriodFactorDtoRepository waitingPeriodFactorDtoRepository, 
            IOccupationDefinitionTypeFactorDtoRepository occupationDefinitionTypeFactorDtoRepository)
        {
            _multiPlanDiscountFactorService = multiPlanDiscountFactorService;
            _largeSumInsuredDiscountFactorDtoRepository = largeSumInsuredDiscountFactorDtoRepository;
            _occupationClassFactorDtoRepository = occupationClassFactorDtoRepository;
            _smokerFactorDtoRepository = smokerFactorDtoRepository;
            _increasingClaimsFactorDtoRepository = increasingClaimsFactorDtoRepository;
            _indemnityFactorDtoRepository = indemnityFactorDtoRepository;
            _waitingPeriodFactorDtoRepository = waitingPeriodFactorDtoRepository;
            _occupationDefinitionTypeFactorDtoRepository = occupationDefinitionTypeFactorDtoRepository;
        }

        public FactorACalculatorInput GetFactorACalculatorInput(PremiumCalculatorFactors premiumCalculatorFactors, IEnumerable<IPlanFactors> allPlans, int brandId)
        {
            var multiPlanDiscountFactor = _multiPlanDiscountFactorService.GetFor(premiumCalculatorFactors.PlanFactors, allPlans, brandId);
            var largeSumInsuredDiscountFactor =
                GetLargeSumInsuredDiscountFactor(premiumCalculatorFactors.PlanFactors.PlanCode,
                    premiumCalculatorFactors.PolicyFactors.BrandId,
                    premiumCalculatorFactors.PlanFactors.CoverAmount);

            var occupationClassFactor = GetOccupationFactor(premiumCalculatorFactors.RiskFactors.Gender,
                premiumCalculatorFactors.RiskFactors.OccupationClass, premiumCalculatorFactors.PlanFactors.PlanCode);
            var smokerFactor = GetSmokerFactor(premiumCalculatorFactors.RiskFactors.Smoker,
                premiumCalculatorFactors.PlanFactors.PlanCode);
            var indemnityOptionFactor = GetIndemnityFactor(premiumCalculatorFactors.PlanFactors.PlanCode,
                premiumCalculatorFactors.PolicyFactors.BrandId);
            var increasingClaimsOptionFactor =
                GetIncreasingClaimsFactor(premiumCalculatorFactors.PlanFactors.IncreasingClaimsSelected,
                    premiumCalculatorFactors.PlanFactors.BenefitPeriod, premiumCalculatorFactors.PlanFactors.PlanCode,
                    premiumCalculatorFactors.PolicyFactors.BrandId);
            var waitingPeriodFactor = GetWaitingPeriodFactor(premiumCalculatorFactors.PlanFactors.WaitingPeriod,
                premiumCalculatorFactors.PlanFactors.PlanCode);

            var occDefinitionFactor = GetOccupationDefinitionFactor(premiumCalculatorFactors.PlanFactors.OccupationDefinition);
            var occLoadingFactor = GetOccupatioLoadingFactor(premiumCalculatorFactors.PlanFactors.OccupationLoading);

            return new FactorACalculatorInput(
                premiumCalculatorFactors.PlanFactors.IncludeInMultiPlanDiscount, 
                multiPlanDiscountFactor, 
                largeSumInsuredDiscountFactor,
                occupationClassFactor, 
                smokerFactor, 
                indemnityOptionFactor, 
                increasingClaimsOptionFactor, 
                waitingPeriodFactor,
                occDefinitionFactor,
                occLoadingFactor);
        }

        private decimal GetOccupatioLoadingFactor(decimal? occupationLoading)
        {
            return occupationLoading.GetValueOrDefault(1m);
        }

        private decimal GetOccupationFactor(Gender gender, string occupationClass, string planCode)
        {
            var lookupResult =
                _occupationClassFactorDtoRepository.GetOccupationClassFactorByGenderOccupationClassAndPlan(gender,
                    occupationClass, planCode, 1);

            if (lookupResult == null)
                return 1;

            return lookupResult.Factor;
        }

        private decimal GetSmokerFactor(bool smoker, string planCode)
        {
            var lookupResult =
                _smokerFactorDtoRepository.GetSmokerFactorBySmokerAndPlan(smoker, planCode, 1);

            if (lookupResult == null)
                return 1;

            return lookupResult.Factor;
        }

        private decimal GetIndemnityFactor(string planCode, int brandId)
        {
            var lookupResult =
                _indemnityFactorDtoRepository.GetIndemnityFactorByPlanCode(planCode, brandId);

            if (lookupResult == null)
                return 1;

            return lookupResult.Factor;
        }

        private decimal GetIncreasingClaimsFactor(bool? increasingClaimsEnabled, int? benefitPeriod, string planCode, int brandId)
        {
            var lookupResult =
                _increasingClaimsFactorDtoRepository.GetIncreasingClaimFactor(planCode, brandId,
                    increasingClaimsEnabled.GetValueOrDefault(false), benefitPeriod);

            if (lookupResult == null)
                return 1;

            return lookupResult.Factor;
        }

        private decimal GetWaitingPeriodFactor(int? waitingPeriod, string planCode)
        {
            var lookupResult =
                _waitingPeriodFactorDtoRepository.GetWaitingPeriodFactorByWaitingPeriod(waitingPeriod, planCode, 1);

            if (lookupResult == null)
                return 1;

            return lookupResult.Factor;
        }

        private decimal GetOccupationDefinitionFactor(OccupationDefinition occupationDefinition)
        {
            var lookupResult =
                _occupationDefinitionTypeFactorDtoRepository.GetOccupationDefinitionTypeFactorForOccupationDefinition(occupationDefinition, 1);

            if (lookupResult == null)
                return 1;

            return lookupResult.Factor;
        }

        private decimal GetLargeSumInsuredDiscountFactor(string planCode, int brandId, decimal coverAmount)
        {
            var lookupResult = _largeSumInsuredDiscountFactorDtoRepository.GetLargeSumInsuredDiscountForSumInsured(coverAmount, planCode, brandId);

            if (lookupResult == null)
            {
                throw new ApplicationException($"No large sum insured discount for plan code {planCode} and cover amount {coverAmount}");
            }

            return lookupResult.Factor;
        }
    }
}