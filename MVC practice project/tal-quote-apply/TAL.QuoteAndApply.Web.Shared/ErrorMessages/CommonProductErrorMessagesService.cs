using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Product.Rules;

namespace TAL.QuoteAndApply.Web.Shared.ErrorMessages
{
    public class CommonProductErrorMessagesService : IProductErrorMessageService
    {
        //TODO: Change Sales portal messages to come from here!


        public string GetAustralianResidentErrorMessage()
        {
            return "Only Australian citizens and permanent residents are eligible";
        }

        public virtual string GetMinimumAgeErrorMessage(int minAgeNextBirthday)
        {
            return string.Format("Must be at least {0} years old at next birthday to be eligible", minAgeNextBirthday);
        }

        public virtual string GetMaximumAgeErrorMessage(int maxAgeNextBirthday)
        {
            return string.Format("Must be younger than {0} years to be eligible", maxAgeNextBirthday);
        }

        public string GetPremiumTypeErrorMessage(string premiumTypeName, int? maxAgeNextBirthday)
        {
            return string.Format("Customer will not be eligible for {0} premiums", premiumTypeName);
        }

        public string GetAnnualIncomeErrorMessage(long? enteredAnnualIncome, int? minRequiredAnnualIncome)
        {
            return string.Format("As you don’t receive any income, we’re unable to offer you a Lifetime Protection quote online. "+ 
                                "However, you can still receive a quote by calling 131 825.");
        }

        //Todo update with monads
        public string GetInvalidCoverSelectedErrorMessage(string invalidCover, IEnumerable<AvailableFeature> unavailableFeatures)
        {
            var unavailableString = unavailableFeatures.Any() ? unavailableFeatures.First(x => x.Code == invalidCover).ReasonIfUnavailable.First() : null;
            return unavailableString;
        }

        public string GetInvalidRiderSelectedErrorMessage(string invalidRider,
            IEnumerable<AvailableFeature> unavailableFeatures)
        {
            var unavailableString = unavailableFeatures.Any() ? unavailableFeatures.First(x => x.Code == invalidRider).ReasonIfUnavailable.First() : null;
            return unavailableString;
        }

        public string GetInvalidRiderCoverSelectedErrorMessage(string invalidCover, IEnumerable<AvailableFeature> unavailableFeatures)
        {
            var unavailableString = unavailableFeatures.Any() ? unavailableFeatures.First(x => x.Code == invalidCover).ReasonIfUnavailable.First() : null;
            return unavailableString;

        }

        public string GetInvalidPlanOptionsSelectedErrorMessage(string invalidOption, IEnumerable<AvailableFeature> unavailableFeatures)
        {
            var unavailableString = unavailableFeatures.Any() ? unavailableFeatures.First(x => x.Code == invalidOption).ReasonIfUnavailable.First() : null;
            return unavailableString;

        }

        public string GetInvalidPlanSelectedErrorMessage(string invalidPlans, IEnumerable<AvailableFeature> unavailableFeatures)
        {
            var unavailableString = unavailableFeatures.Any() ? unavailableFeatures.First(x => x.Code == invalidPlans).ReasonIfUnavailable.First() : null;
            return unavailableString;
        }


        public string GetMaximumCoverAmountErrorMessage(int age, int maxCoverAmount)
        {
            return string.Format("Cover amount cannot exceed $" + string.Format("{0:n0}", maxCoverAmount));
        }

        public string GetMinimumCoverAmountErrorMessage(int age, int minCoverAmount)
        {
            return string.Format("Minimum cover is $" + string.Format("{0:n0}", minCoverAmount));
        }

        public string GetMinGreaterThanMaxCoverAmountErrorMessage(int minCoverAmount, int maxCoverAmount)
        {
            return $"Max sum insured ${maxCoverAmount} does not reach minimum for this product.";
        }

        public string GetMissingIncomeForCoverAmountErrorMessage()
        {
            return "Please provide your customer's annual income.";
        }

        public string GetMinimumSportsCoverErrorMessage()
        {
            return "Sports Cover cannot be selected without Accident Cover";
        }

        public string GetPremiumReliefWithoutIpErrorMessage()
        {
            return "Premium Relief on Life option can only be selected with an IP plan selected";
        }

        public string GetMaximumAgeForOptionErrorMessage(int age, string option)
        {
            return string.Format("A Customer of age {0} will not be eligible for the " + option + " option", age);
        }

        public string GetSelectionRequiredErrorMessage(string field)
        {
            return $"{field} selection required for submission of policy";
        }

        public virtual string GetSumInsuredRequiredErrorMessage()
        {
            return $"Sum Insured is required";
        }

        public string GetInvalidVariableValueErrorMessage(string field, string value)
        {
            return $"{value} is not a valid selection for {field}";
        }

        public string GetPlanHasNoValidOptionsErrorMessage()
        {
            return "There are no valid options for you to choose on this product";
        }
    }
}
