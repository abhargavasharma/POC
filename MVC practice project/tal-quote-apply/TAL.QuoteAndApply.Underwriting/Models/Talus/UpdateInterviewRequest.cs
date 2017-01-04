namespace TAL.QuoteAndApply.Underwriting.Models.Talus
{
    public class UpdateInterviewRequest
    {
        public AnswerQuestionRequest AnswerQuestion { get; set; }
        
        public UpdateInterviewRequest(AnswerQuestionRequest answerQuestion)
        {
            AnswerQuestion = answerQuestion;
        }
    }
}
