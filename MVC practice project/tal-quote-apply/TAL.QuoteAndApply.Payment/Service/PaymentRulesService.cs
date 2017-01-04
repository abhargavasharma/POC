using System.Collections.Generic;
using TAL.QuoteAndApply.Payment.Models;
using TAL.QuoteAndApply.Payment.Rules;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Payment.Service
{
    public interface IPaymentRulesService
    {
        IEnumerable<RuleResult> ValidateCreditCardForSave(ICreditCardPayment creditCardPayment);
        IEnumerable<RuleResult> ValidateCreditCardForInforce(ICreditCardPayment creditCardPayment);
        bool ValidateCreditCardNumberByLuhnAlgorithm(string cardNumber);
        IEnumerable<RuleResult> ValidateDirectDebitForSave(IDirectDebitPayment directDebitPayment);
        IEnumerable<RuleResult> ValidateDirectDebitForInforce(IDirectDebitPayment directDebitPayment);
        IEnumerable<RuleResult> ValidateSelfManagedSuperFundForSave(ISelfManagedSuperFundPayment selfManagedSuperFundPayment);
        IEnumerable<RuleResult> ValidateSelfManagedSuperFundForInforce(ISelfManagedSuperFundPayment selfManagedSuperFundPayment);

        IEnumerable<RuleResult> ValidateSuperAnnuationForSave(ISuperAnnuationPayment superAnnuationPayment);
        IEnumerable<RuleResult> ValidateSuperAnnuationForInforce(ISuperAnnuationPayment superAnnuationPayment);
    }

    public class PaymentRulesService : IPaymentRulesService
    {
        private readonly IPaymentOptionsRuleFactory _paymentOptionsRuleFactory;

        public PaymentRulesService(IPaymentOptionsRuleFactory paymentOptionsRuleFactory)
        {
            _paymentOptionsRuleFactory = paymentOptionsRuleFactory;
        }

        public IEnumerable<RuleResult> ValidateCreditCardForSave(ICreditCardPayment creditCardPayment)
        {
            var cardNumber = _paymentOptionsRuleFactory.GetStringIsNumbersOnlyRule("CreditCardPayment.CardNumber");
            var expiryMonth = _paymentOptionsRuleFactory.GetStringIsNumbersOnlyRule("CreditCardPayment.ExpiryMonth");
            var expiryYear = _paymentOptionsRuleFactory.GetStringIsNumbersOnlyRule("CreditCardPayment.ExpiryYear");

            var cardNumberLength = _paymentOptionsRuleFactory.GetStringIsBetweenMinAndMaxLengthRule("CreditCardPayment.ExpiryYear", 8, 16);
            var cardNumberLuhn = _paymentOptionsRuleFactory.GetLuhnAlgorithmValidationRule("CreditCardPayment.CardNumber");

            yield return cardNumber.IsSatisfiedBy(creditCardPayment.CardNumber);
            yield return expiryMonth.IsSatisfiedBy(creditCardPayment.ExpiryMonth);
            yield return expiryYear.IsSatisfiedBy(creditCardPayment.ExpiryYear);
            yield return cardNumberLength.IsSatisfiedBy(creditCardPayment.ExpiryYear);
            yield return cardNumberLuhn.IsSatisfiedBy(creditCardPayment.CardNumber);
        }

        public IEnumerable<RuleResult> ValidateCreditCardForInforce(ICreditCardPayment creditCardPayment)
        {
            var cardNumber = _paymentOptionsRuleFactory.GetStringIsNumbersOnlyRule("CreditCardPayment.CardNumber");
            var expiryMonth = _paymentOptionsRuleFactory.GetStringIsNumbersOnlyRule("CreditCardPayment.ExpiryMonth");
            var expiryYear = _paymentOptionsRuleFactory.GetStringIsNumbersOnlyRule("CreditCardPayment.ExpiryYear");

            var cardNumberLength = _paymentOptionsRuleFactory.GetStringIsBetweenMinAndMaxLengthRule("CreditCardPayment.CardNumber", 8, 16);
            var cardNumberLuhn = _paymentOptionsRuleFactory.GetLuhnAlgorithmValidationRule("CreditCardPayment.CardNumber");

            var nameOnCard = _paymentOptionsRuleFactory.GetStringIsBetweenMinAndMaxLengthRule("CreditCardPayment.NameOnCard", 1, 10000);

            //only validate before tokenisation
            if (creditCardPayment.Token == null)
            {
                yield return cardNumber.IsSatisfiedBy(creditCardPayment.CardNumber);
                yield return cardNumberLength.IsSatisfiedBy(creditCardPayment.CardNumber);
                yield return cardNumberLuhn.IsSatisfiedBy(creditCardPayment.CardNumber);
                yield return expiryMonth.IsSatisfiedBy(creditCardPayment.ExpiryMonth);
                yield return expiryYear.IsSatisfiedBy(creditCardPayment.ExpiryYear);
            }

            yield return nameOnCard.IsSatisfiedBy(creditCardPayment.NameOnCard);
        }

        public bool ValidateCreditCardNumberByLuhnAlgorithm(string cardNumber)
        {
            var cardNumberValidation = _paymentOptionsRuleFactory.GetLuhnAlgorithmValidationRule("cardNumber");
            return cardNumberValidation.IsSatisfiedBy(cardNumber);
        }

        public IEnumerable<RuleResult> ValidateDirectDebitForSave(IDirectDebitPayment directDebitPayment)
        {
            var bsbNumber = _paymentOptionsRuleFactory.GetStringIsNumbersOnlyRule("DirectDebitPayment.BSBNumber");
            var accountNumber = _paymentOptionsRuleFactory.GetStringIsNumbersOnlyRule("DirectDebit.AccountNumber");
            var accountName = _paymentOptionsRuleFactory.GetStringHasValueRule("DirectDebit.AccountName");
            var bsbNumberLength = _paymentOptionsRuleFactory.GetStringIsBetweenMinAndMaxLengthRule("DirectDebit.BSBNumber", 6, 6);
            var accountNumberLength = _paymentOptionsRuleFactory.GetStringIsBetweenMinAndMaxLengthRule("DirectDebit.AccountNumber", 0, 9);
            var accountNameLength = _paymentOptionsRuleFactory.GetStringIsBetweenMinAndMaxLengthRule("DirectDebit.AccountName", 1, 32);

            yield return bsbNumber.IsSatisfiedBy(directDebitPayment.BSBNumber);
            yield return accountNumber.IsSatisfiedBy(directDebitPayment.AccountNumber);
            yield return accountName.IsSatisfiedBy(directDebitPayment.AccountName);
            yield return bsbNumberLength.IsSatisfiedBy(directDebitPayment.BSBNumber);
            yield return accountNumberLength.IsSatisfiedBy(directDebitPayment.AccountNumber);
            yield return accountNameLength.IsSatisfiedBy(directDebitPayment.AccountName);
        }

        public IEnumerable<RuleResult> ValidateDirectDebitForInforce(IDirectDebitPayment directDebitPayment)
        {
            var bsbNumber = _paymentOptionsRuleFactory.GetStringIsNumbersOnlyRule("DirectDebitPayment.BSBNumber");
            var accountNumber = _paymentOptionsRuleFactory.GetStringIsNumbersOnlyRule("DirectDebitPayment.AccountNumber");
            var bsbNumberLength = _paymentOptionsRuleFactory.GetStringIsBetweenMinAndMaxLengthRule("DirectDebitPayment.BSBNumber", 6, 6);

            yield return bsbNumber.IsSatisfiedBy(directDebitPayment.BSBNumber);
            yield return accountNumber.IsSatisfiedBy(directDebitPayment.AccountNumber);
            yield return bsbNumberLength.IsSatisfiedBy(directDebitPayment.BSBNumber);
        }

        public IEnumerable<RuleResult> ValidateSuperAnnuationForSave(ISuperAnnuationPayment superAnnuationPayment)
        {
            var taxFileNumber = _paymentOptionsRuleFactory.GetStringIsNumbersOnlyRule("SuperannuationPayment.TaxFileNumber");
            var taxFileNumberLength = _paymentOptionsRuleFactory.GetStringIsBetweenMinAndMaxLengthRule("SuperannuationPayment.TaxFileNumber", 9, 9);
            var membershipNumber = _paymentOptionsRuleFactory.GetStringIsNumbersOnlyRule("SuperannuationPayment.MembershipNumber");
            var fundUSI = _paymentOptionsRuleFactory.GetStringIsNumbersOnlyRule("SuperannuationPayment.FundUSI");
            var fundABN = _paymentOptionsRuleFactory.GetStringIsNumbersOnlyRule("SuperannuationPayment.FundABN");

            yield return fundUSI.IsSatisfiedBy(superAnnuationPayment.FundUSI);
            yield return fundABN.IsSatisfiedBy(superAnnuationPayment.FundABN);
            yield return membershipNumber.IsSatisfiedBy(superAnnuationPayment.MembershipNumber);
            yield return taxFileNumber.IsSatisfiedBy(superAnnuationPayment.TaxFileNumber);
            yield return taxFileNumberLength.IsSatisfiedBy(superAnnuationPayment.TaxFileNumber);
        }

        public IEnumerable<RuleResult> ValidateSuperAnnuationForInforce(ISuperAnnuationPayment superAnnuationPayment)
        {
            var fundUSI = _paymentOptionsRuleFactory.GetStringHasValueRule("SuperannuationPayment.FundUSI");
            var fundABN = _paymentOptionsRuleFactory.GetStringIsNumbersOnlyRule("SuperannuationPayment.FundABN");
            var fundName = _paymentOptionsRuleFactory.GetStringHasValueRule("SuperannuationPayment.FundName");
            var fundProduct = _paymentOptionsRuleFactory.GetStringHasValueRule("SuperannuationPayment.FundProduct");

            var membershipNumber = _paymentOptionsRuleFactory.GetStringHasValueRule("SuperannuationPayment.MembershipNumber");
            var taxFileNumber = _paymentOptionsRuleFactory.GetStringHasValueRule("SuperannuationPayment.TaxFileNumber");
            
            yield return fundUSI.IsSatisfiedBy(superAnnuationPayment.FundUSI);
            yield return fundABN.IsSatisfiedBy(superAnnuationPayment.FundABN);
            yield return fundName.IsSatisfiedBy(superAnnuationPayment.FundName);
            yield return fundProduct.IsSatisfiedBy(superAnnuationPayment.FundProduct);
            yield return membershipNumber.IsSatisfiedBy(superAnnuationPayment.MembershipNumber);
            yield return taxFileNumber.IsSatisfiedBy(superAnnuationPayment.TaxFileNumber);
        }

        public IEnumerable<RuleResult> ValidateSelfManagedSuperFundForSave(ISelfManagedSuperFundPayment SelfManagedSuperFundPayment)
        {
            var bsbNumber = _paymentOptionsRuleFactory.GetStringIsNumbersOnlyRule("SelfManagedSuperFundPayment.BSBNumber");
            var accountNumber = _paymentOptionsRuleFactory.GetStringIsNumbersOnlyRule("SelfManagedSuperFundPayment.AccountNumber");
            var accountName = _paymentOptionsRuleFactory.GetStringHasValueRule("SelfManagedSuperFundPayment.AccountName");
            var bsbNumberLength = _paymentOptionsRuleFactory.GetStringIsBetweenMinAndMaxLengthRule("SelfManagedSuperFundPayment.BSBNumber", 6, 6);
            var accountNumberLength = _paymentOptionsRuleFactory.GetStringIsBetweenMinAndMaxLengthRule("SelfManagedSuperFundPayment.AccountNumber", 0, 9);
            var accountNameLength = _paymentOptionsRuleFactory.GetStringIsBetweenMinAndMaxLengthRule("SelfManagedSuperFundPayment.AccountName", 1, 32);

            yield return bsbNumber.IsSatisfiedBy(SelfManagedSuperFundPayment.BSBNumber);
            yield return accountNumber.IsSatisfiedBy(SelfManagedSuperFundPayment.AccountNumber);
            yield return accountName.IsSatisfiedBy(SelfManagedSuperFundPayment.AccountName);
            yield return bsbNumberLength.IsSatisfiedBy(SelfManagedSuperFundPayment.BSBNumber);
            yield return accountNumberLength.IsSatisfiedBy(SelfManagedSuperFundPayment.AccountNumber);
            yield return accountNameLength.IsSatisfiedBy(SelfManagedSuperFundPayment.AccountName);
        }

        public IEnumerable<RuleResult> ValidateSelfManagedSuperFundForInforce(ISelfManagedSuperFundPayment SelfManagedSuperFundPayment)
        {
            var bsbNumber = _paymentOptionsRuleFactory.GetStringIsNumbersOnlyRule("SelfManagedSuperFundPayment.BSBNumber");
            var accountNumber = _paymentOptionsRuleFactory.GetStringIsNumbersOnlyRule("SelfManagedSuperFundPayment.AccountNumber");
            var bsbNumberLength = _paymentOptionsRuleFactory.GetStringIsBetweenMinAndMaxLengthRule("SelfManagedSuperFundPayment.BSBNumber", 6, 6);

            yield return bsbNumber.IsSatisfiedBy(SelfManagedSuperFundPayment.BSBNumber);
            yield return accountNumber.IsSatisfiedBy(SelfManagedSuperFundPayment.AccountNumber);
            yield return bsbNumberLength.IsSatisfiedBy(SelfManagedSuperFundPayment.BSBNumber);
        }
    }
}