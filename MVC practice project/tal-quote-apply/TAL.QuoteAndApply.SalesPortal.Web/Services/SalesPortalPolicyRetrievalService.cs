using TAL.QuoteAndApply.SalesPortal.Web.Models;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.SalesPortal.Web.Services.Converters;
using TAL.QuoteAndApply.SalesPortal.Web.User;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Underwriting;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services
{
    public interface ISalesPortalPolicyRetrievalService
    {
        RetrievePolicyViewModel RetrieveQuote(string quoteReferenceNumber, QuoteEditSource quoteEditSource);
    }

    public class SalesPortalPolicyRetrievalService : ISalesPortalPolicyRetrievalService
    {
        private readonly IRetrievePolicyViewModelConverter _retrievePolicyViewModelConverter;
        private readonly IEditPolicyPermissionsService _editPolicyPermissionsService;
        private readonly ISalesPortalSessionContext _salesPortalSessionContext;
        private readonly IPolicyOverviewProvider _policyOverviewProvider;

        private readonly IPolicyAutoUpdateService _policyAutoUpdateService;
        private readonly IPolicyInteractionService _interactionService;
        private readonly IRiskUnderwritingAnswerSyncService _riskUnderwritingAnswerSyncService;

        public SalesPortalPolicyRetrievalService(IRetrievePolicyViewModelConverter retrievePolicyViewModelConverter, 
            IEditPolicyPermissionsService editPolicyPermissionsService,
            ISalesPortalSessionContext salesPortalSessionContext, 
            IPolicyOverviewProvider policyOverviewProvider, 
            IPolicyAutoUpdateService policyAutoUpdateService, 
            IPolicyInteractionService interactionService, 
            IRiskUnderwritingAnswerSyncService riskUnderwritingAnswerSyncService)
        {
            _retrievePolicyViewModelConverter = retrievePolicyViewModelConverter;
            _editPolicyPermissionsService = editPolicyPermissionsService;
            _salesPortalSessionContext = salesPortalSessionContext;
            _policyOverviewProvider = policyOverviewProvider;
            _policyAutoUpdateService = policyAutoUpdateService;
            _interactionService = interactionService;
            _riskUnderwritingAnswerSyncService = riskUnderwritingAnswerSyncService;
        }

        public RetrievePolicyViewModel RetrieveQuote(string quoteReferenceNumber, QuoteEditSource quoteEditSource)
        {
            var policyOverview = _policyOverviewProvider.GetFor(quoteReferenceNumber);
            if (quoteEditSource == QuoteEditSource.Retrieved 
                && (policyOverview.Status == PolicyStatus.Incomplete || policyOverview.Status == PolicyStatus.ReferredToUnderwriter))
            {
                foreach (var risk in policyOverview.Risks)
                {
                    _riskUnderwritingAnswerSyncService.SyncRiskWithFullInterviewAndUpdatePlanEligibility(risk.RiskId);
                }

                _policyAutoUpdateService.AutoUpdatePlansForEligibililityAndRecalculatePremium(quoteReferenceNumber);

                //reget the policy overview because stuff might have changed
                policyOverview = _policyOverviewProvider.GetFor(quoteReferenceNumber);
            }
            
            _interactionService.PolicyAccessed(policyOverview.PolicyId);

            var editPolicyPermissions = _editPolicyPermissionsService.GetPermissionsFor(policyOverview.Status,
                _salesPortalSessionContext.SalesPortalSession.Roles);

            var model = _retrievePolicyViewModelConverter.CreateFrom(policyOverview, editPolicyPermissions);

            return model;
        }
    }
}