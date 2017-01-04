using System.Collections.Generic;

namespace TAL.QuoteAndApply.Underwriting.Models.Talus
{
    public class Question
    {
        public Question()
        {
            Tags = new List<string>();
            Answers= new List<Answer>();
        }
        public string Id { get; set; }

        public string ParentId { get; set; }

        public int OrderId { get; set; }
    
        public string Category { get; set; }

        public ControlType ControlType { get; set; }
        
        public string Statement { get; set; }

        public string HelpText { get; set; }

        public List<Answer> Answers { get; set; }

        public List<string> Tags { get; set; }
    }
}