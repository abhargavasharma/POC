using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAL.QuoteAndApply.Analytics.Models
{
    public class AdobeHeaderViewModel
    {
        public string JavascriptAssetLocation { get; set; }
        public string UniquePageIdentifier { get; set; }
        public string PageName { get; set; }
        public string Brand { get; set; }
        public string Generator { get; set; }
        public string SubDomain { get; set; }
        public string CategoryPageType { get; set; }


        public string MembershipType { get; set; }
        public string MembershipTypeId { get; set; }


        public DateTime IssueDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public string IssueDateString => IssueDate.ToString("d/MM/yyyy h:mm:ss tt");

        public string UpdatedDateString => UpdatedDate.ToString("d/MM/yyyy h:mm:ss tt");
    }
}
