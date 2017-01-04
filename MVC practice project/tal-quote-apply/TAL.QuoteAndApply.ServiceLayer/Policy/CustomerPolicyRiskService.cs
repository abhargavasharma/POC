namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public interface ICustomerPolicyRiskService
    {
        int? GetPrimaryRiskId(string quoteReference);
    }

    public class CustomerPolicyRiskService : ICustomerPolicyRiskService
    {
        private readonly IPolicyWithRisksService _policyWithRisksService;

        public CustomerPolicyRiskService(IPolicyWithRisksService policyWithRisksService)
        {
            _policyWithRisksService = policyWithRisksService;
        }

        public int? GetPrimaryRiskId(string quoteReference)
        {
            /*
                Given that Customer Journey is only single risk for now, and our model supports multi-risk.
                I'm putting this code as a common place to call that just gets the one risk we're interested in.
                When multi-risk is gonna be a thing...look for references to here as to what should be refactored.
            */
            var policyOwnerRiskId = _policyWithRisksService.GetPolicyOwnderRiskFrom(quoteReference);
            return policyOwnerRiskId != null ? policyOwnerRiskId.Id : (int?)null;

        }
    }
}