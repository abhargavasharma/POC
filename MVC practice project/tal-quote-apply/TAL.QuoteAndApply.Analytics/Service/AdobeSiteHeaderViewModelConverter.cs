using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAL.QuoteAndApply.Analytics.Configuration;
using TAL.QuoteAndApply.Analytics.Models;

namespace TAL.QuoteAndApply.Analytics.Service
{
    public interface IAdobeHeaderViewModelConverter
    {
        AdobeHeaderViewModel GetFor(AnalyticsPageModel pageModel);
    }

    public class AdobeHeaderViewModelConverter : IAdobeHeaderViewModelConverter
    {
        private readonly IAnalyticsConfiguration _analyticsConfiguration;

        public AdobeHeaderViewModelConverter(IAnalyticsConfiguration analyticsConfiguration)
        {
            _analyticsConfiguration = analyticsConfiguration;
        }

        public AdobeHeaderViewModel GetFor(AnalyticsPageModel pageModel)
        {
            return new AdobeHeaderViewModel
            {
                Brand = "TAL",
                UniquePageIdentifier = pageModel.UniquePageIdentifier,
                CategoryPageType = pageModel.CategoryPageType,
                Generator = "Application",
                IssueDate = pageModel.IssueDate,
                UpdatedDate = pageModel.UpdatedDate,
                JavascriptAssetLocation = _analyticsConfiguration.AdobeTagManagerScriptUrl,
                PageName = pageModel.PageName,
                SubDomain = "coverbuilder",
                MembershipType = "",
                MembershipTypeId = ""
            };
        }
    }
}
