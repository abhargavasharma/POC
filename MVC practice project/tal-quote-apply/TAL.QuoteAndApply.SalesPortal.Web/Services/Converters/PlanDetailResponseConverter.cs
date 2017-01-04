using System;
using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Product;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services.Converters
{
    public interface IPlanDetailResponseConverter
    {
        PlanDetailResponse CreateFrom(PlanResponse planResponse, PlanEligibilityResult planEligibilityResult);
    }

    public class PlanDetailResponseConverter : IPlanDetailResponseConverter
    {
        public PlanDetailResponse CreateFrom(PlanResponse planResponse, PlanEligibilityResult planEligibilityResult)
        {
            var returnObj = ConvertPlan(planResponse, planEligibilityResult);
            returnObj.Riders = ConvertRiders(planResponse.Riders, planEligibilityResult.RiderEligibilityResults);
            return returnObj;
        }

        private PlanDetailResponse ConvertPlan(PlanResponse planResponse, PlanEligibilityResult planEligibilityResult)
        {
            var returnObj = new PlanDetailResponse
            {
                PlanId = planResponse.PlanId,
                Code = planResponse.Code,
                Name = planResponse.Name,
                ShortName = planResponse.ShortName,
                PlanType = planResponse.PlanType,
                Selected = planResponse.Selected,
                CoverAmount = planResponse.CoverAmount,
                LinkedToCpi = planResponse.LinkedToCpi,
                Covers = ConvertCovers(planResponse.Covers, planEligibilityResult.CoverEligibilityResults),
                Options = ConvertOptions(planResponse.Options),
                Variables = ConvertVariables(planResponse.Variables),
                IsFilledIn = planResponse.IsFilledIn,
                Premium = planResponse.Premium,
                PremiumIncludingRiders = planResponse.PremiumIncludingRiders,
                PremiumFrequency = planResponse.PremiumFrequency.ToString(),
                PremiumHoliday = planResponse.PremiumHoliday,
				PremiumType = planResponse.PremiumType.ToString(),
                WaitingPeriod = planResponse.WaitingPeriod,
                BenefitPeriod = planResponse.BenefitPeriod,
                EligibleForPlan = planEligibilityResult.EligibleForPlan,
                IneligibleReasons = planEligibilityResult.IneligibleReasons,
                OccupationDefinition = planResponse.OccupationDefinition
            };

            return returnObj;
        }

        private IEnumerable<PlanDetailVariableResponse> ConvertVariables(IEnumerable<VariableResponse> variables)
        {
            if (variables == null)
            {
                return new List<PlanDetailVariableResponse>();
            }

            return variables.Select(variable => new PlanDetailVariableResponse
            {
                Name = variable.Name,
                Code = variable.Code
            });
        }

        private IEnumerable<PlanDetailResponse> ConvertRiders(IEnumerable<PlanResponse> riders, IEnumerable<PlanEligibilityResult> riderEligibilityResults)
        {
            var results = new List<PlanDetailResponse>();

            foreach (var rider in riders)
            {
                var eligbility = riderEligibilityResults.First(x => x.PlanCode == rider.Code);

                results.Add(ConvertPlan(rider, eligbility));
            }

            return results;
        }

        private IEnumerable<PlanDetailOptionResponse> ConvertOptions(IEnumerable<OptionResponse> options)
        {
            return options.Select(optionResponse => new PlanDetailOptionResponse
            {
                Name = optionResponse.Name, Selected = optionResponse.Selected, Code = optionResponse.Code
            });
        }

        private IEnumerable<PlanDetailCoverResponse> ConvertCovers(IEnumerable<CoverResponse> covers, IEnumerable<CoverEligibilityResult> coverEligibilityResults)
        {
            foreach (var coverResponse in covers)
            {
                var eligibility = coverEligibilityResults.Single(e => e.CoverCode == coverResponse.Code);
                yield return new PlanDetailCoverResponse
                {
                    Code = coverResponse.Code,
                    Name = coverResponse.Name,
                    Selected = coverResponse.Selected,
                    UnderwritingIndicator = GetUnderwritingIndicator(coverResponse.CoverUnderwritingStatus),
                    Premium = coverResponse.Premium,
                    Eligible = eligibility.EligibleForCover,
                    IneligibleReasons = eligibility.IneligibleReasons
                };

            }
        }

        private PlanDetailCoverUnderwritingIndicator GetUnderwritingIndicator(CoverUnderwritingStatus coverUnderwritingStatus)
        {
            switch (coverUnderwritingStatus)
            {
                case CoverUnderwritingStatus.Accept:
                    return PlanDetailCoverUnderwritingIndicator.Accepted;
                case CoverUnderwritingStatus.Decline:
                    return PlanDetailCoverUnderwritingIndicator.Declined;
                default:
                    return PlanDetailCoverUnderwritingIndicator.NotCompleted;
            }
        }
    }
}