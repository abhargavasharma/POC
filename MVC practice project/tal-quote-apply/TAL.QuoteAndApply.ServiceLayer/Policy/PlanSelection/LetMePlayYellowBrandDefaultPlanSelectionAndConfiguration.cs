using System;
using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.Product.Models;
using TAL.QuoteAndApply.Product.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.ServiceLayer.Product;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.PlanSelection
{
    public class LetMePlayYellowBrandDefaultPlanSelectionAndConfiguration : BasePlanSelectionAndConfigurationService
    {
        private readonly ICoverAmountService _coverAmountService;
        private readonly IPlanEligibilityService _planEligabilityService;
        private readonly ICoverEligibilityService _coverEligibilityService;
        private readonly ICurrentProductBrandProvider _currentProductBrandProvider;

        public LetMePlayYellowBrandDefaultPlanSelectionAndConfiguration(IPlanService planService, ICoverService coverService,
            IOptionService optionService, ICoverAmountService coverAmountService, IPlanEligibilityService planEligabilityService, ICoverEligibilityService coverEligibilityService, ICurrentProductBrandProvider currentProductBrandProvider) : base(planService, coverService, optionService)
        {
            _coverAmountService = coverAmountService;
            _planEligabilityService = planEligabilityService;
            _coverEligibilityService = coverEligibilityService;
            _currentProductBrandProvider = currentProductBrandProvider;
        }

        private void UpdateOption(IEnumerable<IOption> options, string code, IPlan forPlan, Action<IOption> actionOption)
        {
            actionOption.Invoke(options.First(o => o.PlanId == forPlan.Id && o.Code == code));
        }

        public override void SetupSelections(IRisk risk, IEnumerable<IPlan> plans, IEnumerable<ICover> covers, IEnumerable<IOption> options)
        {
            var lifePlan = plans.First(p => p.Code.Equals(ProductPlanConstants.LifePlanCode));
            var lifeCoverAmount = GetCoverAmountBasedOnRules(risk, lifePlan, 500000);
            lifePlan.LinkedToCpi = true;
            lifePlan.CoverAmount = lifeCoverAmount;
            lifePlan.PremiumHoliday = true;
            lifePlan.Selected = true;
            UpdateOption(options, ProductOptionConstants.LifePremiumRelief, lifePlan, option => option.Selected = false);

            var ciPlan = plans.First(p => p.Code.Equals(ProductPlanConstants.CriticalIllnessPlanCode));
            var ciCoverAmount = GetCoverAmountBasedOnRules(risk, ciPlan, 375000);
            ciPlan.CoverAmount = ciCoverAmount;
            ciPlan.LinkedToCpi = true;
            ciPlan.Selected = false;
            ciPlan.PremiumHoliday = true;
            UpdateOption(options, ProductOptionConstants.CiPremiumRelief, ciPlan, option => option.Selected = false);

            var tpdPlan = plans.First(p => p.Code.Equals(ProductPlanConstants.PermanentDisabilityPlanCode));
            var tpdCoverAmount = GetCoverAmountBasedOnRules(risk, tpdPlan, 375000);
            tpdPlan.CoverAmount = tpdCoverAmount;
            tpdPlan.LinkedToCpi = true;
            tpdPlan.PremiumHoliday = true;
            tpdPlan.Selected = true;
            tpdPlan.OccupationDefinition = (risk.IsTpdAny)
                ? OccupationDefinition.AnyOccupation
                : (risk.IsTpdOwn) ? OccupationDefinition.OwnOccupation : OccupationDefinition.Unknown;
            UpdateOption(options, ProductOptionConstants.TpdPremiumRelief, tpdPlan, option => option.Selected = false);

            var ipPlan = plans.First(p => p.Code.Equals(ProductPlanConstants.IncomeProtectionPlanCode));
            var ipCoverAmount = GetCoverAmountBasedOnRules(risk, ipPlan, (int)(risk.AnnualIncome / 12.0 * 0.75));
            ipPlan.CoverAmount = ipCoverAmount;
            ipPlan.LinkedToCpi = true;
            ipPlan.Selected = false;
            ipPlan.PremiumHoliday = true;
            ipPlan.WaitingPeriod = 4;
            ipPlan.BenefitPeriod = 2;

            var tpdRiderPlan = plans.First(p => p.Code.Equals(ProductRiderConstants.PermanentDisabilityRiderCode));
            tpdRiderPlan.OccupationDefinition = (risk.IsTpdAny)
                ? OccupationDefinition.AnyOccupation
                : (risk.IsTpdOwn) ? OccupationDefinition.OwnOccupation : OccupationDefinition.Unknown;
            UpdateOption(options, ProductOptionConstants.TpdRiderDeathBuyBack, tpdRiderPlan, option => option.Selected = true);

            var ciRiderPlan = plans.First(p => p.Code.Equals(ProductRiderConstants.CriticalIllnessRiderCode));
            UpdateOption(options, ProductOptionConstants.CiRiderDeathBuyBack, ciRiderPlan, option => option.Selected = true);

            UpdateOption(options, ProductOptionConstants.IpDayOneAccident, ipPlan, option => option.Selected = false);
            UpdateOption(options, ProductOptionConstants.IpIncreasingClaims, ipPlan, option => option.Selected = false);

            var lifeCovers = covers.Where(c => c.PlanId == lifePlan.Id);
            foreach (var cover in lifeCovers)
            {
                cover.Selected = true;
            }

            var ciCovers = covers.Where(c => c.PlanId == ciPlan.Id);
            foreach (var cover in ciCovers)
            {
                cover.Selected = true;
            }

            var tpdCovers = covers.Where(c => c.PlanId == tpdPlan.Id);
            foreach (var cover in tpdCovers)
            {
                cover.Selected = true;
            }

            var ipCovers = covers.Where(c => c.PlanId == ipPlan.Id);
            foreach (var cover in ipCovers)
            {
                cover.Selected = true;
            }

            if (!IsPlanAvailable(risk, lifePlan, lifeCovers) && !IsPlanAvailable(risk, ipPlan, ipCovers))
            {
                if (IsPlanAvailable(risk, tpdPlan, tpdCovers))
                {
                    tpdPlan.Selected = true;
                }
                else if (IsPlanAvailable(risk, ciPlan, ciCovers))
                {
                    ciPlan.Selected = true;
                }
            }
        }

        private bool IsPlanAvailable(IRisk risk, IPlan plan, IEnumerable<ICover> planCovers)
        {
            var coverEligibility = _coverEligibilityService.GetCoverEligibilityResults(planCovers);
            var lifeEligibility = _planEligabilityService.IsRiskEligibleForPlan(risk, plan, coverEligibility);
            return lifeEligibility.IsAvailable;
        }

        private int GetCoverAmountBasedOnRules(IRisk risk, IPlan ipPlan, int ipCoverAmount)
        {
            var currentBrand = _currentProductBrandProvider.GetCurrent();
            var minCoverAmount = _coverAmountService.GetMinCover(ipPlan.Code, currentBrand.BrandCode);
            ipCoverAmount = Math.Max(minCoverAmount, ipCoverAmount);
            var maxCoverAmount = _coverAmountService.GetMaxCover(new MaxCoverAmountParam(ipPlan.Code, currentBrand.BrandCode, risk.DateOfBirth.Age(),
                risk.AnnualIncome, ipCoverAmount, null));
            ipCoverAmount = Math.Min(ipCoverAmount, maxCoverAmount);
            return ipCoverAmount;
        }
    }
}