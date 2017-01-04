using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Web.Http;
using TAL.QuoteAndApply.DataModel.Service;
using TAL.QuoteAndApply.SalesPortal.Web.Attributes;
using TAL.QuoteAndApply.SalesPortal.Web.Models;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.SalesPortal.Web.Services;
using TAL.QuoteAndApply.SalesPortal.Web.Services.Converters;
using TAL.QuoteAndApply.SalesPortal.Web.User;
using TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy;
using TAL.QuoteAndApply.ServiceLayer.Validation;
using TAL.QuoteAndApply.Web.Shared.HttpResults;

namespace TAL.QuoteAndApply.SalesPortal.Web.Controllers.Api
{
    [WebApiSalesPortalSessionRequired]
    [RoutePrefix("api/policy")]
    public class PolicyController : ApiController
    {
        private readonly ICreateQuoteService _createQuoteService;
        private readonly ICreateClientRequestConverter _createClientRequestConverter;
        private readonly IRaisePolicyService _raisePolicyService;
        private readonly IPolicySatusProvider _policySatusProvider;
        private readonly IPolicyStatusViewModelConverter _policyStatusViewModelConverter;
        private readonly IPolicyPremiumFrequencyProvider _policyPremiumFrequencyProvider;
        private readonly IPolicyPremiumFrequencyViewModelConverter _policyPremiumFrequencyViewModelConverter;
        private readonly IUpdatePolicyService _updatePolicyService;
        private readonly IPolicyOverviewProvider _policyOverviewProvider;
        private readonly IRetrievePolicyViewModelConverter _retrievePolicyViewModelConverter;
        private readonly IEditPolicyPermissionsService _editPolicyPermissionsService;
        private readonly ISalesPortalSessionContext _salesPortalSessionContext;
        private readonly IPolicyProgressProvider _policyProgressProvider;
        private readonly IPolicyProgressViewModelConverter _policyProgressViewModelConverter;
        private readonly ISetInforceResponseConverter _setInforceResponseConverter;
        private readonly ISalesPortalPolicyRetrievalService _salesPortalPolicyRetrievalService;
        private readonly IPolicyCorrespondenceService _policyCorrespondenceService;
        private readonly IPolicyOwnerDetailsConverter _policyOwnerDetailsConverter;
        private readonly IRaisePolicyOwnershipValidationService _raisePolicyOwnershipValidationService;
        private readonly IBrandAuthorizationService _brandAuthorizationService;

        public PolicyController(ICreateQuoteService createQuoteService,
            ICreateClientRequestConverter createClientRequestConverter,
            IRaisePolicyService raisePolicyService,
            IPolicySatusProvider policySatusProvider,
            IPolicyStatusViewModelConverter policyStatusViewModelConverter,
            IPolicyPremiumFrequencyProvider policyPremiumFrequencyProvider,
            IPolicyPremiumFrequencyViewModelConverter policyPremiumFrequencyViewModelConverter,
            IUpdatePolicyService updatePolicyService, 
            IPolicyOverviewProvider policyOverviewProvider,
            IRetrievePolicyViewModelConverter retrievePolicyViewModelConverter, 
            IEditPolicyPermissionsService editPolicyPermissionsService, 
            ISalesPortalSessionContext salesPortalSessionContext, 
            IPolicyProgressProvider policyProgressProvider, 
            IPolicyProgressViewModelConverter policyProgressViewModelConverter, 
            ISetInforceResponseConverter setInforceResponseConverter, 
            ISalesPortalPolicyRetrievalService salesPortalPolicyRetrievalService, 
            IPolicyCorrespondenceService policyCorrespondenceService, 
            IPolicyOwnerDetailsConverter policyOwnerDetailsConverter,
            IRaisePolicyOwnershipValidationService raisePolicyOwnershipValidationService, 
            IBrandAuthorizationService brandAuthorizationService)
        {
            _createQuoteService = createQuoteService;
            _createClientRequestConverter = createClientRequestConverter;
            _raisePolicyService = raisePolicyService;
            _policySatusProvider = policySatusProvider;
            _policyStatusViewModelConverter = policyStatusViewModelConverter;
            _policyPremiumFrequencyProvider = policyPremiumFrequencyProvider;
            _policyPremiumFrequencyViewModelConverter = policyPremiumFrequencyViewModelConverter;
            _updatePolicyService = updatePolicyService;
            _policyOverviewProvider = policyOverviewProvider;
            _retrievePolicyViewModelConverter = retrievePolicyViewModelConverter;
            _editPolicyPermissionsService = editPolicyPermissionsService;
            _salesPortalSessionContext = salesPortalSessionContext;
            _policyProgressProvider = policyProgressProvider;
            _policyProgressViewModelConverter = policyProgressViewModelConverter;
            _setInforceResponseConverter = setInforceResponseConverter;
            _salesPortalPolicyRetrievalService = salesPortalPolicyRetrievalService;
            _policyCorrespondenceService = policyCorrespondenceService;
            _policyOwnerDetailsConverter = policyOwnerDetailsConverter;
           _raisePolicyOwnershipValidationService = raisePolicyOwnershipValidationService;
            _brandAuthorizationService = brandAuthorizationService;
        }

        [HttpGet, Route("init")]
        public IHttpActionResult Init()
        {
            var createClientRequest = new CreateClientRequest();
            return Ok(createClientRequest);
        }

        [HttpPost, Route("create")]
        public IHttpActionResult Create(CreateClientRequest createClientRequest)
        {
            if (!ModelState.IsValid)
            {
                return new InvalidModelStateActionResult(ModelState);
            }

            var createQuoteParam = _createClientRequestConverter.From(createClientRequest, _salesPortalSessionContext.SalesPortalSession.SelectedBrand);
            var createQuoteResponse = _createQuoteService.CreateQuote(createQuoteParam);

            if (createQuoteResponse.HasErrors)
            {
                ApplyCreateQuoteErrorsToModelState(createQuoteResponse.Errors);
                return new InvalidModelStateActionResult(ModelState);
            }
            
            var quoteRefNo = createQuoteResponse.QuoteReference;
            var redirectUrl = Url.Route("Default",
                new {Controller = "Policy", Action = "Edit", id = quoteRefNo, created = true});

            return
                new RedirectActionResult(redirectUrl);
        }

        [HttpPost, Route("{policyNo}/inforce")]
        public IHttpActionResult SetPolicyToInForce(string policyNo)
        {
            if (!_brandAuthorizationService.CanAccess(policyNo)) return Unauthorized(new List<AuthenticationHeaderValue>());

            var result = _raisePolicyService.PostPolicy(policyNo);
            if (result.SubmittedSuccessfully)
            {
                _policyCorrespondenceService.SendCorrespondence(policyNo, CorrespondenceEmailType.ApplicationConfirmation);
            }
            var setInforceResponse = _setInforceResponseConverter.From(result);
           
            return Ok(setInforceResponse);
        }

        [HttpGet, Route("{policyNo}/status")]
        public IHttpActionResult GetStatusOfPolicy(string policyNo)
        {
            var statusObj = _policySatusProvider.GetStatus(policyNo);
            var retModel = _policyStatusViewModelConverter.From(statusObj);
            return Ok(retModel);
        }

        [HttpGet, Route("{policyNo}/premiumfrequency")]
        public IHttpActionResult GetPolicyPremiumFrequency(string policyNo)
        {
            var premiumFrequencyObj = _policyPremiumFrequencyProvider.GetPremiumFrequency(policyNo);
            var retModel = _policyPremiumFrequencyViewModelConverter.From(premiumFrequencyObj);
            return Ok(retModel);
        }

        [HttpPost, Route("{policyNo}/premiumfrequency")]
        public IHttpActionResult UpdatePolicyPremiumFrequency(string policyNo, PolicyFremiumFrequencyViewModel policyFremiumFrequencyViewModel)
        {
            if (!_brandAuthorizationService.CanAccess(policyNo)) return Unauthorized(new List<AuthenticationHeaderValue>());

            if (!ModelState.IsValid)
            {
                return new InvalidModelStateActionResult(ModelState);
            }

            _updatePolicyService.UpdatePremiumFrequency(policyNo, policyFremiumFrequencyViewModel.PremiumFrequency.MapToPremiumFrequency());

            return Ok(policyFremiumFrequencyViewModel);
        }

        [HttpGet, Route("{policyNo}/summary")]
        public IHttpActionResult GetPolicyOverview(string policyNo)
        {
            if (!_brandAuthorizationService.CanAccess(policyNo)) return Unauthorized(new List<AuthenticationHeaderValue>());

            var policyOverview = _policyOverviewProvider.GetFor(policyNo);
            var editPolicyPermissions = _editPolicyPermissionsService.GetPermissionsFor(policyOverview.Status,
                _salesPortalSessionContext.SalesPortalSession.Roles);

            var model = _retrievePolicyViewModelConverter.CreateFrom(policyOverview, editPolicyPermissions);

            return Ok(model);
        }

        [HttpGet, Route("{policyNo}/edit/{editAction}")]
        public IHttpActionResult Edit(string policyNo, string editAction)
        {
            if (!_brandAuthorizationService.CanAccess(policyNo)) return Unauthorized(new List<AuthenticationHeaderValue>());

            var quoteEditSource = (QuoteEditSource)Enum.Parse(typeof (QuoteEditSource), editAction);

            var model = _salesPortalPolicyRetrievalService.RetrieveQuote(policyNo, quoteEditSource);
            
            return Ok(model);
        }

        [HttpGet, Route("{quoteReferenceNumber}/progress")]
        public IHttpActionResult GetPolicyProgress(string quoteReferenceNumber)
        {
            var progressObj = _policyProgressProvider.GetProgress(quoteReferenceNumber);
            var retModel = _policyProgressViewModelConverter.From(progressObj);
            return Ok(retModel);
        }

        [HttpPost, Route("{quoteReferenceNumber}/progress")]
        public IHttpActionResult UpdatePolicyProgress(string quoteReferenceNumber, PolicyProgressViewModel policyProgressViewModel)
        {
            if (!_brandAuthorizationService.CanAccess(quoteReferenceNumber)) return Unauthorized(new List<AuthenticationHeaderValue>());

            if (!ModelState.IsValid)
            {
                return new InvalidModelStateActionResult(ModelState);
            }

            _updatePolicyService.UpdateProgress(quoteReferenceNumber, policyProgressViewModel.Progress.MapToProgress());

            return Ok(policyProgressViewModel);
        }

        [HttpPost, Route("{quoteReferenceNumber}/ownerType/{newOwnerType}")]
        public IHttpActionResult UpdatePolicyOwnerType(string quoteReferenceNumber, string newOwnerType)
        {
            if (!_brandAuthorizationService.CanAccess(quoteReferenceNumber)) return Unauthorized(new List<AuthenticationHeaderValue>());

            if (!ModelState.IsValid)
            {
                return new InvalidModelStateActionResult(ModelState);
            }
            
            _updatePolicyService.UpdateOwnerType(quoteReferenceNumber, newOwnerType.MapToOwnerType());

            var model= _policyOverviewProvider.GetOwnerDetailsFor(quoteReferenceNumber);

            model.IsCompleted = _raisePolicyOwnershipValidationService.IsCompleted(quoteReferenceNumber);

            return Ok(model);
        }

        [HttpGet, Route("{quoteReferenceNumber}/ownerDetails")]
        public IHttpActionResult GetPolicyOwnerDetails(string quoteReferenceNumber)
        {
            if (!_brandAuthorizationService.CanAccess(quoteReferenceNumber)) return Unauthorized(new List<AuthenticationHeaderValue>());

            var owner = _policyOverviewProvider.GetOwnerDetailsFor(quoteReferenceNumber);
            var model = _policyOwnerDetailsConverter.From(owner);
            model.IsCompleted = _raisePolicyOwnershipValidationService.IsCompleted(quoteReferenceNumber);

            return Ok(model);
        }

        [HttpPost, Route("{quoteReferenceNumber}/ownerDetails")]
        public IHttpActionResult UpdatePolicyOwnerDetails(string quoteReferenceNumber, [FromBody] PolicyOwnerDetailsRequest ownerDetailsRequest)
        {
            if (!ModelState.IsValid)
            {
                return new InvalidModelStateActionResult(ModelState);
            }

            //update party associated with the owner:
            var owner = _policyOwnerDetailsConverter.From(ownerDetailsRequest);
            _updatePolicyService.UpdateOwnerPartyDetails(quoteReferenceNumber, owner);

            var ownerModel= _policyOverviewProvider.GetOwnerDetailsFor(quoteReferenceNumber);
            ownerModel.IsCompleted = _raisePolicyOwnershipValidationService.IsCompleted(quoteReferenceNumber);

            return Ok(ownerModel);
        }

        private void ApplyCreateQuoteErrorsToModelState(IEnumerable<ValidationError> errors)
        {
            const string dateOfBirthModelStateKey = "createClientRequest.PolicyOwner.RatingFactors.DateOfBirth";
            const string residencyModelStateKey = "createClientRequest.PolicyOwner.RatingFactors.AustralianResident";
            const string incomeModelStateKey = "createClientRequest.PolicyOwner.RatingFactors.Income";
            const string adobeLeadModelStateKey = "createClientRequest.PolicyOwner.RatingFactors.AdobeLead";

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
                    case ValidationKey.InvalidAdobeLeadData:
                        ModelState.AddModelError(adobeLeadModelStateKey, brokenRule.Message);
                        break;
                }
            }
        }
    }
}
