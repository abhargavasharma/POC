using System;
using TAL.QuoteAndApply.PremiumCalculation.Data;
using TAL.QuoteAndApply.PremiumCalculation.Models;

namespace TAL.QuoteAndApply.PremiumCalculation.Services
{
    public interface ICoverBaseRateLookupRequestProvider
    {
        CoverBaseRateLookupRequest GetCoverBaseRateLookupRequestFor(PremiumCalculatorFactors premiumCalculatorFactors);
    }

    public class CoverBaseRateLookupRequestProvider : ICoverBaseRateLookupRequestProvider
    {
        private readonly IOccupationMappingDtoRepository _occupationMappingDtoRepository;

        public CoverBaseRateLookupRequestProvider(IOccupationMappingDtoRepository occupationMappingDtoRepository)
        {
            _occupationMappingDtoRepository = occupationMappingDtoRepository;
        }

        public CoverBaseRateLookupRequest GetCoverBaseRateLookupRequestFor(PremiumCalculatorFactors premiumCalculatorFactors)
        {
            switch (premiumCalculatorFactors.CoverFactors.CoverCode)
            {
                case "DTHAC":
                case "DTHIC":
                case "TRSSIN":
                case "TRSCC":
                case "TRSSIC":
                case "TPSAC":
                case "TPSIC":
                    return GetLifeTpdCiBaseRateCriteria(premiumCalculatorFactors);
                case "TPDDTHAC":
                case "TPDDTHIC":
                case "TRADTHSIN":
                case "TRADTHCC":
                case "TRADTHSIC":
                    return GetRiderBaseRateCriteria(premiumCalculatorFactors);
                case "IPSAC":
                case "IPSIC":
                    return GetIpBaseRateCriteria(premiumCalculatorFactors);
                default:
                    throw new ApplicationException($"No base rate criteria available for cover code {premiumCalculatorFactors.CoverFactors.CoverCode}");
            }
        }

        private CoverBaseRateLookupRequest GetIpBaseRateCriteria(PremiumCalculatorFactors premiumCalculatorFactors)
        {
            var occupationGroup = GetOccupationGroup(premiumCalculatorFactors.RiskFactors.OccupationClass);
            var waitingPeriod = GetWaitingPeriod(premiumCalculatorFactors.PlanFactors.WaitingPeriod);

            return new CoverBaseRateLookupRequest(premiumCalculatorFactors.PlanFactors.PlanCode, premiumCalculatorFactors.CoverFactors.CoverCode, premiumCalculatorFactors.RiskFactors.Age,
                premiumCalculatorFactors.RiskFactors.Gender, premiumCalculatorFactors.PlanFactors.PremiumType, null, premiumCalculatorFactors.PlanFactors.BenefitPeriod.Value,
                waitingPeriod.Value, occupationGroup, null, premiumCalculatorFactors.PolicyFactors.BrandId);
        }

        private int? GetWaitingPeriod(int? waitingPeriod)
        {
            if (waitingPeriod.HasValue)
            {
                if (waitingPeriod.Value > 13)
                {
                    return 13;
                }
            }

            return waitingPeriod;
        }

        private CoverBaseRateLookupRequest GetRiderBaseRateCriteria(PremiumCalculatorFactors premiumCalculatorFactors)
        {
            return new CoverBaseRateLookupRequest(premiumCalculatorFactors.PlanFactors.PlanCode, premiumCalculatorFactors.CoverFactors.CoverCode, premiumCalculatorFactors.RiskFactors.Age,
                premiumCalculatorFactors.RiskFactors.Gender, premiumCalculatorFactors.PlanFactors.PremiumType, premiumCalculatorFactors.RiskFactors.Smoker, null, null, null, 
                premiumCalculatorFactors.PlanFactors.BuyBack, premiumCalculatorFactors.PolicyFactors.BrandId);
        }

        private CoverBaseRateLookupRequest GetLifeTpdCiBaseRateCriteria(PremiumCalculatorFactors premiumCalculatorFactors)
        {
            return new CoverBaseRateLookupRequest(premiumCalculatorFactors.PlanFactors.PlanCode, premiumCalculatorFactors.CoverFactors.CoverCode, premiumCalculatorFactors.RiskFactors.Age,
                premiumCalculatorFactors.RiskFactors.Gender, premiumCalculatorFactors.PlanFactors.PremiumType, premiumCalculatorFactors.RiskFactors.Smoker, null, null, null, null, premiumCalculatorFactors.PolicyFactors.BrandId);
        }

        private string GetOccupationGroup(string occupationClass)
        {
            var occupationGroupLookup =
                _occupationMappingDtoRepository.GetOccupationMappingForOccupationClass(occupationClass, 1);

            if (occupationGroupLookup == null)
            {
                throw new ApplicationException($"No occupation mapping for occupation class {occupationClass}");
            }

            return occupationGroupLookup.OccupationGroup;
        }
    }
}