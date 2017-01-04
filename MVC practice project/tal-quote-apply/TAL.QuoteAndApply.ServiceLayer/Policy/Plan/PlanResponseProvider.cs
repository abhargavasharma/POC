using System;
using System.Collections.Generic;
using System.Linq;
using System.Monads;
using TAL.QuoteAndApply.DataModel.Underwriting;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Product.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Product;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Plan
{
    public interface IPlanResponseProvider
    {
        IEnumerable<PlanResponse> MapStoredPlansToProduct(string quoteReferenceNumber, IEnumerable<PlanResponse> plans,
            IEnumerable<PlanOverviewResult> savedPlans);
    }

    public class PlanResponseProvider : IPlanResponseProvider
    {
        private readonly ICoverService _coverService;
        private readonly IOptionService _optionService;
        private readonly IPolicyService _policyService;
        private readonly ICoverExclusionsService _coverExclusionsService;
        private readonly IPlanVariableResponseConverter _variableResponseConverter;

        public PlanResponseProvider(ICoverService coverService, IOptionService optionService,
            IPolicyService policyService, IPlanVariableResponseConverter variableResponseConverter,
            ICoverExclusionsService coverExclusionsService)
        {
            _coverService = coverService;
            _optionService = optionService;
            _policyService = policyService;
            _variableResponseConverter = variableResponseConverter;
            _coverExclusionsService = coverExclusionsService;
        }

        public IEnumerable<PlanResponse> MapStoredPlansToProduct(string quoteReferenceNumber, IEnumerable<PlanResponse> plans, IEnumerable<PlanOverviewResult> savedPlans)
        {
            var policy = _policyService.GetByQuoteReferenceNumber(quoteReferenceNumber);
            return MapStoredPlansToProduct(policy, plans, savedPlans); 
        }

        public IEnumerable<PlanResponse> MapStoredPlansToProduct(IPolicy policy, IEnumerable<PlanResponse> plans, IEnumerable<PlanOverviewResult> savedPlans)
        {
            if (!savedPlans.Any())
            {
                return plans;
            }
            var returnPlans = new List<PlanResponse>();

            foreach (var plan in plans)
            {
                foreach (var savedPlan in savedPlans)
                {
                    if (plan.Code == savedPlan.Code)
                    {
                        AssignPlanVariables(plan, savedPlan);
                        plan.PremiumHoliday = savedPlan.PremiumHoliday;
                        plan.PremiumFrequency = policy.PremiumFrequency;
                        plan.IsRider = savedPlan.IsRider;
                    }

                    if (savedPlan.ParentPlanId != null && plan.Riders.Any())
                    {
                        plan.Riders = plan.Riders.ToList();
                        var riderToUpdate = plan.Riders.First(x => x.Code == savedPlan.Code);

                        AssignPlanVariables(riderToUpdate, savedPlan);

                        SetPlanCoversAndOptions(riderToUpdate);
                    }
                }

                SetPlanCoversAndOptions(plan);

                plan.IsFilledIn = plan.Covers.Any(c => c.Selected)
                    && plan.CoverAmount > 0
                    && plan.Options.All(o => o.Selected != null)
                    && plan.PremiumHoliday != null
                    && plan.LinkedToCpi != null;

                returnPlans.Add(plan);
            }

            foreach (var p in returnPlans)
            {
                p.PremiumIncludingRiders = p.Premium + p.Riders.Where(r => r.Selected).Sum(r => r.Premium);
            }

            return returnPlans;
        }

        private void AssignPlanVariables(PlanResponse plan, PlanOverviewResult savedPlan)
        {
            plan.PlanId = savedPlan.PlanId;
            plan.CoverAmount = savedPlan.CoverAmount;
            plan.Selected = savedPlan.Selected;
            plan.Premium = savedPlan.Premium;
            plan.PremiumType = savedPlan.PremiumType;

            plan.LinkedToCpi = savedPlan.LinkedToCpi;
            plan.WaitingPeriod = savedPlan.WaitingPeriod;
            plan.BenefitPeriod = savedPlan.BenefitPeriod;
            plan.OccupationDefinition = savedPlan.OccupationDefinition;
            plan.Variables = _variableResponseConverter.From(plan, savedPlan);
        }

        private void SetPlanCoversAndOptions(PlanResponse plan)
        {
            var savedCovers = _coverService.GetCoversForPlan(plan.PlanId);
            var returnCovers = new List<CoverResponse>();
            foreach (var cover in plan.Covers)
            {
                foreach (var savedCover in savedCovers)
                {
                    if (cover.Code.Equals(savedCover.Code, StringComparison.OrdinalIgnoreCase))
                    {
                        cover.Selected = savedCover.Selected;
                        cover.CoverUnderwritingStatus =
                            GetCoverUnderwritingStatusFromCoverUnderwritingStatus(savedCover.UnderwritingStatus);
                        cover.Premium = savedCover.Premium;

                        var coverExclusions = _coverExclusionsService.GetExclusionsForCover(savedCover);
                        cover.Exclusions = coverExclusions.Select(e => new ExclusionResponse(e.Name, e.Text));
                    }
                }
                returnCovers.Add(cover);
            }
            plan.Covers = returnCovers;

            var savedOptions = _optionService.GetOptionsForPlan(plan.PlanId);
            var returnOptions = new List<OptionResponse>();
            foreach (var option in plan.Options)
            {
                foreach (var savedOption in savedOptions)
                {
                    if (option.Code.Equals(savedOption.Code, StringComparison.OrdinalIgnoreCase))
                    {
                        option.Selected = savedOption.Selected;
                    }
                }
                returnOptions.Add(option);
            }
            plan.Options = returnOptions;
        }

        private CoverUnderwritingStatus GetCoverUnderwritingStatusFromCoverUnderwritingStatus(UnderwritingStatus underwritingStatus)
        {
            switch (underwritingStatus)
            {
                case UnderwritingStatus.Accept:
                    return CoverUnderwritingStatus.Accept;
                case UnderwritingStatus.Decline:
                    return CoverUnderwritingStatus.Decline;
                case UnderwritingStatus.Defer:
                    return CoverUnderwritingStatus.Defer;
                case UnderwritingStatus.Incomplete:
                    return CoverUnderwritingStatus.Incomplete;
                case UnderwritingStatus.MoreInfo:
                    return CoverUnderwritingStatus.MoreInfo;
                case UnderwritingStatus.Refer:
                    return CoverUnderwritingStatus.Refer;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
