using System;
using System.Linq;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Product;

namespace TAL.QuoteAndApply.ServiceLayer.Underwriting
{
    public interface IGetUnderwritingBenefitCodeForCoverService
    {
        string GetUnderwritingBenefitCodeFrom(ICover cover);
    }

    public class GetUnderwritingBenefitCodeForCoverService : IGetUnderwritingBenefitCodeForCoverService
    {
        private readonly IProductDefinitionProvider _productDefinitionProvider;
        private readonly IProductBrandProvider _productBrandProvider;

        public GetUnderwritingBenefitCodeForCoverService(IProductDefinitionProvider productDefinitionProvider, IProductBrandProvider productBrandProvider)
        {
            _productDefinitionProvider = productDefinitionProvider;
            _productBrandProvider = productBrandProvider;
        }

        public string GetUnderwritingBenefitCodeFrom(ICover cover)
        {
            var brandKey = _productBrandProvider.GetBrandKeyForCover(cover);

            var productDefinition = _productDefinitionProvider.GetProductDefinition(brandKey);
            var allCoverDefinitions = productDefinition.Plans.SelectMany(x => x.Covers).ToList();
            var riderCoverDefinitions = productDefinition.Plans.SelectMany(y => y.Riders).SelectMany(z => z.Covers).ToList();
            allCoverDefinitions.AddRange(riderCoverDefinitions);

            var coverDefinition = allCoverDefinitions.FirstOrDefault(cd => cd.Code == cover.Code);

            if (coverDefinition == null)
            {
                throw new ApplicationException($"The cover code: {cover.Code} does not exist in the product definition");
            }

            return coverDefinition.UnderwritingBenefitCode;
        }
    }
}