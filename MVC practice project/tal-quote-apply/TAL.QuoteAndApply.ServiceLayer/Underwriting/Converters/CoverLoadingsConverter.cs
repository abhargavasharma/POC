using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.Underwriting;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Underwriting.Models.Dto;

namespace TAL.QuoteAndApply.ServiceLayer.Underwriting.Converters
{
    public interface ICoverLoadingsConverter
    {
        IEnumerable<ICoverLoading> From(ICover cover, ReadOnlyTotalLoadings totalLoadings);
    }

    public class CoverLoadingsConverter : ICoverLoadingsConverter
    {
        public IEnumerable<ICoverLoading> From(ICover cover, ReadOnlyTotalLoadings totalLoadings)
        {
            var loadings = new List<ICoverLoading>();

            if (totalLoadings == null)
            {
                return loadings;
            }

            if (totalLoadings.Variable > 0)
            {
                loadings.Add(new CoverLoadingDto {CoverId = cover.Id, LoadingType = LoadingType.Variable, Loading = totalLoadings.Variable});
            }

            if (totalLoadings.Fixed > 0)
            {
                loadings.Add(new CoverLoadingDto { CoverId = cover.Id, LoadingType = LoadingType.Fixed, Loading = totalLoadings.Fixed });
            }

            if (totalLoadings.PerMille > 0)
            {
                loadings.Add(new CoverLoadingDto { CoverId = cover.Id, LoadingType = LoadingType.PerMille, Loading = totalLoadings.PerMille });
            }

            return loadings;
        }
    }
}