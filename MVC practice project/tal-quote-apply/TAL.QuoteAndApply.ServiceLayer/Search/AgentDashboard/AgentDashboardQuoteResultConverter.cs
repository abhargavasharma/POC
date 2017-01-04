using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Product.Service;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Search.AgentDashboard.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Search.AgentDashboard
{
    public interface IAgentDashboardQuoteResultConverter
    {
        AgentDashboardQuotesResult From(IAgentDashboardSearchDto agentDashboardQuotesResult, string brandKey);
    }
        
    public class AgentDashboardQuoteResultConverter : IAgentDashboardQuoteResultConverter
    {
        private readonly INameLookupService _nameLookupService;
        private readonly IProductBrandProvider _brandProvider;

        public AgentDashboardQuoteResultConverter(INameLookupService nameLookupService, IProductBrandProvider brandProvider)
        {
            _nameLookupService = nameLookupService;
            _brandProvider = brandProvider;
        }

        public AgentDashboardQuotesResult From(IAgentDashboardSearchDto agentDashboardQuotesResult, string brandKey)
        {
            IEnumerable<string> planShortNames = new string[] {};
            if (!string.IsNullOrWhiteSpace(agentDashboardQuotesResult.PlanCodes))
            {
                var planCodes = agentDashboardQuotesResult.PlanCodes.Split(',');
                planShortNames = planCodes.Select(c => _nameLookupService.GetPlanShortName(c.Trim(), brandKey));
            }

            return new AgentDashboardQuotesResult
            {
                FullName = agentDashboardQuotesResult.OwnerFullName,
                LastTouchedByTS = agentDashboardQuotesResult.LastTouchedByTS,
                Plans = string.Join(", ", planShortNames),
                PolicyId = agentDashboardQuotesResult.PolicyId,
                Premium = agentDashboardQuotesResult.Premium,
                QuoteReference = agentDashboardQuotesResult.QuoteReference,
                Progress = agentDashboardQuotesResult.LastPipelineStatusDescription,
                ProgressUpdateTs = agentDashboardQuotesResult.LastPipelineStatusTS,
                TotalRecords = agentDashboardQuotesResult.TotalRecords,
                ExternalCustomerReference = agentDashboardQuotesResult.OwnerExternalCustomerReference,
                Brand = _brandProvider.GetBrandKeyByBrandId(agentDashboardQuotesResult.BrandId)
            };
        }
    }
}
