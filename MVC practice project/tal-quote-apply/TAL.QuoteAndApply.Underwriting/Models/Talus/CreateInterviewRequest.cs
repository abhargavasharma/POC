using System.Collections.Generic;

namespace TAL.QuoteAndApply.Underwriting.Models.Talus
{
    public class CreateInterviewRequestBenefits
    {
        public string Benefit { get; set; }
        public string Workflow { get; set; }
    }
    public class CreateInterviewRequest
    {
        public string TemplateType { get; set; }

        public string CreatedBy { get; set; }

        public IEnumerable<CreateInterviewRequestBenefits> Benefits { get; set; }

        public Dictionary<string, string> Variables { get; set; }

        public CreateInterviewRequest()
        {
            Variables = new Dictionary<string, string>();
        }
    }
}
