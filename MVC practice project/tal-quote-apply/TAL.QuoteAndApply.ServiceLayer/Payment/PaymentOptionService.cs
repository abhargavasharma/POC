using System;
using System.Linq;
using System.Text.RegularExpressions;
using TAL.QuoteAndApply.Payment.Service;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Payment.Models;
using TAL.QuoteAndApply.ServiceLayer.Payment.Models.Conveters;
using TAL.QuoteAndApply.Payment.Models;
using CreditCardType = TAL.QuoteAndApply.ServiceLayer.Payment.Models.CreditCardType;

namespace TAL.QuoteAndApply.ServiceLayer.Payment
{
    public interface IPaymentOptionService
    {
        PaymentOptionsParam GetCurrentPaymentOptions(string quoteReference, int riskId);
        PaymentOptionsParam AssignCreditCardPayment(string quoteReference, int riskId,
            CreditCardPaymentParam creditCard);
        PaymentOptionsParam AssignDirectDebitPayment(string quoteReference, int riskId,
            DirectDebitPaymentParam directDebit);

        PaymentOptionsParam AssignSelfManagedSuperFundPayment(string quoteReference, int riskId,
          SelfManagedSuperFundPaymentParam selfManagedSuperFund);

        PaymentOptionsParam AssignSuperannuationPayment(string quoteReference, int riskId,
            SuperannuationPaymentParam superannuation);

        bool ValidateCardNumber(string cardNumber, CreditCardType cardType);
    }

    public class PaymentOptionService : IPaymentOptionService
    {
        private readonly IPaymentOptionsAvailabilityProvider _paymentOptionsAvailabilityProvider;
        private readonly IPlanService _planService;
        private readonly IPolicyService _policyService;
        private readonly ICreditCardPaymentOptionService _creditCardPaymentOptionService;
        private readonly IDirectDebitPaymentOptionService _directDebitPaymentOptionService;
        private readonly ISuperAnnuationPaymentOptionService _superAnnuationPaymentOptionService;
        private readonly ISelfManagedSuperFundPaymentOptionService _selfManagedSuperFundPaymentOptionService;

        private readonly IDirectDebitPaymentParamConverter _debitPaymentParamConverter;
        private readonly ICreditCardPaymentParamConverter _creditCardPaymentParamConverter;
        private readonly ISuperannuationPaymentParamConverter _superannuationPaymentParamConverter;
        private readonly ISelfManagedSuperFundPaymentParamConverter _selfManagedSuperFundPaymentParamConverter;

        
        private readonly IPaymentRulesService _paymentRulesService;
        private const string VisaRegexPattern = "^4[0-9]{6,}$";
        private const string MasterRegexPattern = "^5[1-5][0-9]{5,}$";
        private static readonly Regex VisaRegex;
        private static readonly Regex MasterRegex;
		private readonly IPolicyPaymentService _policyPaymentSerivce;
        private readonly ICreditCardTypeConverter _creditCardTypeConverter;
        
        public PaymentOptionService(IPaymentOptionsAvailabilityProvider paymentOptionsAvailabilityProvider,
            ICreditCardPaymentOptionService creditCardPaymentOptionService,
            IDirectDebitPaymentOptionService directDebitPaymentOptionService,
            ISuperAnnuationPaymentOptionService superAnnuationPaymentOptionService,
            ISelfManagedSuperFundPaymentOptionService  selfManagedSuperFundPaymentOptionService,
            IDirectDebitPaymentParamConverter debitPaymentParamConverter,
            ICreditCardPaymentParamConverter creditCardPaymentParamConverter,
            ISelfManagedSuperFundPaymentParamConverter selfManagedSuperFundPaymentParamConverter,
            ISuperannuationPaymentParamConverter superannuationPaymentParamConverter, IPolicyService policyService,
            IPlanService planService, IPaymentRulesService paymentRulesService, IPolicyPaymentService policyPaymentService, ICreditCardTypeConverter creditCardTypeConverter)
        {
            _paymentOptionsAvailabilityProvider = paymentOptionsAvailabilityProvider;
            _creditCardPaymentOptionService = creditCardPaymentOptionService;
            _directDebitPaymentOptionService = directDebitPaymentOptionService;
            _superAnnuationPaymentOptionService = superAnnuationPaymentOptionService;
            _selfManagedSuperFundPaymentOptionService = selfManagedSuperFundPaymentOptionService;

            _debitPaymentParamConverter = debitPaymentParamConverter;
            _creditCardPaymentParamConverter = creditCardPaymentParamConverter;
            _superannuationPaymentParamConverter = superannuationPaymentParamConverter;
            _selfManagedSuperFundPaymentParamConverter = selfManagedSuperFundPaymentParamConverter;

            _policyService = policyService;
            _planService = planService;
            _paymentRulesService = paymentRulesService;
			_policyPaymentSerivce = policyPaymentService;
            _creditCardTypeConverter = creditCardTypeConverter;
            
            
        }

        static PaymentOptionService()
        {
            VisaRegex = new Regex(VisaRegexPattern, RegexOptions.Compiled);
            MasterRegex = new Regex(MasterRegexPattern, RegexOptions.Compiled);
        }

        public PaymentOptionsParam AssignCreditCardPayment(string quoteReference, int riskId, CreditCardPaymentParam creditCard)
        {
            var plans = _planService.GetPlansForRisk(riskId);
            var availablePlayments = _paymentOptionsAvailabilityProvider.GetAvailablePaymentTypes(plans).ToArray();

            if (!availablePlayments.Any(ap => ap == DataModel.Payment.PaymentType.CreditCard))
                throw new ApplicationException("Credit Card payment is not available for this policy");

            var policy = _policyService.GetByQuoteReferenceNumber(quoteReference);            

            var creditCardPayment = _creditCardPaymentOptionService.AssignCreditCardPayment(policy.Id, creditCard.NameOnCard,
                creditCard.CardNumber, creditCard.ExpiryMonth, creditCard.ExpiryYear, _creditCardTypeConverter.From(creditCard.CardType));

            //for credit card validation before tokenisation
            ICreditCardPayment creditCardPyamentBeforeTokenisation = new CreditCardPaymentDto
            {
                CardType = creditCardPayment.CardType,
                NameOnCard = creditCardPayment.NameOnCard,
                CardNumber = creditCard.CardNumber,
                ExpiryMonth = creditCardPayment.ExpiryMonth,
                ExpiryYear = creditCardPayment.ExpiryYear,
                Id = creditCardPayment.Id
            };

            var isValidForInfoce = _paymentRulesService.ValidateCreditCardForInforce(creditCardPyamentBeforeTokenisation).All(x => x.IsSatisfied);

            if (isValidForInfoce)
            {
                RemoveCurrentDirectDebitPayment(policy.Id, DataModel.Payment.PaymentType.CreditCard);
                RemoveCurrentSuperannuationPayment(policy.Id, DataModel.Payment.PaymentType.CreditCard);
            }

            return GetPaymentOptionsParam(riskId, _creditCardPaymentParamConverter.From(creditCardPayment, isValidForInfoce), 
                GetDirectDebitPaymentParam(policy),
                GetSuperannuationPaymentParam(policy),
                GetSelfManagedSuperFundPaymentParam(policy));
        }

        public PaymentOptionsParam AssignSuperannuationPayment(string quoteReference, int riskId, SuperannuationPaymentParam superannuation)
        {
            var plans = _planService.GetPlansForRisk(riskId);
            var availablePlayments = _paymentOptionsAvailabilityProvider.GetAvailablePaymentTypes(plans).ToArray();

            if (!availablePlayments.Any(ap => ap == DataModel.Payment.PaymentType.SuperAnnuation))
                throw new ApplicationException("Superannuation payment is not available for this policy");
            
            var policy = _policyService.GetByQuoteReferenceNumber(quoteReference);

            RemoveCurrentPaymentOption(policy.Id, DataModel.Payment.PaymentType.SuperAnnuation);

            var superPayment = _superAnnuationPaymentOptionService.AssignSuperAnnuationPayment(policy.Id,
                superannuation.FundName, superannuation.FundProduct, superannuation.FundABN, superannuation.FundUSI, superannuation.MembershipNumber,
                superannuation.TaxFileNumber);

            var isValidForInfoce = _paymentRulesService.ValidateSuperAnnuationForInforce(superPayment).All(x => x.IsSatisfied);

            return GetPaymentOptionsParam(riskId, GetCreditCardPaymentParam(policy), GetDirectDebitPaymentParam(policy),
                _superannuationPaymentParamConverter.From(superPayment, isValidForInfoce),
                GetSelfManagedSuperFundPaymentParam(policy));
        }

        public PaymentOptionsParam AssignDirectDebitPayment(string quoteReference, int riskId, DirectDebitPaymentParam directDebit)
        {
            var ruleResults = _paymentRulesService.ValidateDirectDebitForSave(_debitPaymentParamConverter.From(directDebit));

            var plans = _planService.GetPlansForRisk(riskId);
            var availablePlayments = _paymentOptionsAvailabilityProvider.GetAvailablePaymentTypes(plans).ToArray();

            if (!availablePlayments.Any(ap => ap == DataModel.Payment.PaymentType.DirectDebit))
                throw new ApplicationException("Direct Debit payment is not available for this policy");

            var policy = _policyService.GetByQuoteReferenceNumber(quoteReference);

            RemoveCurrentPaymentOption(policy.Id, DataModel.Payment.PaymentType.DirectDebit);

            var directDebitPayment = _directDebitPaymentOptionService.AssignDebitCardPayment(policy.Id, directDebit.BSBNumber, directDebit.AccountNumber, directDebit.AccountName);
			var returnObj = GetPaymentOptionsParam(riskId, GetCreditCardPaymentParam(policy), _debitPaymentParamConverter.From(directDebitPayment, ruleResults.All(x => x.IsSatisfied)),
                GetSuperannuationPaymentParam(policy), GetSelfManagedSuperFundPaymentParam(policy));
			return returnObj;
        }

        public PaymentOptionsParam AssignSelfManagedSuperFundPayment(string quoteReference, int riskId, SelfManagedSuperFundPaymentParam selfManagedSuperFund)
        {
            var ruleResults = _paymentRulesService.ValidateSelfManagedSuperFundForSave(_selfManagedSuperFundPaymentParamConverter.From(selfManagedSuperFund));

            var plans = _planService.GetPlansForRisk(riskId);
            var availablePlayments = _paymentOptionsAvailabilityProvider.GetAvailablePaymentTypes(plans).ToArray();

            if (!availablePlayments.Any(ap => ap == DataModel.Payment.PaymentType.SelfManagedSuperFund))
                throw new ApplicationException("Self managed super fund payment is not available for this policy");

            var policy = _policyService.GetByQuoteReferenceNumber(quoteReference);

            RemoveCurrentPaymentOption(policy.Id, DataModel.Payment.PaymentType.SelfManagedSuperFund);

            var selfManagedSuperFundPayment = _selfManagedSuperFundPaymentOptionService.AssignSelfManagedSuperFundPayment(policy.Id, selfManagedSuperFund.BSBNumber, selfManagedSuperFund.AccountNumber, selfManagedSuperFund.AccountName);
            var returnObj = GetPaymentOptionsParam(riskId, GetCreditCardPaymentParam(policy), GetDirectDebitPaymentParam(policy),
                GetSuperannuationPaymentParam(policy), _selfManagedSuperFundPaymentParamConverter.From(selfManagedSuperFundPayment, ruleResults.All(x => x.IsSatisfied)));
            return returnObj;
        }
        /// <summary>
        /// Remove existing creditCardPayment, existing directDebitPayment and existing superAnnuationPayment
        /// </summary>
        /// <param name="policyId"></param>
        /// <param name="paymentType"></param>
        private void RemoveCurrentPaymentOption(int policyId, DataModel.Payment.PaymentType paymentType)
        {
            RemoveCurrentCreditCardPayment(policyId, paymentType);
            RemoveCurrentDirectDebitPayment(policyId, paymentType);
            RemoveCurrentSuperannuationPayment(policyId, paymentType);
            RemoveCurrentSelfManagedSuperFundPayment(policyId, paymentType);
        }

        private void RemoveCurrentSelfManagedSuperFundPayment(int policyId, DataModel.Payment.PaymentType paymentType)
        {
            var currentSelfManagedSuperFundPayment = _selfManagedSuperFundPaymentOptionService.GetCurrentSelfManagedSuperFundPayment(policyId);
            if (currentSelfManagedSuperFundPayment != null)
            {
                _selfManagedSuperFundPaymentOptionService.RemoveExistingPayment(currentSelfManagedSuperFundPayment.PolicyPaymentId);
                _policyPaymentSerivce.UpdateExistingPaymentType(policyId, paymentType);
            }
        }

        private void RemoveCurrentCreditCardPayment(int policyId, DataModel.Payment.PaymentType paymentType)
        {
            var currentCreditCardPayment = _creditCardPaymentOptionService.GetCurrentCreditCardPayment(policyId);
            if (currentCreditCardPayment != null)
            {
                _creditCardPaymentOptionService.RemoveExistingPayment(currentCreditCardPayment.PolicyPaymentId);
                _policyPaymentSerivce.UpdateExistingPaymentType(policyId, paymentType);
            }
        }

        private void RemoveCurrentDirectDebitPayment(int policyId, DataModel.Payment.PaymentType paymentType)
        {
            var currentDirectDebitPayment = _directDebitPaymentOptionService.GetCurrentDirectDebitPayment(policyId);
            if (currentDirectDebitPayment != null)
            {
                _directDebitPaymentOptionService.RemoveExistingPayment(currentDirectDebitPayment.PolicyPaymentId);
                _policyPaymentSerivce.UpdateExistingPaymentType(policyId, paymentType);
            }
        }

        private void RemoveCurrentSuperannuationPayment(int policyId, DataModel.Payment.PaymentType paymentType)
        {
            var currentSuperannuationPayment = _superAnnuationPaymentOptionService.GetCurrentSuperAnnuationPayment(policyId);
            if (currentSuperannuationPayment != null)
            {
                _superAnnuationPaymentOptionService.RemoveExistingPayment(currentSuperannuationPayment.PolicyPaymentId);
                _policyPaymentSerivce.UpdateExistingPaymentType(policyId, paymentType);
            }
        }

        public PaymentOptionsParam GetCurrentPaymentOptions(string quoteReference, int riskId)
        {
            var policy = _policyService.GetByQuoteReferenceNumber(quoteReference);

            return GetPaymentOptionsParam(riskId, GetCreditCardPaymentParam(policy), GetDirectDebitPaymentParam(policy),
                GetSuperannuationPaymentParam(policy), GetSelfManagedSuperFundPaymentParam(policy));
        }

      
        public bool ValidateCardNumber(string cardNumber, CreditCardType cardType)
        {
            //if card type is not selected, card number validation will be passed
            if (cardType == CreditCardType.Unknown)
            {
                return true;
            }

            var validCardNumber = ValidateCardNumberFirstSixDigits(cardNumber, cardType);
            return validCardNumber && _paymentRulesService.ValidateCreditCardNumberByLuhnAlgorithm(cardNumber);
        }

        //https://en.wikipedia.org/wiki/Bank_card_number
        private bool ValidateCardNumberFirstSixDigits(string cardNumber, CreditCardType cardType)
        {
            var matchVisaRegex = VisaRegex.Match(cardNumber);
            var matchMasterRegex = MasterRegex.Match(cardNumber);
            
            var matchVisa = matchVisaRegex.Success && cardType == CreditCardType.Visa;
            var matchMaster = matchMasterRegex.Success && cardType == CreditCardType.MasterCard;


            return matchVisa || matchMaster;
        }

        private PaymentOptionsParam GetPaymentOptionsParam(int riskId, CreditCardPaymentParam creditCard, 
            DirectDebitPaymentParam directDebit, SuperannuationPaymentParam super, SelfManagedSuperFundPaymentParam smsf)
        {
            var retVal = new PaymentOptionsParam
            {
                CreditCardPayment = creditCard,
                DirectDebitPayment = directDebit,
                SuperannuationPayment = super,
                SelfManagedSuperFundPayment = smsf
            };

            retVal = DetermineAvailability(riskId, retVal);

            retVal.IsComplete = retVal.SuperannuationPayment.IsValidForInforce ||
                                retVal.DirectDebitPayment.IsValidForInforce ||
                                retVal.CreditCardPayment.IsValidForInforce || 
                                retVal.SelfManagedSuperFundPayment.IsValidForInforce;

            return retVal;
        }

        private CreditCardPaymentParam GetCreditCardPaymentParam(IPolicy policy)
        {
            var paymentParam = new CreditCardPaymentParam();
            var currentCreditCardPayment = _creditCardPaymentOptionService.GetCurrentCreditCardPayment(policy.Id);
            if (currentCreditCardPayment != null)
            {
                paymentParam = _creditCardPaymentParamConverter.From(currentCreditCardPayment, false);
                paymentParam.IsValidForInforce =
                    _paymentRulesService.ValidateCreditCardForInforce(currentCreditCardPayment).All(x => x.IsSatisfied);
            }
            return paymentParam;
        }

        private DirectDebitPaymentParam GetDirectDebitPaymentParam(IPolicy policy)
        {
            var paymentParam = new DirectDebitPaymentParam();
            var currentDirectDebitPayment = _directDebitPaymentOptionService.GetCurrentDirectDebitPayment(policy.Id);
            if (currentDirectDebitPayment != null)
            {
                paymentParam = _debitPaymentParamConverter.From(currentDirectDebitPayment, false);
                paymentParam.IsValidForInforce =
                    _paymentRulesService.ValidateDirectDebitForInforce(currentDirectDebitPayment).All(x => x.IsSatisfied);
            }
            return paymentParam;
        }

        private SelfManagedSuperFundPaymentParam GetSelfManagedSuperFundPaymentParam(IPolicy policy)
        {
            var paymentParam = new SelfManagedSuperFundPaymentParam();
            var currentSelfManagedSuperFundPayment = _selfManagedSuperFundPaymentOptionService.GetCurrentSelfManagedSuperFundPayment(policy.Id);
            if (currentSelfManagedSuperFundPayment != null)
            {
                paymentParam = _selfManagedSuperFundPaymentParamConverter.From(currentSelfManagedSuperFundPayment,false);
                paymentParam.IsValidForInforce =
                    _paymentRulesService.
                    ValidateSelfManagedSuperFundForInforce
                    (currentSelfManagedSuperFundPayment).All(x => x.IsSatisfied);
            }
            return paymentParam;
        }

        private SuperannuationPaymentParam GetSuperannuationPaymentParam(IPolicy policy)
        {
            var paymentParam = new SuperannuationPaymentParam();
            var currentSuperannuationPayment = _superAnnuationPaymentOptionService.GetCurrentSuperAnnuationPayment(policy.Id);
            if (currentSuperannuationPayment != null)
            {
                paymentParam = _superannuationPaymentParamConverter.From(currentSuperannuationPayment);
                paymentParam.IsValidForInforce =
                    _paymentRulesService.ValidateSuperAnnuationForInforce(currentSuperannuationPayment).All(x => x.IsSatisfied);
            }
            return paymentParam;
        }

        private PaymentOptionsParam DetermineAvailability(int riskId, PaymentOptionsParam options)
        {
            var plans = _planService.GetPlansForRisk(riskId);
            var availablePlayments = _paymentOptionsAvailabilityProvider.GetAvailablePaymentTypes(plans).ToArray();

            options.IsCreditCardAvailable = availablePlayments.Any(ap => ap == DataModel.Payment.PaymentType.CreditCard);
            options.IsDirectDebitAvailable = availablePlayments.Any(ap => ap == DataModel.Payment.PaymentType.DirectDebit);
            options.IsSuperFundAvailable = availablePlayments.Any(ap => ap == DataModel.Payment.PaymentType.SuperAnnuation);
            options.IsSelfManagedSuperFundAvailable = availablePlayments.Any(ap => ap == DataModel.Payment.PaymentType.SelfManagedSuperFund);

            return options;
        }
    }
}
