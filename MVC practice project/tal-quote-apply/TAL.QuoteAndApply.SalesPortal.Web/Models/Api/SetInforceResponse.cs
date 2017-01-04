using System.Collections.Generic;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class SetInforceResponse
    {
        public bool Successful { get; set; }
        public bool ServerError { get; set; }
        public IEnumerable<SetInforceRiskSectionValidaitonResponse> RiskValidations { get; set; }
    }

    public class SetInforceRiskSectionValidaitonResponse
    {
        public int RiskId { get; set; }
        public IEnumerable<SetInforceSectionValidaitonResponse> SectionsValidations { get; set; }
    }

    public class SetInforceSectionValidaitonResponse
    {
        public bool Completed { get; set; }

        public string SectionName { get; set; }
        public Dictionary<string, IEnumerable<string>> Errors { get; set; }
        public Dictionary<string, IEnumerable<string>> Warnings { get; set; }
    }
}