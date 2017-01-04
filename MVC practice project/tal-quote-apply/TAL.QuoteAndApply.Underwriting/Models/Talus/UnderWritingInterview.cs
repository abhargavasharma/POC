using System.Collections.Generic;
using TAL.QuoteAndApply.Infrastructure.Concurrency;

namespace TAL.QuoteAndApply.Underwriting.Models.Talus
{
    public class UnderwritingInterview : IConcurrencyToken
    {
        public string InterviewIdentifier { get; set; }

        public string TemplateType { get; set; }

        // for some reason template version is used when you create and templateid is used when getting an interview
        public string TemplateId { get; set; }

        public string TemplateVersion { get; set; }

        public string ActualTemplateVersion
        {
            get
            {
                if (!string.IsNullOrEmpty(TemplateVersion))
                {
                    return TemplateVersion;
                }
                if (!string.IsNullOrEmpty(TemplateId))
                {
                    return TemplateId;
                }
                return "";
            }
        }

        public bool Completed { get; set; }
        public string CompletedBy { get; set; }

        public List<InterviewBenefitResponse> Benefits { get; set; }

        public string ConcurrencyToken { get; set; }
    }
}