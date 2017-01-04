namespace TAL.QuoteAndApply.ServiceLayer.Search.Underwriting
{
    public interface ISearchPhraseSanitizer
    {
        string SanitizePhrase(string phrase);
    }

    public class SearchPhraseSanitizer : ISearchPhraseSanitizer
    {
        public string SanitizePhrase(string phrase)
        {
            return phrase.Replace("/", " ").Replace("-", " ");
        }
    }
}