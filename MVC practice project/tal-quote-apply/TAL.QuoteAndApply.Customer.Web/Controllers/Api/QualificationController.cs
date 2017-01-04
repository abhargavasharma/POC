using System.ComponentModel;
using System.Linq;
using System.Web.Http;
using TAL.QuoteAndApply.Customer.Web.Attributes;
using TAL.QuoteAndApply.Customer.Web.Converters;
using TAL.QuoteAndApply.Customer.Web.Models.Api;
using TAL.QuoteAndApply.Customer.Web.Models.Api.Underwriting;
using TAL.QuoteAndApply.Customer.Web.Services;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk;
using TAL.QuoteAndApply.ServiceLayer.Underwriting;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Models;
using TAL.QuoteAndApply.Web.Shared.HttpResults;
using TAL.QuoteAndApply.Web.Shared.Session;

namespace TAL.QuoteAndApply.Customer.Web.Controllers.Api
{
    [WebApiQuoteSessionRequired]
    [RoutePrefix("api/qualification")]
    public class QualificationController : BaseCustomerPortalApiController
    {
        
        private readonly IRiskUnderwritingQuestionService _riskUnderwritingQuestionService;
        private readonly IUnderwritingViewModelConverter _underwritingViewModelConverter;
        private readonly IPolicyWithRisksService _policyWithRisksService;
        private readonly ICustomerPolicyStateService _customerPolicyStateService;
        private readonly IRiskUnderwritingService _riskUnderwritingService;

        public QualificationController(IRiskUnderwritingQuestionService riskUnderwritingQuestionService,
            IQuoteSessionContext quoteSessionContext, IUnderwritingViewModelConverter underwritingViewModelConverter,
            IPolicyWithRisksService policyWithRisksService, ICustomerPolicyStateService customerPolicyStateService,
            IPolicyOverviewProvider policyOverviewProvider, IRiskUnderwritingService riskUnderwritingService) : base(quoteSessionContext, policyOverviewProvider)
        {
            _riskUnderwritingQuestionService = riskUnderwritingQuestionService;
            _underwritingViewModelConverter = underwritingViewModelConverter;
            _policyWithRisksService = policyWithRisksService;
            _customerPolicyStateService = customerPolicyStateService;
            _riskUnderwritingService = riskUnderwritingService;
        }

        [HttpGet, Route("risks")]
        public IHttpActionResult GetUnderwritingRisks()
        {
            var policyRiskIds = _policyWithRisksService.GetRiskIdsFrom(_quoteSessionContext.QuoteSession.QuoteReference);
            var response = policyRiskIds.Select(riskId => new RiskResponse(riskId));

            return Ok(response);
        }

        [HttpGet, Route("risk/{riskId:int}")]
        public IHttpActionResult GetUnderwriting(int riskId)
        {
            if (!IsRiskValidForApplicationSession(riskId))
            {
                return BadRequest();
            }

            var underwritingPosition = _riskUnderwritingQuestionService.GetCurrentUnderwriting(riskId);
            var response = _underwritingViewModelConverter.From(underwritingPosition);

            return Ok(response);
        }


        [HttpGet, Route("risk/{riskId:int}/validate")]
        public IHttpActionResult ValidateUnderwriting(int riskId)
        {
            if (!IsRiskValidForApplicationSession(riskId))
            {
                return BadRequest();
            }

            _customerPolicyStateService.FinaliseCustomerUnderwriting(_quoteSessionContext.QuoteSession.QuoteReference, riskId);

            string redirectUrl = Url.Route("Default", new { Controller = "Review", Action = "Index" });
            return new RedirectActionResult(redirectUrl);
        }

        [HttpPost, Route("risk/{riskId:int}")]
        public IHttpActionResult AnswerQuestion(int riskId, UpdateQuestionRequest updateQuestionRequest)
        {
            if (!IsRiskValidForApplicationSession(riskId))
            {
                return BadRequest();
            }

            var answerQuestionResult = _riskUnderwritingQuestionService.AnswerQuestionWithoutSyncing(
                riskId,
                updateQuestionRequest.QuestionId,
                updateQuestionRequest.SelectedAnswers.Select(
                    a => new UnderwritingAnswer(a.Id, a.Text)).ToList());
            
            var response = _underwritingViewModelConverter.From(answerQuestionResult);
            return Ok(response);
        }
    }
}