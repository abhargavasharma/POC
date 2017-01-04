namespace TAL.QuoteAndApply.Underwriting.Models.Talus
{
    public class ChangedAnswer
    {
        public string ResponseId { get; set; }
        public bool Selected  { get; set; }
        public string SelectedId  { get; set; }
        public ValueContainer<string> SelectedText  { get; set; }
    }
}
