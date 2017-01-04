using TAL.QuoteAndApply.DataModel.Product;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Plan
{
    public interface IPlanErrorMessageService
    {
        string GetSelectedCoverUndwritingDeclinedMessage();
        string GetCoverErrorMessageCode(string planCode, string coverCode, string coverName);
        string GetPremiumTypeNotAvailableMessage(PremiumType premiumType, int maxAge);
        string GetPlanNeedsCoverSelectedErrorMessage(string planCode);
    }
}