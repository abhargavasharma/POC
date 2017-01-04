using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAL.QuoteAndApply.Underwriting.Models.Talus
{
    public class TemplateInformation
    {
        public string TemplateId { get; set; }
        public string TemplateType { get; set; }
        public int SourceSystemTemplateId { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime EffectiveTo { get; set; }
        public DateTime Created { get; set; }
        public List<BenefitDescription> Benefits { get; set; } 
    }

    public class BenefitDescription
    {
        public string BenefitCode { get; set; }
    }
}
