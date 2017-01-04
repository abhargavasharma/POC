using System;
using System.Collections.Generic;
using System.Linq;
using System.Monads;
using TAL.QuoteAndApply.Underwriting.Models.Talus;

namespace TAL.QuoteAndApply.Underwriting.Models.Dto
{
    public class ReadOnlyTemplateInformation
    {
        public string TemplateId { get; private set; }
        public string TemplateType { get; private set; }
        public int SourceSystemTemplateId { get; private set; }
        public DateTime EffectiveFrom { get; private set; }
        public DateTime EffectiveTo { get; private set; }
        public DateTime Created { get; private set; }
        public IReadOnlyList<string> Benefits { get; private set; }

        public ReadOnlyTemplateInformation(TemplateInformation templateInformation)
        {
            TemplateId = templateInformation.TemplateId;
            TemplateType = templateInformation.TemplateType;
            SourceSystemTemplateId = templateInformation.SourceSystemTemplateId;
            EffectiveFrom = templateInformation.EffectiveFrom;
            EffectiveTo = templateInformation.EffectiveTo;
            Created = templateInformation.Created;
            Benefits = templateInformation.Benefits.With(b => b.BenefitCode).Return(list => list.ToList(), null);
        }
    }
}