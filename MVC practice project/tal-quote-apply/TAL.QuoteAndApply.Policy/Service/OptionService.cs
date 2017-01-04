using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Service
{
    public interface IOptionService
    {
        IOption GetById(int id);
        IOption CreateOption(OptionDto optionDto);
        IEnumerable<OptionDto> GetOptionsForPlan(int planId);
        void UpdateOption(IOption optionDto);
    }
    public class OptionService : IOptionService
    {
        private readonly IOptionDtoRepository _optionDtoRepository;

        public OptionService(IOptionDtoRepository optionDtoRepository)
        {
            _optionDtoRepository = optionDtoRepository;
        }

        IOption IOptionService.GetById(int id)
        {
            return _optionDtoRepository.GetOption(id);
        }

        public IOption CreateOption(OptionDto optionDto)
        {
            return _optionDtoRepository.InsertOption(optionDto);
        }

        public IEnumerable<OptionDto> GetOptionsForPlan(int planId)
        {
            return _optionDtoRepository.GetOptionsForPlan(planId);
        }

        public void UpdateOption(IOption optionDto)
        {
            _optionDtoRepository.UpdateOption((OptionDto)optionDto);
        }
    }
}
