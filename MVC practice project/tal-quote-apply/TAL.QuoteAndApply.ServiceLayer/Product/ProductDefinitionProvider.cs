using System;
using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.DataModel.Service;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.Product.Models.Definition;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters;

namespace TAL.QuoteAndApply.ServiceLayer.Product
{
    public interface IProductDefinitionProvider
    {
        ProductDetailsResult GetProductDefinition(string brandKey);
        PlanResponse GetPlanDefinition(string planCode, string brandKey);
        PremiumTypeDefinition GetPremiumTypeDefinition(PremiumType premiumType, string brandKey);
        IEnumerable<string> GetParentPlanCodes(IEnumerable<string> coverCodes, string brandKey);
        List<string> GetAvailablePremiumFrequencies(string brandKey);
        ExternalCustomerRefConfigSettings GetExternalCustomerRefConfigDetails (string brandKey);
    }

    public class ProductDefinitionProvider : IProductDefinitionProvider
    {
        private readonly IProductDefinitionBuilder _productDefinitionBuilder;
        private readonly IProductDtoConverter _productDtoConverter;
        private readonly IPlanDefinitionProvider _planDefinitionProvider;



        public ProductDefinitionProvider(IProductDefinitionBuilder productDefinitionBuilder, 
            IProductDtoConverter productDtoConverter,
            IPlanDefinitionProvider planDefinitionProvider)
        {
            _productDefinitionBuilder = productDefinitionBuilder;
            _productDtoConverter = productDtoConverter;
            _planDefinitionProvider = planDefinitionProvider;
        }

        public ProductDetailsResult GetProductDefinition(string brandKey)
        {
            var productDefinition = _productDefinitionBuilder.BuildProductDefinition(brandKey);

            var productConverted = _productDtoConverter.CreateFrom(productDefinition);

            return productConverted;
        }

        public PlanResponse GetPlanDefinition(string planCode, string brandKey)
        {
            var planDefinition  = _planDefinitionProvider.GetPlanByCode(planCode, brandKey);
            var planConverted = _productDtoConverter.CreateFrom(planDefinition);

            return planConverted;
        }

        public PremiumTypeDefinition GetPremiumTypeDefinition(PremiumType premiumType, string brandKey)
        {
            var productDefinition = _productDefinitionBuilder.BuildProductDefinition(brandKey);

            var premiumTypeDefinition = productDefinition.PremiumTypes.FirstOrDefault(pt => pt.PremiumType == premiumType);

            if (premiumTypeDefinition == null)
            {
                throw new ApplicationException($"No definition for premium type: {premiumType}");
            }

            return premiumTypeDefinition;
        }

        public IEnumerable<string> GetParentPlanCodes(IEnumerable<string> coverCodes, string brandKey)
        {
            var productDefinition = _productDefinitionBuilder.BuildProductDefinition(brandKey);
            var parentPlanCodes = new List<string>();

            foreach (var coverCode in coverCodes)
            {
                var parentPlan = productDefinition.Plans.SingleOrDefault(p => p.Covers.Any(c => c.Code == coverCode));
                if (parentPlan != null)
                {
                    parentPlanCodes.Add(parentPlan.Code);
                }
            }

            return parentPlanCodes.Distinct();
        }

        public List<string> GetAvailablePremiumFrequencies(string brandKey)
        {
            var productDefinition = _productDefinitionBuilder.BuildProductDefinition(brandKey);
            return productDefinition.AvailablePremiumFrequencies.Select(f => f.ToFriendlyString()).ToList();
        }

        public ExternalCustomerRefConfigSettings GetExternalCustomerRefConfigDetails(string brandKey)
        {
            var productDefinition = _productDefinitionBuilder.BuildProductDefinition(brandKey);
            return productDefinition.ExternalCustomerRefConfigSettings;
        }
    }
}
