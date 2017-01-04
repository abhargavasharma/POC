using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Search.AgentDashboard.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services.Converters
{
    public interface IAgentDashboardResultConverter
    {
        AgentDashboardDetailsResponse From(IEnumerable<AgentDashboardQuotesResult> agentDashboardQuotesResult, int currentPage, int pageSize);
    }

    public class AgentDashboardResultConverter : IAgentDashboardResultConverter
    {
        public AgentDashboardDetailsResponse From(IEnumerable<AgentDashboardQuotesResult> agentDashboardQuotesResult, int currentPage, int pageSize)
        {
            var totalRecords = 0;
            var pageCount = 0;
            
            if (agentDashboardQuotesResult.Any())
            {
                totalRecords = agentDashboardQuotesResult.First().TotalRecords;
                pageCount = (int)Math.Ceiling((double)totalRecords / pageSize); ;
            }

            var quotes = agentDashboardQuotesResult.Select(x => new AgentDashboardQuoteResult
            {
                FullName = x.FullName,
                LastTouchedByTS = x.LastTouchedByTS,
                Plans = x.Plans,
                PolicyId = x.PolicyId,
                Premium = x.Premium,
                QuoteReference = x.QuoteReference,
                Progress = x.Progress,
                ProgressUpdateTs = x.ProgressUpdateTs,
                ExternalCustomerReference = x.ExternalCustomerReference,
                Brand = x.Brand
            }).ToList();
            return new AgentDashboardDetailsResponse(){ Quotes = quotes, TotalRecords = totalRecords, CurrentPage = currentPage, PageCount = pageCount };
        }
    }
}