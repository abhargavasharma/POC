using System;

namespace TAL.QuoteAndApply.Analytics.Models
{
    public class AnalyticsPageModel
    {
        public AnalyticsPageModel(string uniquePageIdentifier, string pageName, string categoryPageType,
            DateTime issueDate, DateTime updatedDate)
        {
            UniquePageIdentifier = uniquePageIdentifier;
            PageName = pageName;
            CategoryPageType = categoryPageType;
            IssueDate = issueDate;
            UpdatedDate = updatedDate;
        }

        public string UniquePageIdentifier { get; private set; }
        public string PageName { get; private set; }
        public string CategoryPageType { get; private set; }
        public DateTime IssueDate { get; private set; }
        public DateTime UpdatedDate { get; private set; }


    }
}