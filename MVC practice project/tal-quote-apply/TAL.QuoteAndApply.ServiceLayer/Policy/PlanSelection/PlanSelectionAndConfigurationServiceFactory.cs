using System;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Product.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.ServiceLayer.Product;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.PlanSelection
{
    public enum PlanSelectionType
    {
        NotSet,
        LetMePlayDefault
    }

    public interface IPlanSelectionAndConfigurationServiceFactory 
    {
        IPlanSelectionAndConfigurationService GetService(PlanSelectionType planSelectionType);
    }

    public class PlanSelectionAndConfigurationServiceFactory : IPlanSelectionAndConfigurationServiceFactory
    {
        private readonly IPlanService _planService;
        private readonly ICoverService _coverService;
        private readonly IOptionService _optionService;
        private readonly ICoverAmountService _coverAmountService;
        private readonly IPlanEligibilityService _planEligabilityService;
        private readonly ICoverEligibilityService _coverEligibilityService;
        private readonly ICurrentProductBrandProvider _currentProductBrandProvider;

        public PlanSelectionAndConfigurationServiceFactory(IPlanService planService, ICoverService coverService,
            IOptionService optionService, ICoverAmountService coverAmountService, IPlanEligibilityService planEligabilityService, ICoverEligibilityService coverEligibilityService, ICurrentProductBrandProvider currentProductBrandProvider)
        {
            _planService = planService;
            _coverService = coverService;
            _optionService = optionService;
            _coverAmountService = coverAmountService;
            _planEligabilityService = planEligabilityService;
            _coverEligibilityService = coverEligibilityService;
            _currentProductBrandProvider = currentProductBrandProvider;
        }

        public IPlanSelectionAndConfigurationService GetService(PlanSelectionType planSelectionType)
        {
            var productBrand = _currentProductBrandProvider.GetCurrent();

            if (productBrand != null)
            {
                if ((planSelectionType == PlanSelectionType.LetMePlayDefault) &&
                    productBrand.BrandCode.Equals(BrandConstants.Tal, StringComparison.InvariantCultureIgnoreCase))
                {
                    return new LetMePlayTALDefaultPlanSelectionAndConfiguration(_planService, _coverService,
                        _optionService,
                        _coverAmountService, _planEligabilityService, _coverEligibilityService,
                        _currentProductBrandProvider);
                }
                if ((planSelectionType == PlanSelectionType.LetMePlayDefault) &&
                    productBrand.BrandCode.Equals(BrandConstants.YellowBrand,
                        StringComparison.InvariantCultureIgnoreCase))
                {
                    return new LetMePlayYellowBrandDefaultPlanSelectionAndConfiguration(_planService, _coverService,
                        _optionService,
                        _coverAmountService, _planEligabilityService, _coverEligibilityService,
                        _currentProductBrandProvider);
                }
            }
            return new SetNothingPlanSelectionAndConfiguration(_planService, _coverService, _optionService);
        }
    }
}
