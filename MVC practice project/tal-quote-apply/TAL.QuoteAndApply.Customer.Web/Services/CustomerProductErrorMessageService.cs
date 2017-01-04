using TAL.QuoteAndApply.Web.Shared.ErrorMessages;

namespace TAL.QuoteAndApply.Customer.Web.Services
{
    public class CustomerProductErrorMessageService : CommonProductErrorMessagesService
    {
        //Can override any error messages for the customer portal here
        //Check the base class CommonProductErrorMessagesService for where the default messages are and override appropriate message here

        public override string GetMinimumAgeErrorMessage(int minAgeNextBirthday)
        {
            var currentMinAge = minAgeNextBirthday - 1;
            return string.Format("Minimum age is {0}", currentMinAge);
        }

        public override string GetMaximumAgeErrorMessage(int maxAgeNextBirthday)
        {
            var currentMaxAge = maxAgeNextBirthday - 1;
            return string.Format("Maximum age is {0}", currentMaxAge);
        }

        public override string GetSumInsuredRequiredErrorMessage()
        {
            return "An amount above $0 is required";
        }
    }
}