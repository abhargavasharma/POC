using System.Collections.Generic;

namespace TAL.QuoteAndApply.Underwriting.Models.Talus
{
    public class Answer
    {
        public Answer()
        {
            Tags = new List<string>();
        }
        public string ResponseId { get; set; }

        public int OrderId { get; set; }

        public string Text { get; set; }

        public bool Selected { get; set; }

        public string SelectedId { get; set; }

        public string SelectedText { get; set; }

        public string HelpText { get; set; }

        public List<string> Tags { get; set; }

        public List<Loading> Loadings { get; set; }
        public List<Exclusion> Exclusions { get; set; }
    }
}