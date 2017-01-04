using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Infrastructure.Features;
using TAL.QuoteAndApply.Party.Models;
using TAL.QuoteAndApply.Payment.Service;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Rules.Risk;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.ServiceLayer.Payment.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy;
using TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Beneficiary;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Product.Models.Converters;
using TAL.QuoteAndApply.ServiceLayer.Validation;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    using TAL.QuoteAndApply.DataModel.Policy;

    public interface IPolicySubmissionValidationService
    {
        PolicySubmissionValidationResult ValidatePolicy(string quoteReference);
        PolicySubmissionValidationResult ValidatePolicy(RaisePolicy.Models.RaisePolicy policy);
    }

    public class PolicySubmissionValidationService : IPolicySubmissionValidationService
    {
        private readonly IRaisePolicyFactory _raisePolicyFactory;
        private readonly IRaisePolicyValidationService _raisePolicyValidationService;
        private readonly IRiskRulesService _riskRulesService;
        private readonly IBeneficiaryValidationService _beneficiaryValidationService;
        private readonly IRiskUnderwritingService _riskUnderwritingService;
        private readonly IActivePlanValidationService _activePlanValidationService;
        private readonly IFeatures _features;
        private readonly IProductDefinitionBuilder _productDefinitionBuilder;
        private readonly IRiskProductDefinitionConverter _riskProductDefinitionConverter;
        private readonly IPlanStateParamProvider _planStateParamProvider;
        private readonly IPlanService _planService;
        private readonly IPaymentRulesService _paymentRulesService;
        private readonly IRiskRulesFactory _riskRulesFactory;
        private readonly IProductBrandProvider _productBrandProvider;

        public PolicySubmissionValidationService(IRaisePolicyFactory raisePolicyFactory, 
			IRaisePolicyValidationService raisePolicyValidationService,
            IRiskRulesService riskRulesService, 
			IBeneficiaryValidationService beneficiaryValidationService,
            IProductDefinitionBuilder productDefinitionBuilder,
            IRiskProductDefinitionConverter riskProductDefinitionConverter, 
            IRiskUnderwritingService riskUnderwritingService, 
            IActivePlanValidationService activePlanValidationService,
            IFeatures features, 
            IPlanStateParamProvider planStateParamProvider,
            IPlanService planService, 
            IPaymentRulesService paymentRulesService, 
			IRiskRulesFactory riskRulesFactory, 
			IProductBrandProvider productBrandProvider)
        {
            _raisePolicyFactory = raisePolicyFactory;
            _riskRulesService = riskRulesService;
            _beneficiaryValidationService = beneficiaryValidationService;
            _productDefinitionBuilder = productDefinitionBuilder;
            _riskProductDefinitionConverter = riskProductDefinitionConverter;
            _riskUnderwritingService = riskUnderwritingService;
            _activePlanValidationService = activePlanValidationService;
            _features = features;
            _planStateParamProvider = planStateParamProvider;
            _planService = planService;
            _paymentRulesService = paymentRulesService;
            _riskRulesFactory = riskRulesFactory;
			_raisePolicyValidationService = raisePolicyValidationService;
			_productBrandProvider = productBrandProvider;
        }

        public PolicySubmissionValidationResult ValidatePolicy(string quoteReference)
        {
            var uberPolicy = _raisePolicyFactory.GetFromQuoteReference(quoteReference);
            var returnPolicy = ValidatePolicy(uberPolicy);
            return returnPolicy;
        }

        public PolicySubmissionValidationResult ValidatePolicy(RaisePolicy.Models.RaisePolicy policy)
        {
            var riskResults = new List<RiskSubmissionResult>();
            var ownerType = policy.Owner.OwnerType;
            
            foreach (var risk in policy.Risks)
            {
                var sectionResults = new List<SectionSubmissionResult>();

                sectionResults.Add(ValidatePersonalDetails(risk, policy.Owner));
                sectionResults.Add(ValidateRatingFactors(risk));
                sectionResults.Add(ValidateBeneficiaries(risk));

                //todo: remove this feature toggle. Only here because UW is still in progress
                if (_features.ValidateUnderwritingForPolicySubmission)
                {
                    sectionResults.Add(ValidateUnderwriting(risk));
                }

                sectionResults.Add(ValidatePlansAndCovers(risk));
                sectionResults.Add(ValidatePayments(policy.Payment, ownerType));

                riskResults.Add(new RiskSubmissionResult(risk.Id, sectionResults));
            }

            return new PolicySubmissionValidationResult(riskResults);
        }

        private SectionSubmissionResult ValidatePayments(RaisePolicyPayment payment, PolicyOwnerType ownerType)
        {
            var errors = new List<ValidationError>();
            var warnings = new List<ValidationError>();
            var errorSection = PolicySection.Payment;

            if (payment.PaymentType == PaymentType.CreditCard && ownerType!= PolicyOwnerType.SelfManagedSuperFund)
            {
                var validations = _paymentRulesService.ValidateCreditCardForInforce(payment.CreditCard);
                errors.AddRange(validations.Where(rr => !rr.IsSatisfied).Select(rr => new ValidationError(rr.Key, ValidationKey.Submission, rr.DefaultMessage(), ValidationType.Error, rr.Key)));
            }
            else if (payment.PaymentType == PaymentType.DirectDebit && ownerType != PolicyOwnerType.SelfManagedSuperFund)
            {
                var validations = _paymentRulesService.ValidateDirectDebitForInforce(payment.DirectDebit);
                errors.AddRange(validations.Where(rr => !rr.IsSatisfied).Select(rr => new ValidationError(rr.Key, ValidationKey.Submission, rr.DefaultMessage(), ValidationType.Error, rr.Key)));
            }else if (payment.PaymentType == PaymentType.SelfManagedSuperFund && ownerType == PolicyOwnerType.SelfManagedSuperFund)
            {
                var validations = _paymentRulesService.ValidateSelfManagedSuperFundForInforce(payment.SelfManagedSuperFund);
                errors.AddRange(validations.Where(rr => !rr.IsSatisfied).Select(rr => new ValidationError(rr.Key, ValidationKey.Submission, rr.DefaultMessage(), ValidationType.Error, rr.Key)));
            }else if ((payment.PaymentType != PaymentType.SelfManagedSuperFund && ownerType == PolicyOwnerType.SelfManagedSuperFund) || 
                      (payment.PaymentType==PaymentType.SelfManagedSuperFund && ownerType!= PolicyOwnerType.SelfManagedSuperFund))
            {
                errors.Add(new ValidationError("InvalidPaymentMethod", ValidationKey.Submission, "Invalid payment type selected for SMSF ownership", ValidationType.Error, "Payment"));
                errorSection = PolicySection.PaymentType;
            }
            else
            {
                errors.Add(new ValidationError("InvalidPayment", ValidationKey.Submission, "Invalid payment type selected", ValidationType.Error, "Payment"));

            }

            return new SectionSubmissionResult(errorSection, errors, warnings);
        }

        //todo this function is a bit of hacked together job and duplicates most of the PolicyRiskPlanStatusProvider, think about calling that instead
        private SectionSubmissionResult ValidatePlansAndCovers(RaisePolicyRisk risk)
        {
            var errors = new List<ValidationError>();
            var warnings = new List<ValidationError>();

            //reget get allplans because RaisePolicyRisk.Plans only has selected plans and is not all properties are mapping. 
            //Bit of a hack but works for now and causes less risk than change to include all plans
            var allPlans = _planService.GetPlansForRisk(risk.Id);

            if (!_riskRulesFactory.GetAtLeastOnePlanMustBeSelectedRule("RiskPlans").IsSatisfiedBy(allPlans))
            {
                errors.Add(new ValidationError("RiskPlans", ValidationKey.Submission, "At least one plan must be selected", ValidationType.Error, "RiskPlans"));
            }

            var parentPlans = _planService.GetParentPlansFromAllPlans(allPlans);

            foreach (var plan in parentPlans)
            {
                if (plan.Selected)
                {
                    var planStateParam = _planStateParamProvider.CreateFrom(risk, plan, allPlans);

                    var validationResult = _activePlanValidationService.ValidateCurrentActivePlan(planStateParam);
                    if (validationResult.Errors != null)
                    {
                        errors.AddRange(validationResult.Errors.Select(ve => new ValidationError(ve.Code, ValidationKey.Submission, ve.Message, ValidationType.Error, ve.Location)));
                    }
                    if (validationResult.Warnings != null)
                    {
                        warnings.AddRange(validationResult.Warnings.Select(ve => new ValidationError(ve.Code, ValidationKey.Submission, ve.Message, ValidationType.Warning, ve.Location)));
                    }
                }
            }

            return new SectionSubmissionResult(PolicySection.Quote, errors, warnings);
        }

        private SectionSubmissionResult ValidateUnderwriting(IRisk risk)
        {
            var underwritingStatus = _riskUnderwritingService.GetRiskUnderwritingStatus(risk.Id);

            var errors = new List<ValidationError>();
            var warnings = new List<ValidationError>();

            foreach (var uw in underwritingStatus.CoverUnderwritingCompleteResults.Where(u=> !u.IsUnderwritingComplete))
            {
                if (uw.ValidationType == ValidationType.Error)
                {
                    errors.Add(new ValidationError(uw.CoverCode, ValidationKey.Submission, "Underwriting is declined", ValidationType.Error, uw.CoverCode));
                }
                if (uw.ValidationType == ValidationType.Warning)
                {
                    warnings.Add(new ValidationError(uw.CoverCode, ValidationKey.Submission, "Underwriting is declined", ValidationType.Warning, uw.CoverCode));
                }
            }

            return new SectionSubmissionResult(PolicySection.Underwriting, errors, warnings);
        }

        private SectionSubmissionResult ValidateBeneficiaries(RaisePolicyRisk risk)
        {
            var errors = new List<ValidationError>();

            var validationErrors = _beneficiaryValidationService.ValidateBeneficiariesForInforceForRisk(risk.Id);

            errors.AddRange(
                validationErrors.SelectMany(benValidationErrors => benValidationErrors.ValidationErrors.Select(
                    rr => new ValidationError(rr.Key, ValidationKey.Submission, string.Join(", ", rr.Messages), ValidationType.Error, rr.Key)))
                );

            return new SectionSubmissionResult(PolicySection.Beneficiaries, errors, null);
        }

        private SectionSubmissionResult ValidatePersonalDetails(IParty party, RaisePolicyOwner owner)
        {
            var errors = new List<ValidationError>();
            
            var ownerValidationErrors = _raisePolicyValidationService.ValidateOwnerForInforce(owner);
            var ownerErrors = ownerValidationErrors.Where(rr => !rr.IsSatisfied)
                            .Select(rr => new ValidationError(rr.Key, ValidationKey.Submission, rr.DefaultMessage(), ValidationType.Error, rr.Key));

            errors.AddRange(ownerErrors);

            //if the ownership is SMSF then the life insured details are separate:
            if (owner.OwnerType == PolicyOwnerType.SelfManagedSuperFund)
            {
                var lifeInsuredValidationErrors = _raisePolicyValidationService.ValidatePartyForInforce(party);
                
                var lifeInsuredErrors = lifeInsuredValidationErrors.Where(rr => !rr.IsSatisfied)
                            .Select(rr => new ValidationError(rr.Key, ValidationKey.Submission, rr.DefaultMessage(), ValidationType.Error, rr.Key));
                
                
                errors.AddRange(lifeInsuredErrors);
            }

            return new SectionSubmissionResult(PolicySection.PersonalDetails, errors, null);
        }

        private SectionSubmissionResult ValidateRatingFactors(IRisk risk)
        {
            var brandKey = _productBrandProvider.GetBrandKeyForRisk(risk);
            var productDefinition = _productDefinitionBuilder.BuildProductDefinition(brandKey);
            var riskProductDefinition = _riskProductDefinitionConverter.CreateFrom(productDefinition);

            var validationErrors = _riskRulesService.ValidateRiskForInforce(riskProductDefinition, risk);
            var errors = validationErrors.Where(rr => !rr.IsSatisfied).Select(rr => new ValidationError(rr.Key, ValidationKey.Submission, rr.DefaultMessage(), ValidationType.Error, rr.Key));

            return new SectionSubmissionResult(PolicySection.RatingFactors, errors, null);
        }
    }
}
