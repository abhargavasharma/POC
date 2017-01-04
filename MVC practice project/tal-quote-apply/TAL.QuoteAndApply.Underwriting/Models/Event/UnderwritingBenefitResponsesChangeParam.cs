using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.Underwriting;
using TAL.QuoteAndApply.Underwriting.Models.Dto;

namespace TAL.QuoteAndApply.Underwriting.Models.Event
{
    public class UnderwritingBenefitResponsesChangeParam
    {
        public string InterviewId { get; }
        public IEnumerable<UnderwritingBenefitResponseStatus> BenefitResponseStatuses { get; }

        public UnderwritingBenefitResponsesChangeParam(string interviewId, IEnumerable<UnderwritingBenefitResponseStatus> benefitResponseStatuses)
        {
            InterviewId = interviewId;
            BenefitResponseStatuses = benefitResponseStatuses;
        }
    }

    public class UnderwritingBenefitResponseStatus
    {
        public string BenefitCode { get; }
        public UnderwritingStatus UnderwritingStatus { get; }
        public ReadOnlyTotalLoadings TotalLoadings { get; }
        public IReadOnlyList<ReadOnlyExclusion> ReadOnlyExclusions { get; }

        public UnderwritingBenefitResponseStatus(string benefitCode, UnderwritingStatus underwritingStatus, ReadOnlyTotalLoadings totalLoadings, IReadOnlyList<ReadOnlyExclusion> readOnlyExclusions)
        {
            BenefitCode = benefitCode;
            UnderwritingStatus = underwritingStatus;
            TotalLoadings = totalLoadings;
            ReadOnlyExclusions = readOnlyExclusions;
        }
    }
}