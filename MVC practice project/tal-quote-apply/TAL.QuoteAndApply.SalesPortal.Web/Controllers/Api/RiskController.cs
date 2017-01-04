using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Web.Http;
using TAL.QuoteAndApply.SalesPortal.Web.Attributes;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.SalesPortal.Web.Services;
using TAL.QuoteAndApply.SalesPortal.Web.Services.Converters;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.PremiumCalculation;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk;
using TAL.QuoteAndApply.ServiceLayer.Validation;
using TAL.QuoteAndApply.Web.Shared.HttpResults;

namespace TAL.QuoteAndApply.SalesPortal.Web.Controllers.Api
{
    [WebApiSalesPortalSessionRequired]
    [RoutePrefix("api/policy/{quoteReferenceNumber}/risk/{riskId:int}")]
    public class RiskController : ApiController
    {
        private readonly IRiskPersonalDetailsProvider _riskPersonalDetailsProvider;
        private readonly IRiskRatingFactorsProvider _riskRatingFactorsProvider;
        private readonly IUpdateRiskService _updateRiskService;
        private readonly IPersonalDetailsRequestConverter _personalDetailsRequestConverter;
        private readonly IRatingFactorsRequestConverter _ratingFactorsRequestConverter;
        private readonly IPolicyPremiumCalculation _policyPremiumCalculation;
        private readonly IPolicyPremiumSummaryProvider _policyPremiumSummaryProvider;
        private readonly IRiskPremiumSummaryViewModelConverter _riskPremiumSummaryViewModelConverter;
        private readonly IPartyConsentProvider _partyConsentProvider;
        private readonly IPartyConsentRequestConverter _partyConsentRequestConverter;
        private readonly ISalesPortalSummaryProvider _salesPortalSummaryProvider;
        private readonly IBrandAuthorizationService _brandAuthorizationService;

        public RiskController(IRiskPersonalDetailsProvider riskPersonalDetailsProvider, 
            IRiskRatingFactorsProvider riskRatingFactorsProvider,
            IUpdateRiskService updateRiskService,
            IPersonalDetailsRequestConverter personalDetailsRequestConverter,
            IRatingFactorsRequestConverter ratingFactorsRequestConverter, 
            IPolicyPremiumCalculation policyPremiumCalculation, 
            IPolicyPremiumSummaryProvider policyPremiumSummaryProvider, 
            IRiskPremiumSummaryViewModelConverter riskPremiumSummaryViewModelConverter, 
            IPartyConsentProvider partyConsentProvider, 
            IPartyConsentRequestConverter partyConsentRequestConverter, 
            ISalesPortalSummaryProvider salesPortalSummaryProvider, 
            IBrandAuthorizationService brandAuthorizationService)
        {
            _riskPersonalDetailsProvider = riskPersonalDetailsProvider;
            _riskRatingFactorsProvider = riskRatingFactorsProvider;
            _updateRiskService = updateRiskService;
            _personalDetailsRequestConverter = personalDetailsRequestConverter;
            _ratingFactorsRequestConverter = ratingFactorsRequestConverter;
            _policyPremiumCalculation = policyPremiumCalculation;
            _policyPremiumSummaryProvider = policyPremiumSummaryProvider;
            _riskPremiumSummaryViewModelConverter = riskPremiumSummaryViewModelConverter;
            _partyConsentProvider = partyConsentProvider;
            _partyConsentRequestConverter = partyConsentRequestConverter;
            _salesPortalSummaryProvider = salesPortalSummaryProvider;
            _brandAuthorizationService = brandAuthorizationService;
        }

        [HttpGet, Route("lifeInsuredDetails")]
        public IHttpActionResult LifeInsuredPersonalDetails(string quoteReferenceNumber, int riskId)
        {
            if (!_brandAuthorizationService.CanAccess(quoteReferenceNumber)) return Unauthorized(new List<AuthenticationHeaderValue>());

            var risk = _riskPersonalDetailsProvider.GetFor(riskId);

            var model = _personalDetailsRequestConverter.From(risk);

            return Ok(model);
        }
        
        [HttpPost, Route("lifeInsuredDetails")]
        public IHttpActionResult LifeInsuredPersonalDetails(string quoteReferenceNumber, int riskId, [FromBody]LifeInsuredDetailsRequest lifeInsuredDetailsRequest)
        {
            if (!ModelState.IsValid)
            {
                return new InvalidModelStateActionResult(ModelState);
            }

            var lifeInsuredDetailsParam = _personalDetailsRequestConverter.From(riskId, lifeInsuredDetailsRequest);

            var model=_updateRiskService.UpdateLifeInsuredDetails(lifeInsuredDetailsParam, quoteReferenceNumber);
          
            return Ok(model);
        }

        [HttpGet, Route("ratingfactors")]
        public IHttpActionResult RatingFactors(string quoteReferenceNumber, int riskId)
        {
            if (!_brandAuthorizationService.CanAccess(quoteReferenceNumber)) return Unauthorized(new List<AuthenticationHeaderValue>());

            var risk = _riskRatingFactorsProvider.GetFor(quoteReferenceNumber, riskId);
            var model = _ratingFactorsRequestConverter.From(risk);

            var salesPortalSummary = _salesPortalSummaryProvider.GetFor(quoteReferenceNumber, riskId);
            
            return Ok(RatingFactorsResponse.From(model, risk.IsRatingFactorsValidForInforce, salesPortalSummary));
        }

        [HttpPost, Route("ratingfactors")]
        public IHttpActionResult RatingFactors(string quoteReferenceNumber, int riskId, [FromBody]RatingFactorsRequest ratingFactorsRequest)
        {
            if (!ModelState.IsValid)
            {
                return new InvalidModelStateActionResult(ModelState);
            }

            var updateRatingFactorsResponse = _updateRiskService.UpdateRiskRatingFactors(riskId, _ratingFactorsRequestConverter.From(ratingFactorsRequest), true);

            if (updateRatingFactorsResponse.HasErrors)
            {
                ApplyCreateQuoteErrorsToModelState(updateRatingFactorsResponse.Errors);
                return new InvalidModelStateActionResult(ModelState);
            }

            _policyPremiumCalculation.CalculateAndSavePolicy(quoteReferenceNumber);

            var salesPortalSummary = _salesPortalSummaryProvider.GetFor(quoteReferenceNumber, riskId);
            return Ok(RatingFactorsResponse.From(ratingFactorsRequest, updateRatingFactorsResponse.IsRatingFactorsValidForInforce, salesPortalSummary));
        }

        [HttpGet, Route("premium")]
        public IHttpActionResult Premium(string quoteReferenceNumber, int riskId)
        {
            var policyPremiumSummary = _policyPremiumSummaryProvider.GetFor(quoteReferenceNumber);

            var riskPremium = _riskPremiumSummaryViewModelConverter.CreateFrom(riskId, policyPremiumSummary);

            if (riskPremium == null)
                return NotFound();

            return Ok(riskPremium);
        }

        [HttpGet, Route("partyConsent")]
        public IHttpActionResult PartyConsent(string quoteReferenceNumber, int riskId)
        {
            var model = _partyConsentProvider.GetFor(riskId);

            return Ok(model);
        }

        [HttpPost, Route("partyConsent")]
        public IHttpActionResult PartyConsent(string quoteReferenceNumber, int riskId, [FromBody]PartyConsentRequest partyConsentRequest)
        {
            if (!ModelState.IsValid)
            {
                return new InvalidModelStateActionResult(ModelState);
            }

            _updateRiskService.UpdateRiskConsent(_partyConsentRequestConverter.From(partyConsentRequest), riskId);

            return Ok();
        }

        private bool ApplyCreateQuoteErrorsToModelState(IEnumerable<ValidationError> errors)
        {
            const string dateOfBirthModelStateKey = "ratingFactorsRequest.DateOfBirth";
            const string residencyModelStateKey = "ratingFactorsRequest.AustralianResident";
            const string incomeModelStateKey = "ratingFactorsRequest.Income";

            foreach (var brokenRule in errors)
            {
                switch (brokenRule.Key)
                {
                    case ValidationKey.AustralianResidency:
                        ModelState.AddModelError(residencyModelStateKey, brokenRule.Message);
                        break;
                    case ValidationKey.MinimumAge:
                    case ValidationKey.MaximumAge:
                        ModelState.AddModelError(dateOfBirthModelStateKey, brokenRule.Message);
                        break;
                    case ValidationKey.AnnualIncome:
                        ModelState.AddModelError(incomeModelStateKey, brokenRule.Message);
                        break;
                }
            }

            return ModelState.IsValid;
        }
    }
}
