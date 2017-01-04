using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataModel.Underwriting;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Service
{
    public interface ICoverLoadingService
    {
        void UpdateLoadingsForCover(ICover cover, IEnumerable<ICoverLoading> coverLoadings);
        IEnumerable<ICoverLoading> GetCoverLoadingsForCover(ICover cover);

        ICoverLoading GetPercentageLoading(IEnumerable<ICoverLoading> allLoadings);
        ICoverLoading GetPerMilleLoading(IEnumerable<ICoverLoading> allLoadings);
    }

    public class CoverLoadingService : ICoverLoadingService
    {
        private readonly ICoverLoadingDtoRepository _coverLoadingDtoRepository;

        public CoverLoadingService(ICoverLoadingDtoRepository coverLoadingDtoRepository)
        {
            _coverLoadingDtoRepository = coverLoadingDtoRepository;
        }

        public void UpdateLoadingsForCover(ICover cover, IEnumerable<ICoverLoading> coverLoadings)
        {
            _coverLoadingDtoRepository.RemoveAllLoadingsForCover(cover.Id);
            _coverLoadingDtoRepository.InsertLoadings(coverLoadings.Cast<CoverLoadingDto>());
        }

        public IEnumerable<ICoverLoading> GetCoverLoadingsForCover(ICover cover)
        {
            return _coverLoadingDtoRepository.GetAllLoadingsForCover(cover.Id);
        }

        public ICoverLoading GetPercentageLoading(IEnumerable<ICoverLoading> allLoadings)
        {
            return allLoadings.FirstOrDefault(l => l.LoadingType == LoadingType.Variable);
        }

        public ICoverLoading GetPerMilleLoading(IEnumerable<ICoverLoading> allLoadings)
        {
            return allLoadings.FirstOrDefault(l => l.LoadingType == LoadingType.PerMille);
        }
    }
}
