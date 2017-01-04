using System.Collections.Generic;
using System.Data;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Data
{
    public interface IAgentDashboardSearchDtoRepository
    {
        IEnumerable<IAgentDashboardSearchDto> SearchByQuoteReference(string quoteReference);
        IEnumerable<IAgentDashboardSearchDto> SearchByAgentDashboardSearchRequest(AgentDashboardSearchRequest agentDashboardRequest);
    }

    public class AgentDashboardSearchDtoRepository : BaseRepository<AgentDashboardSearchDto>, IAgentDashboardSearchDtoRepository
    {
        public AgentDashboardSearchDtoRepository(IPolicyConfigurationProvider policyConfigurationProvider, ICurrentUserProvider currentUserProvider, IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService) 
            : base(policyConfigurationProvider.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
        }

        public IEnumerable<IAgentDashboardSearchDto> SearchByQuoteReference(string quoteReference)
        {
            return Where(policySearch => policySearch.QuoteReference, Op.Eq, quoteReference);
        }

        public IEnumerable<IAgentDashboardSearchDto> SearchByAgentDashboardSearchRequest(AgentDashboardSearchRequest agentDashboardRequest)
        {
            return Query("spAgentDashboardSearch", new
            {
                agent = agentDashboardRequest.User,
                dateTimeFrom = agentDashboardRequest.DateTimeFrom.ToString("yyyy-MM-dd"),
                dateTimeTo = agentDashboardRequest.DateTimeTo.ToString("yyyy-MM-dd"),
                PipelineStatusUnknown = agentDashboardRequest.Unknown,
                PipelineStatusInProgressPreUw = agentDashboardRequest.InProgressPreUw,
                PipelineStatusInProgressUwReferral = agentDashboardRequest.InProgressUwReferral,
                PipelineStatusInProgressRecommendation = agentDashboardRequest.InProgressRecommendation,
                PipelineStatusInProgressCantContact = agentDashboardRequest.InProgressCantContact,
                PipelineStatusClosedSale = agentDashboardRequest.ClosedSale,
                PipelineStatusClosedNoSale = agentDashboardRequest.ClosedNoSale,
                PipelineStatusClosedTriage = agentDashboardRequest.ClosedTriage,
                PipelineStatusClosedCantContact = agentDashboardRequest.ClosedCantContact,
                PageNumber = agentDashboardRequest.PageNumber,
                PageSize = agentDashboardRequest.PageSize,
                Brand = agentDashboardRequest.BrandId
            }, false, true, null, CommandType.StoredProcedure);
        }
    }
}