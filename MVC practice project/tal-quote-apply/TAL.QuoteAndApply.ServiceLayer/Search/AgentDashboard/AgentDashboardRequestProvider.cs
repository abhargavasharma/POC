using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Search.AgentDashboard.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Search.AgentDashboard
{
    public interface IAgentDashboardRequestProvider
    {
        AgentDashboardSearchRequest From(AgentDashboardRequest agentDashboardRequest, int pageSize);
    }

    public class AgentDashboardRequestProvider : IAgentDashboardRequestProvider
    {
        private readonly IProductBrandProvider _brandProvider;

        public AgentDashboardRequestProvider(IProductBrandProvider brandProvider)
        {
            _brandProvider = brandProvider;
        }

        public AgentDashboardSearchRequest From(AgentDashboardRequest agentDashboardRequest, int pageSize)
        {
            return new AgentDashboardSearchRequest
            {
                User = agentDashboardRequest.User,
                Unknown = agentDashboardRequest.Unknown,
                ClosedCantContact = agentDashboardRequest.ClosedCantContact,
                ClosedNoSale = agentDashboardRequest.ClosedNoSale,
                ClosedSale = agentDashboardRequest.ClosedSale,
                ClosedTriage = agentDashboardRequest.ClosedTriage,
                DateTimeFrom = agentDashboardRequest.StartDate,
                DateTimeTo = agentDashboardRequest.EndDate,
                InProgressCantContact = agentDashboardRequest.InProgressCantContact,
                InProgressUwReferral = agentDashboardRequest.InProgressUwReferral,
                InProgressPreUw = agentDashboardRequest.InProgressPreUw,
                InProgressRecommendation = agentDashboardRequest.InProgressRecommendation,
                PageSize = pageSize,
                PageNumber = agentDashboardRequest.PageNumber,
                BrandId = _brandProvider.GetBrandIdByKey(agentDashboardRequest.Brand)
            };
        }
    }
}
