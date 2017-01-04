using System.Collections.Generic;
using TAL.QuoteAndApply.Interactions.Data;
using TAL.QuoteAndApply.Interactions.Models;

namespace TAL.QuoteAndApply.Interactions.Service
{
    public interface IInteractionsService
    {
        IPolicyInteraction CreateInteraction(IPolicyInteraction policyInteractionDto);
        IPolicyInteraction GetById (int id);
        IEnumerable<IPolicyInteraction> GetInteractionsByPolicyId(int policyId);
        void UpdateInteraction(IPolicyInteraction policyInteractionDto);
    }
    public class InteractionsService : IInteractionsService
    {
        private readonly IPolicyInteractionDtoRepository _policyInteractionDtoRepository;

        public InteractionsService(IPolicyInteractionDtoRepository policyInteractionDtoRepository)
        {
            _policyInteractionDtoRepository = policyInteractionDtoRepository;
        }

        public IPolicyInteraction CreateInteraction (IPolicyInteraction policyInteractionDto)
        {
            return _policyInteractionDtoRepository.InsertInteraction((PolicyInteractionDto) policyInteractionDto);
        }

        public IPolicyInteraction GetById(int id)
        {
            return _policyInteractionDtoRepository.GetInteraction(id);
        }
        public IEnumerable<IPolicyInteraction> GetInteractionsByPolicyId(int id)
        {
            return _policyInteractionDtoRepository.GetInteractionsByPolicyId(id);
        }
        public void UpdateInteraction(IPolicyInteraction policyInteractionDto)
        {
            _policyInteractionDtoRepository.UpdateInteraction((PolicyInteractionDto)policyInteractionDto);
        }
    }
}
