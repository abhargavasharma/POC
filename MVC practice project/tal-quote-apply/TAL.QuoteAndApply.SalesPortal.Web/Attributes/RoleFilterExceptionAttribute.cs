using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;

namespace TAL.QuoteAndApply.SalesPortal.Web.Attributes
{
    public class RoleFilterAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        private readonly string _errorMessage;
        private readonly string _section;
        private readonly Role _requiredRole;

        public RoleFilterAttribute(Role requiredRole, string errorMessage = "user role does not have the correct permissions to access this endpoint", string section = "")
        {
            _requiredRole = requiredRole;
            _errorMessage = errorMessage;
            _section = section;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var applicationCurrentUserProvider = DependencyResolver.Current.GetService<ICurrentUserProvider>();
            var user = applicationCurrentUserProvider.GetForApplication();
            
            if (!user.Roles.Contains(_requiredRole))
            {
                var result = new UnauthorisedRoleResult { Result = string.Join(" ", _requiredRole, _errorMessage)};

                actionContext.Response = new HttpResponseMessage()
                {
                    Content = new StringContent(result.ToJson()),
                    ReasonPhrase = _section,
                    StatusCode = HttpStatusCode.BadRequest
                };
            }
        }
    }
}