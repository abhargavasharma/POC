using System.Web.Mvc;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.SalesPortal.Web.Attributes;
using TAL.QuoteAndApply.SalesPortal.Web.Configuration;
using TAL.QuoteAndApply.SalesPortal.Web.Models;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.SalesPortal.Web.Models.View;
using TAL.QuoteAndApply.SalesPortal.Web.Services.Converters;
using TAL.QuoteAndApply.SalesPortal.Web.User;
using TAL.QuoteAndApply.ServiceLayer.Search.Clients;
using TAL.QuoteAndApply.SalesPortal.Web.Services;

namespace TAL.QuoteAndApply.SalesPortal.Web.Controllers.View
{
    [MvcSalesPortalSessionRequired]
    public class PolicyController : BaseSalesPortalViewController
    {
        private readonly IBrandExternalRefSettings _brandExternalRefSettings;

        public PolicyController(ISalesPortalUiBrandingHelper brandingHelper,
            IBrandExternalRefSettings brandExternalRefSettings) : base(brandingHelper)
        {
            _brandExternalRefSettings = brandExternalRefSettings;
        }
        public ActionResult Edit(string id, bool? created)
        {
            var externalRefDetails = _brandExternalRefSettings.ExternalCustomerRefSettings(); 
            var model = new EditPolicyInitViewModel
            {
                QuoteReferenceNumber = id,
                QuoteEditSource = created == true ? QuoteEditSource.Created : QuoteEditSource.Retrieved,
                BrandSettingsViewModel =externalRefDetails
            };
            SetUiBrandForQuote(id);
            return View(model);
        }
    }
}