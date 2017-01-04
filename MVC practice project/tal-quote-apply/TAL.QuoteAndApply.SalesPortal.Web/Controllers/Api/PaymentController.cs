using System.Net;
using System.Web.Http;
using TAL.QuoteAndApply.Infrastructure.Http.Exceptions;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.SalesPortal.Web.Services.Converters;
using TAL.QuoteAndApply.ServiceLayer.Payment;
using TAL.QuoteAndApply.Web.Shared.HttpResults;
using TAL.QuoteAndApply.SalesPortal.Web.Attributes;

namespace TAL.QuoteAndApply.SalesPortal.Web.Controllers.Api
{
    [WebApiSalesPortalSessionRequired]
    [RoutePrefix("api/policy/{quoteReferenceNumber}/payment")]
    public class PaymentController : ApiController
    {
        private readonly ISuperFundPaymentViewModelConverter _superFundPaymentViewModelConverter;
        private readonly IDirectDebitPaymentViewModelConverter _debitPaymentViewModelConverter;
        private readonly ICreditCardPaymentViewModelConverter _creditCardPaymentViewModelConverter;
        private readonly IPaymentOptionsViewModelConverter _paymentOptionsViewModelConverter;
        private readonly IPaymentOptionService _paymentOptionService;
		private readonly ISelfManagedSuperFundPaymentViewModelConverter _selfManagedSuperFundPaymentViewModelConverter;

        public PaymentController(ISuperFundPaymentViewModelConverter superFundPaymentViewModelConverter,
            IDirectDebitPaymentViewModelConverter debitPaymentViewModelConverter,
            ICreditCardPaymentViewModelConverter credCreateClientRequestConverter,
            IPaymentOptionService paymentOptionService,
            IPaymentOptionsViewModelConverter paymentOptionsViewModelConverter,
            ISelfManagedSuperFundPaymentViewModelConverter selfManagedSuperFundPaymentViewModelConverter)
        {
            _superFundPaymentViewModelConverter = superFundPaymentViewModelConverter;
            _debitPaymentViewModelConverter = debitPaymentViewModelConverter;
            _creditCardPaymentViewModelConverter = credCreateClientRequestConverter;
            _paymentOptionService = paymentOptionService;
            _paymentOptionsViewModelConverter = paymentOptionsViewModelConverter;
			_selfManagedSuperFundPaymentViewModelConverter = selfManagedSuperFundPaymentViewModelConverter;
        }

        [HttpGet, Route("risk/{riskId}/paymentOptions")]
        public IHttpActionResult GetAvailablePaymentOptions(string quoteReferenceNumber, int riskId)
        {
            var model = _paymentOptionService.GetCurrentPaymentOptions(quoteReferenceNumber, riskId);

            var retVal = _paymentOptionsViewModelConverter.From(model);

            return Ok(retVal);
        }


        [HttpPost, Route("risk/{riskId}/superannuation")]
        public IHttpActionResult PayViaSuperanuation(string quoteReferenceNumber, int riskId, [FromBody]SuperFundPaymentViewModel superFundPayment)
        {
            if (!ModelState.IsValid)
            {
                return new InvalidModelStateActionResult(ModelState);
            }

            var serviceModel = _superFundPaymentViewModelConverter.From(superFundPayment);

            var retVal = _paymentOptionService.AssignSuperannuationPayment(quoteReferenceNumber, riskId, serviceModel);

            return Ok(retVal);
        }

        [HttpPost, Route("risk/{riskId}/creditcard")]
        public IHttpActionResult PayViaCreditCard(string quoteReferenceNumber, int riskId, [FromBody]CreditCardPaymentViewModel creditCardPayment)
        {
            if (!ModelState.IsValid)
            {
                return new InvalidModelStateActionResult(ModelState);
            }

            var serviceModel = _creditCardPaymentViewModelConverter.From(creditCardPayment);

            try
            {
                // in a try catch because the tokenization service could fail and we want to know this before web API swallows the exception
                var retVal = _paymentOptionService.AssignCreditCardPayment(quoteReferenceNumber, riskId, serviceModel);
                return Ok(retVal);
            }
            catch (ThirdPartyServiceException)
            {
                return new CustomExceptionActionResult(HttpStatusCode.InternalServerError,
                    "The credit card details could not be successfully validated as the card system is down. Please try again later.",
                    "CreditCardPayment");
            }

        }

        [HttpPost, Route("risk/{riskId}/directdebit")]
        public IHttpActionResult PayViaDirectDebit(string quoteReferenceNumber, int riskId, [FromBody]DirectDebitPaymentViewModel directDebitPayment)
        {
            if (!ModelState.IsValid)
            {
                return new InvalidModelStateActionResult(ModelState);
            }
            var serviceModel = _debitPaymentViewModelConverter.From(directDebitPayment);

            var retVal = _paymentOptionService.AssignDirectDebitPayment(quoteReferenceNumber, riskId, serviceModel);

            return Ok(retVal);
        }
		
		[HttpPost, Route("risk/{riskId}/selfmanagedsuperfund")]
        public IHttpActionResult PayViaSelfManagedsuperFund(string quoteReferenceNumber, int riskId, [FromBody]SelfManagedSuperFundPaymentViewModel selfManagedSuperFundPayment)
        {
            if (!ModelState.IsValid)
            {
                return new InvalidModelStateActionResult(ModelState);
            }
            var serviceModel = _selfManagedSuperFundPaymentViewModelConverter.From(selfManagedSuperFundPayment);

            var retVal = _paymentOptionService.AssignSelfManagedSuperFundPayment(quoteReferenceNumber, riskId, serviceModel);

            return Ok(retVal);
        }
    }
}
