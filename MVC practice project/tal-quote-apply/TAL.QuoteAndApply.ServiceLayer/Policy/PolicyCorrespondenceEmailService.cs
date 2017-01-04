using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Notifications.Service;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Product.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Validation;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public interface IPolicyCorrespondenceEmailService
    {
        bool SendEmail(string quoteReferenceNumber, int riskId, PolicyCorrespondenceRequest policyCorrespondenceRequest, CorrespondenceEmailType correspondenceEmailType);
        SaveQuoteEmailRequest CreateSaveQuoteRequest(string quoteReferenceNumber, int riskId);
    }

    public class PolicyCorrespondenceEmailService : IPolicyCorrespondenceEmailService
    {
        private readonly IPolicyInteractionService _policyInteractionService;
        private readonly IEmailQuoteService _emailQuoteService;
        private readonly IPolicyCorrespondenceRequestConverter _policyCorrespondenceRequestConverter;
        private readonly IPolicyOverviewProvider _policyOverviewProvider;
        private readonly IPlanStateParamProvider _planStateParamProvider;
        private readonly IPlanService _planService;
        private readonly IRiskService _riskService;
        private readonly IActivePlanValidationService _activePlanValidationService;
        private readonly INameLookupService _nameLookupService;
        private readonly IProductBrandProvider _productBrandProvider;

        public PolicyCorrespondenceEmailService(IPolicyInteractionService policyInteractionService, 
            IEmailQuoteService emailQuoteService, 
            IPolicyCorrespondenceRequestConverter policyCorrespondenceRequestConverter, 
            IPolicyOverviewProvider policyOverviewProvider, 
            IPlanStateParamProvider planStateParamProvider, 
            IPlanService planService, 
            IRiskService riskService, 
            IActivePlanValidationService activePlanValidationService, 
            INameLookupService nameLookupService, 
            IProductBrandProvider productBrandProvider)
        {
            _policyInteractionService = policyInteractionService;
            _emailQuoteService = emailQuoteService;
            _policyCorrespondenceRequestConverter = policyCorrespondenceRequestConverter;
            _policyOverviewProvider = policyOverviewProvider;
            _planStateParamProvider = planStateParamProvider;
            _planService = planService;
            _riskService = riskService;
            _activePlanValidationService = activePlanValidationService;
            _nameLookupService = nameLookupService;
            _productBrandProvider = productBrandProvider;
        }

        public bool SendEmail(string quoteReferenceNumber, int riskId, PolicyCorrespondenceRequest policyCorrespondenceRequest, CorrespondenceEmailType correspondenceEmailType)
        {
            if (correspondenceEmailType == CorrespondenceEmailType.SaveQuote)
            {
                var emailSent = _emailQuoteService.SendSalesPortalQuoteSavedEmail(
                    _policyCorrespondenceRequestConverter.From(policyCorrespondenceRequest), quoteReferenceNumber);
                _policyInteractionService.QuoteEmailSentFromSalesPortal(quoteReferenceNumber);
                return emailSent;
            }
            if (correspondenceEmailType == CorrespondenceEmailType.ApplicationConfirmation)
            {
                var emailSent = _emailQuoteService.SendSalesPortalApplicationConfirmationEmail(
                    _policyCorrespondenceRequestConverter.From(policyCorrespondenceRequest), quoteReferenceNumber);
                _policyInteractionService.ApplicationConfirmationEmailSentFromSalesPortal(quoteReferenceNumber);
                return emailSent;
            }
            return false;
        }

        public SaveQuoteEmailRequest CreateSaveQuoteRequest(string quoteReferenceNumber, int riskId)
        {
            var brandKey = _productBrandProvider.GetBrandKeyForQuoteReferenceNumber(quoteReferenceNumber);

            var returnObj = new SaveQuoteEmailRequest();
            var errorsAndWarnings = new List<ValidationError>();

            var ownerOverview = _policyOverviewProvider.GetOwnerDetailsFor(quoteReferenceNumber);
            var risk = _riskService.GetRisk(riskId);
            var allPlans = _planService.GetPlansForRisk(riskId);
            var parentPlans = _planService.GetParentPlansFromAllPlans(allPlans);

            returnObj.ClientFirstName = ownerOverview.FirstName;
            if (!ownerOverview.FirstName.IsNullOrWhiteSpace() && !ownerOverview.Surname.IsNullOrWhiteSpace())
            {
                returnObj.ClientFullName = string.Concat(ownerOverview.FirstName.ToTitleCase(), " ",
                    ownerOverview.Surname.ToTitleCase());
            }
            returnObj.ClientEmailAddress = ownerOverview.EmailAddress;
            returnObj.PlanSummaries = new List<PlanCorrespondenceSummaryViewModel>();

            foreach (var plan in parentPlans)
            {
                if (plan.Selected)
                {
                    var planStateParam = _planStateParamProvider.CreateFrom(risk, plan, allPlans);

                    errorsAndWarnings = ValidatePlan(planStateParam, errorsAndWarnings);

                    var coverSummaries = new List<CoverCorrespondenceSummaryViewModel>();

                    foreach (var coverCode in planStateParam.SelectedCoverCodes)
                    {
                        coverSummaries.Add(new CoverCorrespondenceSummaryViewModel()
                        {
                            Name = _nameLookupService.GetCoverName(plan.Code, coverCode, brandKey)
                        });
                    }

                    returnObj.PlanSummaries.Add(new PlanCorrespondenceSummaryViewModel()
                    {
                        Name = _nameLookupService.GetPlanName(planStateParam.PlanCode, brandKey),
                        CoverAmount = plan.CoverAmount,
                        Premium = plan.Premium,
                        CoverCorrespondenceSummaries = coverSummaries
                    });
                }
            }

            returnObj.IsValidForEmailCorrespondence = !errorsAndWarnings.Any() 
                && !returnObj.ClientEmailAddress.IsNullOrWhiteSpace() 
                && !returnObj.ClientFirstName.IsNullOrWhiteSpace()
                && !ownerOverview.Surname.IsNullOrWhiteSpace();

            return returnObj;
        }

        private List<ValidationError> ValidatePlan(PlanStateParam planStateParam, List<ValidationError> errorsAndWarnings)
        {
            var validationResult = _activePlanValidationService.ValidateCurrentActivePlan(planStateParam);
            if (validationResult.Errors != null)
            {
                errorsAndWarnings.AddRange(validationResult.Errors.Select(ve => new ValidationError(ve.Code, ValidationKey.Submission, ve.Message, ValidationType.Error, ve.Location)));
            }
            if (validationResult.Warnings != null)
            {
                errorsAndWarnings.AddRange(validationResult.Warnings.Select(ve => new ValidationError(ve.Code, ValidationKey.Submission, ve.Message, ValidationType.Warning, ve.Location)));
            }
            return errorsAndWarnings;
        }
    }
}
