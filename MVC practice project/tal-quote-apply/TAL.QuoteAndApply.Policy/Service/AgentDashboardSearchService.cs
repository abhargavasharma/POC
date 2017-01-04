using System.Collections.Generic;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Service
{
    public interface IAgentDashboardSearchService
    {
        IEnumerable<IAgentDashboardSearchDto> SearchByAgentDashboardRequest(AgentDashboardSearchRequest agentDashboardRequest);
    }

    public class AgentDashboardSearchService : IAgentDashboardSearchService
    {
        private readonly IAgentDashboardSearchDtoRepository _agentDashboardSearchDtoRepository;

        public AgentDashboardSearchService(IAgentDashboardSearchDtoRepository agentDashboardSearchDtoRepository)
        {
            _agentDashboardSearchDtoRepository = agentDashboardSearchDtoRepository;
        }

        public IEnumerable<IAgentDashboardSearchDto> SearchByAgentDashboardRequest(AgentDashboardSearchRequest agentDashboardRequest)
        {
            return _agentDashboardSearchDtoRepository.SearchByAgentDashboardSearchRequest(agentDashboardRequest);
        }
    }
}
