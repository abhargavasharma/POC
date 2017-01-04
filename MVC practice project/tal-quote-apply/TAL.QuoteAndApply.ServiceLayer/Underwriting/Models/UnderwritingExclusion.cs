namespace TAL.QuoteAndApply.ServiceLayer.Underwriting.Models
{
    public class UnderwritingExclusion
    {
        public string Name { get; private set; }
        public string Text { get; private set; }

        public UnderwritingExclusion(string name, string text)
        {
            Name = name;
            Text = text;
        }
    }
}
