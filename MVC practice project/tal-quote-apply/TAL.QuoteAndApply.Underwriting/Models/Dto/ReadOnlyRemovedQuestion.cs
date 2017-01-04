using TAL.QuoteAndApply.Underwriting.Models.Talus;

namespace TAL.QuoteAndApply.Underwriting.Models.Dto
{
    public class ReadOnlyRemovedQuestion
    {
        public string Id { get; private set; }

        public ReadOnlyRemovedQuestion(RemovedQuestion question)
        {
            Id = question.Id;
        }
    }
}