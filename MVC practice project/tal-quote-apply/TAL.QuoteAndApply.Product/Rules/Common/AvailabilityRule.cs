using System;
using System.Collections.Generic;
using System.Linq;
using System.Monads;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.Product.Models;
using TAL.QuoteAndApply.Product.Models.Definition;
using TAL.QuoteAndApply.Product.Service;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Product.Rules.Common
{
    public class AvailabilityRule : IRule<SelectedProductPlanOptions>
    {
        protected readonly IAvailability _availability;
        protected readonly INameLookupService _nameLookupService;

        public AvailabilityRule(IAvailability availability, INameLookupService nameLookupService)
        {
            _availability = availability;
            _nameLookupService = nameLookupService;
        }

        public RuleResult IsSatisfiedBy(SelectedProductPlanOptions target)
        {
            if (_availability.RuleDefinition == null)
            {
                // Make the assumption that if there is no rule present, then this feature is always on
                return RuleResult.ToResult(true);
            }

            var allSelectedRidersAndPlansArray = target.SelectedRiders.Concat(target.SelectedPlans).ToArray();
            
            var planRules = 
                _availability.RuleDefinition.With(
                    x => x.PlanConfigRules).With(
                        ccr =>
                            ccr.SelectMany(
                                coverConfigRule =>
                                    GeneratePlanRulesToAdd(coverConfigRule, target, allSelectedRidersAndPlansArray))
                                .ToArray());

            var allSelectedCoversArray = target.SelectedRiderCovers.Concat(target.SelectedCovers).ToArray();
            var coverRules =
                _availability.RuleDefinition.With(
                    x => x.CoverConfigRules).With(
                        ccr =>
                            ccr.SelectMany(
                                coverConfigRule =>
                                    GenerateCoverRulesToAdd(coverConfigRule, target, allSelectedCoversArray))
                                .ToArray());

            if ((planRules?.All(r => r.IsSatisfied) ?? true) && (coverRules?.All(r => r.IsSatisfied) ?? true))
            {
                return RuleResult.ToResult(true);
            }

            var messages = planRules?.Concat(coverRules ?? new RuleRequest[0]).Where(cr => !cr.IsSatisfied).Select(r => r.Message);
            messages = messages ?? coverRules?.Concat(new RuleRequest[0]).Where(cr => !cr.IsSatisfied).Select(r => r.Message);

            return RuleResult.ToResult(false, messages.ToArray());
        }

        private IEnumerable<RuleRequest> GenerateCoverRulesToAdd(ProductConfigRule coverConfigRule, SelectedProductPlanOptions target, string[] allSelectedCoversArray)
        {
            var returnObj = new []
            {
                new RuleRequest(coverConfigRule.If(cc => cc.Requirement == Should.BeAllSelected && cc.Scope == From.CurrentPlan)
                            .Return(cc => cc.Features.All(target.SelectedCovers.Contains), true),
                    $"Plan Cover '{_availability.Name}' requires the following list of covers to also be selected. '{coverConfigRule.With(p => p.Features).Return(r => _nameLookupService.GetCoverNames(coverConfigRule.Features, target.PlanCode, target.BrandKey), null)}'"
                    ),
                new RuleRequest(coverConfigRule.If(cc => cc.Requirement == Should.BeSomeSelected && cc.Scope == From.CurrentPlan)
                            .Return(cc => cc.Features.Any(target.SelectedCovers.Contains), true),
                    $"{_availability.Name} cannot be selected without {coverConfigRule.With(p => p.Features).Return(r => _nameLookupService.GetCoverNames(coverConfigRule.Features, target.PlanCode, target.BrandKey), null)}."
                    ),
                new RuleRequest(coverConfigRule.If(cc => cc.Requirement == Should.NotAnyBeSelected && cc.Scope == From.CurrentPlan)
                            .Return(cc => !cc.Features.Any(target.SelectedCovers.Contains), true),
                    $"{GetCoverName(_availability.Name)} must be attached to Life or taken standalone"
                    ),
                new RuleRequest(coverConfigRule.If(cc => cc.Requirement == Should.BeAllSelected && cc.Scope == From.Rider)
                        .Return(cc => cc.Features.All(allSelectedCoversArray.Contains), true),
                    $"{_availability.Name} cannot be selected without selecting {coverConfigRule.With(p => p.Features).Return(r => _nameLookupService.GetCoverNamesForRider(coverConfigRule.Features, target.PlanCode, target.BrandKey), null)}"
                    ),
                new RuleRequest(coverConfigRule.If(cc => cc.Requirement == Should.BeSomeSelected && cc.Scope == From.Rider)
                            .Return(cc => cc.Features.Any(allSelectedCoversArray.Contains), true),
                    $"{_availability.Name} cannot be selected without selecting {coverConfigRule.With(p => p.Features).Return(r => _nameLookupService.GetCoverNamesForRider(coverConfigRule.Features, target.PlanCode, target.BrandKey), null)}"
                    ),
                new RuleRequest(coverConfigRule.If(cc => cc.Requirement == Should.NotAnyBeSelected && cc.Scope == From.Rider)
                            .Return(cc => !cc.Features.Any(allSelectedCoversArray.Contains), true),
                    $"Rider Cover '{_availability.Name}' cannot be selected if one of the following covers is also selected. '{coverConfigRule.With(p => p.Features).Return(r => _nameLookupService.GetCoverNamesForRider(coverConfigRule.Features, target.PlanCode, target.BrandKey), null)}'"
                    )
            };
            return returnObj;
        }

        private IEnumerable<RuleRequest> GeneratePlanRulesToAdd(ProductConfigRule planConfigRule, SelectedProductPlanOptions target, string[] allSelectedRidersAndPlansArray)
        {

            var returnObj = new[]
            {
                new RuleRequest(
                    planConfigRule.If(
                        pc => pc.Requirement == Should.BeAllSelected && pc.Scope == From.Product && pc.Features != null)
                        .Return(pc => pc.Features.All(allSelectedRidersAndPlansArray.Contains), true),
                    $"{_availability.Name} requires {planConfigRule.With(p => p.Features).Return(r => _nameLookupService.GetPlanNames(planConfigRule.Features, target.PlanCode, target.BrandKey), null)} to be selected"
                    ),
                new RuleRequest(
                    planConfigRule.If(
                        pc => pc.Requirement == Should.BeSomeSelected && pc.Scope == From.Product && pc.Features != null)
                        .Return(pc => pc.Features.Any(allSelectedRidersAndPlansArray.Contains), true),
                    $"'{_availability.Name}' requires at least one of the following plans to also be selected. '{planConfigRule.With(p => p.Features).Return(r => _nameLookupService.GetPlanNames(planConfigRule.Features, target.PlanCode, target.BrandKey), null)}'"
                    ),
                new RuleRequest(
                    planConfigRule.If(
                        pc =>
                            pc.Requirement == Should.NotAnyBeSelected && pc.Scope == From.Product && pc.Features != null)
                        .Return(pc => !pc.Features.Any(allSelectedRidersAndPlansArray.Contains), true),
                    $"{GetCoverName(_availability.Name)} must be attached to Life or taken standalone"
                    ),
                //The below is to handle option selection rules other than other plan/rider/cover/option selection dependencies
                new RuleRequest(
                    planConfigRule.If(pc => pc.MaximumEntryAgeNextBirthday.HasValue)
                        .Return(pc => target.AgeNextBirthday <= pc.MaximumEntryAgeNextBirthday, true),
                    $"{_availability.Name} cannot be selected when the insured is older than {planConfigRule.With(y => y.MaximumEntryAgeNextBirthday)} at their next birthday"
                    ),
                //The below is to handle rider selection rules other than other plan/rider/cover/option selection dependencies
                new RuleRequest(
                    planConfigRule.If(pc => pc.MinimumEntryAgeNextBirthday.HasValue)
                        .Return(pc => target.AgeNextBirthday >= pc.MinimumEntryAgeNextBirthday, true),
                    $"'{_availability.Name}' cannot be selected when the insured is younger than {planConfigRule.With(y => y.MinimumEntryAgeNextBirthday)} next birthday"
                    ),
                new RuleRequest(
                    planConfigRule.If(pc => pc.SupportedWaitingPeriods != null && target.WaitingPeriod.HasValue)
                        .Return(pc => pc.SupportedWaitingPeriods.Contains(target.WaitingPeriod.Value), true),
                    $"{_availability.Name} cannot be selected when the Waiting Period is {target.WaitingPeriod} weeks"
                    ),
                new RuleRequest(
                    planConfigRule.If(
                        pc =>
                            pc.SupportedOccupationClasses != null && !string.IsNullOrWhiteSpace(target.OccupationClass))
                        .Return(pc => pc.SupportedOccupationClasses.Contains(target.OccupationClass), true),
                    $"{_availability.Name} cannot be selected for the selected occupation"
                    ),
                new RuleRequest(
                    planConfigRule.If(pc => pc.LinkedToCpiRequired)
                        .Return(pc => target.LinkedToCpi.GetValueOrDefault(false), true),
                    $"{_availability.Name} requires {_nameLookupService.GetVariableName(target.PlanCode, ProductPlanVariableConstants.LinkedToCpi, target.BrandKey)} to be selected"
                    ),
                new RuleRequest(
                    planConfigRule.If(
                        pc =>
                            pc.UnsupportedOccupationClasses != null && pc.UnsupportedOccupationClasses.Contains(target.OccupationClass))
                        .Return(pc => false, true),
                    $"{_availability.Name} cannot be selected for the current occupation")
            };

            return returnObj;
        }

        private string GetCoverName(string name)
        {
            string coverName = string.Empty;
            if (!string.IsNullOrEmpty(name))
            {
                if (name.IndexOf("disability", StringComparison.OrdinalIgnoreCase) != -1)
                    coverName = "TPD";
                else if (name.IndexOf("illness", StringComparison.OrdinalIgnoreCase) != -1)
                    coverName = "CI";
                else if (name.IndexOf("protection", StringComparison.OrdinalIgnoreCase) != -1)
                    coverName = "IP";
                else
                    coverName = name;
            }
            return coverName;
        }
    }
}