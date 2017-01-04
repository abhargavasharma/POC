using System.Linq;
using System.Web.Mvc;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.SalesPortal.Web.Attributes;
using TAL.QuoteAndApply.SalesPortal.Web.Configuration;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.SalesPortal.Web.Models.View;
using TAL.QuoteAndApply.SalesPortal.Web.Services.Converters;
using TAL.QuoteAndApply.SalesPortal.Web.User;
using TAL.QuoteAndApply.ServiceLayer.Search.Clients;
using TAL.QuoteAndApply.SalesPortal.Web.Services;

namespace TAL.QuoteAndApply.SalesPortal.Web.Controllers.View
{
    [MvcSalesPortalSessionRequired]
    public class ClientController : BaseSalesPortalViewController
    {
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IBrandExternalRefSettings _brandExternalRefSettings;
        public ClientController(ISalesPortalUiBrandingHelper brandingHelper, 
            ICurrentUserProvider currentUserProvider,
            IBrandExternalRefSettings brandExternalRefSettings
            ): base(brandingHelper)
        {
            _currentUserProvider = currentUserProvider;
            _brandExternalRefSettings = brandExternalRefSettings;
        }

        // GET: Client
        public ActionResult Create()
        {
            return View();
        }

        
        public ActionResult Search()
        {
            var externalRefDetails = _brandExternalRefSettings.ExternalCustomerRefSettings();

            var model = new ClientSearchViewModel 
            {
                UserRole = _currentUserProvider.GetForApplication().Roles.Contains(Role.Agent)
                            ? Role.Agent.ToString() : Role.ReadOnly.ToString(),
                BrandSettingsViewModel = externalRefDetails
            };

            return View(model);

        }
    }
}