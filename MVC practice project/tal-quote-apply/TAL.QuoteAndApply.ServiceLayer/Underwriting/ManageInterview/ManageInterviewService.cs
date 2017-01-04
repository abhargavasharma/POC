using TAL.QuoteAndApply.Underwriting.Service;

namespace TAL.QuoteAndApply.ServiceLayer.Underwriting.ManageInterview
{
    public interface IManageInterviewService
    {
        bool InterviewExists(string interviewId);
    }
    public class ManageInterviewService: IManageInterviewService
    {
        private readonly IGetUnderwritingInterview _getUnderwritingInterview;

        public ManageInterviewService(IGetUnderwritingInterview getUnderwritingInterview)
        {
            _getUnderwritingInterview = getUnderwritingInterview;
        }

        public bool  InterviewExists(string interviewId)
        {
            return _getUnderwritingInterview.InterviewExists(interviewId) ;
        }
    }
}
