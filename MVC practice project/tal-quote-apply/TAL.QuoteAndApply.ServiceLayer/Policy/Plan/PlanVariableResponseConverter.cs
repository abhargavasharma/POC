using System;
using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Product.Rules;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Plan
{
    public interface IPlanVariableResponseConverter
    {
        IList<VariableResponse> From(PlanResponse plan, PlanOverviewResult savedPlan);
        VariableResponse WithSelectedValue(VariableResponse variableReponse, PlanOverviewResult savedPlan);
        bool? CpiFrom(List<VariableResponse> variableResponses);
        int? WaitingPeriodFrom(List<VariableResponse> variableResponses);
        int? BenefitPeriodFrom(List<VariableResponse> variableResponses);
        string OccupationDefinitionFrom(List<VariableResponse> variableResponses);
        object SelectedValueFrom(AvailableFeature variableAvailableFeaturem, PlanStateParam planStateParam);
    }

    public class PlanVariableResponseConverter : IPlanVariableResponseConverter
    {
        public IList<VariableResponse> From(PlanResponse plan, PlanOverviewResult savedPlan)
        {
            if (plan.Variables == null)
            {
                return new List<VariableResponse>();
            }
            
            return plan.Variables.Select(v => WithSelectedValue(v, savedPlan)).ToList();
        }

        public bool? CpiFrom(List<VariableResponse> variableResponses)
        {
            return GetVariableValue<bool>(variableResponses, ProductPlanVariableConstants.LinkedToCpi);
        }

        public int? WaitingPeriodFrom(List<VariableResponse> variableResponses)
        {
            //Need to cast to long then cast to int just to get around generic cast conversion of object
            return (int?)GetVariableValue<long>(variableResponses, ProductPlanVariableConstants.WaitingPeriod);
        }

        public int? BenefitPeriodFrom(List<VariableResponse> variableResponses)
        {
            //Need to cast to long then cast to int just to get around generic cast conversion of object
            return (int?)GetVariableValue<long>(variableResponses, ProductPlanVariableConstants.BenefitPeriod);
        }

        public string OccupationDefinitionFrom(List<VariableResponse> variableResponses)
        {
            var variable = variableResponses.SingleOrDefault(v => v.Code.Equals(ProductPlanVariableConstants.OccupationDefinition,
                            StringComparison.OrdinalIgnoreCase))?.SelectedValue;
            return variable?.ToString(); 
        }

        public object SelectedValueFrom(AvailableFeature variableAvailableFeaturem, PlanStateParam planStateParam)
        {
            object selectedValue;
            switch (variableAvailableFeaturem.Code)
            {
                //TODO: once again, I've just used a switch statement, is a bit lame but gets the job done, could do something a bit more elegant
                case ProductPlanVariableConstants.LinkedToCpi:
                    selectedValue = planStateParam.LinkedToCpi;
                    break;
                case ProductPlanVariableConstants.PremiumHoliday:
                    selectedValue = planStateParam.PremiumHoliday;
                    break;
                case ProductPlanVariableConstants.PremiumType:
                    selectedValue = planStateParam.PremiumType;
                    break;
                case ProductPlanVariableConstants.WaitingPeriod:
                    selectedValue = planStateParam.WaitingPeriod;
                    break;
                case ProductPlanVariableConstants.BenefitPeriod:
                    selectedValue = planStateParam.BenefitPeriod;
                    break;
                case ProductPlanVariableConstants.OccupationDefinition:
                    selectedValue = planStateParam.OccupationDefinition;
                    break;
                default:
                    throw new ApplicationException($"No matching Plan attribute for Available Feature code '{variableAvailableFeaturem.Code}'");
            }

            return selectedValue;
        }

        public VariableResponse WithSelectedValue(VariableResponse variableReponse, PlanOverviewResult savedPlan)
        {
            object selectedValue;
            switch (variableReponse.Code)
            {
                //TODO: switch statement is a bit lame but gets the job done, could do something a bit more elegant
                case ProductPlanVariableConstants.LinkedToCpi:
                    selectedValue = savedPlan.LinkedToCpi;
                    break;
                case ProductPlanVariableConstants.PremiumHoliday:
                    selectedValue = savedPlan.PremiumHoliday;
                    break;
                case ProductPlanVariableConstants.PremiumType:
                    selectedValue = savedPlan.PremiumType;
                    break;
                case ProductPlanVariableConstants.WaitingPeriod:
                    selectedValue = savedPlan.WaitingPeriod;
                    break;
                case ProductPlanVariableConstants.BenefitPeriod:
                    selectedValue = savedPlan.BenefitPeriod;
                    break;
                case ProductPlanVariableConstants.OccupationDefinition:
                    selectedValue = savedPlan.OccupationDefinition;
                    break;
                default:
                    throw new ApplicationException($"No matching Plan attribute for variable code '{variableReponse.Code}'");
            }

            return new VariableResponse
            {
                Code = variableReponse.Code,
                Name = variableReponse.Name,
                SelectedValue = selectedValue,
                Options = variableReponse.Options?.Select(v => new VariableOptionResponse(v.Name, v.Value)).ToList(),
                AllowEditingBy = variableReponse.AllowEditingBy
            };
        }

        private static T? GetVariableValue<T>(IEnumerable<VariableResponse> variableResponses, string code) where T:struct 
        {            
            var variable =
                variableResponses.SingleOrDefault(
                    v => v.Code.Equals(code, StringComparison.OrdinalIgnoreCase));            
            
            return (T?) variable?.SelectedValue;
        }
    }
}
