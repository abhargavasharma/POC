using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataModel.Payment;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Rules.Plan;

namespace TAL.QuoteAndApply.Policy.Service
{
    public interface IPaymentOptionsAvailabilityProvider
    {
        IEnumerable<PaymentType> GetAvailablePaymentTypes(IEnumerable<IPlan> policyPlans);
    }

    public class PaymentOptionsAvailabilityProvider : IPaymentOptionsAvailabilityProvider
    {
        private readonly IEnumerable<IPaymentOptionsAvailabilityRule> _paymentOptionsAvailabilityRules;

        public PaymentOptionsAvailabilityProvider(IEnumerable<IPaymentOptionsAvailabilityRule> paymentOptionsAvailabilityRules)
        {
            _paymentOptionsAvailabilityRules = paymentOptionsAvailabilityRules;
        }

        public IEnumerable<PaymentType> GetAvailablePaymentTypes(IEnumerable<IPlan> policyPlans)
        {
            var selectedPlans = policyPlans.Where(p => p.Selected).ToArray();
            return 
                from paymentOptionsAvailabilityRule 
                in _paymentOptionsAvailabilityRules
                where selectedPlans.All(pp => paymentOptionsAvailabilityRule.IsSatisfiedBy(pp))
                select paymentOptionsAvailabilityRule.PaymentType;
        }
    }
}
