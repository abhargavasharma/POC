using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using TAL.QuoteAndApply.Customer.Web.Attributes;
using TAL.QuoteAndApply.Customer.Web.Models;
using TAL.QuoteAndApply.Customer.Web.Services.Converters;
using TAL.QuoteAndApply.Customer.Web.Models.Api;
using TAL.QuoteAndApply.Web.Shared.HttpResults;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.Web.Shared.Session;

namespace TAL.QuoteAndApply.Customer.Web.Controllers.Api
{
    [RoutePrefix("api/chat")]
    public class ChatController : ApiController
    {
        private readonly ISaveCustomerRequestConverter _saveCustomerRequestConverter;
        private readonly IQuoteSessionContext _quoteSessionContext;
        private readonly IChatService _chatService;
        
        public ChatController(ISaveCustomerRequestConverter saveCustomerRequestConverter, ICustomerSaveService customerSaveService, IQuoteSessionContext quoteSessionContext, ICustomerCallbackService customerCallbackService, IChatService chatService)
        {
            _saveCustomerRequestConverter = saveCustomerRequestConverter;
            _quoteSessionContext = quoteSessionContext;
            _chatService = chatService;
        }


        [HttpPost, Route("callback")]
        public async Task<IHttpActionResult> RequestCallback(ChatRequestCallbackRequest request)
        {
            if (!ModelState.IsValid)
            {
                return new InvalidModelStateActionResult(ModelState);
            }

            var saveRequest = _saveCustomerRequestConverter.From(request);
            try
            {
                var response = await _chatService.RequestCallback(_quoteSessionContext.QuoteSession.QuoteReference, saveRequest);
                if (!response) return BadRequest("unable to request callback");
                _quoteSessionContext.ExtendSessionWithChatCallBack(_quoteSessionContext.QuoteSession.QuoteReference);
                return Ok(true);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }
        
        [HttpGet, Route("available"), AllowAnonymous]
        public IHttpActionResult IsWebChatAvailable()
        {
            return Ok(new ChatAvailabilityResponse
            {
                WebChatAvailableFrom = _chatService.WebChatStartTime,
                WebChatAvailableTo = _chatService.WebChatEndTime,
                WebChatIsAvailable = _chatService.IsWebChatAvailable(),
                WebChatUrl = _chatService.GetWebChatUrl(_quoteSessionContext.QuoteSession?.QuoteReference)
            });
        }

        [HttpGet, Route("availableAndCreateInteraction"), AllowAnonymous]
        public IHttpActionResult IsWebChatAvailableAndCreateInteraction()
        {
            bool isChatAvailable = _chatService.IsWebChatAvailable();
            string quoteRefNum = null;
            if (_quoteSessionContext != null && _quoteSessionContext.HasValue())
            {
                quoteRefNum = _quoteSessionContext.QuoteSession.QuoteReference;
            }
            
            if (isChatAvailable)
            {
                if (!string.IsNullOrEmpty(quoteRefNum))
                {
                    _chatService.IsWebChatAvailableAndCreateInteraction(quoteRefNum);
                }

            }
            return Ok(new ChatAvailabilityResponse
            {
                WebChatAvailableFrom = _chatService.WebChatStartTime,
                WebChatAvailableTo = _chatService.WebChatEndTime,
                WebChatIsAvailable = isChatAvailable,
                WebChatUrl = _chatService.GetWebChatUrl(quoteRefNum)
            });
        }

    }
}