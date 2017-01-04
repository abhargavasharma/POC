namespace TAL.QuoteAndApply.ServiceLayer.Product
{
    public class ExclusionResponse
    {
        public string Name { get; }
        public string Description { get; }

        public ExclusionResponse(string name, string description)
        {
            Name = name;
            Description = description;
        }

    }
}
