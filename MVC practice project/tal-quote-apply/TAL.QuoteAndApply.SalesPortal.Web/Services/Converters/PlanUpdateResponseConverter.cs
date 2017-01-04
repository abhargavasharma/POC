using System.Collections.Generic;
using System.Linq;
using System.Web.Http.ModelBinding;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.Web.Shared.Converters;
using TAL.QuoteAndApply.Web.Shared.Extensions;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services.Converters
{
    public interface IPlanUpdateResponseConverter
    {
        PlansUpdateResponse From(PlanUpdateRequest updatePlanRequest, EditPolicyResult editPolicyResult, IEnumerable<PlanPremiumResult> planPremiumResult, PolicyRiskPlanStatusesResult policyRiskPlanStatusesResult, RiskPremiumSummaryViewModel riskPremiumSummary);
    }

    public class PlanUpdateResponseConverter : IPlanUpdateResponseConverter
    {
        private readonly IErrorsAndWarningsConverter _errorsAndWarningsConverter;

        public PlanUpdateResponseConverter(IErrorsAndWarningsConverter errorsAndWarningsConverter)
        {
            _errorsAndWarningsConverter = errorsAndWarningsConverter;
        }

        public PlansUpdateResponse From(PlanUpdateRequest updatePlanRequest, EditPolicyResult editPolicyResult, IEnumerable<PlanPremiumResult> planPremiumResult, PolicyRiskPlanStatusesResult policyRiskPlanStatusesResult, RiskPremiumSummaryViewModel riskPremiumSummary)
        {
            var comprehensive = From(updatePlanRequest.CurrentActivePlan, editPolicyResult, planPremiumResult);
            AddStatusErrorsAndWarnings(comprehensive, policyRiskPlanStatusesResult);

            var plans = new List<PlanUpdateResponse>();

            foreach (var plan in planPremiumResult)
            {
                var planUpdateResponse = new PlanUpdateResponse()
                {
                    Code = plan.PlanCode,
                    Selected = updatePlanRequest.SelectedPlanCodes.Contains(plan.PlanCode),
                    PlanId = plan.PlanId,
                    Premium = plan.Premium,
                    PremiumIncludingRiders = plan.PremiumIncludingRiders
                };

                AddStatusErrorsAndWarnings(planUpdateResponse, policyRiskPlanStatusesResult);

                plans.Add(planUpdateResponse);
            }

            return new PlansUpdateResponse
            {
                CurrentActivePlan = comprehensive,
                Plans = plans,
                OverallStatus = policyRiskPlanStatusesResult.OverallPlanStatus,
                RiskPremiumSummary = riskPremiumSummary
            };
        }

        private void AddStatusErrorsAndWarnings(PlanUpdateResponse planUpdateResponse, PolicyRiskPlanStatusesResult policyRiskPlanStatusesResult)
        {
            var errorModelState = new ModelStateDictionary();
            var warningModelState = new ModelStateDictionary();
            var status = PlanStatus.Completed;

            var planStatus =
                policyRiskPlanStatusesResult.PlanValidationStatuses.FirstOrDefault(
                    p => p.PlanCode == planUpdateResponse.Code);

            if (planStatus != null)
            {
                _errorsAndWarningsConverter.MapPlanValidationsToModelState(errorModelState, planStatus.Errors, planStatus.PlanCode);
                _errorsAndWarningsConverter.MapPlanValidationsToModelState(warningModelState, planStatus.Warnings, planStatus.PlanCode);
                status = planStatus.PlanStatus;
            }

            planUpdateResponse.Status = status;
            planUpdateResponse.Errors = AppendErrorsToErrorDictionary(planUpdateResponse.Errors, errorModelState.ToErrorDictionary());
            planUpdateResponse.Warnings = AppendErrorsToErrorDictionary(planUpdateResponse.Warnings, warningModelState.ToErrorDictionary());
        }

        private Dictionary<string, IEnumerable<string>> AppendErrorsToErrorDictionary(Dictionary<string, IEnumerable<string>> currentErrors, Dictionary<string, IEnumerable<string>> newErrors)
        {
            if (currentErrors == null)
                return newErrors;

            //loop all new errors
            foreach (var newError in newErrors)
            {
                if (currentErrors.ContainsKey(newError.Key))
                {
                    //error key already exists

                    //get the current list of messages for this error key
                    var messages = currentErrors[newError.Key].ToList();

                    //loop the new messages
                    foreach (var newMessage in newError.Value)
                    {
                        if (!currentErrors[newError.Key].Contains(newMessage))
                        {
                            //the current key doesn't have the new message, so add it
                            messages.Add(newMessage);
                        }
                    }

                    //re-asign the messages with all the current and new messages
                    currentErrors[newError.Key] = messages;
                }
                else
                {
                    //key doesnt exist so add it in
                    currentErrors.Add(newError.Key, newError.Value);
                }
            }

            return currentErrors;
        }

        private ComprehensivePlanUpdateResponse From(PlanConfigurationRequest plan, EditPolicyResult editPolicyResult, IEnumerable<PlanPremiumResult> planPremiumResults)
        {
            var planPremium = planPremiumResults.FirstOrDefault(pr => pr.PlanCode == plan.PlanCode);

            var warnings = new ModelStateDictionary();
            _errorsAndWarningsConverter.MapPlanValidationsToModelState(warnings,
                editPolicyResult.Warnings, plan.PlanCode);

            var retVal = new ComprehensivePlanUpdateResponse()
            {
                Code = plan.PlanCode,
                PlanId = plan.PlanId,
                Selected = plan.Selected,
                CoverAmount = plan.CoverAmount,
                LinkedToCpi = plan.LinkedToCpi,
                Covers = plan.SelectedCoverCodes.Select(selectedCoverCode => new UpdateCoverResponse
                {
                    Code = selectedCoverCode,
                    Selected = true,
                    Premium = planPremium.CoverPremiumResults.First(cpr=> cpr.CoverCode == selectedCoverCode).Premium
                }),
                Warnings = warnings.ToErrorDictionary(),
                Premium = planPremium.Premium,
                PremiumIncludingRiders = planPremium.PremiumIncludingRiders,
                PremiumFrequency = planPremium.PremiumFrequency.ToString(),
                PremiumHoliday = plan.PremiumHoliday,
                Riders = plan.SelectedRiders.Select(x => From(x, editPolicyResult, planPremiumResults)).ToList(),
                WaitingPeriod = plan.WaitingPeriod,
                BenefitPeriod = plan.BenefitPeriod,
                OccupationDefinition = plan.OccupationDefinition
            };
            return retVal;
        }
    }
}