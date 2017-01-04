using System;
using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.PremiumCalculation.Configuration;
using TAL.QuoteAndApply.PremiumCalculation.Data;
using TAL.QuoteAndApply.PremiumCalculation.Models;

namespace TAL.QuoteAndApply.PremiumCalculation.Services
{
    public interface IMultiPlanDiscountFactorService
    {
        decimal GetFor(IPlanFactors planFactors, IEnumerable<IPlanFactors> allPlans, int brandId);
    }

    public class MultiPlanDiscountFactorService : IMultiPlanDiscountFactorService
    {
        private readonly IPlanMinimumCoverAmountForMultiPlanDiscountDtoRepository _planMinimumCoverAmountForMultiPlanDiscount;
        private readonly IMultiPlanDiscountFactorDtoRepository _multiPlanDiscountFactor;
        private readonly IPremiumCalculationConfigurationProvider _premiumCalculationConfigurationProvider;

        public MultiPlanDiscountFactorService(IPlanMinimumCoverAmountForMultiPlanDiscountDtoRepository planMinimumCoverAmountForMultiPlanDiscount, 
            IMultiPlanDiscountFactorDtoRepository multiPlanDiscountFactor, IPremiumCalculationConfigurationProvider premiumCalculationConfigurationProvider)
        {
            _planMinimumCoverAmountForMultiPlanDiscount = planMinimumCoverAmountForMultiPlanDiscount;
            _multiPlanDiscountFactor = multiPlanDiscountFactor;
            _premiumCalculationConfigurationProvider = premiumCalculationConfigurationProvider;
        }

        public decimal GetFor(IPlanFactors planFactors, IEnumerable<IPlanFactors> allPlans, int brandId)
        {
            if (planFactors.IncludeInMultiPlanDiscount)
            {
                int includedPlanCount = 0;

                foreach (var plan in allPlans.Where(p=> p.Active))
                {
                    var minCoverAmount = GetMinimumCoverAmountForMultiPlanDiscount(plan.PlanCode, brandId);

                    if (plan.CoverAmount >= minCoverAmount)
                    {
                        includedPlanCount++;
                    }
                }

                if (includedPlanCount > _premiumCalculationConfigurationProvider.MultiPlanDiscountPlanLimit)
                {
                    includedPlanCount = _premiumCalculationConfigurationProvider.MultiPlanDiscountPlanLimit;
                }

                return GetMultiPlanDiscountFactor(includedPlanCount, brandId);
            }

            return 1;
        }

        private decimal GetMinimumCoverAmountForMultiPlanDiscount(string planCode, int brandId)
        {
            var lookupResult = _planMinimumCoverAmountForMultiPlanDiscount.GetMinimumCoverAmountForMultiPlanDiscount(planCode, brandId);

            if (lookupResult == null)
            {
                throw new ApplicationException($"No plan minimum cover amount for multi plan discount for: {planCode}");
            }

            return lookupResult.MinimumCoverAmount;
        }

        private decimal GetMultiPlanDiscountFactor(int planCount, int brandId)
        {
            var lookupResult = _multiPlanDiscountFactor.GetMultiPlanDiscountFactorForPlanCount(planCount, brandId);

            if (lookupResult == null)
            {
                throw new ApplicationException($"No multi plan discount for plan count of {planCount}");
            }

            return lookupResult.Factor;
        }
    }
}
