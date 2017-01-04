using System.Collections.Generic;
using System.Linq;
using System.Web.Http.ModelBinding;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Validation;

namespace TAL.QuoteAndApply.Customer.Web.Services
{
    public interface ICustomerReviewValidationService
    {
        void ValidateReview(string quoteReference, int riskId, ModelStateDictionary modelState);
        IReadOnlyList<ValidationError> ValidatePremiumType(string quoteReference, int riskId, PremiumType premiumType);
    }

    public class CustomerReviewValidationService : ICustomerReviewValidationService
    {
        private readonly IPlanDetailsService _planDetailsService;
        private readonly IPlanRulesService _planRulesService;
        private readonly IRiskRatingFactorsProvider _riskRatingFactorsProvider;
        private readonly IProductDefinitionProvider _productDefinitionProvider;
        private readonly ICurrentProductBrandProvider _currentProductBrandProvider;

        public CustomerReviewValidationService(IPlanRulesService planRulesService,
            IRiskRatingFactorsProvider riskRatingFactorsProvider, IProductDefinitionProvider productDefinitionProvider,
            IPlanDetailsService planDetailsService, ICurrentProductBrandProvider currentProductBrandProvider)
        {
            _planRulesService = planRulesService;
            _riskRatingFactorsProvider = riskRatingFactorsProvider;
            _productDefinitionProvider = productDefinitionProvider;
            _planDetailsService = planDetailsService;
            _currentProductBrandProvider = currentProductBrandProvider;
        }

        public void ValidateReview(string quoteReference, int riskId, ModelStateDictionary modelState)
        {

            var plans = _planDetailsService.GetPlanDetailsForRisk(quoteReference, riskId);

            var noAvailablePlans = plans.Plans.All(p => !p.Availability.IsAvailable);

            if (noAvailablePlans)
            {
                return;
            }

            var plansWithoutAnySelectedCovers = plans.Plans.Where(p => p.IsSelected && !p.Covers.Any(c => c.IsSelected))
                .Select(p => p.PlanCode).ToArray();

            if (plans.Plans.All(p => !p.IsSelected))
            {
                modelState.AddModelError("minimumSelectionCriteria", "Sorry, to have this insurance added to your quote you must have more than $0 of cover. Please reconsider your selection and resubmit, or call 131 825 or Click-To-Chat to get in touch with one of our Life Insurance specialists.");
                return;
            }

            if (plansWithoutAnySelectedCovers.Any())
            {
                modelState.AddModelError("minimumSelectionCriteria", "Sorry, to have this insurance added to your quote you must select at least one of its cover options. Please reconsider your selection and resubmit, or call 131 825 or Click-To-Chat to get in touch with one of our Life Insurance specialists.");
                foreach (var planCode in plansWithoutAnySelectedCovers)
                {
                    modelState.AddModelError(planCode.ToLower(), "You must include at least one of the cover options below");
                }
            }
        }

        public IReadOnlyList<ValidationError> ValidatePremiumType(string quoteReference, int riskId, PremiumType premiumType)
        {
            if (!IsValidPremiumType(premiumType))
            {
                return new List<ValidationError>
                {
                    new ValidationError(null, ValidationKey.EligiblePremiumType, "Invalid Premium Type",
                        ValidationType.Error, null)
                };
            }

            var riskRatingFactors = _riskRatingFactorsProvider.GetFor(quoteReference, riskId);
            var currentBrand = _currentProductBrandProvider.GetCurrent();
            var errors = _planRulesService.ValidatePlanPremiumType(premiumType, riskRatingFactors.DateOfBirth.Value, currentBrand.BrandCode);
            return errors.ToList();
        }

        private bool IsValidPremiumType(PremiumType premiumType)
        {
            var currentBrand = _currentProductBrandProvider.GetCurrent();
            var productDefinition = _productDefinitionProvider.GetProductDefinition(currentBrand.BrandCode);
            return productDefinition.PremiumTypes.Any(pt => pt.IsUserSelectable && pt.PremiumType == premiumType);
        }
    }
}