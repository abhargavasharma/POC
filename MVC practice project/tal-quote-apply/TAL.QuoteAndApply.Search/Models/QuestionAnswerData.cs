using System.Security.Cryptography.X509Certificates;

namespace TAL.QuoteAndApply.Search.Models
{
    public interface IQuestionAnswerData<T>
    {
        string Id { get; }
        string Value { get; }
        string Similies { get; }
        T RawData { get; }
        string AnswerId { get; }
        float Score { get; }

    }

    public class QuestionAnswerData<T> : IQuestionAnswerData<T>
    {
        public string Id { get; set; }
        public string AnswerId { get; set; }
        public float Score { get; private set; }
        public string Value { get; set; }
        public string Similies { get; set; }
        public T RawData { get; set; }

        public QuestionAnswerData<T> WithScore(float score)
        {
            Score = score;
            return this;
        }
    }
}
