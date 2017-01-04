using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.View
{
    public class AgentDashboardViewModel
    {
        public ICurrentUser User { get; set; }

        public BrandSettingsViewModel BrandSettingsViewModel { get; set; }
    }
}