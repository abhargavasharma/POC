namespace TAL.QuoteAndApply.Infrastructure.Http
{
    public interface IMimeTypeProvider
    {
        string Get(SourceType source);
    }

    public class MimeTypeProvider : IMimeTypeProvider
    {
        public string Get(SourceType source)
        {
            switch (source)
            {
                case SourceType.Json:
                    return "application/json";
                case SourceType.Xml:
                    return "text/xml";
            }
            return "";
        }
    }
}