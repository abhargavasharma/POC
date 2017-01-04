using System.Linq;
using System.Web.Http;
using TAL.QuoteAndApply.Infrastructure.Features;
using TAL.QuoteAndApply.SalesPortal.Web.Attributes;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.SalesPortal.Web.Services;
using TAL.QuoteAndApply.SalesPortal.Web.Services.Converters;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.PremiumCalculation;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk;
using TAL.QuoteAndApply.ServiceLayer.Underwriting;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Controllers.Api
{
    [WebApiSalesPortalSessionRequired]
    [RoutePrefix("api/policy/{quoteReferenceNumber}/risk/{riskId}/underwriting")]
    public class UnderwritingController : ApiController
    {
        private readonly IRiskUnderwritingAnswerSyncService _riskUnderwritingAnswerSyncService;
        private readonly IRiskUnderwritingQuestionService _riskUnderwritingQuestionService;
        private readonly IUnderwritingViewModelConverter _riskUnderwritingViewModelConverter;
        private readonly IRiskUnderwritingService _riskUnderwritingService;
        private readonly IPolicyPremiumCalculation _policyPremiumCalculation;
        private readonly ITalusUiUrlService _talusUiUrlService;
        private readonly IFeatures _features;

        private readonly ISalesPortalSummaryProvider _salesPortalSummaryProvider;

        public UnderwritingController(IRiskUnderwritingAnswerSyncService riskUnderwritingAnswerSyncService,
            IRiskUnderwritingService riskUnderwritingService,
            IPolicyPremiumCalculation policyPremiumCalculation, IFeatures features,
            ITalusUiUrlService talusUiUrlService, 
            ISalesPortalSummaryProvider salesPortalSummaryProvider, IRiskUnderwritingQuestionService riskUnderwritingQuestionService, IUnderwritingViewModelConverter riskUnderwritingViewModelConverter)
        {
            _riskUnderwritingAnswerSyncService = riskUnderwritingAnswerSyncService;
            _riskUnderwritingService = riskUnderwritingService;
            _policyPremiumCalculation = policyPremiumCalculation;
            _features = features;
            _talusUiUrlService = talusUiUrlService;
            _salesPortalSummaryProvider = salesPortalSummaryProvider;
            _riskUnderwritingQuestionService = riskUnderwritingQuestionService;
            _riskUnderwritingViewModelConverter = riskUnderwritingViewModelConverter;
        }

        [HttpGet, Route("status")]
        public IHttpActionResult IsUnderwritingCompleteForSelectedCovers(string quoteReferenceNumber, int riskId)
        {
            var riskUnderwritingStatusReult = _riskUnderwritingService.GetRiskUnderwritingStatus(riskId);

            //todo: remove this feature toggle. Only here because UW is still in progress
            var response = new UnderwritingCompleteResponse
            {
                UnderwritingCompletedForRiskStatus = UnderwritingCompletedForRiskStatus.Completed
            };
            if (_features.ValidateUnderwritingForPolicySubmission)
            {
                response =
                    UnderwritingCompleteResponse.CreateFrom(
                        riskUnderwritingStatusReult.CoverUnderwritingCompleteResults.Count(),
                        riskUnderwritingStatusReult.IsUnderwritingCompleteForRisk);
            }

            return Ok(response);
        }

        [HttpGet, Route("talusUiUrl")]
        public IHttpActionResult GetTalusUiUrl(string quoteReferenceNumber, int riskId)
        {   
            return Ok(_talusUiUrlService.GetTalusUiUrlWithPermissionsFor(quoteReferenceNumber, riskId));
        }

        [HttpGet, Route("sync")]
        public IHttpActionResult SyncUnderwriting(string quoteReferenceNumber, int riskId)
        {
            _riskUnderwritingAnswerSyncService.SyncRiskWithFullInterviewAndUpdatePlanEligibility(riskId);
            _policyPremiumCalculation.CalculateAndSavePolicy(quoteReferenceNumber);

            var salesPortalSummary = _salesPortalSummaryProvider.GetFor(quoteReferenceNumber, riskId);

            return Ok(UnderwritingSyncResponse.From(salesPortalSummary));
        }

        [HttpPost, Route("answer")]
        public IHttpActionResult UpdateQuestionAnswered(string quoteReferenceNumber, int riskId, UnderwritingQuestionAnswerRequest questionAnswerRequest)
        {
            var underwritingQuestionAnswer = 
                new UnderwritingQuestionAnswer(questionAnswerRequest.QuestionId, questionAnswerRequest.Answers.Select(x=> new UnderwritingAnswer(x.Id, x.Text)));

            _riskUnderwritingAnswerSyncService.SyncRiskWithUnderwritingAnswer(riskId, questionAnswerRequest.ConcurrencyToken, underwritingQuestionAnswer);
            _policyPremiumCalculation.CalculateAndSavePolicy(quoteReferenceNumber);

            var salesPortalSummary = _salesPortalSummaryProvider.GetFor(quoteReferenceNumber, riskId);

            return Ok(UnderwritingSyncResponse.From(salesPortalSummary));
        }

        [HttpPost, Route("question")]
        public IHttpActionResult AnswerQuestion(string quoteReferenceNumber, int riskId, UpdateQuestionRequest updateQuestionRequest)
        {
            var answerQuestionResult = _riskUnderwritingQuestionService.AnswerQuestionWithoutSyncingRaw(
                riskId,
                updateQuestionRequest.QuestionId,
                updateQuestionRequest.SelectedAnswers.Select(
                    a => new UnderwritingAnswer(a.Id, a.Text)).ToList());

            var response = _riskUnderwritingViewModelConverter.From(answerQuestionResult);
            return Ok(response);
        }

        [HttpGet, Route("interview")]
        public IHttpActionResult GetUnderwriting(string quoteReferenceNumber, int riskId)
        {
            
            var underwritingPosition = _riskUnderwritingQuestionService.GetCurrentUnderwritingRaw(riskId);
            var response = _riskUnderwritingViewModelConverter.From(underwritingPosition);

            return Ok(response);
        }

    }
}