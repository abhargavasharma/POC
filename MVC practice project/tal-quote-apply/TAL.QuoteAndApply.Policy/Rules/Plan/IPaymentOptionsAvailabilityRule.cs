using System.Security.Cryptography.X509Certificates;
using TAL.QuoteAndApply.DataModel.Payment;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Policy.Rules.Plan
{
    public interface IPaymentOptionsAvailabilityRule : IRule<IPlan>
    {
        PaymentType PaymentType { get; }
    }
}