using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using TAL.QuoteAndApply.Customer.Web.Attributes;
using TAL.QuoteAndApply.Customer.Web.Converters;
using TAL.QuoteAndApply.Customer.Web.Models.Api;
using TAL.QuoteAndApply.Customer.Web.Models.View;
using TAL.QuoteAndApply.Customer.Web.Services;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.SalesPortal.Web.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Product.Rules;
using TAL.QuoteAndApply.ServiceLayer.Validation;
using TAL.QuoteAndApply.Web.Shared.HttpResults;
using TAL.QuoteAndApply.Web.Shared.Models.Api;
using TAL.QuoteAndApply.Web.Shared.Session;

namespace TAL.QuoteAndApply.Customer.Web.Controllers.Api
{
    [RoutePrefix("api/policy")]
    public class PolicyController : ApiController
    {
        private readonly ICreateQuoteService _createQuoteService;
        private readonly IQuoteSessionContext _quoteSessionContext;
        private readonly IQuoteParamConverter _quoteParamConverter;
        private readonly IPolicyInitialisationMetadataService _policyInitialisationMetadataService;
        private readonly IPolicyInitialisationMetadataProvider _policyInitialisationMetadataProvider;
        private readonly ICaptchaService _captchaService;
        private readonly IPostcodeService _postcodeService;

        public PolicyController(ICreateQuoteService createQuoteService, IQuoteSessionContext quoteSessionContext,
            IQuoteParamConverter quoteParamConverter,
            IPolicyInitialisationMetadataService policyInitialisationMetadataService, ICaptchaService captchaService,
            IPostcodeService postcodeService, IPolicyInitialisationMetadataProvider policyInitialisationMetadataProvider)
        {
            _createQuoteService = createQuoteService;
            _quoteSessionContext = quoteSessionContext;
            _quoteParamConverter = quoteParamConverter;
            _policyInitialisationMetadataService = policyInitialisationMetadataService;
            _captchaService = captchaService;
            _postcodeService = postcodeService;
            _policyInitialisationMetadataProvider = policyInitialisationMetadataProvider;
        }

        [HttpGet, Route("init")]
        public IHttpActionResult InitQuote()
        {
            return Ok(new BasicInfoViewModel());
        }

        [HttpPost, Route("validate")]
        public IHttpActionResult Validate(BasicInfoViewModel basicInfoViewModel)
        {
            if (!ModelState.IsValid)
            {
                return new InvalidModelStateActionResult(ModelState);
            }

            var createQuoteParam = _quoteParamConverter.FromBasicInfoViewModel(basicInfoViewModel);
            var errors = _createQuoteService.ValidateQuote(createQuoteParam);

            if (errors.Count > 0)
            {
                ApplyCreateQuoteErrorsToModelState(errors);
                return new InvalidModelStateActionResult(ModelState);
            }

            return Ok();
        }

        [HttpPost, Route("validate/income")]
        public IHttpActionResult ValidateIncome(IncomeViewModel vm)
        {
            if (!ModelState.IsValid || !vm.AnnualIncome.HasValue)
            {
                return new InvalidModelStateActionResult(ModelState);
            }

            var errors = _createQuoteService.ValidateIncome(vm.AnnualIncome.Value);

            if (errors.Count > 0)
            {
                ApplyCreateQuoteErrorsToModelState(errors);
                return new InvalidModelStateActionResult(ModelState);
            }

            return Ok();
        }

        [HttpPost, Route("validate/age")]
        public IHttpActionResult ValidateGeneralInformation(ValidateAgeViewModel basicInfoViewModel)
        {
            if (!ModelState.IsValid)
            {
                return new InvalidModelStateActionResult(ModelState);
            }

            var dob = _quoteParamConverter.FromDateString(basicInfoViewModel.DateOfBirth);
            var errors = _createQuoteService.ValidateAge(dob);

            if (errors.Count > 0)
            {
                ApplyCreateQuoteErrorsToModelState(errors);
                return new InvalidModelStateActionResult(ModelState);
            }

            return Ok();
        }

        [HttpPost, Route("validate/generalInformation")]
        public IHttpActionResult ValidateGeneralInformation(GeneralInformationViewModel basicInfoViewModel)
        {
            if (!_captchaService.Verify(basicInfoViewModel.RecaptchaResponse))
            {
                ModelState.AddModelError("RecaptchaResponse", "Please ensure you are not a robot");
            }

            if (!ModelState.IsValid)
            {
                return new InvalidModelStateActionResult(ModelState);
            }

            var dob = _quoteParamConverter.FromDateString(basicInfoViewModel.DateOfBirth);
            var errors = _createQuoteService.ValidateAge(dob);

            if (errors.Count > 0)
            {
                ApplyCreateQuoteErrorsToModelState(errors);
                return new InvalidModelStateActionResult(ModelState);
            }

            return Ok();
        }

        [HttpPost, Route("create")]
        public IHttpActionResult CreateQuoteFromBasicInfo(BasicInfoViewModel basicInfoViewModel)
        {
            if (!_captchaService.Verify(basicInfoViewModel.RecaptchaResponse))
            {
                ModelState.AddModelError("RecaptchaResponse", "Please ensure you are not a robot");
            }

            if (CreateQuote(basicInfoViewModel))
            {
                var redirectUrl = Url.Route("Default", new {Controller = "SelectCover", Action = "Index"});
                return new RedirectActionResult(redirectUrl);
            }

            return new InvalidModelStateActionResult(ModelState);
        }

        [HttpPost, Route("create-for-help-me-choose")]
        public IHttpActionResult CreateQuoteFromHelpMeChoose(BasicInfoViewModel basicInfoViewModel)
        {
            if (CreateQuote(basicInfoViewModel))
            {
                return Ok(_quoteSessionContext.QuoteSession.QuoteReference);
            }

            return new InvalidModelStateActionResult(ModelState);
        }

        private bool CreateQuote(BasicInfoViewModel basicInfoViewModel)
        {
            if (!ModelState.IsValid)
            {
                return false;
            }

            var createQuoteParam = _quoteParamConverter.FromBasicInfoViewModel(basicInfoViewModel);
            var createQuoteResult = _createQuoteService.CreateQuote(createQuoteParam);

            if (createQuoteResult.HasErrors)
            {
                ApplyCreateQuoteErrorsToModelState(createQuoteResult.Errors);
                return false;
            }

            _quoteSessionContext.Set(createQuoteResult.QuoteReference);
            return true;
        }


        [HttpPost, Route("set-calc-results")]
        public IHttpActionResult SetCalculatorResults(CalculatorResults results)
        {
            if (!ModelState.IsValid)
            {
                return new InvalidModelStateActionResult(ModelState);
            }

            var policyInitialisationMetadata =
                _policyInitialisationMetadataProvider.GetPolicyInitialisationMetadata(results);

            _policyInitialisationMetadataService.SetPolicyInitialisationMetadataForSession(policyInitialisationMetadata);

            return Ok();
        }

        [HttpPost, Route("use-calc-results")]
        public IHttpActionResult UseCalculatorResults()
        {
            var policyInitialisationMetaData = _policyInitialisationMetadataService.GetPolicyInitialisationMetadataForSession();
            if (policyInitialisationMetaData?.CalculatorResultsJson == null)
            {
                return NotFound();
            }

            policyInitialisationMetaData.SetResultsUsed();
            _policyInitialisationMetadataService.SetPolicyInitialisationMetadataForSession(policyInitialisationMetaData);

            return Ok();
        }

        private void ApplyCreateQuoteErrorsToModelState(IEnumerable<ValidationError> errors)
        {
            const string dateOfBirthModelStateKey = "basicInfoViewModel.DateOfBirth";
            const string incomeModelStateKey = "basicInfoViewModel.AnnualIncome";

            foreach (var brokenRule in errors)
            {
                switch (brokenRule.Key)
                {
                    case ValidationKey.MinimumAge:
                    case ValidationKey.MaximumAge:
                        ModelState.AddModelError(dateOfBirthModelStateKey, brokenRule.Message);
                        break;
                    case ValidationKey.AnnualIncome:
                        ModelState.AddModelError(incomeModelStateKey, brokenRule.Message);
                        break;
                }
            }

        }


    }
}