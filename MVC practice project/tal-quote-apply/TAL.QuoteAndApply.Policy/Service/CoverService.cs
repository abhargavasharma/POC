using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Service
{
    public interface ICoverService
    {
        ICover GetById(int id);
        ICover CreateCover(ICover coverDto);
        IEnumerable<ICover> GetCoversForPlan(int planId);
        void UpdateCover(ICover coverDto);
    }
    public class CoverService : ICoverService
    {
        private readonly ICoverDtoRepository _coverDtoRepository;

        public CoverService(ICoverDtoRepository coverDtoRepository)
        {
            _coverDtoRepository = coverDtoRepository;
        }

        public ICover GetById(int id)
        {
            return _coverDtoRepository.GetCover(id);
        }

        public ICover CreateCover(ICover coverDto)
        {
            return _coverDtoRepository.InsertCover((CoverDto)coverDto);
        }

        public IEnumerable<ICover> GetCoversForPlan(int planId)
        {
            return _coverDtoRepository.GetCoversForPlan(planId);
        }

        public void UpdateCover(ICover coverDto)
        {
            _coverDtoRepository.UpdateCover((CoverDto)coverDto);
        }
    }
}
