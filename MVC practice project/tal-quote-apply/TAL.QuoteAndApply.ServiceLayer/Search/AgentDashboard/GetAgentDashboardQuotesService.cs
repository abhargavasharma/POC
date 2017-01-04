using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Search.AgentDashboard.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Search.AgentDashboard
{
    public interface IGetAgentDashboardQuotesService
    {
        IEnumerable<AgentDashboardQuotesResult> GetQuotes(AgentDashboardRequest agentDashboardRequest, int pageSize);
    }
    public class GetAgentDashboardQuotesService : IGetAgentDashboardQuotesService
    {
        private readonly IAgentDashboardSearchService _agentDashboardSearchService;
        private readonly IAgentDashboardQuoteResultConverter _agentDashboardQuoteResultConverter;
        private readonly IAgentDashboardRequestProvider _agentDashboardRequestProvider;
        private readonly ICurrentProductBrandProvider _currentProductBrandProvider;

        public GetAgentDashboardQuotesService(IAgentDashboardSearchService agentDashboardSearchService, 
            IAgentDashboardQuoteResultConverter agentDashboardQuoteResultConverter, 
            IAgentDashboardRequestProvider agentDashboardRequestProvider, 
			ICurrentProductBrandProvider currentProductBrandProvider)
        {
            _agentDashboardSearchService = agentDashboardSearchService;
            _agentDashboardQuoteResultConverter = agentDashboardQuoteResultConverter;
            _agentDashboardRequestProvider = agentDashboardRequestProvider;
            _currentProductBrandProvider = currentProductBrandProvider;
        }

        public IEnumerable<AgentDashboardQuotesResult> GetQuotes(AgentDashboardRequest agentDashboardRequest, int pageSize)
        {
            var searchRequest = _agentDashboardRequestProvider.From(agentDashboardRequest, pageSize);
            var searchResults = _agentDashboardSearchService.SearchByAgentDashboardRequest(searchRequest);

            var brandKey = _currentProductBrandProvider.GetCurrent().BrandCode;

            return searchResults.Select(x => _agentDashboardQuoteResultConverter.From(x, brandKey)).ToList();
        }
    }
}
