using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Service
{
    public interface ICoverExclusionsService
    {
        void UpdateCoverExclusions(ICover cover, IEnumerable<ICoverExclusion> coverExclusions);
        IEnumerable<ICoverExclusion> GetExclusionsForCover(ICover cover);
    }

    public class CoverExclusionsService : ICoverExclusionsService
    {
        private readonly ICoverExclusionDtoRepository _coverExclusionDtoRepository;

        public CoverExclusionsService(ICoverExclusionDtoRepository coverExclusionDtoRepository)
        {
            _coverExclusionDtoRepository = coverExclusionDtoRepository;
        }

        public void UpdateCoverExclusions(ICover cover, IEnumerable<ICoverExclusion> coverExclusions)
        {
            _coverExclusionDtoRepository.RemoveAllExclusionsForCover(cover.Id);
            _coverExclusionDtoRepository.InsertExclusions(coverExclusions.Cast<CoverExclusionDto>());
        }

        public IEnumerable<ICoverExclusion> GetExclusionsForCover(ICover cover)
        {
            return _coverExclusionDtoRepository.GetAllExclusionsForCover(cover.Id);
        }
    }
}
