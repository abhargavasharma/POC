namespace TAL.QuoteAndApply.Underwriting.Models.Talus
{
    /// <summary>
    /// CompleteInterviewRequest parameters are options
    /// </summary>
    public class CompleteInterviewRequest
    {
        /// <summary>
        /// User answering the question (Optional)
        /// </summary>
        public readonly string CompletedBy;

        public CompleteInterviewRequest(string completedBy)
        {
            CompletedBy = completedBy;
        }
    }
}
