
namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public enum UnderwritingCompletedForRiskStatus
    {
        NoCoversSelected,
        Completed,
        NotCompleted
    }

    public class UnderwritingCompleteResponse
    {
        public UnderwritingCompletedForRiskStatus UnderwritingCompletedForRiskStatus { get; set; }

        public static UnderwritingCompleteResponse CreateFrom(int noOfCovers, bool isUnderwitingCompleted)
        {
            var underwritingCompletedForRiskStatus = UnderwritingCompletedForRiskStatus.Completed;

            if (noOfCovers > 0)
            {
                underwritingCompletedForRiskStatus = isUnderwitingCompleted ? UnderwritingCompletedForRiskStatus.Completed : UnderwritingCompletedForRiskStatus.NotCompleted;
            }

            return new UnderwritingCompleteResponse
            {
                UnderwritingCompletedForRiskStatus = underwritingCompletedForRiskStatus
            };
        }
    }
}