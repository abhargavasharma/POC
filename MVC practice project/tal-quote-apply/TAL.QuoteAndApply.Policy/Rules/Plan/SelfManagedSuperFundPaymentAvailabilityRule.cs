using TAL.QuoteAndApply.DataModel.Payment;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Policy.Rules.Plan
{
    public class SelfManagedSuperFundPaymentAvailabilityRule : IPaymentOptionsAvailabilityRule
    {
        public RuleResult IsSatisfiedBy(IPlan target)
        {
            return new RuleResult(true, string.Format("Self Managed Super fund payment is not available for the plan {0}", target.Code)); // always available unless otherwise changed in the future
        }

        public PaymentType PaymentType
        {
            get { return PaymentType.SelfManagedSuperFund; }
        }
    }
}