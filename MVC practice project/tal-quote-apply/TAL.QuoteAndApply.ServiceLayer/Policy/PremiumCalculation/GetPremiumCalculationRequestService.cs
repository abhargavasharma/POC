using System;
using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.DataModel.Underwriting;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.PremiumCalculation.Models;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.ServiceLayer.Product;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.PremiumCalculation
{
    public interface IGetPremiumCalculationRequestService
    {
        PremiumCalculationRequest From(PolicyWithRisks policyWithRisks, PremiumType? overridePremiumType = null,
            PremiumFrequency? overridePremiumFrequency = null, IPremiumCalculationRequestLoadingAdjuster loadingAdjuster = null);
    }

    public class GetPremiumCalculationRequestService : IGetPremiumCalculationRequestService
    {
        private readonly IPlanDefinitionProvider _planDefinitionProvider;
        private readonly ICoverDefinitionProvider _coverDefinitionProvider;
        private readonly ICoverLoadingService _coverLoadingService;
        private readonly IPlanService _planService;
        private readonly ICurrentProductBrandProvider _currentProductBrandProvider;
        private readonly IProductBrandProvider _productBrandProvider;

        public GetPremiumCalculationRequestService(IPlanDefinitionProvider planDefinitionProvider,
            ICoverDefinitionProvider coverDefinitionProvider,
            ICoverLoadingService coverLoadingService, IPlanService planService, ICurrentProductBrandProvider currentProductBrandProvider, IProductBrandProvider productBrandProvider)
        {
            _planDefinitionProvider = planDefinitionProvider;
            _coverDefinitionProvider = coverDefinitionProvider;
            _coverLoadingService = coverLoadingService;
            _planService = planService;
            _currentProductBrandProvider = currentProductBrandProvider;
            _productBrandProvider = productBrandProvider;
        }

        public PremiumCalculationRequest From(PolicyWithRisks policyWithRisks, PremiumType? overridePremiumType = null,
            PremiumFrequency? overridePremiumFrequency = null, IPremiumCalculationRequestLoadingAdjuster loadingAdjuster = null)
        {
            var riskRequests = new List<RiskCalculationRequest>();

            foreach (var riskWrapper in policyWithRisks.Risks)
            {
                var planRequests = new List<PlanCalculationRequest>();

                var allPlans = riskWrapper.Plans.Select(p => p.Plan);

                var brandKey = _productBrandProvider.GetBrandKeyForRisk(riskWrapper.Risk);

                foreach (var planWrapper in riskWrapper.Plans)
                {
                    var coverRequests = planWrapper.Covers.Select(c => From(c, loadingAdjuster, brandKey)).ToList();

                    PlanWithCovers parentPlanWrapper = null;
                    
                    var parentPlan = _planService.GetParentPlanForPlan(planWrapper.Plan, allPlans);

                    if (parentPlan != null)
                    {
                        parentPlanWrapper = riskWrapper.Plans.First(p => p.Plan.Code == parentPlan.Code);
                    }

                    planRequests.Add(From(planWrapper.Plan, parentPlanWrapper, riskWrapper.Risk, brandKey, planWrapper.Options, coverRequests, overridePremiumType));
                }

                riskRequests.Add(From(riskWrapper.Risk, planRequests));
            }

            var calcPremiumFrequency = policyWithRisks.Policy.PremiumFrequency;
            if (overridePremiumFrequency.HasValue)
            {
                calcPremiumFrequency = overridePremiumFrequency.Value;
            }

            return new PremiumCalculationRequest(calcPremiumFrequency, riskRequests, _currentProductBrandProvider.GetCurrent().Id);
        }

        private RiskCalculationRequest From(IRisk risk, IReadOnlyList<PlanCalculationRequest> planCalculationRequests)
        {
            //assumes SmokerStatus.Unkown is not a smoker
            var smoker = risk.SmokerStatus == SmokerStatus.Yes;

            //todo: should this stay, or should occupation be a mandatory field?
            var occupationClass = risk.OccupationClass;
            if (string.IsNullOrWhiteSpace(occupationClass))
            {
                occupationClass = "AAA";
            }

            return new RiskCalculationRequest(risk.Id, risk.DateOfBirth.AgeNextBirthday(), risk.Gender, smoker,
                occupationClass, planCalculationRequests);
        }

        private PlanCalculationRequest From(IPlan plan, 
            PlanWithCovers parentPlan, 
            IRisk risk, 
            string brandKey,
            IEnumerable<IOption> options,
            IReadOnlyList<CoverCalculationRequest> coverCalculationRequests, 
            PremiumType? overridePremiumType = null)
        {
            bool? buyBackSelected = GetOptionValue(options, plan.Code + "DBB");

            bool? premiumReliefOptionSelected;
            bool planSelected = plan.Selected;

            if (parentPlan == null)
            {
                premiumReliefOptionSelected = GetOptionValue(options, "PR");
            }
            else
            {
                premiumReliefOptionSelected = GetOptionValue(parentPlan.Options, "PR");
                planSelected = _planService.IsPlanSelected(plan, parentPlan.Plan);
            }


            bool? increasingClaims = GetOptionValue(options, "IC");
            bool? dayOneAccident = GetOptionValue(options, "DOA");

            var planDefinition = _planDefinitionProvider.GetPlanByCode(plan.Code, brandKey);

            var includedInMultiPlanDiscount =
                planDefinition.IncludedInMultiPlanDiscount;

            var occupationDefinition = OccupationDefinition.Unknown;
            decimal? occupationLoading = null;

            if (planDefinition.UseOccupationDefinition)
            {
                occupationDefinition = plan.OccupationDefinition;
                occupationLoading = risk.TpdLoading;
            }
            
            return new PlanCalculationRequest(plan.Code,
                planSelected, 
                includedInMultiPlanDiscount,
                plan.CoverAmount, 
                overridePremiumType ?? plan.PremiumType, 
                buyBackSelected, 
                plan.WaitingPeriod, 
                plan.BenefitPeriod,
                occupationDefinition,
                occupationLoading,
                premiumReliefOptionSelected,
                increasingClaims,
                dayOneAccident,
                coverCalculationRequests);
        }

        private bool? GetOptionValue(IEnumerable<IOption> options, string optionCode)
        {
            var matchingOption =
                options.FirstOrDefault(o => o.Code.Equals(optionCode, StringComparison.InvariantCultureIgnoreCase));

            if (matchingOption == null)
            {
                return null;
            }

            return matchingOption.Selected;
        }

        private CoverCalculationRequest From(ICover cover, IPremiumCalculationRequestLoadingAdjuster loadingAdjuster, string brandKey)
        {
            var coverDefinition = _coverDefinitionProvider.GetCoverDefinitionByCode(cover.Code, brandKey);

            bool isRatableCover = coverDefinition.IsRateableCover;
            
            return new CoverCalculationRequest(cover.Code, 
                cover.Selected, 
                isRatableCover, 
                coverDefinition.SupportedLoadingTypes.Contains(LoadingType.Variable), 
                coverDefinition.SupportedLoadingTypes.Contains(LoadingType.PerMille), 
                LoadingsFrom(cover, loadingAdjuster), _currentProductBrandProvider.GetCurrent().Id);
        }

        private Loadings LoadingsFrom(ICover cover, IPremiumCalculationRequestLoadingAdjuster loadingAdjuster)
        {
            var allLoadings = _coverLoadingService.GetCoverLoadingsForCover(cover);

            var percentageLoadings = 0m;

            var pcLoading = _coverLoadingService.GetPercentageLoading(allLoadings);
            if (pcLoading != null)
            {
                percentageLoadings = pcLoading.Loading;
            }

            var perMilleLoadings = 0m;
            var pmLoading = _coverLoadingService.GetPerMilleLoading(allLoadings);
            if (pmLoading != null)
            {
                perMilleLoadings = pmLoading.Loading;
            }

            //Return loadings (that may need to be adjusted)
            var unadjustedLoadings = new Loadings(percentageLoadings, perMilleLoadings);
            var loadingAdjusterOrDefault = loadingAdjuster ?? new NullPremiumCalcLoadingAdjuster();
            return loadingAdjusterOrDefault.GetAdjustedLoadings(unadjustedLoadings, cover.Code);
        }
    }
}