using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Customer.Web.Extensions;
using TAL.QuoteAndApply.Customer.Web.Models.Api;
using TAL.QuoteAndApply.Customer.Web.Services.Converters;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.ServiceLayer.Policy.PremiumCalculation;
using TAL.QuoteAndApply.ServiceLayer.Product;

namespace TAL.QuoteAndApply.Customer.Web.Services
{
    public interface IPlanDetailsService
    {
        GetPlanResponse GetPlanDetailsForRisk(string quoteReferenceNumber, int riskId);
    }

    public class PlanDetailsService : IPlanDetailsService
    {
        private readonly IPlanOverviewResultProvider _planOverviewResultProvider;
        private readonly IPlanResponseProvider _planResponseProvider;
        private readonly IProductDefinitionProvider _productDefinitionProvider;
        private readonly IPlanOptionParamConverter _planOptionParamConverter;
        private readonly IAvailablePlanOptionsProvider _availablePlanOptionsProvider;
        private readonly IPolicyPremiumCalculation _policyPremiumCalculation;
        private readonly IRiskRatingFactorsProvider _riskRatingFactorsProvider;
        private readonly IPlanRulesService _planRulesService;
        private readonly IPlanEligibilityProvider _planEligibilityProvider;
        private readonly IPolicyOverviewProvider _policyOverviewProvider;
        private readonly IProductBrandProvider _productBrandProvider;

        public PlanDetailsService(IPlanOverviewResultProvider planOverviewResultProvider,
            IPlanResponseProvider planResponseProvider,
            IProductDefinitionProvider productDefinitionProvider,
            IAvailablePlanOptionsProvider availablePlanOptionsProvider,
            IPlanOptionParamConverter planOptionParamConverter, IPolicyPremiumCalculation policyPremiumCalculation,
            IRiskRatingFactorsProvider riskRatingFactorsProvider,
            IPlanRulesService planRulesService,
            IPlanEligibilityProvider planEligibilityProvider, IPolicyOverviewProvider policyOverviewProvider, IProductBrandProvider productBrandProvider)
        {
            _planOverviewResultProvider = planOverviewResultProvider;
            _planResponseProvider = planResponseProvider;
            _productDefinitionProvider = productDefinitionProvider;
            _availablePlanOptionsProvider = availablePlanOptionsProvider;
            _planOptionParamConverter = planOptionParamConverter;
            _policyPremiumCalculation = policyPremiumCalculation;
            _riskRatingFactorsProvider = riskRatingFactorsProvider;
            _planRulesService = planRulesService;
            _planEligibilityProvider = planEligibilityProvider;
            _policyOverviewProvider = policyOverviewProvider;
            _productBrandProvider = productBrandProvider;
        }

        public GetPlanResponse GetPlanDetailsForRisk(string quoteReferenceNumber, int riskId)
        {
            var brandKey = _productBrandProvider.GetBrandKeyForQuoteReferenceNumber(quoteReferenceNumber);
            var productDefinition = _productDefinitionProvider.GetProductDefinition(brandKey);
            var policyPlans = _planOverviewResultProvider.GetFor(riskId).ToArray();
            var planResponses = _planResponseProvider.MapStoredPlansToProduct(quoteReferenceNumber, productDefinition.Plans, policyPlans)
                .ToArray();

            var selectedPlanOptions = _planOptionParamConverter.From(riskId, brandKey, planResponses);
            var availability = _availablePlanOptionsProvider.GetForAllPlans(selectedPlanOptions);

            var defaultSelectedPremiumType = policyPlans.DefaultCustomerPremiumType();
            var availablePremiumTypes = GetPremiumTypeOptions(quoteReferenceNumber, riskId, defaultSelectedPremiumType,
                productDefinition.PremiumTypes);

            var planEligibilities = _planEligibilityProvider.GetPlanEligibilitiesFor(riskId);

            var riskOverview = _policyOverviewProvider.GetFor(quoteReferenceNumber, riskId);

            return _planOptionParamConverter.From(planResponses, availability, availablePremiumTypes, planEligibilities, riskOverview);
        }

        private PremiumTypeOptions GetPremiumTypeOptions(string quoteReferenceNumber, int riskId,
            PremiumType defaultSelectedPremiumType, IEnumerable<PremiumTypeResponse> premiumTypeDefinintions)
        {
            var brandKey = _productBrandProvider.GetBrandKeyForQuoteReferenceNumber(quoteReferenceNumber);
            var premiumTypeOptions = new PremiumTypeOptions {AvailablePremiumTypes = new List<AvailablePremiumType>()};

            var ratingFactors = _riskRatingFactorsProvider.GetFor(quoteReferenceNumber, riskId);
            var riskDateOfBirth = ratingFactors.DateOfBirth.Value;

            foreach (var premiumType in premiumTypeDefinintions)
            {
                if (!premiumType.IsUserSelectable)
                    continue;

                if (premiumType.PremiumType == defaultSelectedPremiumType)
                {
                    premiumTypeOptions.SelectedPremiumType = premiumType.PremiumType.ToString();
                }

                var premiumSummary = _policyPremiumCalculation.CalculateForPremiumType(quoteReferenceNumber,
                    premiumType.PremiumType);
                var totalPremium = premiumSummary.RiskPremiums.Single(r => r.RiskId == riskId).RiskPremium;

                var isEnabled = !_planRulesService.ValidatePlanPremiumType(premiumType.PremiumType, riskDateOfBirth, brandKey).Any();

                premiumTypeOptions.AvailablePremiumTypes.Add(new AvailablePremiumType(premiumType.Name,
                    premiumType.PremiumType.ToString(), totalPremium, isEnabled));
            }

            return premiumTypeOptions;
        }
    }
}