using System.Collections.Generic;
using System.Text;
using System.Web.Http.ModelBinding;
using TAL.QuoteAndApply.ServiceLayer.Validation;

namespace TAL.QuoteAndApply.Web.Shared.Converters
{
    public interface IErrorsAndWarningsConverter
    {
        void MapPlanValidationsToModelState(ModelStateDictionary modelStateDictionary, IEnumerable<ValidationError> errors,
            string planCode);

        void MapGenericValidationToModelState(ModelStateDictionary modelStateDictionary,
            IEnumerable<ValidationError> validations);
    }

    public class ErrorsAndWarningsConverter : IErrorsAndWarningsConverter
    {
        public void MapPlanValidationsToModelState(ModelStateDictionary modelStateDictionary, IEnumerable<ValidationError> errors,
            string planCode)
        {
            var keyFormat = "{0}";
            const string coverAmount = "CoverAmount";
            var coverAmountModelStateKey = new StringBuilder(planCode.ToLower()).Append(coverAmount);

            foreach (var brokenRule in errors)
            {
                switch (brokenRule.Key)
                {
                    case ValidationKey.AnnualIncomeMissingForCoverAmount:
                        modelStateDictionary.AddModelError(string.Format(keyFormat, coverAmountModelStateKey), brokenRule.Message);
                        break;
                    case ValidationKey.MaxGreaterThanMinCoverAmount:
                    case ValidationKey.MinimumCoverAmount:
                    case ValidationKey.MaximumCoverAmount:
                        modelStateDictionary.AddModelError($"{brokenRule.Location.ToLower()}{coverAmount}", brokenRule.Message);
                        break;
                    case ValidationKey.MinimumSportsCover:
                        modelStateDictionary.AddModelError($"{planCode.ToLower()}AdventureSportsCover", brokenRule.Message);
                        break;
                    case ValidationKey.SelectedCoverUnderwritingDeclined:
                        modelStateDictionary.AddModelError(brokenRule.Code, brokenRule.Message);
                        break;
                    case ValidationKey.EligiblePremiumType:
                        modelStateDictionary.AddModelError($"{planCode.ToLower()}PremiumType", brokenRule.Message);
                        break;
                    case ValidationKey.InvalidOptionSelection:
                    case ValidationKey.RequiredPlanVariable:
                    case ValidationKey.RequiredPlanOption:
                    case ValidationKey.RequiredPlanCoverAmount:
                        modelStateDictionary.AddModelError($"{planCode.ToLower()}{brokenRule.Code}", brokenRule.Message);
                        break;
                    case ValidationKey.InvalidPlanSelection:
                    case ValidationKey.InvalidRiderSelection:
                    case ValidationKey.InvalidCoverSelection:
                    case ValidationKey.InvalidRiderCoverSelection:
                        modelStateDictionary.AddModelError(brokenRule.Location.ToLower(), brokenRule.Message);
                        break;
                    case ValidationKey.PlanVariableInvalid:
                        modelStateDictionary.AddModelError($"{planCode.ToLower()}{brokenRule.Code}", brokenRule.Message);
                        break;
                }
            }
        }

        public void MapGenericValidationToModelState(ModelStateDictionary modelStateDictionary,
            IEnumerable<ValidationError> validations)
        {
            if (validations != null)
            {
                foreach (var brokenRule in validations)
                {
                    modelStateDictionary.AddModelError(brokenRule.Location.ToLower(), brokenRule.Message);
                }
            }
            
        }
    }
}