using TAL.QuoteAndApply.DataModel;

namespace TAL.QuoteAndApply.Policy.Models
{
    public interface ICoverExclusion
    {
        int CoverId { get; }
        string Name { get; }
        string Text { get; }
    }

    public class CoverExclusionDto : DbItem, ICoverExclusion
    {
        public int CoverId { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
    }
}