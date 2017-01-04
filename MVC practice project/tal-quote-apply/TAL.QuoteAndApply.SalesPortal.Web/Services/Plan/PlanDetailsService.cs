using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.SalesPortal.Web.Services.Converters;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.ServiceLayer.Product;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services.Plan
{
    public interface IPlanDetailsService
    {
        IEnumerable<PlanDetailResponse> GetPlanDetailsForRisk(string quoteReferenceNumber, int riskId);
        RiskPlanDetailReposone GetRiskPlanDetailsResponse(string quoteReferenceNumber, int riskId);
    }

    public class PlanDetailsService : IPlanDetailsService
    {
        private readonly IPlanOverviewResultProvider _planOverviewResultProvider;
        private readonly IPlanResponseProvider _planResponseProvider;
        private readonly IProductDefinitionProvider _productDefinitionProvider;
        private readonly IPlanDetailResponseConverter _planDetailResponseConverter;
        private readonly IPlanEligibilityProvider _planEligibilityProvider;
        private readonly IPolicyOverviewProvider _policyOverviewProvider;
        private readonly IProductBrandProvider _productBrandProvider;

        public PlanDetailsService(IPlanOverviewResultProvider planOverviewResultProvider,
            IPlanResponseProvider planResponseProvider,
            IProductDefinitionProvider productDefinitionProvider, 
            IPlanDetailResponseConverter planDetailResponseConverter,
            IPlanEligibilityProvider planEligibilityProvider, IPolicyOverviewProvider policyOverviewProvider, IProductBrandProvider productBrandProvider)
        {
            _planOverviewResultProvider = planOverviewResultProvider;
            _planResponseProvider = planResponseProvider;
            _productDefinitionProvider = productDefinitionProvider;
            _planDetailResponseConverter = planDetailResponseConverter;
            _planEligibilityProvider = planEligibilityProvider;
            _policyOverviewProvider = policyOverviewProvider;
            _productBrandProvider = productBrandProvider;
        }

        public IEnumerable<PlanDetailResponse> GetPlanDetailsForRisk(string quoteReferenceNumber, int riskId)
        {
            var brandKey = _productBrandProvider.GetBrandKeyForQuoteReferenceNumber(quoteReferenceNumber);

            var policyPlans = _planOverviewResultProvider.GetFor(riskId);
            var planResponses =
                _planResponseProvider.MapStoredPlansToProduct(quoteReferenceNumber, _productDefinitionProvider.GetProductDefinition(brandKey).Plans,
                    policyPlans);

            var planEligibilities = _planEligibilityProvider.GetPlanEligibilitiesFor(riskId);

            var planDetailResponses = new List<PlanDetailResponse>();

            foreach (var pr in planResponses)
            {
                var planEl = planEligibilities.First(x => x.PlanCode == pr.Code);

                planDetailResponses.Add(_planDetailResponseConverter.CreateFrom(pr, planEl));
            }

            return planDetailResponses;
        }

        public RiskPlanDetailReposone GetRiskPlanDetailsResponse(string quoteReferenceNumber, int riskId)
        {
            var risk = _policyOverviewProvider.GetFor(quoteReferenceNumber, riskId);
            return new RiskPlanDetailReposone
            {
                Plans = GetPlanDetailsForRisk(quoteReferenceNumber, riskId),
                IsOccupationTpdAny = risk.AvailableDefinitions.Contains(OccupationDefinition.AnyOccupation),
                IsOccupationTpdOwn = risk.AvailableDefinitions.Contains(OccupationDefinition.OwnOccupation)
            };
        }
    }
}