using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Plan.Models.Mappers;
using TAL.QuoteAndApply.ServiceLayer.Policy.MarketingStatus;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters;
using TAL.QuoteAndApply.ServiceLayer.Policy.PremiumCalculation;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Plan
{
    public interface IUpdatePlanService
    {
        IEnumerable<PlanPremiumResult> UpdateActivePlanAndCalculatePremium(string quoteReferenceNumber, PlanStateParam planOptionsParam);
        void UpdateSelectedPlans(PlanStateParam planOptionsParam);
        void UpdatePremiumTypeOnAllPlans(string quoteReferenceNumber, int riskId, PremiumType premiumType);
    }

    public class UpdatePlanService : IUpdatePlanService
    {
        private readonly IPlanService _planService;
        private readonly IPlanDtoUpdater _planDtoUpdater;
        private readonly ICoverDtoUpdater _coverDtoUpdater;
        private readonly IOptionService _optionService;
        private readonly IOptionDtoUpdater _optionDtoUpdater;
        private readonly ICoverService _coverService;
        private readonly IPlanOverviewResultProvider _planOverviewResultProvider;
        private readonly IPolicyPremiumCalculation _policyPremiumCalculation;
        private readonly IUpdateMarketingStatusService _updateMarketingStatusService;

        public UpdatePlanService(IPlanService planService, 
            IPlanDtoUpdater planDtoUpdater, 
            ICoverDtoUpdater coverDtoUpdater, 
            IOptionService optionService, 
            IOptionDtoUpdater optionDtoUpdater, 
            ICoverService coverService,
            IPlanOverviewResultProvider planOverviewResultProvider, 
            IPolicyPremiumCalculation policyPremiumCalculation, 
            IUpdateMarketingStatusService updateMarketingStatusService)
        {
            _planService = planService;
            _planDtoUpdater = planDtoUpdater;
            _coverDtoUpdater = coverDtoUpdater;
            _optionService = optionService;
            _optionDtoUpdater = optionDtoUpdater;
            _coverService = coverService;
            _planOverviewResultProvider = planOverviewResultProvider;
            _policyPremiumCalculation = policyPremiumCalculation;
            _updateMarketingStatusService = updateMarketingStatusService;
        }

        private void UpdatePlan(PlanStateParam planOptionsParam, int activePlanId)
        {
            var planDto = _planService.GetById(activePlanId);
            planDto = _planDtoUpdater.UpdateFrom(planDto, planOptionsParam);
            _planService.UpdatePlan(planDto);
        }

        private void UpdateCovers(PlanStateParam planOptionsParam, int activePlanId)
        {
            var dbCoversForPlan = _coverService.GetCoversForPlan(activePlanId);
            foreach (var cover in dbCoversForPlan)
            {
                var coverDto = _coverDtoUpdater.UpdateFrom(cover, planOptionsParam);
                _coverService.UpdateCover(coverDto);
            }
        }

        private void UpdateOptions(PlanStateParam planOptionsParam, int activePlanId)
        {
            var dbOptionsForPlan = _optionService.GetOptionsForPlan(activePlanId);

            foreach (var option in dbOptionsForPlan)
            {
                var optionDto = _optionDtoUpdater.UpdateFrom(option, planOptionsParam);
                _optionService.UpdateOption(optionDto);
            }
        }

        public void UpdateSelectedPlans(PlanStateParam planOptionsParam)
        {
            var plansForRisk = _planOverviewResultProvider.GetFor(planOptionsParam.RiskId);

            foreach (var plan in plansForRisk)
            {
                if (!plan.IsRider)
                {
                    var planSelected =
                        planOptionsParam.AllPlans.FirstOrDefault(p => p.PlanCode == plan.Code)?.Selected ?? false;

                    _planService.SetPlanSelected(planOptionsParam.RiskId, plan.Code, planSelected);
                }
            }
        }

        public void UpdatePremiumTypeOnAllPlans(string quoteReferenceNumber, int riskId, PremiumType premiumType)
        {
            _planService.SetPremiumTypeForAllPlans(riskId, premiumType);
            _policyPremiumCalculation.CalculateAndSavePolicy(quoteReferenceNumber); //Need to recalculate premium since premium type is updated
        }

        public IEnumerable<PlanPremiumResult> UpdateActivePlanAndCalculatePremium(string quoteReferenceNumber, PlanStateParam planOptionsParam)
        {
            UpdatePlan(planOptionsParam, planOptionsParam.PlanId);
            UpdateCovers(planOptionsParam, planOptionsParam.PlanId);
            UpdateOptions(planOptionsParam, planOptionsParam.PlanId);

            if (planOptionsParam.Riders != null)
            {
                foreach (var rider in planOptionsParam.Riders)
                {
                    UpdatePlan(rider, rider.PlanId);
                    UpdateCovers(rider, rider.PlanId);
                    UpdateOptions(rider, rider.PlanId);
                }
            }

            var result = _policyPremiumCalculation.CalculateAndSavePolicy(quoteReferenceNumber);

            //find plans premium, default to first risk..shit but will do for now
            var risk = result.RiskPremiums.First();

            _updateMarketingStatusService.UpdateMarketingStatusForRisk(risk.RiskId);

            foreach (var rider in planOptionsParam.Riders)
            {
                var planCalcSummaryResult = risk.PlanPremiums.First(x => x.PlanCode == rider.PlanCode);

                yield return
                    new PlanPremiumResult(
                        rider.PlanId,
                        rider.PlanCode,
                        planCalcSummaryResult.PlanPremium,
                        planCalcSummaryResult.PlanIncludingRidersPremium,
                        planCalcSummaryResult.PremiumFrequency,
                        planCalcSummaryResult.CoverPremiums.Select(cr => new CoverPremiumResult(cr.CoverCode, cr.CoverPremium)).ToList(),
                        PlanStatus.Valid);
            }

            foreach (var plan in planOptionsParam.AllPlans)
            {
                var planCalcSummaryResult = risk.PlanPremiums.First(x => x.PlanCode == plan.PlanCode);

                yield return
                    new PlanPremiumResult(
                        plan.PlanId, 
                        plan.PlanCode, 
                        planCalcSummaryResult.PlanPremium,
                        planCalcSummaryResult.PlanIncludingRidersPremium,
                        planCalcSummaryResult.PremiumFrequency,
                        planCalcSummaryResult.CoverPremiums.Select(cr=> new CoverPremiumResult(cr.CoverCode, cr.CoverPremium)).ToList(),
                        plan.Status);
            }
        }
    }
}
