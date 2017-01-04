using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Party.Data;
using TAL.QuoteAndApply.Party.Models;
using TAL.QuoteAndApply.Party.Services;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.Product.Models.Definition;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Events.Models;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Product.Models.Converters;
using TAL.QuoteAndApply.ServiceLayer.Product.Rules;
using TAL.QuoteAndApply.ServiceLayer.Underwriting;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Events;
using TAL.QuoteAndApply.ServiceLayer.Validation;
using TAL.QuoteAndApply.Underwriting.Models.Dto;
using TAL.QuoteAndApply.Underwriting.Models.Event;
using TAL.QuoteAndApply.Underwriting.Models.Talus;
using TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy;

namespace TAL.QuoteAndApply.ServiceLayer.UnitTests.Policy.Risks
{
    [TestFixture]
    public class UpdateRiskServiceTests
    {
        private Mock<IPartyDtoUpdater> _partyDtoUpdater;
        private Mock<IPartyService> _partyService;
        private Mock<IPartyRulesService> _partyRulesService;
        private Mock<IRiskService> _riskService;
        private Mock<IRiskDtoConverter> _riskDtoConverter;
        private Mock<IUnderwritingRatingFactorsService> _underwritingRatingFactorsService;
        private Mock<IProductRulesService> _productRulesService;
        private Mock<IProductDefinitionBuilder> _productDefinitionBuilder;
        private Mock<IRiskProductDefinitionConverter> _riskProductDefinitionConverter;
        private MockRepository _mockRepository;
        private Mock<IPlanAutoUpdateService> _planAutoUpdateService;
        private Mock<IRiskOccupationDtoRepository> _riskOccupationService;
        private Mock<IPartyConsentService> _partyConsentService;
        private Mock<IPartyConsentDtoUpdater> _partyConsentDtoUpdater;
        private Mock<IUnderwritingBenefitsResponseChangeObserver> _coverUnderwritingSyncService;
        private Mock<IUpdatePlanOccupationDefinitionService> _updatePlanOccupationDefinitionService;
        private Mock<IPolicySourceProvider> _policySourceProvider;
        private Mock<IProductBrandProvider> _mockProductBrandProvider;
		private  Mock<IRaisePolicyOwnershipValidationService> _raisePolicyOwnershipValidationService;

        [TestFixtureSetUp]
        public void Setup()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _partyDtoUpdater = _mockRepository.Create<IPartyDtoUpdater>();
            _partyService = _mockRepository.Create<IPartyService>();
            _partyRulesService = _mockRepository.Create<IPartyRulesService>();
            _riskService = _mockRepository.Create<IRiskService>();
            _riskDtoConverter = _mockRepository.Create<IRiskDtoConverter>();
            _underwritingRatingFactorsService = _mockRepository.Create<IUnderwritingRatingFactorsService>();
            _productDefinitionBuilder = _mockRepository.Create<IProductDefinitionBuilder>();
            _riskProductDefinitionConverter = _mockRepository.Create<IRiskProductDefinitionConverter>();
            _productRulesService = _mockRepository.Create<IProductRulesService>();
            _planAutoUpdateService = _mockRepository.Create<IPlanAutoUpdateService>();
            _riskOccupationService = _mockRepository.Create<IRiskOccupationDtoRepository>();
            _partyConsentService = _mockRepository.Create<IPartyConsentService>();
            _partyConsentDtoUpdater = _mockRepository.Create<IPartyConsentDtoUpdater>();
            _coverUnderwritingSyncService = _mockRepository.Create<IUnderwritingBenefitsResponseChangeObserver>();
            _updatePlanOccupationDefinitionService = _mockRepository.Create<IUpdatePlanOccupationDefinitionService>();
            _policySourceProvider = _mockRepository.Create<IPolicySourceProvider>();
            _mockProductBrandProvider = _mockRepository.Create<IProductBrandProvider>();
			_raisePolicyOwnershipValidationService = _mockRepository.Create<IRaisePolicyOwnershipValidationService>();
        }

        [Test]
        public void UpdateRiskRatingFactors_RatingFactorsParamNotValid_BrokenRulesReturned()
        {
            var mockRatingFactorsParam = new RatingFactorsParam(
                'M', new DateTime(1980, 01, 01), true, new SmokerStatusHelper(false), "AC", "BC", 100000);

            var ruleResults = new List<ValidationError>();
            ruleResults.Add(new ValidationError(null, ValidationKey.AnnualIncome, "Test message", ValidationType.Error, null));

            _productRulesService.Setup(call => call.ValidateRatingFactors(mockRatingFactorsParam, true)).Returns(ruleResults);

            var svc = GetService();
            var result = svc.UpdateRiskRatingFactors(1, mockRatingFactorsParam, true);

            Assert.That(result.HasErrors, Is.True);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void UpdateRiskRatingFactors_RiskIsValidForInforce_SetToPassedValue(bool validForInforce)
        {
            const string interviewId = "12345";
            const string concurrencyToken = "ABC123";
            const string templateVersion = "1";
            const int riskId = 1;
            const int partyId = 100;

            var mockRatingFactorsParam = new RatingFactorsParam(
                'M', new DateTime(1980, 01, 01), true, new SmokerStatusHelper(false), "AC", "BC", 1000000);

            var mockRisk = _mockRepository.Create<IRisk>();
            mockRisk.Setup(call => call.Id).Returns(riskId);
            mockRisk.Setup(call => call.InterviewId).Returns(interviewId);
            mockRisk.Setup(call => call.InterviewConcurrencyToken).Returns(concurrencyToken);
            mockRisk.Setup(call => call.PartyId).Returns(partyId);

            var mockParty = _mockRepository.Create<IParty>();

            var mockReadonlyInterview = new ReadOnlyUnderwritingInterview(new UnderwritingInterview {InterviewIdentifier = "ABC123", TemplateVersion = "1", ConcurrencyToken = "AAAABBBB"});
            var mockReferenceInfo = new InterviewReferenceInformation("ABC123", "1", "AAAABBBB", new InterviewOccupationInformation(null, null, null, null), new List<UnderwritingBenefitResponseStatus>());
            var mockProductDefinition = _mockRepository.Create<ProductDefinition>();
            var mockRiskProductDefinition = _mockRepository.Create<RiskProductDefinition>();

            var mockPolicySource = PolicySource.CustomerPortalBuildMyOwn;

            _productRulesService.Setup(call => call.ValidateRatingFactors(mockRatingFactorsParam, false)).Returns(new List<ValidationError>());
            _riskService.Setup(call => call.GetRisk(riskId)).Returns(mockRisk.Object);

            _underwritingRatingFactorsService.Setup(
                call => call.UpdateUnderwritingWithRatingFactorValues(mockRisk.Object, mockRatingFactorsParam)).Returns(mockReferenceInfo);
            _riskDtoConverter.Setup(
                call => call.UpdateFrom(mockRisk.Object, mockRatingFactorsParam, mockReferenceInfo)).Returns(mockRisk.Object);

            _riskService.Setup(call => call.UpdateRisk(mockRisk.Object));
            _productDefinitionBuilder.Setup(call => call.BuildProductDefinition("tal")).Returns(mockProductDefinition.Object);
            _riskProductDefinitionConverter.Setup(call => call.CreateFrom(mockProductDefinition.Object))
                .Returns(mockRiskProductDefinition.Object);
            _mockProductBrandProvider.Setup(call => call.GetBrandKeyForRisk(mockRisk.Object)).Returns("tal");

            _planAutoUpdateService.Setup(call => call.UpdatePlansToConformWithPlanEligiblityRules(mockRisk.Object));

            _riskService.Setup(call => call.IsRiskValidForInforce(mockRiskProductDefinition.Object, mockRisk.Object))
                .Returns(validForInforce);

            _partyService.Setup(call => call.GetParty(partyId)).Returns(mockParty.Object);
            _partyDtoUpdater.Setup(call => call.UpdateFrom(mockParty.Object, mockRatingFactorsParam))
                .Returns(mockParty.Object);

            _policySourceProvider.Setup(call => call.From(riskId)).Returns(mockPolicySource);

            _partyService.Setup(call => call.UpdateParty(mockParty.Object, mockPolicySource));

            _coverUnderwritingSyncService.Setup(call => call.Update(It.IsAny<UnderwritingBenefitResponsesChangeParam>()));

            _updatePlanOccupationDefinitionService.Setup(call => call.Update(It.IsAny<IRisk>()));

            var svc = GetService();
            var result = svc.UpdateRiskRatingFactors(riskId, mockRatingFactorsParam, false);

            Assert.That(result.IsRatingFactorsValidForInforce, Is.EqualTo(validForInforce));
        }

        [Test]
        public void Update_NewOccupationInformation_AllFieldsUpdated()
        {
            var existingRisk = new RiskOccupationDto()
            {
                IndustryCode = "2d",
                OccupationCode = "5z",
                IndustryTitle = "Blh",
                OccupationTitle = "Label",
                IsTpdAny = false,
                IsTpdOwn = false,
                TpdLoading = 0,
                OccupationClass = "B",
                PasCode = "B",
                RiskId = 1
            };

            _riskOccupationService.Setup(call => call.GetForRisk(1, false))
                .Returns(existingRisk);
            _riskOccupationService.Setup(call => call.UpdateOccupationRisk(It.IsAny<RiskOccupationDto>()));
            
            var updatePayload = new UpdateOccupationParam(1, 1, "AAA", "3w",
                "Business Analyst/Consultant/Development Manager - Uni Qual", "2e", "Financial Services", true, true,
                null, "AAA");

            _updatePlanOccupationDefinitionService.Setup(call => call.Update(1, true, true));

            var svc = GetService();
            svc.Update(updatePayload);

            Assert.That(existingRisk.IndustryCode, Is.EqualTo("2e"));
            Assert.That(existingRisk.OccupationCode, Is.EqualTo("3w"));
            Assert.That(existingRisk.IndustryTitle, Is.EqualTo("Financial Services"));
            Assert.That(existingRisk.OccupationTitle, Is.EqualTo("Business Analyst/Consultant/Development Manager - Uni Qual"));
            Assert.That(existingRisk.IsTpdAny, Is.EqualTo(true));
            Assert.That(existingRisk.IsTpdOwn, Is.EqualTo(true));
            Assert.That(existingRisk.TpdLoading, Is.EqualTo(null));
            Assert.That(existingRisk.OccupationClass, Is.EqualTo("AAA"));
            Assert.That(existingRisk.PasCode, Is.EqualTo("AAA"));
        }

        private UpdateRiskService GetService()
        {
            return new UpdateRiskService(_partyDtoUpdater.Object, 
                _partyService.Object, 
                _partyRulesService.Object, 
                _riskService.Object, 
                _riskDtoConverter.Object, 
                _underwritingRatingFactorsService.Object,
                _productRulesService.Object, 
                _productDefinitionBuilder.Object, 
                _riskProductDefinitionConverter.Object,
                _planAutoUpdateService.Object, 
                _riskOccupationService.Object, 
                _partyConsentService.Object, 
                _partyConsentDtoUpdater.Object,
                _coverUnderwritingSyncService.Object, 
                _updatePlanOccupationDefinitionService.Object, 
                _policySourceProvider.Object, 
                _mockProductBrandProvider.Object,
                _raisePolicyOwnershipValidationService.Object);
        }
    }
}
