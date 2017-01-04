using System.Web.Http;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.SalesPortal.Web.Attributes;
using TAL.QuoteAndApply.SalesPortal.Web.Services.Converters;
using TAL.QuoteAndApply.SalesPortal.Web.User;
using TAL.QuoteAndApply.ServiceLayer.Referral;
using TAL.QuoteAndApply.ServiceLayer.Search.AgentDashboard;
using TAL.QuoteAndApply.ServiceLayer.Search.AgentDashboard.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Controllers.Api
{
    [WebApiSalesPortalSessionRequired]
    [RoutePrefix("api/dashboard")]
    public class DashboardController : ApiController
    {
        private readonly IGetReferralsService _getReferralsService;
        private readonly IReferralDetailsResultConverter _referralDetailsResultConverter;
        private readonly IGetUnderwritersService _getUnderwritersService;
        private readonly IGetAgentDashboardQuotesService _getAgentDashboardQuotesService;
        private readonly ISalesPortalSessionContext _salesPortalSessionContext;
        private readonly IAgentDashboardResultConverter _agentDashboardResultConverter;
        private readonly ICurrentUserProvider _currentUserProvider;

        public DashboardController(IGetReferralsService getReferralsService, IReferralDetailsResultConverter referralDetailsResultConverter, IGetUnderwritersService getUnderwritersService, IGetAgentDashboardQuotesService getAgentDashboardQuotesService, IAgentDashboardResultConverter agentDashboardResultConverter, ICurrentUserProvider currentUserProvider, ISalesPortalSessionContext salesPortalSessionContext)
        {
            _getReferralsService = getReferralsService;
            _referralDetailsResultConverter = referralDetailsResultConverter;
            _getUnderwritersService = getUnderwritersService;
            _getAgentDashboardQuotesService = getAgentDashboardQuotesService;
            _agentDashboardResultConverter = agentDashboardResultConverter;
            _currentUserProvider = currentUserProvider;
            _salesPortalSessionContext = salesPortalSessionContext;
        }

        [RoleFilter(Role.Underwriter)]
        [HttpGet, Route("referrals")]
        public IHttpActionResult GetReferrals()
        {
            var results = _referralDetailsResultConverter.From(_getReferralsService.GetAll(), _getUnderwritersService.GetAll());
            return Ok(results);
        }

        [RoleFilter(Role.Underwriter)]
        [HttpPost, Route("searchReferrals")]
        public IHttpActionResult SearchReferrals()
        {
            //todo implement this for server-side filtering
            return Ok();
        }

        [RoleFilter(Role.Agent)]
        [HttpPost, Route("quotes")]
        public IHttpActionResult Quotes(AgentDashboardRequest agentDashboardRequest)
        {
            const int pageSize = 10;

            var user = _currentUserProvider.GetForApplication();
            agentDashboardRequest.User = user.UserName;
            agentDashboardRequest.Brand = _salesPortalSessionContext.SalesPortalSession.SelectedBrand;

            var results = _agentDashboardResultConverter.From(_getAgentDashboardQuotesService.GetQuotes(agentDashboardRequest, pageSize), agentDashboardRequest.PageNumber, pageSize);
            return Ok(results);
        }
    }
}