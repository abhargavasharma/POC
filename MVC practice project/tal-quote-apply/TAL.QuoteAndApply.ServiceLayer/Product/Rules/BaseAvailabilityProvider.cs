using System.Collections.Generic;
using System.Linq;
using System.Monads;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.Product.Models.Definition;
using TAL.QuoteAndApply.Product.Rules.Common;
using TAL.QuoteAndApply.Product.Service;
using TAL.QuoteAndApply.ServiceLayer.Models;
using TAL.QuoteAndApply.ServiceLayer.Models.Mappers;

namespace TAL.QuoteAndApply.ServiceLayer.Product.Rules
{
    public abstract class BaseAvailabilityProvider
    {
        protected readonly IProductDefinitionBuilder _productDefinitionBuilder;
        protected readonly ISelectedProductPlanOptionsConverter _selectedProductPlanOptionsConverter;
        protected readonly INameLookupService _nameLookupService;

        protected BaseAvailabilityProvider(IProductDefinitionBuilder productDefinitionBuilder,
            ISelectedProductPlanOptionsConverter selectedProductPlanOptionsConverter, INameLookupService nameLookupService)
        {
            _productDefinitionBuilder = productDefinitionBuilder;
            _selectedProductPlanOptionsConverter = selectedProductPlanOptionsConverter;
            _nameLookupService = nameLookupService;
        }

        public IEnumerable<AvailableFeature> GetResult(AvailabilityPlanStateParam planOptionsParam)
        {
            var availabilityItems = GetAvailabilityItems(planOptionsParam);
            return GetResultForAvailabiltyItems(availabilityItems, planOptionsParam);
        }

        protected IEnumerable<AvailableFeature> GetResultForAvailabiltyItems(IEnumerable<IAvailability> availabilityItems,
            AvailabilityPlanStateParam planOptionsParam)
        {
            if (availabilityItems != null)
            {
                foreach (var availabilityItem in availabilityItems)
                {
                    var rule = new AvailabilityRule(availabilityItem, _nameLookupService);
                    var selectedPlanOptions = _selectedProductPlanOptionsConverter.From(planOptionsParam);
                    var ruleResult = rule.IsSatisfiedBy(selectedPlanOptions);
                    if (ruleResult)
                    {
                        yield return AvailableFeature.Available(availabilityItem.Code);
                    }
                    else
                    {
                        yield return AvailableFeature.Unavailable(availabilityItem.Code, ruleResult.Messages);
                    }
                }
            }
        }

        protected IEnumerable<AvailableFeature> GetAllAvailabiltyItems(
            IEnumerable<IAvailability> availabilityItems)
        {
            return availabilityItems.Return(options => options.Select(o => AvailableFeature.Available(o.Code)),
               new List<AvailableFeature>());
        }

        protected IEnumerable<AvailableFeature> GetAllItems(AvailabilityPlanStateParam planOptionsParam)
        {
            var availabilityItems = GetAvailabilityItems(planOptionsParam);
            return GetAllAvailabiltyItems(availabilityItems);
        }

        public abstract IEnumerable<IAvailability> GetAvailabilityItems(AvailabilityPlanStateParam planOptionsParam);
    }
}