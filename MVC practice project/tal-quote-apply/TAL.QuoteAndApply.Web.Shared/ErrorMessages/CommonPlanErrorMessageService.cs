using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;

namespace TAL.QuoteAndApply.Web.Shared.ErrorMessages
{
    public class CommonPlanErrorMessageService : IPlanErrorMessageService
    {
        public string GetSelectedCoverUndwritingDeclinedMessage()
        {
            return "Due to the answers you've provided, we're unable to offer you this cover. Please call us on 131 825 to find out more.";
        }

        public string GetCoverErrorMessageCode(string planCode, string coverCode, string coverName)
        {
            return string.Format("{0}{1}", planCode.ToLower(), coverName.Replace(" ", ""));
        }

        public string GetPremiumTypeNotAvailableMessage(PremiumType premiumType, int maxAge)
        {
            return $"{premiumType} cannot be selected when the insured is older than {maxAge} at their next birthday";
        }

        public string GetPlanNeedsCoverSelectedErrorMessage(string planName)
        {
            return $"{planName} cannot be selected without any selected covers";
        }
    }
}