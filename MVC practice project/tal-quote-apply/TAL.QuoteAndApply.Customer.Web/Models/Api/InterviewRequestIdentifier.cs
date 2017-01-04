using System;

namespace TAL.QuoteAndApply.Customer.Web.Models.Api
{
    public class InterviewRequestIdentifier
    {
        public InterviewRequestIdentifier(Guid interviewId)
        {
            InterviewId = interviewId;
        }

        public Guid InterviewId { get; private set; }
    }
}