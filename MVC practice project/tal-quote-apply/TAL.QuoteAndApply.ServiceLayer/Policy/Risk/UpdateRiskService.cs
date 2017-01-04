using System;
using System.Linq;
using System.Text;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Service;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Party.Services;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Events;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Events.Models;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Product.Models.Converters;
using TAL.QuoteAndApply.ServiceLayer.Product.Rules;
using TAL.QuoteAndApply.ServiceLayer.Underwriting;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Events;
using TAL.QuoteAndApply.Underwriting.Models.Event;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Risk
{
    public interface IUpdateRiskService
    {
        UpdateRiskPersonalDetailsResult UpdateRiskPersonalDetails(RiskPersonalDetailsParam riskPersonalDetailsParam);
        UpdateRiskRatingFactorsResult UpdateRiskRatingFactors(int riskId, RatingFactorsParam ratingFactors, bool validateResidency);
        UpdateRiskPersonalDetailsResult UpdateLifeInsuredDetails(RiskPersonalDetailsParam riskPersonalDetailsParam,string quoteReferenceNumber);
        void UpdateRiskConsent(PartyConsentParam partyConsentParam, int riskId);
        
    }

    public class UpdateRiskService : IUpdateRiskService, IDateOfBirthChangeObserver, IGenderChangeObserver, IOccupationChangeObserver, IResidencyChangeObserver, ISmokerStatusChangeObserver, IAnnualIncomeChangeObserver
    {
        private readonly IPartyDtoUpdater _partyDtoUpdater;
        private readonly IPartyService _partyService;
        private readonly IPartyRulesService _partyRulesService;
        private readonly IRiskService _riskService;
        private readonly IRiskDtoConverter _riskDtoConverter;
        private readonly IUnderwritingRatingFactorsService _underwritingRatingFactorsService;
        private readonly IProductRulesService _productRulesService;
        private readonly IProductDefinitionBuilder _productDefinitionBuilder;
        private readonly IRiskProductDefinitionConverter _riskProductDefinitionConverter;
        private readonly IPlanAutoUpdateService _planAutoUpdateService;
        private readonly IRiskOccupationDtoRepository _occupationDtoRepository;
        private readonly IPartyConsentService _partyConsentService;
        private readonly IPartyConsentDtoUpdater _partyConsentDtoUpdater;
        private readonly IUnderwritingBenefitsResponseChangeObserver _coverUnderwritingSyncService;
        private readonly IUpdatePlanOccupationDefinitionService _planOccupationDefinitionService;
        private readonly IPolicySourceProvider _policySourceProvider;
		private readonly IProductBrandProvider _productBrandProvider;
		private readonly IRaisePolicyOwnershipValidationService _raisePolicyOwnershipValidationService;
		
        public UpdateRiskService(IPartyDtoUpdater partyDtoUpdater,
            IPartyService partyService,
            IPartyRulesService partyRulesService,
            IRiskService riskService,
            IRiskDtoConverter riskDtoConverter,
            IUnderwritingRatingFactorsService underwritingRatingFactorsService,
            IProductRulesService productRulesService,
            IProductDefinitionBuilder productDefinitionBuilder,
            IRiskProductDefinitionConverter riskProductDefinitionConverter,
            IPlanAutoUpdateService planAutoUpdateService,
            IRiskOccupationDtoRepository occupationDtoRepository,
            IPartyConsentService partyConsentService,
            IPartyConsentDtoUpdater partyConsentDtoUpdater,
            IUnderwritingBenefitsResponseChangeObserver coverUnderwritingSyncService,
            IUpdatePlanOccupationDefinitionService planOccupationDefinitionService, 
            IPolicySourceProvider policySourceProvider, 
			IProductBrandProvider productBrandProvider,
			IRaisePolicyOwnershipValidationService raisePolicyOwnershipValidationService)
        {
            _partyDtoUpdater = partyDtoUpdater;
            _partyService = partyService;
            _partyRulesService = partyRulesService;
            _riskService = riskService;
            _riskDtoConverter = riskDtoConverter;
            _underwritingRatingFactorsService = underwritingRatingFactorsService;
            _productRulesService = productRulesService;
            _productDefinitionBuilder = productDefinitionBuilder;
            _riskProductDefinitionConverter = riskProductDefinitionConverter;
            _planAutoUpdateService = planAutoUpdateService;
            _occupationDtoRepository = occupationDtoRepository;
            _partyConsentService = partyConsentService;
            _partyConsentDtoUpdater = partyConsentDtoUpdater;
            _coverUnderwritingSyncService = coverUnderwritingSyncService;
            _planOccupationDefinitionService = planOccupationDefinitionService;
            _policySourceProvider = policySourceProvider;
            _productBrandProvider = productBrandProvider;
			_raisePolicyOwnershipValidationService = raisePolicyOwnershipValidationService;
        }

        public UpdateRiskPersonalDetailsResult UpdateRiskPersonalDetails(RiskPersonalDetailsParam riskPersonalDetailsParam)
        {
            var risk = _riskService.GetRisk(riskPersonalDetailsParam.RiskId);
            var partyDto = _partyService.GetParty(risk.PartyId);
            partyDto = _partyDtoUpdater.UpdateFrom(partyDto, riskPersonalDetailsParam);

            if (riskPersonalDetailsParam.PartyConsentParam != null && riskPersonalDetailsParam.PartyConsentParam.Consents != null)
            {
                UpdateRiskConsent(riskPersonalDetailsParam.PartyConsentParam, riskPersonalDetailsParam.RiskId);
            }

            var validationResult = _partyRulesService.ValidatePartyForSave(partyDto);

            if (validationResult.All(x => x.IsSatisfied))
            {
                var policySource = _policySourceProvider.From(risk.Id);
                _partyService.UpdateParty(partyDto, policySource);
            }
            else
            {
                var sb = new StringBuilder();
                sb.AppendLine("Party Save Rules Failure");
                sb.AppendLine($"Current state of Party: {partyDto.ToJson()}");

                foreach (var failedRule in validationResult.Where(r => r.IsBroken))
                {
                    sb.AppendLine($"Failed key: {failedRule.Key}");
                }

                //todo: ViewModel validaiton should cover this, I just want to see if we ever end up here
                throw new ApplicationException(sb.ToString());
            }

            return new UpdateRiskPersonalDetailsResult
            {
                IsPersonalDetailsValidForInforce = _partyService.IsPartyValidForInforce(partyDto)
            };
        }
        
        public UpdateRiskPersonalDetailsResult UpdateLifeInsuredDetails(RiskPersonalDetailsParam lifeInsureDetailsParam, string quoteReferenceNumber)
        {
            var risk = _riskService.GetRisk(lifeInsureDetailsParam.RiskId);
            var partyDto = _partyService.GetParty(risk.PartyId);
            
            partyDto.Title = lifeInsureDetailsParam.Title.MapToTitle();
            partyDto.FirstName = lifeInsureDetailsParam.FirstName;
            partyDto.Surname = lifeInsureDetailsParam.Surname;
            partyDto.Postcode = lifeInsureDetailsParam.Postcode;
            
            if (lifeInsureDetailsParam.PartyConsentParam != null && lifeInsureDetailsParam.PartyConsentParam.Consents != null)
            {
                UpdateRiskConsent(lifeInsureDetailsParam.PartyConsentParam, lifeInsureDetailsParam.RiskId);
            }

            var policySource = _policySourceProvider.From(risk.Id);
            _partyService.UpdateParty(partyDto, policySource);
            
            
            var  IsPersonalDetailsValidForInforce = _raisePolicyOwnershipValidationService.IsCompleted(quoteReferenceNumber);
            
         
            return new UpdateRiskPersonalDetailsResult
            {
                IsPersonalDetailsValidForInforce = IsPersonalDetailsValidForInforce
            };
        }

        public UpdateRiskPersonalDetailsResult UpdateCustomerPortalRiskPersonalDetails(RiskPersonalDetailsParam riskPersonalDetailsParam)
        {
            var risk = _riskService.GetRisk(riskPersonalDetailsParam.RiskId);
            var partyDto = _partyService.GetParty(risk.PartyId);
            partyDto = _partyDtoUpdater.UpdateFrom(partyDto, riskPersonalDetailsParam);

            return new UpdateRiskPersonalDetailsResult
            {
                IsPersonalDetailsValidForInforce = _partyService.IsPartyValidForInforce(partyDto)
            };
        }

        public UpdateRiskRatingFactorsResult UpdateRiskRatingFactors(int riskId, RatingFactorsParam ratingFactors, bool validateResidency)
        {
            var brokenRules = _productRulesService.ValidateRatingFactors(ratingFactors, validateResidency);

            if (brokenRules.Any())
            {
                return new UpdateRiskRatingFactorsResult(brokenRules);
            }

            var risk = _riskService.GetRisk(riskId);
            var interview = _underwritingRatingFactorsService.UpdateUnderwritingWithRatingFactorValues(risk, ratingFactors);
            
            //reget the risk here, just in case something has been updated on it in the mean time
            risk = _riskService.GetRisk(riskId);
            risk = _riskDtoConverter.UpdateFrom(risk, ratingFactors, interview);
            _riskService.UpdateRisk(risk);

            _planOccupationDefinitionService.Update(risk);
            
            _coverUnderwritingSyncService.Update(new UnderwritingBenefitResponsesChangeParam(interview.InterviewIdentifier, interview.BenefitStatuses));

            _planAutoUpdateService.UpdatePlansToConformWithPlanEligiblityRules(risk);

            var party = _partyService.GetParty(risk.PartyId);
            party = _partyDtoUpdater.UpdateFrom(party, ratingFactors);

            var policySource = _policySourceProvider.From(risk.Id);

            _partyService.UpdateParty(party, policySource);

            var brandKey = _productBrandProvider.GetBrandKeyForRisk(risk);

            var productDefinition = _productDefinitionBuilder.BuildProductDefinition(brandKey);
            var riskProductDefinition = _riskProductDefinitionConverter.CreateFrom(productDefinition);

            return new UpdateRiskRatingFactorsResult(_riskService.IsRiskValidForInforce(riskProductDefinition, risk));
        }

        public void UpdateRiskConsent(PartyConsentParam partyConsentParam, int riskId)
        {
            var risk = _riskService.GetRisk(riskId);
            var partyConsent = _partyConsentService.GetPartyConsentByPartyId(risk.PartyId);
            if (partyConsentParam.ExpressConsent != partyConsent.ExpressConsent)
            {
                partyConsent.ExpressConsentUpdatedTs = DateTime.Now;
            }
            var updatedPartyConsent = _partyConsentDtoUpdater.UpdateFrom(partyConsent, partyConsentParam);
            var party = _partyService.GetParty(risk.PartyId);
            _partyConsentService.UpdatePartyConsent(updatedPartyConsent, party);
        }

        public void Update(UpdateDateOfBirthParam updateDateOfBirthParam)
        {
            var risk = _riskService.GetRisk(updateDateOfBirthParam.RiskId);
            risk.DateOfBirth = updateDateOfBirthParam.DateOfBirth;
            _riskService.UpdateRisk(risk);

            _planAutoUpdateService.UpdatePlansToConformWithPlanEligiblityRules(risk);
        }

        public void Update(UpdateGenderParam updateGenderParam)
        {
            var risk = _riskService.GetRisk(updateGenderParam.RiskId);
            risk.Gender = MapGender(updateGenderParam.Gender);
            _riskService.UpdateRisk(risk);
        }

        private Gender MapGender(char gender)
        {
            if (gender == 'F')
                return Gender.Female;

            return Gender.Male;
        }

        public void Update(UpdateOccupationParam updateOccupationParam)
        {
            var riskOccupation = _occupationDtoRepository.GetForRisk(updateOccupationParam.RiskId);
            riskOccupation.OccupationCode = updateOccupationParam.OccupationCode;
            riskOccupation.PasCode = updateOccupationParam.PasCode;
            riskOccupation.OccupationClass = updateOccupationParam.OccupationClass;
            riskOccupation.OccupationTitle = updateOccupationParam.OccupationTitle;
            riskOccupation.IndustryCode = updateOccupationParam.IndustryCode;
            riskOccupation.IndustryTitle = updateOccupationParam.IndustryTitle;
            riskOccupation.IsTpdAny = updateOccupationParam.IsTpdAny;
            riskOccupation.IsTpdOwn = updateOccupationParam.IsTpdOwn;
            riskOccupation.TpdLoading = updateOccupationParam.TpdLoading;

            _occupationDtoRepository.UpdateOccupationRisk(riskOccupation);

            _planOccupationDefinitionService.Update(updateOccupationParam.RiskId, riskOccupation.IsTpdAny,
                riskOccupation.IsTpdOwn);
        }

        public void Update(UpdateResidencyParam updateResidencyParam)
        {
            var risk = _riskService.GetRisk(updateResidencyParam.RiskId);
            risk.Residency = updateResidencyParam.ResidencyStatus;
            _riskService.UpdateRisk(risk);
        }

        public void Update(UpdateSmokerStatusParam updateSmokerStatusParam)
        {
            var risk = _riskService.GetRisk(updateSmokerStatusParam.RiskId);
            risk.SmokerStatus = updateSmokerStatusParam.SmokerStatus;
            _riskService.UpdateRisk(risk);
        }

        public void Update(UpdateAnnualIncomeParam updateAnnualIncomeParam)
        {
            var risk = _riskService.GetRisk(updateAnnualIncomeParam.RiskId);
            risk.AnnualIncome = updateAnnualIncomeParam.AnnualIncome;
            _riskService.UpdateRisk(risk);
        }
    }
}
