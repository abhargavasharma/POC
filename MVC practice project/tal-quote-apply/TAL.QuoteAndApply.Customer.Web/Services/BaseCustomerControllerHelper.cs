using System;
using System.Linq;
using TAL.QuoteAndApply.Customer.Web.Configuration;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.Web.Shared.Session;

namespace TAL.QuoteAndApply.Customer.Web.Services
{
    public interface IBaseCustomerControllerHelper
    {
        Lazy<PolicyOverviewResult> PolicyOverview { get; }
        string QuoteReferenceNumber { get; }
        void SetMainQuoteValues(dynamic viewBag);
        IQuoteSessionContext Session { get; }
    }

    public class BaseCustomerControllerHelper : IBaseCustomerControllerHelper
    {
        private readonly IPlanDetailsService _planDetailsService;
        private readonly IQuoteSessionContext _quoteSessionContext;
        private readonly IPolicySatusProvider _policyStatusProvider;
        private readonly IAngularConfigurationProvider _angularConfigurationProvider;
        private readonly IProductDefinitionProvider _productDefinitionProvider;
        private readonly ICurrentProductBrandProvider _currentProductBrandProvider;

        public Lazy<PolicyOverviewResult> PolicyOverview { get; }

        public string QuoteReferenceNumber => _quoteSessionContext.QuoteSession.QuoteReference;

        public IQuoteSessionContext Session => _quoteSessionContext;

        public BaseCustomerControllerHelper(IPlanDetailsService planDetailsService, IQuoteSessionContext quoteSessionContext,
            IPolicyOverviewProvider policyOverviewProvider,
            IPolicySatusProvider policyStatusProvider, IAngularConfigurationProvider angularConfigurationProvider, IProductDefinitionProvider productDefinitionProvider, ICurrentProductBrandProvider currentProductBrandProvider)
        {
            _planDetailsService = planDetailsService;
            _quoteSessionContext = quoteSessionContext;
            _policyStatusProvider = policyStatusProvider;
            _angularConfigurationProvider = angularConfigurationProvider;
            _productDefinitionProvider = productDefinitionProvider;
            _currentProductBrandProvider = currentProductBrandProvider;
            PolicyOverview =
                new Lazy<PolicyOverviewResult>(() => _quoteSessionContext.QuoteSession.QuoteReference == null ? null : policyOverviewProvider.GetFor(_quoteSessionContext.QuoteSession.QuoteReference));
        }

        public void SetMainQuoteValues(dynamic viewBag)
        {
            if (PolicyOverview.Value == null)
                return;

            var currentBrand = _currentProductBrandProvider.GetCurrent();
            var productDefinition = _productDefinitionProvider.GetProductDefinition(currentBrand.BrandCode);

            var risks = PolicyOverview.Value.Risks.Select(risk => risk.RiskId);

            //Here we are catering for the first risk only, but in future, we need to change it to cater for multi-risk.
            var riskId = risks.First();
            var policyDetails = _policyStatusProvider.GetStatus(QuoteReferenceNumber);
            var planResponse = _planDetailsService.GetPlanDetailsForRisk(QuoteReferenceNumber, riskId);

            viewBag.QuoteReference = _quoteSessionContext.QuoteSession.QuoteReference;
            viewBag.TotalPremium = planResponse.TotalPremium;
            viewBag.PremiumPeriod = planResponse.PaymentFrequency;
            viewBag.SavedStatus = policyDetails.SaveStatus == PolicySaveStatus.PersonalDetailsEntered
                ? "personalDetails"
                : policyDetails.SaveStatus == PolicySaveStatus.CreatedLogin
                    ? "passwordCreated"
                    : "notSaved";
            viewBag.JourneyType = policyDetails.Source;
            viewBag.DebugEnabled = _angularConfigurationProvider.DebugEnabled;
            viewBag.CallBackSubmitted = _quoteSessionContext.QuoteSession.SessionData.CallBackRequested;
            viewBag.IsQuoteSaveLoadEnabled = productDefinition.IsQuoteSaveLoadEnabled;
            viewBag.SaveGatePosition = productDefinition.SaveGatePosition;
            
        }
    }
}