using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Underwriting.Models.Dto;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Risk
{
    public class CreateRiskResult
    {
        public IRisk Risk { get; set; }
        public InterviewReferenceInformation InterviewReferenceInformation { get; set; }
    }
}
