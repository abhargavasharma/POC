using System.Linq;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.SalesPortal.Web.Services.Converters;
using TAL.QuoteAndApply.SalesPortal.Web.Services.Plan;
using TAL.QuoteAndApply.ServiceLayer.Policy;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services
{
    public interface IPolicyCorrespondenceService
    {
        PolicyCorrespondenceViewModel GetCorrespondenceSummary(string quoteReferenceNumber, int riskId);
        bool SendCorrespondence(string quoteReferenceNumber, CorrespondenceEmailType correspondenceEmailType);
    }

    public class PolicyCorrespondenceService : IPolicyCorrespondenceService
    {
        private readonly IPlanDetailsService _planDetailsService;
        private readonly IPolicyCorrespondenceResultConverter _policyCorrespondenceResultConverter;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IPolicyOverviewProvider _policyOverviewProvider;
        private readonly IPolicyCorrespondenceEmailService _policyCorrespondenceEmailService;

        public PolicyCorrespondenceService(IPlanDetailsService planDetailsService,
            IPolicyCorrespondenceResultConverter policyCorrespondenceResultConverter, 
            ICurrentUserProvider currentUserProvider, 
            IPolicyOverviewProvider policyOverviewProvider, 
            IPolicyCorrespondenceEmailService policyCorrespondenceEmailService)
        {
            _planDetailsService = planDetailsService;
            _policyCorrespondenceResultConverter = policyCorrespondenceResultConverter;
            _currentUserProvider = currentUserProvider;
            _policyOverviewProvider = policyOverviewProvider;
            _policyCorrespondenceEmailService = policyCorrespondenceEmailService;
        }

        public PolicyCorrespondenceViewModel GetCorrespondenceSummary(string quoteReferenceNumber, int riskId)
        {
            var currentUser = _currentUserProvider.GetForApplication();
            var riskPlanDetailReposone = _planDetailsService.GetRiskPlanDetailsResponse(quoteReferenceNumber, riskId);
            var emailCreated = _policyCorrespondenceEmailService.CreateSaveQuoteRequest(quoteReferenceNumber, riskId);
            return _policyCorrespondenceResultConverter.From(riskPlanDetailReposone, currentUser.EmailAddress, emailCreated);
        }

        public bool SendCorrespondence(string quoteReferenceNumber, CorrespondenceEmailType correspondenceEmailType)
        {
            var currentUser = _currentUserProvider.GetForApplication();
            var policyOverview = _policyOverviewProvider.GetFor(quoteReferenceNumber);

            //this will only send an email with details of one risk for now
            var riskId = policyOverview.Risks.First().RiskId;

            var returnObject = _planDetailsService.GetRiskPlanDetailsResponse(quoteReferenceNumber, riskId);
            var saveQuoteEmailRequest = _policyCorrespondenceEmailService.CreateSaveQuoteRequest(quoteReferenceNumber, riskId);
            var correspondenceRequest = _policyCorrespondenceResultConverter.ToPolicyCorrespondenceRequest(currentUser, returnObject, saveQuoteEmailRequest, policyOverview.Brand);
            var emailCreated = _policyCorrespondenceEmailService.SendEmail(quoteReferenceNumber, riskId, correspondenceRequest, correspondenceEmailType);
            return emailCreated;
        }
    }
}