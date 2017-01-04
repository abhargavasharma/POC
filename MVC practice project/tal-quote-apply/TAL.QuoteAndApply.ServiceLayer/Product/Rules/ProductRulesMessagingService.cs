using System.Collections.Generic;
using TAL.QuoteAndApply.Product.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Product.Rules
{
    public interface IProductErrorMessageService
    {
        string GetAustralianResidentErrorMessage();
        string GetMinimumAgeErrorMessage(int minAgeNextBirthday);
        string GetMaximumAgeErrorMessage(int maxAgeNextBirthday);
        string GetPremiumTypeErrorMessage(string premiumTypeName, int? maxAgeNextBirthday);
        string GetMaximumCoverAmountErrorMessage(int age, int coverAmount);
        string GetMinimumCoverAmountErrorMessage(int age, int coverAmount);
        string GetMinGreaterThanMaxCoverAmountErrorMessage(int minCoverAmount, int maxCoverAmount);
        string GetMissingIncomeForCoverAmountErrorMessage();
        string GetMinimumSportsCoverErrorMessage();
        string GetPremiumReliefWithoutIpErrorMessage();
		string GetAnnualIncomeErrorMessage(long? enteredAnnualIncome, int? minRequiredAnnualIncome);
        string GetInvalidPlanSelectedErrorMessage(string invalidPlan, IEnumerable<AvailableFeature> UnavailableFeatures);
        string GetInvalidCoverSelectedErrorMessage(string invalidCover, IEnumerable<AvailableFeature> UnavailableFeatures);
        string GetInvalidRiderCoverSelectedErrorMessage(string invalidCover, IEnumerable<AvailableFeature> UnavailableFeatures);
        string GetInvalidRiderSelectedErrorMessage(string invalidRider, IEnumerable<AvailableFeature> UnavailableFeatures);
        string GetInvalidPlanOptionsSelectedErrorMessage(string invalidOption, IEnumerable<AvailableFeature> UnavailableFeatures);
        string GetMaximumAgeForOptionErrorMessage(int age, string option);
        string GetSelectionRequiredErrorMessage(string field);
        string GetSumInsuredRequiredErrorMessage();
        string GetInvalidVariableValueErrorMessage(string field, string value);
        string GetPlanHasNoValidOptionsErrorMessage();
    }
}
