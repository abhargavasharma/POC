using System;
using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Party.Leads;
using TAL.QuoteAndApply.Party.Services;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.ServiceLayer.Policy.MarketingStatus;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.ServiceLayer.Policy.PlanSelection;
using TAL.QuoteAndApply.ServiceLayer.Policy.PremiumCalculation;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Product.Rules;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Events;
using TAL.QuoteAndApply.ServiceLayer.Validation;
using TAL.QuoteAndApply.Underwriting.Models.Event;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public interface ICreateQuoteService
    {
        IList<ValidationError> ValidateQuote(CreateQuoteParam createQuoteParam);
        CreateQuoteResult CreateQuote(CreateQuoteParam createQuoteParam);
        IList<ValidationError> ValidateIncome(long income);
        IList<ValidationError> ValidateAge(DateTime dob);
    }

    public class CreateQuoteService : ICreateQuoteService
    {
        private readonly IProductRulesService _productRulesService;
        private readonly IPartyRulesService _partyRulesService;
        private readonly IPartyService _partyService;
        private readonly ICreatePolicyService _createPolicyService;
        private readonly ICreateRiskService _createRiskService;
        private readonly IProductDefinitionProvider _productDefinitionBuilder;
        private readonly ICreatePlanService _createPlanService;
        private readonly IPolicyPremiumCalculation _policyPremiumCalculation;
        private readonly IPolicyAnalyticsIdentifierService _policyAnalyticsIdentifierService;
        private readonly IPolicyInitialisationMetadataService _policyInitialisationMetadataService;
        private readonly IAnalyticsIdentifiersConverter _analyticsIdentifiersConverter;
        private readonly IPolicyOwnerService _policyOwnerService;
        private readonly IPlanOptionsParamService _planOptionsParamService;
        private readonly IPlanDefaultsProvider _planDefaultsProvider;
        private readonly IPolicyDefaultsProvider _policyDefaultsProvider;
        private readonly IPlanSelectionAndConfigurationServiceFactory _configurationServiceFactory;
        private readonly IPartyConsentService _partyConsentService;
        private readonly IPartyConsentDtoConverter _partyConsentDtoConverter;
        private readonly IUnderwritingBenefitsResponseChangeObserver _coverUnderwritingSyncService;
        private readonly IPlanAutoUpdateService _planAutoUpdateService;
        private readonly ICreateNewPartyDtoService _createNewPartyDtoService;
        private readonly IPartyConsentDtoUpdater _partyConsentDtoUpdater;
        private readonly IPolicySourceConverter _policySourceConverter;
        private readonly IPolicyInteractionService _policyInteractionService;
        private readonly IGetLeadService _getLeadService;
        private readonly IUpdatePartyWithLeadService _updatePartyWithLeadService;
        private readonly ICurrentProductBrandProvider _currentProductBrandProvider;


        public CreateQuoteService(IProductRulesService productRulesService,
            IPartyRulesService partyRulesService,
            IPartyService partyService,
            ICreatePolicyService createPolicyService,
            ICreateRiskService createRiskService,
            IProductDefinitionProvider productDefinitionBuilder,
            ICreatePlanService createPlanService,
            IPolicyPremiumCalculation policyPremiumCalculation,
            IPolicyAnalyticsIdentifierService policyAnalyticsIdentifierService,
            IPolicyInitialisationMetadataService policyInitialisationMetadataService,
            IAnalyticsIdentifiersConverter analyticsIdentifiersConverter,
            IPolicyOwnerService policyOwnerService,
            IPlanOptionsParamService planOptionsParamService,
            IPlanDefaultsProvider planDefaultsProvider,
            IPolicyDefaultsProvider policyDefaultsProvider,
            IPlanSelectionAndConfigurationServiceFactory configurationServiceFactory,
            IPartyConsentService partyConsentService,
            IPartyConsentDtoConverter partyConsentDtoConverter,
            IUnderwritingBenefitsResponseChangeObserver coverUnderwritingSyncService,
            IPlanAutoUpdateService planAutoUpdateService, 
            ICreateNewPartyDtoService createNewPartyDtoService, 
            IGetLeadService getLeadService, 
            IPartyConsentDtoUpdater partyConsentDtoUpdater, 
            IPolicySourceConverter policySourceConverter, 
            IPolicyInteractionService policyInteractionService, 
            IUpdatePartyWithLeadService updatePartyWithLeadService, ICurrentProductBrandProvider currentProductBrandProvider)
        {
            _productRulesService = productRulesService;
            _partyRulesService = partyRulesService;
            _partyService = partyService;
            _createPolicyService = createPolicyService;
            _createRiskService = createRiskService;
            _productDefinitionBuilder = productDefinitionBuilder;
            _createPlanService = createPlanService;
            _policyPremiumCalculation = policyPremiumCalculation;
            _policyAnalyticsIdentifierService = policyAnalyticsIdentifierService;
            _policyInitialisationMetadataService = policyInitialisationMetadataService;
            _analyticsIdentifiersConverter = analyticsIdentifiersConverter;
            _policyOwnerService = policyOwnerService;
            _planOptionsParamService = planOptionsParamService;
            _planDefaultsProvider = planDefaultsProvider;
            _policyDefaultsProvider = policyDefaultsProvider;
            _configurationServiceFactory = configurationServiceFactory;
            _partyConsentService = partyConsentService;
            _partyConsentDtoConverter = partyConsentDtoConverter;
            _coverUnderwritingSyncService = coverUnderwritingSyncService;
            _planAutoUpdateService = planAutoUpdateService;
            _createNewPartyDtoService = createNewPartyDtoService;
            _getLeadService = getLeadService;
            _partyConsentDtoUpdater = partyConsentDtoUpdater;
            _policySourceConverter = policySourceConverter;
            _policyInteractionService = policyInteractionService;
            _updatePartyWithLeadService = updatePartyWithLeadService;
            _currentProductBrandProvider = currentProductBrandProvider;
        }

        public IList<ValidationError> ValidateQuote(CreateQuoteParam createQuoteParam)
        {
            var brokenRules = _productRulesService.ValidateRatingFactors(createQuoteParam.RatingFactors, createQuoteParam.ValidateResidency);

            return brokenRules.ToList();
        }

        public IList<ValidationError> ValidateIncome(long income)
        {
            var brokenRules = _productRulesService.ValidateIncome(income);

            return brokenRules.ToList();
        }

        public IList<ValidationError> ValidateAge(DateTime dob)
        {
            var brokenRules = _productRulesService.ValidateAge(dob);

            return brokenRules.ToList();
        }

        public CreateQuoteResult CreateQuote(CreateQuoteParam createQuoteParam)
        {
            var brokenRules = _productRulesService.ValidateRatingFactors(createQuoteParam.RatingFactors, createQuoteParam.ValidateResidency);

            if (brokenRules.Any())
            {
                return new CreateQuoteResult(brokenRules);
            }

            var policySource = _policySourceConverter.ToDataModel(createQuoteParam.Source);

            var partyDto = _createNewPartyDtoService.Create(createQuoteParam);

            //validate party
            var partyRules = _partyRulesService.ValidatePartyForSave(partyDto);

            if (partyRules.Any(x => x.IsBroken))
            {
                //todo: ViewModel validaiton should cover this, I just want to see if we ever end up here - yip, we can end up here with dirty adobe lead data!
                var brokenPartyValidationRules = new []
                {
                    new ValidationError(null, ValidationKey.InvalidAdobeLeadData,
                        "Adobe lead data failed sales portal validation.", ValidationType.Error, null)
                };
                return new CreateQuoteResult(brokenPartyValidationRules);
            }

            //create party
            partyDto = _partyService.CreateParty(partyDto, policySource);
            var partyConsentDto = _partyConsentDtoConverter.CreateFrom(partyDto.Id);
            //if adobe lead exists update current party
            if (createQuoteParam.PersonalInformation?.LeadId != null && createQuoteParam.PersonalInformation.LeadId != 0)
            {
                partyConsentDto = _partyConsentDtoUpdater.UpdateFrom(partyConsentDto,
                        _getLeadService.GetCommunicationPreferences(createQuoteParam.PersonalInformation.LeadId.Value));
            }
            _partyConsentService.CreatePartyConsent(partyConsentDto, partyDto);

            //create policy
            var currentBrand = _currentProductBrandProvider.GetCurrent();
            var policyDto = _createPolicyService.CreatePolicy(_policyDefaultsProvider.GetPolicyDefaults(currentBrand.Id, currentBrand.BrandCode, currentBrand.DefaultOrganisationId), policySource);

            _policyInteractionService.PolicyCreatedByPortal(createQuoteParam.Source, policyDto.QuoteReference);

            SetAnalyticsIdentifiers(policyDto);
            SetPolicyOwner(policyDto.Id, partyDto.Id);

            //create risk
            var createRiskResult = _createRiskService.CreateRisk(policyDto, partyDto, createQuoteParam.RatingFactors);
            var riskDto = createRiskResult.Risk;

            
            /*
             * TODO: revisit this logic as it could do with some attention
             * The next steps below we currently do this:
             *  1. create the plan/cover stucture for the policy in the database (setting some defaults)
             *  2. set some more defaults via configuration service
             *  3. update the set defaults to turn off anything that is ineligible
             *  
             *  Possible things to think about for refactoring are:
             *  - when creating plan structure in db, maybe don't set any defaults and move all default logic into configuration service
             *  - incorporate eligibility into config service for possible efficiency? not necessary but worth thinking about if it makes sense
             * */

            //create plans
            var productDefinitionPlans = _productDefinitionBuilder.GetProductDefinition(currentBrand.BrandCode).Plans;

            foreach (var planResponse in productDefinitionPlans)
            {
                var planDefaults = _planDefaultsProvider.GetPlanDefaultsByCode(planResponse.Code);
                var planOptionsParam = _planOptionsParamService.BuildFrom(policyDto.Id, policyDto.BrandKey, riskDto.Id, planResponse,
                    planDefaults);

                var createdPlan = _createPlanService.CreatePlan(planOptionsParam);
                foreach (var rider in planResponse.Riders)
                {
                    planDefaults = _planDefaultsProvider.GetPlanDefaultsByCode(rider.Code);
                    planOptionsParam = _planOptionsParamService.BuildFrom(policyDto.Id, policyDto.BrandKey, riskDto.Id, rider,
                        planDefaults);

                    _createPlanService.CreateRider(planOptionsParam, createdPlan.Id);
                }
            }

            //Now that risk and policy has been created, sync up underwriting status for covers
            _coverUnderwritingSyncService.Update(new UnderwritingBenefitResponsesChangeParam(riskDto.InterviewId,
                createRiskResult.InterviewReferenceInformation.BenefitStatuses));

            //Set any defaults
            var setupPlanService = _configurationServiceFactory.GetService(_policySourceConverter.From(createQuoteParam.Source));
            setupPlanService.Run(riskDto);

            //Update plan based on eligibility, in case any defaults are not available (like for age or occupation)
            _planAutoUpdateService.UpdatePlansToConformWithPlanEligiblityRules(riskDto);

            //calculate initial premium
            _policyPremiumCalculation.CalculateAndSavePolicy(policyDto.QuoteReference);

            if (createQuoteParam.PersonalInformation?.LeadId != null && createQuoteParam.PersonalInformation.LeadId != 0)
            {
                //update partyDto with adobe lead
                _updatePartyWithLeadService.Update(createQuoteParam.PersonalInformation.LeadId.Value,
                    createQuoteParam, partyDto, policySource);
            }

            return new CreateQuoteResult(policyDto.QuoteReference, riskDto.InterviewId);
        }

        private void SetAnalyticsIdentifiers(PolicyDto policyDto)
        {
            var policyInitialisationMetaData =
                _policyInitialisationMetadataService.GetPolicyInitialisationMetadataForSession();

            _policyAnalyticsIdentifierService.SetPolicyAnalyticsIdentifiersForPolicy(policyDto, _analyticsIdentifiersConverter.From(policyInitialisationMetaData));
        }

        private void SetPolicyOwner(int policyId, int partyId)
        {
            _policyOwnerService.SetPolicyOwnerPartyForPolicy(policyId, partyId);
        }
    }
}
