using System.Linq;
using TAL.QuoteAndApply.Underwriting.Models.Dto;
using TAL.QuoteAndApply.Underwriting.Models.Event;

namespace TAL.QuoteAndApply.Underwriting.Models.Converters
{
    public interface IUnderwritingBenefitResponsesChangeParamConverter
    {
        UnderwritingBenefitResponsesChangeParam CreateFrom(ReadOnlyUnderwritingInterview underwritingInterview);
        UnderwritingBenefitResponsesChangeParam CreateFrom(string interviewIdentifier, ReadOnlyUpdatedUnderwritingInterview updatedUnderwritingInterview);
    }

    public class UnderwritingBenefitResponsesChangeParamConverter : IUnderwritingBenefitResponsesChangeParamConverter
    {
        public UnderwritingBenefitResponsesChangeParam CreateFrom(
            ReadOnlyUnderwritingInterview underwritingInterview)
        {
            var benResponses = underwritingInterview.Benefits.Select(readOnlyInterviewBenefitResponse => 
            new UnderwritingBenefitResponseStatus(readOnlyInterviewBenefitResponse.BenefitCode, 
            readOnlyInterviewBenefitResponse.Status,
            readOnlyInterviewBenefitResponse.TotalLoadings,
            readOnlyInterviewBenefitResponse.Exclusions)).ToList();

            return new UnderwritingBenefitResponsesChangeParam(underwritingInterview.InterviewIdentifier, benResponses);
        }

        public UnderwritingBenefitResponsesChangeParam CreateFrom(string interviewIdentifier, ReadOnlyUpdatedUnderwritingInterview updatedUnderwritingInterview)
        {
            var benResponses = updatedUnderwritingInterview.BenefitResponses.Select(readOnlyInterviewBenefitResponse 
                => new UnderwritingBenefitResponseStatus(readOnlyInterviewBenefitResponse.BenefitCode, 
                readOnlyInterviewBenefitResponse.Status, 
                readOnlyInterviewBenefitResponse.TotalLoadings,
                readOnlyInterviewBenefitResponse.Exclusions)).ToList();

            return new UnderwritingBenefitResponsesChangeParam(interviewIdentifier, benResponses);
        }
    }
}