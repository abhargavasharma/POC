using TAL.QuoteAndApply.Underwriting.Models.Talus;

namespace TAL.QuoteAndApply.Underwriting.Models.Dto
{
    public class ReadOnlyExclusion
    {
        public string Name { get; private set; }
        public string Text { get; private set; }

        public ReadOnlyExclusion(Exclusion exclusion)
        {
            Name = exclusion.Name;
            Text = exclusion.Text;
        }
    }
}