using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using TAL.QuoteAndApply.Product.Models.Definition;

namespace TAL.QuoteAndApply.Product.Definition
{
    public interface ICoverDefinitionProvider
    {
        CoverDefinition GetCoverDefinitionByCode(string coverCode, string brandKey);
    }

    public class CoverDefinitionProvider : ICoverDefinitionProvider
    {
        private readonly IProductDefinitionBuilder _productDefinitionBuilder;

        public CoverDefinitionProvider(IProductDefinitionBuilder productDefinitionBuilder)
        {
            _productDefinitionBuilder = productDefinitionBuilder;
        }

        public CoverDefinition GetCoverDefinitionByCode(string coverCode, string brandKey)
        {
            var productDefinition = _productDefinitionBuilder.BuildProductDefinition(brandKey);
            var planCovers = productDefinition.Plans.SelectMany(p => p.Covers).ToList();
            var riders = productDefinition.Plans.Where(p=> p.Riders != null).SelectMany(p => p.Riders).ToList();
            var riderCovers = riders.SelectMany(r => r.Covers).ToList();

            var allCovers = new List<CoverDefinition>();
            allCovers.AddRange(planCovers);
            allCovers.AddRange(riderCovers);

            var coverDefinition = allCovers.FirstOrDefault(x => x.Code.Equals(coverCode, StringComparison.OrdinalIgnoreCase));

            if (coverDefinition != null)
            {
                return coverDefinition;
            }

            throw new InstanceNotFoundException($"Could not locate cover '{coverCode}' in product definition");
        }
    }
}