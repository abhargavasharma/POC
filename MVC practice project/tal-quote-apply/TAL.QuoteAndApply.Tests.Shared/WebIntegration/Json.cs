using TAL.Performance.Infrastructure.Core.ApiClients;
using TAL.QuoteAndApply.Infrastructure.Extensions;

namespace TAL.QuoteAndApply.Tests.Shared.WebIntegration
{
    public class Json : ISerializer
    {
        public string MediaType { get { return "application/json"; } }

        public string Encode<T>(T item)
        {
            return item.ToJson();
        }

        public T Decode<T>(string content)
        {
            return content.FromJson<T>();
        }
    }
}