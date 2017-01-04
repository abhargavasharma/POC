namespace TAL.QuoteAndApply.Search.LuceneWrapper
{
    /// <summary>
    /// Basic interface where every document should inherit from
    /// </summary>
    public interface IDocument
    {
        string Id { get; set; }
    }
}
