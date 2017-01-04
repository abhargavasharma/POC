using System.Web.Http;
using TAL.QuoteAndApply.Customer.Web.Attributes;
using TAL.QuoteAndApply.Customer.Web.Configuration;
using TAL.QuoteAndApply.Customer.Web.Services.Converters;
using TAL.QuoteAndApply.Customer.Web.Models.Api;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Web.Shared.HttpResults;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.Web.Shared.Session;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.Customer.Web.Controllers.Api
{
    [WebApiQuoteSessionRequired]
    [RoutePrefix("api/save/risk/{riskId:int}")]
    public class SaveController : BaseCustomerPortalApiController
    {
        private readonly ISaveCustomerRequestConverter _saveCustomerRequestConverter;
        private readonly ICustomerSaveService _customerSaveService;
        private readonly IQuoteSessionContext _quoteSessionContext;
        private readonly IBrandSettingsProvider _brandSettingsProvider;

        public SaveController(ISaveCustomerRequestConverter saveCustomerRequestConverter,
                                ICustomerSaveService customerSaveService,
                                IQuoteSessionContext quoteSessionContext, 
                                IPolicyOverviewProvider policyOverviewProvider, 
                                IBrandSettingsProvider brandSettingsProvider) : base(quoteSessionContext, policyOverviewProvider)
        {
            _saveCustomerRequestConverter = saveCustomerRequestConverter;
            _customerSaveService = customerSaveService;
            _quoteSessionContext = quoteSessionContext;
            _brandSettingsProvider = brandSettingsProvider;
        }

        [HttpPost, Route("")]
        public IHttpActionResult SaveCustomer(int riskId, SaveCustomerRequest saveCustomerRequest)
        {
            if (!ModelState.IsValid)
            {
                return new InvalidModelStateActionResult(ModelState);
            }
            var saveRequest = _saveCustomerRequestConverter.From(riskId, saveCustomerRequest);
            _customerSaveService.SaveCustomer(_quoteSessionContext.QuoteSession.QuoteReference, saveRequest);

            /*  TODO: account existance is only UI concern at the moment so keeping this logic in controller.
                Move into service if saving a quote always needs to know if account exists in save result */
            var accountExists = _customerSaveService.AccountExists(_quoteSessionContext.QuoteSession.QuoteReference);
            var response = new SaveCustomerResponse {AccountExists = accountExists};

            return Ok(response);
        }

        [HttpPost, Route("createLogin")]
        public IHttpActionResult CreateLogin(int riskId, CreateLoginRequest createLoginRequest)
        {
            if (!ModelState.IsValid)
            {
                return new InvalidModelStateActionResult(ModelState);
            }
            var loginRequest = _saveCustomerRequestConverter.From(riskId, createLoginRequest);
            var loginCreated = _customerSaveService.CreateLogin(_quoteSessionContext.QuoteSession.QuoteReference, loginRequest);
            var emailCreated = _customerSaveService.SendEmail(riskId, _quoteSessionContext.QuoteSession.QuoteReference, _brandSettingsProvider.BrandKey);
            if (loginCreated)
            {
                return Ok();
            }

            ModelState.AddModelError("createLoginRequestError", "We couldn't set a password for your quote at this time. Please continue your quote or call 131 825");
            return new InvalidModelStateActionResult(ModelState);            
        }

        [HttpGet, Route("contactDetails")]
        public IHttpActionResult ContactDetails(int riskId)
        {
            if (!IsRiskValidForApplicationSession(riskId))
            {
                return BadRequest(new SaveCustomerParam().ToJson());
            }
            var contactDetails = _customerSaveService.GetSaveCustomerParamByRiskId(riskId, _quoteSessionContext.QuoteSession.SessionData.CallBackRequested);
            return Ok(contactDetails);
        }
    }
}