using System;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.PremiumCalculation.Calculators;
using TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs;
using TAL.QuoteAndApply.PremiumCalculation.Data;
using TAL.QuoteAndApply.PremiumCalculation.Models;

namespace TAL.QuoteAndApply.PremiumCalculation.Services
{
    public interface IGetDayOneAccidentCalculatorInputService
    {
        DayOneAccidentCalculatorInput GetDayOneAccidentCalculatorInput(PremiumCalculatorFactors premiumCalculatorFactors);
    }
     
    public class GetDayOneAccidentCalculatorInputService : IGetDayOneAccidentCalculatorInputService
    {
        private readonly IDayOneAccidentBaseRateDtoRepository _dayOneAccidentBaseRateDtoRepository;
        private readonly IGetFactorBCalculatorInputService _getFactorBCalculatorInputService;
        private readonly IOccupationClassFactorDtoRepository _occupationClassFactorDtoRepository;

        public GetDayOneAccidentCalculatorInputService(IDayOneAccidentBaseRateDtoRepository dayOneAccidentBaseRateDtoRepository, 
            IGetFactorBCalculatorInputService getFactorBCalculatorInputService, 
            IOccupationClassFactorDtoRepository occupationClassFactorDtoRepository)
        {
            _getFactorBCalculatorInputService = getFactorBCalculatorInputService;
            _occupationClassFactorDtoRepository = occupationClassFactorDtoRepository;
            _dayOneAccidentBaseRateDtoRepository = dayOneAccidentBaseRateDtoRepository;
        }

        public DayOneAccidentCalculatorInput GetDayOneAccidentCalculatorInput(PremiumCalculatorFactors premiumCalculatorFactors)
        {
            var baseRate = GetDayOneAccidentBaseRate(premiumCalculatorFactors.PlanFactors.PlanCode, 
                premiumCalculatorFactors.CoverFactors.CoverCode,
                premiumCalculatorFactors.PolicyFactors.BrandId,
                premiumCalculatorFactors.RiskFactors.Age,
                premiumCalculatorFactors.RiskFactors.Gender,
                premiumCalculatorFactors.PlanFactors.PremiumType,
                premiumCalculatorFactors.PlanFactors.WaitingPeriod);

            var occupationFactor = GetOccupationFactor(premiumCalculatorFactors.RiskFactors.Gender, premiumCalculatorFactors.RiskFactors.OccupationClass, premiumCalculatorFactors.PlanFactors.PlanCode);
            var factorB = GetFactorB(premiumCalculatorFactors);
            var coverAmount = premiumCalculatorFactors.PlanFactors.CoverAmount;

            return new DayOneAccidentCalculatorInput(premiumCalculatorFactors.PlanFactors.DayOneAccidentSelected, baseRate, occupationFactor,
                coverAmount, factorB);
        }

        private decimal GetDayOneAccidentBaseRate(string planCode, string coverCode, int brandId, int age, Gender gender, PremiumType premiumType, int? waitingPeriod)
        {
            var lookupResult = _dayOneAccidentBaseRateDtoRepository.GetAccidentBaseRate(planCode, coverCode, brandId, age, gender, premiumType, waitingPeriod);

            if (lookupResult == null)
            {
                return 0;
            }

            return lookupResult.BaseRate;
        }

        private decimal GetOccupationFactor(Gender gender, string occupationClass, string planCode)
        {
            var lookupResult =
                _occupationClassFactorDtoRepository.GetOccupationClassFactorByGenderOccupationClassAndPlan(gender,
                    occupationClass, planCode, 1);

            if (lookupResult == null)
            {
                return 0;
            }

            return lookupResult.Factor;
        }

        private decimal GetFactorB(PremiumCalculatorFactors premiumCalculatorFactors)
        {
            var input = _getFactorBCalculatorInputService.GetFactorBCalculatorInput(premiumCalculatorFactors);
            return FactorBCalculator.Calculate(input);
        }
    }
}
