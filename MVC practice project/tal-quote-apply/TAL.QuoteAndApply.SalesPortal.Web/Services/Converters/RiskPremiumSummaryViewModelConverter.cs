using System.Linq;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Product;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services.Converters
{
    public interface IRiskPremiumSummaryViewModelConverter
    {
        RiskPremiumSummaryViewModel CreateFrom(int riskId, PolicyPremiumSummary policyPremiumSummary);
    }

    public class RiskPremiumSummaryViewModelConverter : IRiskPremiumSummaryViewModelConverter
    {
        private readonly IProductDefinitionProvider _productDefinitionProvider;
        private readonly IProductBrandProvider _productBrandProvider;

        public RiskPremiumSummaryViewModelConverter(IProductDefinitionProvider productDefinitionProvider, IProductBrandProvider productBrandProvider)
        {
            _productDefinitionProvider = productDefinitionProvider;
            _productBrandProvider = productBrandProvider;
        }

        public RiskPremiumSummaryViewModel CreateFrom(int riskId, PolicyPremiumSummary policyPremiumSummary)
        {
            var riskPremiumSummary = policyPremiumSummary.RiskPremiums.FirstOrDefault(r => r.RiskId == riskId);

            if (riskPremiumSummary == null)
            {
                return null;
            }

            var brandKey = _productBrandProvider.GetBrandKeyForRiskId(riskId);

            return new RiskPremiumSummaryViewModel(riskPremiumSummary.RiskPremium, riskPremiumSummary.PremiumFrequency.ToString(), riskPremiumSummary.MultiPlanDiscount,
                riskPremiumSummary.PlanPremiums.Where(pp=> pp.PlanSelected && pp.PlanPremium > 0).Select(pp => 
                new PlanPremiumSummaryViewModel(_productDefinitionProvider.GetPlanDefinition(pp.PlanCode, brandKey).ShortName, pp.PlanPremium, pp.CoverAmount, pp.IsRider)).ToList());
        }
    }
}