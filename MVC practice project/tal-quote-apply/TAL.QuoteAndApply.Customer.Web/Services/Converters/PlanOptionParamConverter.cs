using System;
using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Customer.Web.Models.Api;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.DataModel.Service;
using TAL.QuoteAndApply.ServiceLayer.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Product.Rules;

namespace TAL.QuoteAndApply.Customer.Web.Services.Converters
{
    public interface IPlanOptionParamConverter
    {
        GetPlanResponse From(IEnumerable<PlanResponse> planResponse, AvailablePlanOptionsAndConfigResult availability,
            PremiumTypeOptions premiumTypeOptions, IEnumerable<PlanEligibilityResult> planEligibilityResults,
            RiskOverviewResult risk);
        AvailabilityPlanStateParam From(int riskId, string brandKey, IEnumerable<PlanResponse> planResponse);
    }

    public class PlanOptionParamConverter : IPlanOptionParamConverter
    {
        public GetPlanResponse From(IEnumerable<PlanResponse> planResponse, AvailablePlanOptionsAndConfigResult availability, 
            PremiumTypeOptions premiumTypeOptions, IEnumerable<PlanEligibilityResult> planEligibilityResults, 
            RiskOverviewResult risk)
        {
            var selectedPremiumTypeOption =
                premiumTypeOptions.AvailablePremiumTypes.Single(
                    pt => pt.PremiumType == premiumTypeOptions.SelectedPremiumType);

            return new GetPlanResponse()
            {
                Plans = planResponse.Select(x => From(x, planResponse, availability, GetEligibilityFor(planEligibilityResults, x.Code))).ToList(),
                TotalPremium = selectedPremiumTypeOption.Total,
                PaymentFrequency = planResponse.First().PremiumFrequency.ToFriendlyString(), //Assumption that all plans have same PremiumFrequency
                PremiumTypeOptions = premiumTypeOptions,
                IsOccupationTpdAny = risk.AvailableDefinitions.Contains(OccupationDefinition.AnyOccupation),
                IsOccupationTpdOwn = risk.AvailableDefinitions.Contains(OccupationDefinition.OwnOccupation)
            };
        }

        private PlanEligibilityResult GetEligibilityFor(IEnumerable<PlanEligibilityResult> planEligibilityResults, string code)
        {
            return planEligibilityResults.First(p => p.PlanCode == code);
        }

        public AvailabilityPlanStateParam From(int riskId, string brandKey, IEnumerable<PlanResponse> planResponse)
        {
            var firstPlan = planResponse.First();

            return new AvailabilityPlanStateParam
            {
                PlanCode = firstPlan.Code,
                BrandKey = brandKey,
                RiskId = riskId,
                SelectedCoverCodes = planResponse.SelectMany(p => p.Covers).Where(c => c.Selected).Select(c => c.Code),
                SelectedPlanCodes = planResponse.Where(p => p.Selected).Select(p => p.Code),
                SelectedRiderOptionCodes =
                    planResponse.SelectMany(p => p.Riders)
                        .SelectMany(r => r.Options)
                        .Where(o => o.Selected.HasValue && o.Selected.Value)
                        .Select(p => p.Code),
                SelectedRiderCoverCodes = planResponse.SelectMany(p => p.Riders)
                    .SelectMany(r => r.Covers)
                    .Where(o => o.Selected)
                    .Select(p => p.Code),
                SelectedRiderCodes = planResponse.SelectMany(p => p.Riders)
                    .Where(o => o.Selected)
                    .Select(r => r.Code),
                SelectedPlanOptionCodes =
                    planResponse.SelectMany(p => p.Options)
                        .Where(o => o.Selected.HasValue && o.Selected.Value)
                        .Select(p => p.Code)
            };
        }

        public PlanSelectionResponse From(PlanResponse currentPlan, IEnumerable<PlanResponse> allPlans, AvailablePlanOptionsAndConfigResult availability, 
            PlanEligibilityResult planEligibilityResult)
        {
            return new PlanSelectionResponse
            {
                CoverTitle = currentPlan.Name,
                IsSelected = currentPlan.Selected,
                PlanId = currentPlan.PlanId,
                PlanCode = currentPlan.Code,
                Premium = currentPlan.Premium,
                TotalPremium = currentPlan.PremiumIncludingRiders,
                Covers =
                    currentPlan.Covers.Select(x => From(x, availability)).ToList(),
                Options =
                    currentPlan.Options.Select(o => From(o, availability))
                        .ToList(),
                SelectedCoverAmount = currentPlan.CoverAmount,
                Riders = currentPlan.Riders?.Select(x => From(x, currentPlan.Riders, availability,
                    GetEligibilityFor(planEligibilityResult.RiderEligibilityResults, x.Code))).ToList(),
                PremiumHoliday = currentPlan.PremiumHoliday.HasValue && currentPlan.PremiumHoliday.Value,
                PremiumType = currentPlan.PremiumType.ToString(),
                RiderFor = currentPlan.RelatedPlanCode,
                OccupationDefinition = currentPlan.OccupationDefinition.ToString(),
                AttachesTo =
                    allPlans.Where(p => p.Riders != null)
                        .FirstOrDefault(p => p.Riders.Any(r => r.RelatedPlanCode == currentPlan.Code))?.Code,
                IsEligibleForPlan = planEligibilityResult.EligibleForPlan,
                IsBundled =
                    allPlans.Where(p => p.Riders != null)
                        .Any(p => p.Riders.Any(r => r.Selected && r.RelatedPlanCode == currentPlan.Code)),
                //TODO: move variables to own method to make a bit tidier
                Variables =
                    currentPlan.Variables?.Where(v => v.CustomerCanEdit)
                        .Select(v => ConvertVariable(v, availability.VariableAvailability.ToList()))
                        .ToList(),
                Availability = From(currentPlan.Code, availability.UnavailableFeatures)
            };
        }

        public PlanOptionReponse From(OptionResponse option, AvailablePlanOptionsAndConfigResult availability)
        {
            return new PlanOptionReponse()
            {
                Code = option.Code,
                IsSelected = option.Selected.HasValue && option.Selected.Value,
                IsAvailable = availability.AvailableOptions.Any(a => a.Equals(option.Code, StringComparison.OrdinalIgnoreCase))
            };
        }

        public PlanCoversReponse From(CoverResponse cover, AvailablePlanOptionsAndConfigResult availability)
        {
            return new PlanCoversReponse
            {
                Code = cover.Code,
                CoverFor = cover.CoverFor,
                IsSelected = cover.Selected,
                CoverAmount = cover.CoverAmount,
                Premium = cover.Premium,
                Availability = From(cover.Code, availability.UnavailableFeatures),
                ExclusionNames = cover.Exclusions.Select(e => e.Name)
            };
        }

        private static PlanVariableResponse ConvertVariable(VariableResponse variableResponse, IList<AvailableFeature> variableAvailabilities)
        {
            var variableAvailability = variableAvailabilities.SingleOrDefault(va => va.Code == variableResponse.Code);

            var options = new List<PlanVariableOptionResponse>();

            if (variableAvailability != null)
            {
                foreach (var variableOption in variableResponse.Options)
                {
                    var variableOptionAvailability =
                        variableAvailability.ChildAvailableFeatures.Single(
                            va => va.Code == variableOption.Value.ToString());
                    options.Add(new PlanVariableOptionResponse
                    {
                        Name = variableOption.Name,
                        Value = variableOption.Value,
                        IsAvailable = variableOptionAvailability.IsAvailable
                    });
                }
            }

            return new PlanVariableResponse
            {
                Code = variableResponse.Code,
                Name = variableResponse.Name,
                SelectedValue = variableResponse.SelectedValue,                
                Options = options
            };
        }

        private AvailabilityResponse From(string code, IEnumerable<AvailableFeature> unAvailableFeatures)
        {
            var unavailableFeature = unAvailableFeatures.SingleOrDefault(f => f.Code == code);
            return new AvailabilityResponse
            {
                IsAvailable = unavailableFeature?.IsAvailable ?? true,
                UnavailableReasons = unavailableFeature?.ReasonIfUnavailable.ToList() ?? new List<string>()
            };
        }
    }
}