using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure.Ioc;
using TAL.QuoteAndApply.Notifications.Service;
using TAL.QuoteAndApply.Party.Leads;
using TAL.QuoteAndApply.Party.Leads.LeadsService;
using TAL.QuoteAndApply.Party.Services;
using TAL.QuoteAndApply.Payment.Service;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Policy.Service.MarketingStatus;
using TAL.QuoteAndApply.Product.Contracts;
using TAL.QuoteAndApply.Product.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Models.Mappers;
using TAL.QuoteAndApply.ServiceLayer.Payment;
using TAL.QuoteAndApply.ServiceLayer.Payment.Models.Conveters;
using TAL.QuoteAndApply.ServiceLayer.Plan.Models.Mappers;
using TAL.QuoteAndApply.ServiceLayer.Policy.MarketingStatus;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.ServiceLayer.Policy.PlanSelection;
using TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy;
using TAL.QuoteAndApply.ServiceLayer.Policy.PremiumCalculation;
using TAL.QuoteAndApply.ServiceLayer.Policy.Referral;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Beneficiary;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Events;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Product.Models.Converters;
using TAL.QuoteAndApply.ServiceLayer.Product.Rules;
using TAL.QuoteAndApply.ServiceLayer.Referral;
using TAL.QuoteAndApply.ServiceLayer.RulesProxy;
using TAL.QuoteAndApply.ServiceLayer.Search.AgentDashboard;
using TAL.QuoteAndApply.ServiceLayer.Search.Clients;
using TAL.QuoteAndApply.ServiceLayer.Search.Underwriting;
using TAL.QuoteAndApply.ServiceLayer.Underwriting;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Converters;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Events;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.ManageInterview;
using TAL.QuoteAndApply.ServiceLayer.User;

namespace TAL.QuoteAndApply.ServiceLayer.Ioc
{
    public class RegistrationModule : IocModuleRegistration
    {
        public override void Register(ISimpleDependencyMapper mapper)
        {
            mapper.Add<SearchUnderwritingQuestionAnswerService>()
                .Add<SingleLevelDrillDownSearchUnderwritingQuestionAnswerService>()
                .Add<QuestionAnswerSearcherProvider>()
                .Add<QuestionAnswerDataProvider>()
                .Add<ProductRulesService>()
                .Add<CreateQuoteService>()
                .Add<ChatService>()
                .Add<CreateRiskService>()
                .Add<UnderwritingInitializationService>()
                .Add<RiskDtoConverter>()
                .Add<PlanOptionAvailabilityProvider>()
                .Add<AvailablePlanOptionsProvider>()
                .Add<PlanRiderAvailabilityProvider>()
                .Add<SelectedProductPlanOptionsConverter>()
                .Add<PartyDtoConverter>()
                .Add<CoverAvailabilityProvider>()
                .Add<CreateQuoteService>()
                .Add<CreateRiskService>()
                .Add<PolicyOverviewProvider>()
                .Add<PlanAvailabilityProvider>()
                .Add<PlanDtoConverter>()
                .Add<RiskPersonalDetailsProvider>()
                .Add<ProductDefinitionService>()
                .Add<GenericRules>()
                .Add<ProductDefinitionProvider>()
                .Add<RiderAttachmentService>()
                .Add<BeneficiaryDetailsService>()
                .Add<BeneficiaryValidationService>()
                .Add<RiskRatingFactorsProvider>()
                .Add<PolicyNoteService>()
                .Add<SearchPhraseSanitizer>()
                .Add<ActivePlanValidationService>()
                .Add<RiskUnderwritingService>()
                .Add<SuperannuationFundSearch>()
                .Add<PaymentOptionsAvailabilityProvider>()
                .Add<CreditCardPaymentOptionService>()
                .Add<DirectDebitPaymentOptionService>()
                .Add<SelfManagedSuperFundPaymentOptionService>()
                .Add<SuperAnnuationPaymentOptionService>()
                .Add<DirectDebitPaymentParamConverter>()
                .Add<SelfManagedSuperFundPaymentParamConverter>()
                .Add<CreditCardPaymentParamConverter>()
                .Add<SuperannuationPaymentParamConverter>()
                .Add<PolicyInitialisationMetadataService>()
                .Add<CreditCardTypeConverter>()
                .Add<RiskUnderwritingQuestionService>()
                .Add<UnderwritingModelConverter>()
                .Add<PolicyDeclarationService>()
                .Add<UnderwritingQuestionFilterService>()
                .Add<PlanOptionsParamService>()
                .Add<PolicyPremiumFrequencyProvider>()
                .Add<ExternalRefDetailsFactory>()
                .Add<PlanRiderService>()
                .Add<UpdatePolicyService>()
                .Add<UnderwritingConfiguration>()
                .Add<PolicyInteractionsService>()
                .Add<TalusUiUrlService>()
                .Add<GetReferralsService>()
                .Add<GetUnderwritersService>()
                .Add<PlanVariableResponseConverter>()
                .Add<PartyConsentDtoConverter>()
                .Add<PartyConsentService>()
                .Add<SyncCommPreferenceService>()
                .Add<ManageInterviewService>()
                .Add<AgentDashboardSearchService>()
                .Add<GetAgentDashboardQuotesService>()
                .Add<GetLeadService>()
                .Add<CreateNewPartyDtoService>()
                .Add<PolicyProgressProvider>()
                .Add<PolicyRiskPlanStatusProvider>()
                .Add<PolicyRetrievalStatusProvider>()
                .Add<PolicyAutoUpdateService>()
                .Add<UpdatePlanOccupationDefinitionService>()
                .Add<CustomerCallbackService>()
                .Add<NameLookupService>()
                .Add<PolicySourceProvider>()
                .Add<LoadedQuestionPremiumCalculationService>()
                .Add<PolicyCorrespondenceEmailService>()
                .Add<CustomerReferralService>()
                .Add<PolicyDocumentUrlProvider>()
                .Add<PostcodeService>()
                .Add<VariableAvailabilityProvider>()
                .Add<UpdatePartyWithLeadService>()
                .Add<RiskMarketingStatusService>()
                .Add<UpdateMarketingStatusService>()
                .Add<RiskMarketingStatusUpdater>()
                .Add<PlanMarketingStatusUpdater>()
                .Add<CoverMarketingStatusUpdater>()
                .Add<ProductBrandProvider>()
                .Add<PolicyOwnershipService>()
                .Add<CustomerPolicyRiskService>()
                .Add<RaisePolicyValidationService>()
                .Add<RaisePolicyOwnershipValidationService>()
                .Add<OrganisationProvider>();

            mapper.WhenRequesting<IPlanAutoUpdateService>().ProvideImplementationOf<PlanAutoUpdateService>();
            mapper.WhenRequesting<IProductBrandSettingsProvider>().ProvideImplementationOf<ProductBrandSettingsProvider>();

            mapper.WhenRequesting<IBeneficiaryValidationServiceAdapter>().ProvideImplementationOf<BeneficiaryValidationServiceAdapter>();
            mapper.WhenRequesting<ICurrentUserProvider>().ProvideImplementationOf<CurrentUserProvider>();
            mapper.WhenRequesting<IUpdateRiskService>().ProvideImplementationOf<UpdateRiskService>();
            mapper.WhenRequesting<ICustomerSaveService>().ProvideImplementationOf<CustomerSaveService>();
            mapper.WhenRequesting<IPaymentOptionService>().ProvideImplementationOf<PaymentOptionService>();
            mapper.WhenRequesting<IPolicySubmissionValidationService>()
                .ProvideImplementationOf<PolicySubmissionValidationService>();

            mapper.WhenRequesting<IRaisePolicyOwnershipValidationService>()
                .ProvideImplementationOf<RaisePolicyOwnershipValidationService>();

            mapper.WhenRequesting<IPolicySatusProvider>().ProvideImplementationOf<PolicySatusProvider>();
            mapper.WhenRequesting<IUnderwritingUiAuthenticationService>()
                .ProvideImplementationOf<UnderwritingUiAuthenticationService>();

            mapper.WhenRequesting<IPlanSelectionAndConfigurationServiceFactory>()
                .ProvideImplementationOf<PlanSelectionAndConfigurationServiceFactory>();

            RegisterConverters(mapper);
            RegisterUnderwritingDependencies(mapper);
            RegisterSubjectsAndObservers(mapper);
            RegisterPlanDependencies(mapper);
            RegisterRaisePolicyDependencies(mapper);
            RegisterPremiumCalculationDependencies(mapper);
            RegisterSearchDependencies(mapper);
            RegisterReferralDependencies(mapper);
        }

        private void RegisterReferralDependencies(ISimpleDependencyMapper mapper)
        {
            mapper.Add<CreateReferralService>()
                .Add<CompleteReferralService>();
        }

        private void RegisterSearchDependencies(ISimpleDependencyMapper mapper)
        {
            mapper.Add<SearchQuotesClientsAndProspectsService>();
            mapper.Add<QuotesClientsAndProspectSearcherProvider>();
            mapper.Add<SearchByQuoteReferenceNumberService>();
            mapper.Add<SearchByLeadIdService>();
            mapper.Add<SearchQuoteResultProvider>();
            mapper.Add<SearchByPartyInformationService>();
        }

        public void RegisterRaisePolicyDependencies(ISimpleDependencyMapper mapper)
        {
            mapper.WhenRequesting<IRaisePolicyService>().ProvideImplementationOf<RaisePolicyService>();
            mapper.WhenRequesting<IRaisePolicyRiskConverter>().ProvideImplementationOf<RaisePolicyRiskConverter>();
            mapper.WhenRequesting<IRaisePolicyPlanConverter>().ProvideImplementationOf<RaisePolicyPlanConverter>();
            mapper.WhenRequesting<IRaisePolicyCoverConverter>().ProvideImplementationOf<RaisePolicyCoverConverter>();
            mapper.WhenRequesting<IRaisePolicyOptionConverter>().ProvideImplementationOf<RaisePolicyOptionConverter>();
            mapper.WhenRequesting<IRaisePolicyBeneficiaryConverter>().ProvideImplementationOf<RaisePolicyBeneficiaryConverter>();
            mapper.WhenRequesting<IRaisePolicyFactory>().ProvideImplementationOf<RaisePolicyFactory>();
            mapper.WhenRequesting<IRaisePolicyConverter>().ProvideImplementationOf<RaisePolicyConverter>();
            mapper.WhenRequesting<IRaisePaymentConverter>().ProvideImplementationOf<RaisePaymentConverter>();
            mapper.WhenRequesting<IPlanResponseToStateParamConverter>().ProvideImplementationOf<PlanResponseToStateParamConverter>();

            mapper.Add<RaisePolicyPlanSpecificMappingBuilder>().InSingletonScope();
            mapper.Add<RaisePolicyPlanSpecificMappingProvider>();
            mapper.Add<RaisePolicyFeedbackService>();
        }

        private void RegisterPremiumCalculationDependencies(ISimpleDependencyMapper mapper)
        {
            mapper
                .Add<PolicyPremiumCalculation>()
                .Add<PolicyWithRisksService>()
                .Add<GetPremiumCalculationRequestService>()
                .Add<PolicyWithRisksPremiumUpdater>()
                .Add<PolicyPremiumSummaryProvider>();
        }

        private void RegisterPlanDependencies(ISimpleDependencyMapper mapper)
        {
            mapper.Add<CreatePlanService>()
                .Add<UpdatePlanService>()
                .Add<PlanOverviewResultProvider>()
                .Add<PlanResponseProvider>()
                .Add<PlanRulesService>()
                .Add<PlanStateParamValidationService>()
                .Add<PlanEligibilityService>()
                .Add<PlanEligibilityRulesFactory>()
                .Add<PlanEligibilityProvider>()
                .Add<CoverEligibilityService>()
                .Add<PlanStateParamProvider>()
                .Add<PlanCoverAmountService>();
        }

        private void RegisterSubjectsAndObservers(ISimpleDependencyMapper mapper)
        {
            mapper.Add<RiskEventsRegistration>();
            mapper.Add<UnderwritingEventsRegistration>();

            mapper.Add<DateOfBirthChangeSubject>().InSingletonScope();
            mapper.Add<GenderChangeSubject>().InSingletonScope();
            mapper.Add<OccupationChangeSubject>().InSingletonScope();
            mapper.Add<ResidencyChangeSubject>().InSingletonScope();
            mapper.Add<SmokerStatusChangeSubject>().InSingletonScope();
            mapper.Add<AnnualIncomeChangeSubject>().InSingletonScope();

            mapper.Add<UnderwritingBenefitsResponseChangeSubject>().InSingletonScope();

            mapper.WhenRequesting<IDateOfBirthChangeObserver>().ProvideImplementationOf<UpdateRiskService>();
            mapper.WhenRequesting<IDateOfBirthChangeObserver>().ProvideImplementationOf<UpdatePartyService>();

            mapper.WhenRequesting<IGenderChangeObserver>().ProvideImplementationOf<UpdateRiskService>();
            mapper.WhenRequesting<IGenderChangeObserver>().ProvideImplementationOf<UpdatePartyService>();

            mapper.WhenRequesting<IOccupationChangeObserver>().ProvideImplementationOf<UpdateRiskService>();
            mapper.WhenRequesting<IResidencyChangeObserver>().ProvideImplementationOf<UpdateRiskService>();
            mapper.WhenRequesting<ISmokerStatusChangeObserver>().ProvideImplementationOf<UpdateRiskService>();
            mapper.WhenRequesting<IAnnualIncomeChangeObserver>().ProvideImplementationOf<UpdateRiskService>();

            mapper.WhenRequesting<IUnderwritingBenefitsResponseChangeObserver>().ProvideImplementationOf<CoverUnderwritingSyncService>();
        }

        private void RegisterConverters(ISimpleDependencyMapper mapper)
        {
            mapper.Add<RiskDtoConverter>()
                .Add<PolicyOverviewResultConverter>()
                .Add<RiskOverviewResultConverter>()
                .Add<PlanDtoConverter>()
                .Add<RiskPersonalDetailsResultConverter>()
                .Add<ProductDtoConverter>()
                .Add<RatingFactorsResultConverter>()
                .Add<MaxCoverAmountParamConverter>()
                .Add<CoverDtoConverter>()
                .Add<RiskProductDefinitionConverter>()
                .Add<OptionDtoConverter>()
                .Add<PlanDtoConverter>()
                .Add<BeneficiaryDtoConverter>()
                .Add<RatingFactorsResultConverter>()
                .Add<MaxCoverAmountParamConverter>()
                .Add<RiskProductDefinitionConverter>()
                .Add<MinAnnualIncomeParamConverter>()
                .Add<SuperannuationFundSearchResponseConverter>()
                .Add<SuperannuationPaymentParamConverter>()
                .Add<CreditCardPaymentParamConverter>()
                .Add<DirectDebitPaymentParamConverter>()
                .Add<PolicyPremiumSummaryConverter>()
                .Add<AnalyticsIdentifiersConverter>()
                .Add<PolicySourceConverter>()
                .Add<PolicyInteractionsResultConverter>()
                .Add<PolicyNotesResultConverter>()
                .Add<PartyConsentResultConverter>()
                .Add<PartyConsentParamConverter>()
                .Add<AgentDashboardQuoteResultConverter>()
                .Add<GetLeadResultConverter>()
                .Add<PolicySourceConverter>()
                .Add<AgentDashboardRequestProvider>()
                .Add<EmailQuoteService>();
            mapper.WhenRequesting<IPartyDtoConverter>().ProvideImplementationOf<PartyDtoConverter>();
            mapper.WhenRequesting<IPartyDtoUpdater>().ProvideImplementationOf<PartyDtoConverter>();
            mapper.WhenRequesting<IPlanDtoUpdater>().ProvideImplementationOf<PlanDtoConverter>();
            mapper.WhenRequesting<ICoverDtoConverter>().ProvideImplementationOf<CoverDtoConverter>();
            mapper.WhenRequesting<ICoverDtoUpdater>().ProvideImplementationOf<CoverDtoConverter>();
            mapper.WhenRequesting<IOptionDtoConverter>().ProvideImplementationOf<OptionDtoConverter>();
            mapper.WhenRequesting<IOptionDtoUpdater>().ProvideImplementationOf<OptionDtoConverter>();
            mapper.WhenRequesting<IBeneficiaryRelationshipConverter>().ProvideImplementationOf<BeneficiaryRelationshipConverter>();
            mapper.WhenRequesting<IReferralConverter>().ProvideImplementationOf<ReferralConverter>();
            mapper.WhenRequesting<IPartyConsentDtoConverter>().ProvideImplementationOf<PartyConsentDtoConverter>();
            mapper.WhenRequesting<IPartyConsentDtoUpdater>().ProvideImplementationOf<PartyConsentDtoConverter>();
            mapper.WhenRequesting<IPartyConsentResultConverter>().ProvideImplementationOf<PartyConsentResultConverter>();
            mapper.WhenRequesting<IPartyConsentDtoUpdater>().ProvideImplementationOf<PartyConsentDtoConverter>();
            mapper.WhenRequesting<IPartyCommunicationMessageConverter>().ProvideImplementationOf<PartyCommunicationMessageConverter>();
            mapper.WhenRequesting<IPartyConsentParamConverter>().ProvideImplementationOf<PartyConsentParamConverter>();
            mapper.WhenRequesting<IAgentDashboardQuoteResultConverter>().ProvideImplementationOf<AgentDashboardQuoteResultConverter>();
            mapper.WhenRequesting<IGetLeadResultConverter>().ProvideImplementationOf<GetLeadResultConverter>();
            mapper.WhenRequesting<IPolicySourceConverter>().ProvideImplementationOf<PolicySourceConverter>();
            mapper.WhenRequesting<IAgentDashboardRequestProvider>().ProvideImplementationOf<AgentDashboardRequestProvider>();
            mapper.WhenRequesting<IPolicyCorrespondenceRequestConverter>().ProvideImplementationOf<PolicyCorrespondenceRequestConverter>();

        }

        private void RegisterUnderwritingDependencies(ISimpleDependencyMapper mapper)
        {
            mapper
                .Add<UnderwritingInitializationService>()
                .Add<KnownQuestionIdProvider>()
                .Add<RiskUnderwritingAnswerSyncService>()
                .Add<OccupationQuestionProvider>()
                .Add<UnderwritingRatingFactorsService>()
                .Add<GetUnderwritingBenefitCodeForCoverService>()
                .Add<CoverLoadingsConverter>()
                .Add<TalusUiUrlService>()
                .Add<CoverExclusionsConverter>();
        }
    }
}
