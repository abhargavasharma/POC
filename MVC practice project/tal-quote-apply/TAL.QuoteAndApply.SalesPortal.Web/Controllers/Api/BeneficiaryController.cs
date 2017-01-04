using System.Collections.Generic;
using System.Linq;
using System.Monads;
using System.Net.Http.Headers;
using System.Web.Http;
using TAL.QuoteAndApply.SalesPortal.Web.Attributes;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.SalesPortal.Web.Services;
using TAL.QuoteAndApply.SalesPortal.Web.Services.Converters;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Beneficiary;
using TAL.QuoteAndApply.Web.Shared.HttpResults;

namespace TAL.QuoteAndApply.SalesPortal.Web.Controllers.Api
{
    [WebApiSalesPortalSessionRequired]
    [RoutePrefix("api/policy/{quoteReferenceNumber}/risk/{riskId:int}/beneficiaries")]
    public class BeneficiaryController : ApiController
    {
        private readonly IBeneficiaryDetailsService _beneficiaryDetailsService;
        private readonly IBeneficiaryValidationServiceAdapter _beneficiaryValidationService;
        private readonly IBeneficiaryDetailsRequestConverter _modelMapper;
        private readonly IBrandAuthorizationService _brandAuthorizationService;

        public BeneficiaryController(IBeneficiaryDetailsService beneficiaryDetailsService,
            IBeneficiaryDetailsRequestConverter modelMapper, 
            IBeneficiaryValidationServiceAdapter beneficiaryValidationService,
            IBrandAuthorizationService brandAuthorizationService)
        {
            _beneficiaryDetailsService = beneficiaryDetailsService;
            _modelMapper = modelMapper;
            _beneficiaryValidationService = beneficiaryValidationService;
            _brandAuthorizationService = brandAuthorizationService;
        }

        [HttpGet, Route("")]
        public IHttpActionResult GetAllForRisk(string quoteReferenceNumber, int riskId)
        {
            if (!_brandAuthorizationService.CanAccess(quoteReferenceNumber)) return Unauthorized(new List<AuthenticationHeaderValue>());

            var beneficiaries = _beneficiaryDetailsService.GetBeneficiariesForRisk(riskId);
            var model = beneficiaries.Select(_modelMapper.From);
            return Ok(model);
        }

        [HttpGet, Route("{beneficiaryId:int}")]
        public IHttpActionResult GetSingleForRisk(string quoteReferenceNumber, int riskId, int beneficiaryId)
        {
            if (!_brandAuthorizationService.CanAccess(quoteReferenceNumber)) return Unauthorized(new List<AuthenticationHeaderValue>());

            var beneficiary = _beneficiaryDetailsService.GetBeneficiaryForRisk(riskId, beneficiaryId);
            var model = _modelMapper.From(beneficiary);
            return Ok(model);
        }

        [HttpPost, Route("")]
        public IHttpActionResult CreateOrUpdateBeneficiaries(string quoteReferenceNumber, int riskId,
            [FromBody] List<BeneficiaryDetailsRequest> beneficiaryDetailsRequest)
        {
            var result = _beneficiaryValidationService.ValidateBeneficiariesForSave(beneficiaryDetailsRequest.Select(_modelMapper.From).ToArray()).ToArray();
            UpdateModelState(result);

            if (!ModelState.IsValid)
            {
                return new InvalidModelStateActionResult(ModelState);
            }
            var retVal = beneficiaryDetailsRequest.Select(_modelMapper.From)
                .Select(beneficiary => _beneficiaryDetailsService.CreateOrUpdateBeneficiary(beneficiary, riskId))
                .Select(_modelMapper.From).ToList();

            retVal.ForEach(
                b => b.IsCompleted = result.FirstOrDefault(r => r.BeneficiaryId == b.Id).With(r => r.IsValid) && !b.HasEmptyFields());

            return Ok(retVal);
        }

        [HttpPost, Route("options")]
        public IHttpActionResult UpdateOptions(int riskId, BeneficiaryOptionsRequest options)
        {
            _beneficiaryDetailsService.UpdateLprForRisk(riskId, options.NominateLpr);
            return Ok();
        }

        [HttpGet, Route("options")]
        public IHttpActionResult GetOptions(int riskId)
        {
            var retVal = new BeneficiaryOptionsRequest
            {
                NominateLpr = _beneficiaryDetailsService.GetLprForRisk(riskId)
            };
            
            return Ok(retVal);
        }

        [HttpDelete, Route("{beneficiaryId:int}")]
        public IHttpActionResult RemoveBeneficiary(int riskId, int beneficiaryId)
        {
            if (!_brandAuthorizationService.CanAccess(riskId)) return Unauthorized(new List<AuthenticationHeaderValue>());

            _beneficiaryDetailsService.RemoveBeneficiary(riskId, beneficiaryId);
            return Ok();
        }

        [HttpGet, Route("validate")]
        public IHttpActionResult ValidationOfBeneficiaries(string quoteReferenceNumber, int riskId)
        {
            var result = _beneficiaryValidationService.ValidateBeneficiariesForInforceForRisk(riskId).ToArray();

            UpdateModelState(result);

            if (!ModelState.IsValid)
            {
                return new InvalidModelStateActionResult(ModelState);
            }

            return Ok();
        }
        
        private void UpdateModelState(ICollection<RiskBeneficiaryValidationModel> validations)
        {
            var result = validations.ToArray();
            for (var idx = 0; idx < result.Length; idx++)
            {
                var riskBeneficiaryValidationModel = result[idx];
                if (!riskBeneficiaryValidationModel.IsValid)
                {
                    foreach (var validationError in riskBeneficiaryValidationModel.ValidationErrors)
                    {
                        foreach (var messages in validationError.Messages)
                        {
                            ModelState.AddModelError(string.Format("beneficiaryDetailsRequest[{0}].{1}", idx, validationError.Key), messages);
                        }
                    }
                }
            }
        }
    }
}
