using System;
using System.Linq;
using TAL.QuoteAndApply.DataModel.Payment;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Policy.Rules.Plan
{
    public class SuperAnnuationPaymentAvailabilityRule : IPaymentOptionsAvailabilityRule
    {
        public RuleResult IsSatisfiedBy(IPlan target)
        {
            var unavailablePlans = new[] {"TPS", "TRS"};

            return new RuleResult(unavailablePlans.All(code => !target.Code.Equals(code, StringComparison.OrdinalIgnoreCase)),
                $"Super Annuation payment is not available for the plan {target.Code}");
        }

        public PaymentType PaymentType
        {
            get { return PaymentType.SuperAnnuation; }
        }
    }
}
